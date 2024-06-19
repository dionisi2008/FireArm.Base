using System.Text.Json;
using System.Net;
using System.Text;

namespace CSO
{
    public class FireArm_API_Server
    {
        public string IPServer { get; set; }
        public int PORTServer { get; set; }
        public System.Net.HttpListener ServerListener;
        public FireArm_API_Server(string GetIp, int GetPort)
        {
            this.IPServer = GetIp;
            this.PORTServer = GetPort;
            ServerListener = new HttpListener();
            ServerListener.Prefixes.Add("http://*:" + GetPort + "/");
            ServerListener.Start();
            StartServer(ServerListener);
        }

        public async static void StartServer(HttpListener GetServer)
        {
            do
            {
                var context = await GetServer.GetContextAsync();
                var ws = await context.AcceptWebSocketAsync("");
                GetWebSocket(ws.WebSocket);

            } while (GetServer.IsListening);
        }
        public async static void GetWebSocket(System.Net.WebSockets.WebSocket GetContext)
        {
            string GetInfo = "";
            ArraySegment<byte> test = new ArraySegment<byte>();
            GetContext.ReceiveAsync(test, CancellationToken.None);
            GetInfo = Encoding.UTF8.GetString(test.ToArray());

        }

    }
}