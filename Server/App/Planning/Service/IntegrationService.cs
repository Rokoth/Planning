using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class IntegrationService : IIntegrationService
    {
        public Task<bool> BuhgalteryAddReserve(string taskData, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<List<IntegrationProduct>> BuhgalteryGetProducts(string name, CancellationToken token)
        {
            using HttpClient httpClient = new HttpClient();
            var message = new HttpRequestMessage()
            { 
                Method = HttpMethod.Get,
                RequestUri = new Uri("")                
            };
            var response = await httpClient.SendAsync();
        }
    }

    public class IntegrationProduct
    {

    }
}
