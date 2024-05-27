namespace CSO
{
    
public class User
{
    public string Id { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }

    public User(string id, string login, string passwordHash, string name, string role)
    {
        Id = id;
        Login = login;
        PasswordHash = passwordHash;
        Name = name;
        Role = role;
    }
}

}