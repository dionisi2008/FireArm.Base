using System;

namespace CSO
{
    public class UserGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> UserIds { get; set; } // Коллекция идентификаторов пользователей

        public UserGroup()
        {

        }
        public UserGroup(string id, string name)
        {
            Id = id;
            Name = name;
            UserIds = new List<string>(); // Инициализация коллекции
        }

        public UserGroup(string id, string name, List<string> userIds)
        {
            Id = id;
            Name = name;
            UserIds = userIds;
        }
    }
}