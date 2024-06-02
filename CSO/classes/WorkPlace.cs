using System;
public class WorkPlace
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string IpAddress { get; set; }
    public int Port { get; set; }

    public WorkPlace(string id, string name, string ipAddress, int port)
    {
        Id = id;
        Name = name;
        IpAddress = ipAddress;
        Port = port;
    }
}