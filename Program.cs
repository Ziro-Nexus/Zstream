using Sockets;
using System.Net;
using System.Text;
using System.Net.Sockets;

Sockets.Zsocket sock = new("127.0.0.1:8090");
sock.CreateTcpListener();
var task_socket = sock.WaitClients("ZIROX SERVER 0.1.\n");



while(true) {
    var input = Console.ReadLine();
    if (input == "exit") {
        sock.CancelToken();
        sock.CloseAllConnections();
        break;
    }
    sock.BroadCast(input + "\n");
}




