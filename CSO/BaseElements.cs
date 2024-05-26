using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using System.Globalization;

namespace CSO
{
    public class Storage
    {
        public const int HeaderSize = 128 * 1024 * 1024; // Размер заголовка в байтах
        public List<User> Users { get; set; } // Список пользователей
        public List<UserGroup> UserGroups { get; set; } // Список групп пользователей
        public List<Session> Sessions { get; set; } // Список сеансов
        public List<ActionHistory> ActionHistories { get; set; } // Список истории действий
        public List<DeviceConnection> DeviceConnections { get; set; } // Список подключений к устройствам
        public List<WorkPlace> WorkPlaces { get; set; } // Список рабочих мест
        public Stream FileStream { get; set; } // Переменная типа Stream для работы с файлом

        public byte[] Header { get; set; } // Массив байт для хранения заголовка
        public static List<string> HeaderNames { get; set; } // Список имен заголовков
        // public static Dictionary<string, int> PositionDictionary { get; set; } // Словарь для хранения идентификаторов и позиций
        public Storage() : this("default_filename.txt")
        {
            // Пустой конструктор, вызывает конструктор с указанием имени файла по умолчанию
        }
        public Storage(string filePath)
        {
            Users = new List<User>();
            UserGroups = new List<UserGroup>();
            Sessions = new List<Session>();
            ActionHistories = new List<ActionHistory>();
            DeviceConnections = new List<DeviceConnection>();
            WorkPlaces = new List<WorkPlace>();
            HeaderNames = new List<string>();
            // PositionDictionary = new Dictionary<string, int>();
            if (File.Exists(filePath))
            {
                FileStream = new FileStream(filePath, FileMode.Open);
            }
            else
            {
                try
                {
                    FileStream = new FileStream(filePath, FileMode.OpenOrCreate);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while opening the file: {ex.Message}");
                    FileStream = new FileStream("default_filename.txt", FileMode.Create);
                }

            }


            // Проверка размера файла
            if (FileStream.Length >= HeaderSize)
            {
                Header = new byte[HeaderSize];
                FileStream.Read(Header, 0, Header.Length);
                string[] strings = Encoding.UTF8.GetString(Header).Split('\n');

                // PositionDictionary = new Dictionary<string, int>();

                foreach (var str in strings)
                {
                    HeaderNames.Add(str);
                    HeaderString headerStringGet = new HeaderString(str);

                    // PositionDictionary.Add(headerStringGet.Id, headerStringGet.startindex);
                    byte[] dataBuffer = new byte[headerStringGet.Size];
                    FileStream.Position = headerStringGet.startindex;
                    FileStream.Read(dataBuffer, 0, headerStringGet.Size);

                    switch (headerStringGet.Type)
                    {
                        case "User":

                            Users.Add(JsonSerializer.Deserialize<User>(Encoding.UTF8.GetString(dataBuffer)));
                            break;
                        case "UserGroup":

                            UserGroups.Add(JsonSerializer.Deserialize<UserGroup>(Encoding.UTF8.GetString(dataBuffer)));
                            break;
                        case "Session":
                            Sessions.Add(JsonSerializer.Deserialize<Session>(Encoding.UTF8.GetString(dataBuffer)));
                            break;
                        case "ActionHistory":
                            ActionHistories.Add(JsonSerializer.Deserialize<ActionHistory>(Encoding.UTF8.GetString(dataBuffer)));
                            break;
                        case "DeviceConnection":
                            DeviceConnections.Add(JsonSerializer.Deserialize<DeviceConnection>(Encoding.UTF8.GetString(dataBuffer)));
                            break;
                        case "WorkPlace":
                            WorkPlaces.Add(JsonSerializer.Deserialize<WorkPlace>(Encoding.UTF8.GetString(dataBuffer)));
                            break;
                    }
                }
            }

            else
            {
                string UserId = GenerateUniqueId();
                string GroupId = GenerateUniqueId();
                UserGroup Group = new UserGroup(GroupId, "Administrators");
                Group.UserIds.Add(UserId);
                User admin = new User(UserId, "Admin", CalculateSHA256("12345678"), "Administrator", GroupId);
                WriteBase(admin);
                WriteBase(Group);
            }
        }

        public int GetNextFreePosition(int requiredSize)
        {
            if (HeaderNames.Count > 1)
            {
                for (int shag = 0; shag <= HeaderNames.Count - 1; shag++)
                {
                    if ((new HeaderString(HeaderNames[shag + 1]).startindex - new HeaderString(HeaderNames[shag]).GetSizeAndPosition()) >= requiredSize)
                    {
                        return new HeaderString(HeaderNames[shag]).GetSizeAndPosition();
                    }
                }
                return new HeaderString(HeaderNames[HeaderNames.Count]).GetSizeAndPosition();
            }
            else if (HeaderNames.Count == 1)
            {
                return new HeaderString(HeaderNames[0]).GetSizeAndPosition();
            }
            return HeaderSize;

        }

        public byte[] WriteBaseToJson<T>(T data)
        {
            var json = JsonSerializer.Serialize(data);
            return Encoding.UTF8.GetBytes(json);
        }
        public void WriteBase<T>(T data)
        {
            switch (data)
            {
                case User user:
                    Users.Add(user);
                    byte[] userDataBytes = WriteBaseToJson(user);
                    int userPosition = GetNextFreePosition(userDataBytes.Length);
                    // PositionDictionary.Add(user.Id, userPosition);

                    string userHeader = $"{typeof(User).Name} {user.Id} {userPosition} {userDataBytes.Length}" + ' ';
                    System.Console.WriteLine(userHeader);

                    HeaderNames.Add(userHeader);

                    FileStream.Position = userPosition;
                    FileStream.Write(userDataBytes);

                    FileStream.Position = 0;
                    FileStream.Write(Encoding.UTF8.GetBytes(string.Join("\n", HeaderNames)));

                    break;

                case UserGroup userGroup:
                    UserGroups.Add(userGroup);
                    byte[] userGroupDataBytes = WriteBaseToJson(userGroup);
                    int userGroupPosition = GetNextFreePosition(userGroupDataBytes.Length);
                    // PositionDictionary.Add(userGroup.Id, userGroupPosition);

                    string groupHeader = $"{typeof(UserGroup).Name} {userGroup.Id} {userGroupPosition} {userGroupDataBytes.Length}" + ' ';

                    HeaderNames.Add(groupHeader);

                    FileStream.Position = userGroupPosition;
                    FileStream.Write(userGroupDataBytes);

                    FileStream.Position = 0;
                    FileStream.Write(Encoding.UTF8.GetBytes(string.Join("\n", HeaderNames)));
                    break;

                case Session session:
                    Sessions.Add(session);
                    byte[] sessionDataBytes = WriteBaseToJson(session);
                    int sessionPosition = GetNextFreePosition(sessionDataBytes.Length);
                    // PositionDictionary.Add(session.Id, sessionPosition);

                    string sessionHeader = $"{typeof(Session).Name} {session.Id} {sessionPosition} {sessionDataBytes.Length}" + ' ';
                    HeaderNames.Add(sessionHeader);

                    FileStream.Position = sessionPosition;
                    FileStream.Write(sessionDataBytes);

                    FileStream.Position = 0;
                    FileStream.Write(Encoding.UTF8.GetBytes(string.Join("\n", HeaderNames)));
                    break;

                case ActionHistory actionHistory:
                    ActionHistories.Add(actionHistory);
                    byte[] actionHistoryDataBytes = WriteBaseToJson(actionHistory);
                    int actionHistoryPosition = GetNextFreePosition(actionHistoryDataBytes.Length);
                    // PositionDictionary.Add(actionHistory.Id, actionHistoryPosition);

                    string actionHistoryHeader = $"{typeof(ActionHistory).Name} {actionHistory.Id} {actionHistoryPosition} {actionHistoryDataBytes.Length}" + ' ';
                    HeaderNames.Add(actionHistoryHeader);

                    FileStream.Position = actionHistoryPosition;
                    FileStream.Write(actionHistoryDataBytes);

                    FileStream.Position = 0;
                    FileStream.Write(Encoding.UTF8.GetBytes(string.Join("\n", HeaderNames)));
                    break;

                case DeviceConnection deviceConnection:
                    DeviceConnections.Add(deviceConnection);
                    byte[] deviceConnectionDataBytes = WriteBaseToJson(deviceConnection);
                    int deviceConnectionPosition = GetNextFreePosition(deviceConnectionDataBytes.Length);
                    // PositionDictionary.Add(deviceConnection.Name, deviceConnectionPosition);

                    string deviceConnectionHeader = $"{typeof(DeviceConnection).Name} {deviceConnection.Name} {deviceConnectionPosition} {deviceConnectionDataBytes.Length}" + ' ';
                    HeaderNames.Add(deviceConnectionHeader);

                    FileStream.Position = deviceConnectionPosition;
                    FileStream.Write(deviceConnectionDataBytes);

                    FileStream.Position = 0;
                    FileStream.Write(Encoding.UTF8.GetBytes(string.Join("\n", HeaderNames)));
                    break;

                case WorkPlace workPlace:
                    WorkPlaces.Add(workPlace);
                    byte[] workPlaceDataBytes = WriteBaseToJson(workPlace);
                    int workPlacePosition = GetNextFreePosition(workPlaceDataBytes.Length);
                    // PositionDictionary.Add(workPlace.Id, workPlacePosition);

                    string workPlaceHeader = $"{typeof(WorkPlace).Name} {workPlace.Id} {workPlacePosition} {workPlaceDataBytes.Length}" + ' ';
                    HeaderNames.Add(workPlaceHeader);

                    FileStream.Position = workPlacePosition;
                    FileStream.Write(workPlaceDataBytes);

                    FileStream.Position = 0;
                    FileStream.Write(Encoding.UTF8.GetBytes(string.Join("\n", HeaderNames)));
                    break;

                default:
                    throw new ArgumentException("Unsupported type.");
            }
            FileStream.Flush();
        }
        public static string CalculateSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    stringBuilder.Append(hash[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        public static string GenerateUniqueId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string sb = "";
            Random random = new Random(DateTime.Now.Microsecond);
            for (int i = 0; i < 16; i++)
            {
                sb = sb + chars[random.Next(chars.Length)];
            }
            if (HeaderNames.Count >= 1)
            {
                for (int shag = 0; shag <= HeaderNames.Count - 1; shag++)
                {
                    if (new HeaderString(HeaderNames[shag]).Id == sb)
                    {
                        sb = "";
                        for (int i = 0; i < 16; i++)
                        {
                            sb = sb + chars[random.Next(chars.Length)];
                        }
                    }
                }
            }
            return sb;
        }
        public async static void ReadCommand(string GetCommand)
        {
            string[] DataWork = GetCommand.Split('\n');
            switch (DataWork[0])
            {
                case "GET":
                    break;
                case "SET":
                    break;
                case "UPDATE":
                    break;
                case "DEL":
                    break;
            }

            
        }
    }
}

public class HeaderString
{
    public string Type { get; set; }
    public string Id { get; set; }
    public int startindex { get; set; }
    public int Size { get; set; }
    public HeaderString(string GetString)
    {
        string[] ReadData = GetString.Split(' ');
        Type = ReadData[0];
        Id = ReadData[1];
        startindex = int.Parse(ReadData[2]);
        Size = int.Parse(ReadData[3]);
    }

    public int GetSizeAndPosition()
    {
        return startindex + Size;
    }

}

// Класс пользователя
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

// Класс группы пользователей
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

// Класс рабочего места WorkPlace
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

// Класс сессии
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

// Класс истории действий
public class ActionHistory
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Action { get; set; }
    public DateTime Time { get; set; }

    public ActionHistory(string id, string userId, string action, DateTime time)
    {
        Id = id;
        UserId = userId;
        Action = action;
        Time = time;
    }
}

// Класс для подключения к устройству
public class DeviceConnection
{
    public string Name { get; set; }
    public string IpAddress { get; set; }
    public int Port { get; set; }
    public string UdpIdentifier { get; set; }

    public DeviceConnection(string name, string ipAddress, int port, string udpIdentifier)
    {
        Name = name;
        IpAddress = ipAddress;
        Port = port;
        UdpIdentifier = udpIdentifier;
    }
}
