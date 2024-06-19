using System.Net.WebSockets;
using System.Text;

namespace CSO
{

    public class FireArm_API_Client
    {
        public string WSURL;
        public ClientWebSocket WSClient;
        public FireArm_API_Client(string GetWSURL)
        {
            WSClient.ConnectAsync(new Uri(GetWSURL), CancellationToken.None);
            
        }
        public void GetWebSocket(WebSocket GetContext)
        {
            string GetInfo = "";
            ArraySegment<byte> test = new ArraySegment<byte>();
            GetContext.ReceiveAsync(test, CancellationToken.None);
            GetInfo = Encoding.UTF8.GetString(test.ToArray());
        }

    }
}