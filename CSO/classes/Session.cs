using System;
public class Session
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string IpAddress { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime ExpirationTime { get; set; }

    public Session(string id, string userId, string ipAddress, DateTime creationTime, DateTime expirationTime)
    {
        Id = id;
        UserId = userId;
        IpAddress = ipAddress;
        CreationTime = creationTime;
        ExpirationTime = expirationTime;
    }
}