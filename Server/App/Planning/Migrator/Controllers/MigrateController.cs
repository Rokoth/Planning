using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.DB.Context;
using Planning.DB.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> Migrate([FromQuery]Guid userId)
        {
            try
            {
                var repoProjs = _serviceProvider.GetRequiredService<IRepository<Project>>();
                var fbContext = _serviceProvider.GetRequiredService<MigrateContext>();
                var projs = fbContext.Set<MigrateProject>().ToList();
                var rootProj = projs.FirstOrDefault(s => s.PARENT == -1);
                var source = new CancellationTokenSource(300000);
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
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at MigrateController::Migrate: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Error at MigrateController::Migrate: {ex.Message}");
            }
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
                        Priority = 4400 - (child.TIMINGS ?? 4320 ),
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
