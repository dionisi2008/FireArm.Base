public class FireArm_API_Command
{
    public string ID { get; set; }
    public string Type { get; set; }
    public string Metode { get; set; }
    public byte[] BytesWrite { get; set; }

    public FireArm_API_Command(string GetID, string GetType, byte[] GetBytesWrite, string GetMetode)
    {
        ID = GetID;
        Type = GetType;
        BytesWrite = GetBytesWrite;
        Metode = GetMetode;
    }
}
