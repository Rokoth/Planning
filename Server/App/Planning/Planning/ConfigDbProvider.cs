using System;
using System.Linq;

using Deploy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Planning.DB.Context;

namespace Planning
{
    public class ConfigDbProvider : ConfigurationProvider
    {
        private readonly Action<DbContextOptionsBuilder> _options;
        private readonly IDeployService _deployService;

        public ConfigDbProvider(Action<DbContextOptionsBuilder> options,
            IDeployService deployService)
        {
            _options = options;
            _deployService = deployService;
        }

        public override void Load()
        {
            try
            {
                LoadInternal();
            }
            catch
            {
                try
                {
                    _deployService.Deploy().Wait();
                    LoadInternal();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void LoadInternal()
        {
            var builder = new DbContextOptionsBuilder<DbPgContext>();
            _options(builder);

            using (var context = new DbPgContext(builder.Options))
            {
                var items = context.Settings
                    .AsNoTracking()
                    .ToList();

                foreach (var item in items)
                {
                    Data.Add(item.ParamName, item.ParamValue);
                }
            }
        }
    }
}
