using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Sockets {
    public class Zsocket
    {
        public CancellationTokenSource Token;
        private List<TcpClient>? _clients { get; set; }
        private TcpListener _listener;

        private int ServerPort { get; set; }
        private string ServerIp { get; set; }

        public Zsocket(string connection_string)
        {
            List<string> conn_args = connection_string.Split(':').ToList();
            this.ServerIp = conn_args[0];
            this.ServerPort = int.Parse(conn_args[1]);
            this._clients = new();

        }

        public void CancelToken() {
            this.Token.Cancel();
        }

        public List<TcpClient>? GetClients() {
            return this._clients;
        }

        public TcpListener CreateTcpListener()
        {
            Console.WriteLine(this.ServerIp);
            // parsing my own ipAdress using type safe memory checking
            ReadOnlySpan<char> ipBytes = new(this.ServerIp.ToCharArray());
            IPAddress ipAddress = IPAddress.Parse(ipBytes);
            // creating listener
            this._listener = new TcpListener(ipAddress, this.ServerPort);
            _listener.Start();
            Console.WriteLine("Server is listening");
            return _listener;
        }

        public void BroadCast(string message) {
            foreach (var client in this.GetClients())
            {
                byte[] msg = Encoding.ASCII.GetBytes(message);
                client.GetStream().Write(msg, 0, msg.Length);
            }
        }

        public void CloseAllConnections() {
            foreach (var client in this.GetClients())
                client.Close();
        }

        public async Task WaitClients(string message)
        {
            int num = 0;
            this.Token = new CancellationTokenSource();
            await Task.Run(() => {
                while(true)
                {
                    if(this.Token.IsCancellationRequested)
                        break;
                    
                    TcpClient client = this._listener.AcceptTcpClient();
                    this._clients.Add(client);
                    // welcome message to all entering users
                    byte[] welcomeMessage = Encoding.ASCII.GetBytes(message);
                    client.GetStream().Write(welcomeMessage, 0, welcomeMessage.Length);
                    num++;
                }
            }, this.Token.Token);
            
        }
    }
}