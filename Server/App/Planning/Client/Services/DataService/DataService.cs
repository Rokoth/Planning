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
        private IServiceProvider _serviceProvider;
        private ILogger _logger;
        private IClientHttpClient _httpclient;

        public DataService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<DataService<T>>>();
            _httpclient = _serviceProvider.GetRequiredService<IClientHttpClient>();
        }

    }
}
