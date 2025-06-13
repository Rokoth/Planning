using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planning.Client.ClientHttpClient;
using Planning.Contract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanningClient.Services.DataService
{
    public abstract class DataService<T> where T:Entity
    {
        protected IServiceProvider _serviceProvider;
        protected ILogger _logger;
        protected IClientHttpClient _httpclient;

        public DataService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<DataService<T>>>();
            _httpclient = _serviceProvider.GetRequiredService<IClientHttpClient>();
        }

    }

    public class FormulaDataService : DataService<Formula>
    {
        public FormulaDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        { 
        
        }
    }
}
