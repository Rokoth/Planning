using Planning.Contract.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Planning.Client.ClientHttpClient
{
    public interface IClientHttpClient
    {
        bool IsConnected { get; }

        event EventHandler OnConnect;

        Task<bool> Auth(UserIdentity identity);
        void ConnectToServer(string server, Action<bool, bool, string> onResult);
        void Dispose();       
    }
}