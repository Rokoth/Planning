using System;
using System.Collections.Generic;

namespace Planning.Client.ClientHttpClient
{
    public interface IHttpClientSettings
    {
        Dictionary<Type, string> Apis { get; }
        string Server { get; }
    }
}
