using System.Text.Json;
using System.Net;
using System.Text;

namespace CSO
{
    public class FireArm_API_prot
    {
        public string IPServer { get; set; }
        public int PORTServer { get; set; }
        public System.Net.HttpListener ServerListener;
        public FireArm_API_prot(string GetIp, int GetPort)
        {
            this.IPServer = GetIp;
            this.PORTServer = GetPort;
            ServerListener = new HttpListener();
            ServerListener.Prefixes.Add("http://*:" + GetPort + "/");
            ServerListener.Start();
        }

    }
}