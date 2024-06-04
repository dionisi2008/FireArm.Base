using System.Text;
using System.Text.Json;
public class WorkPlace
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string IpAddress { get; set; }
    public int Port { get; set; }

    public WorkPlace()
    {

    }
    public WorkPlace(string id, string name, string ipAddress, int port)
    {
        Id = id;
        Name = name;
        IpAddress = ipAddress;
        Port = port;
    }

    public WorkPlace(string name, string ipAddress, int port)
    {
        Name = name;
        IpAddress = ipAddress;
        Port = port;
    }
    public WorkPlace(string GetJson)
    {
        var tempshablon = JsonSerializer.Deserialize<WorkPlace>(GetJson);
        Id = tempshablon.Id;
        Name = tempshablon.Name;
        IpAddress = tempshablon.IpAddress;
        Port = tempshablon.Port;
    }

    public string GetJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public byte[] GetBytes()
    {
        return Encoding.UTF8.GetBytes(GetJson());
    }


}