using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.DB.Context;
using Planning.DB.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Migrator.Controllers
{
    [Route("api/v1/migrate")]
    [ApiController]
    public class MigrateController : ControllerBase
    {
        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        public MigrateController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<MigrateController>>();
        }
        
        [HttpGet]
        public async Task<IActionResult> Migrate([FromQuery]Guid userId, [FromQuery]bool migrateDb = true, 
            [FromQuery] bool migrateAddTime = true, [FromQuery] bool clearOldBase = true)
        {
            try
            {
                var repoProjs = _serviceProvider.GetRequiredService<IRepository<Project>>();
                var fbContext = _serviceProvider.GetRequiredService<MigrateContext>();
                var projs = fbContext.Set<MigrateProject>().ToList();
                var rootProj = projs.FirstOrDefault(s => s.PARENT == -1);
                var source = new CancellationTokenSource(300000);
                if (migrateDb)
                {
                    var newRootProj = new Project()
                    {
                        AddTime = 0,
                        Id = Guid.NewGuid(),
                        IsDeleted = false,
                        IsLeaf = false,
                        LastUsedDate = DateTimeOffset.Parse(rootProj.LAST_USE),
                        Name = rootProj.PRJT,
                        ParentId = null,
                        Path = rootProj.PRJT,
                        Period = null,
                        Priority = 10,
                        UserId = userId,
                        VersionDate = DateTimeOffset.Now
                    };
                    await repoProjs.AddAsync(newRootProj, true, source.Token);
                    var rootChilds = projs.Where(s => s.PARENT == rootProj.ID);

                    await AddChilds(userId, repoProjs, projs, source, newRootProj, rootChilds, new List<string> { "Покупки" });
                }

                if (migrateAddTime)
                {
                    using (var file = new StreamReader("F:\\Data\\Remains.txt"))
                    {
                        var rows = file.ReadToEnd().Split("\r\n");
                        foreach (var row in rows)
                        {
                            var matches = Regex.Match(row, "^(.*?) \\+(\\d+)$");
                            var projPath = matches.Groups[1].Value.Split("\\");
                            if (!projPath.Any()) throw new Exception($"fail to parse project string: {row}");
                            var curProj = (await repoProjs.GetAsync(new Filter<Project>() { 
                               Selector = s=>s.UserId == userId && s.ParentId == null
                            }, source.Token)).Data.FirstOrDefault();
                            for ( int i = 1; i< projPath.Length; i++)
                            {
                                var path = projPath[i];
                                curProj = (await repoProjs.GetAsync(new Filter<Project>()
                                {
                                    Selector = s => s.ParentId == curProj.Id && s.Name == path
                                }, source.Token)).Data.FirstOrDefault();
                                if(curProj == null ) throw new Exception($"fail to find project: {row}");
                            }
                            if (!int.TryParse(matches.Groups[2].Value, out int test))
                            {
                                Console.WriteLine();
                            }
                            curProj.AddTime += int.Parse(matches.Groups[2].Value);
                            await repoProjs.UpdateAsync(curProj, true, source.Token);
                        }
                    }
                }

                if (clearOldBase)
                {
                    var toDel = projs.Where(s=>s.PARENT == rootProj.ID.Value && s.PRJT!= "Покупки");
                    foreach(var prj in toDel) ClearProjectRecursive(prj.ID.Value, projs, fbContext);
                    fbContext.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at MigrateController::Migrate: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Error at MigrateController::Migrate: {ex.Message}");
            }
        }

        private void ClearProjectRecursive(int id, List<MigrateProject> projs, MigrateContext context)
        {
            var toDel = projs.FirstOrDefault(s=>s.ID == id);
            var projects = projs.Where(s=>s.PARENT == id);
            foreach (var proj in projects)
            {
                ClearProjectRecursive(proj.ID.Value, projs, context);
            }
            context.Remove(toDel);            
        }

        private List<MigrateProject> migrateProjects = new List<MigrateProject>();

        private async Task AddChilds(Guid userId, IRepository<Project> repoProjs, 
            List<MigrateProject> projs, CancellationTokenSource source, 
            Project rootProj, IEnumerable<MigrateProject> rootChilds, List<string> ignore = null)
        {
            foreach (var child in rootChilds)
            {
                if (rootChilds.Any(s => s.PRJT == child.PRJT && s.ID != child.ID))
                {
                    throw new ArgumentException($"Double in FB base: {child.PRJT} : {child.ID} - {rootChilds.FirstOrDefault(s => s.PRJT == child.PRJT && s.ID != child.ID).ID}");
                }

                if (ignore==null || !ignore.Contains(child.PRJT))
                {
                    var childs = projs.Where(s => s.PARENT == child.ID);
                    var newProj = new Project()
                    {
                        AddTime = 0,
                        Id = Guid.NewGuid(),
                        IsDeleted = false,
                        IsLeaf = !childs.Any(),
                        LastUsedDate = DateTimeOffset.Parse(child.LAST_USE),
                        Name = child.PRJT,
                        ParentId = rootProj.Id,
                        Path = child.PRJT,
                        Period = child.PRJT == "Управление"? (int?)null : 60,
                        Priority = 10000 - (child.TIMINGS ?? 4320 ),
                        UserId = userId,
                        VersionDate = DateTimeOffset.Now
                    };
                    await repoProjs.AddAsync(newProj, true, source.Token);
                    if (childs.Any() && !childs.Any(s => s.PRJT == "Управление"))
                    {
                        migrateProjects.Add(child);
                        var magmtProj = new Project()
                        {
                            AddTime = 0,
                            Id = Guid.NewGuid(),
                            IsDeleted = false,
                            IsLeaf = true,
                            LastUsedDate = DateTimeOffset.Now,
                            Name = "Управление",
                            ParentId = newProj.Id,
                            Path = "Управление",
                            Period = null,
                            Priority = 10,
                            UserId = userId,
                            VersionDate = DateTimeOffset.Now
                        };
                        await repoProjs.AddAsync(magmtProj, true, source.Token);
                    }    
                    
                    await AddChilds(userId, repoProjs, projs, source, newProj, childs);
                }
            }
        }
    }
}
