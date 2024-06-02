using System.Text.Json;

namespace CSO
{
    public class FireArm_API
    {
        public string Id { get; set; }
        public string IdWorkPlace { get; set; }
        public string HostName { get; set; }
        public int PortServer { get; set; }
        public FireArm_API(string GetId, string GetHostName, int GetPort, string GetIdWorkPlace)
        {
            this.HostName = GetHostName;
            this.PortServer = GetPort;
            this.Id = GetId;
            this.IdWorkPlace = GetIdWorkPlace;
        }
        public FireArm_API(string GetJson)
        {
            var tempshablon = JsonSerializer.Deserialize<FireArm_API>(GetJson);
            if (tempshablon != null)
            {
                HostName = tempshablon.HostName;
                PortServer = tempshablon.PortServer;
                Id = tempshablon.Id;
                IdWorkPlace = tempshablon.IdWorkPlace;
            }
        }

        public FireArm_API(string GetHostName, int GetPort, string GetIdWorkPlace)
        {
            Id = "";
            HostName = GetHostName;
            PortServer = GetPort;
            IdWorkPlace = GetIdWorkPlace;
        }
        public string GetJson()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}