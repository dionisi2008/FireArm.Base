using System.Text;

namespace CSO
{

    public class User
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string Name { get; set; }
        public List<string> IDGroups { get; set; }

        public User()
        {
            IDGroups = new List<string>();
        }

        public byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(this));
        }
    }

}