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
        public static List<HeaderString> HeaderNames { get; set; } // Список имен заголовков
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
            HeaderNames = new List<HeaderString>();
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
                    HeaderNames.Add(new HeaderString(str.Split("\0")[0]));
                    HeaderString TmpHeaderString = new HeaderString(str);
                    string jsonObjectRead = ReadDataJson(TmpHeaderString);

                    switch (TmpHeaderString.Type)
                    {
                        case "User":
                            Console.WriteLine(jsonObjectRead);
                            Users.Add(JsonSerializer.Deserialize<User>(jsonObjectRead));
                            break;
                        case "UserGroup":
                            Console.WriteLine(jsonObjectRead);
                            UserGroups.Add(JsonSerializer.Deserialize<UserGroup>(jsonObjectRead));
                            break;
                        case "Session":
                            Sessions.Add(JsonSerializer.Deserialize<Session>(jsonObjectRead));
                            break;
                        case "ActionHistory":
                            ActionHistories.Add(JsonSerializer.Deserialize<ActionHistory>(jsonObjectRead));
                            break;
                        case "DeviceConnection":
                            DeviceConnections.Add(JsonSerializer.Deserialize<DeviceConnection>(jsonObjectRead));
                            break;
                        case "WorkPlace":
                            WorkPlaces.Add(JsonSerializer.Deserialize<WorkPlace>(jsonObjectRead));
                            break;
                    }
                }
            }

            else
            {
                FileStream.Write(new ReadOnlySpan<byte>(new byte[HeaderSize]));
                FileStream.Flush();

                User admin = new User();
                admin.Name = "Administrator";
                admin.Login = "Admin";
                admin.PasswordHash = CalculateSHA256("12345678");
                admin.Id = ReadCommand(new FireArm_API_Command("", "User", admin.GetBytes(), "SET"));


                UserGroup TmpGroup = new UserGroup();
                TmpGroup.Name = "Administrators";
                TmpGroup.Id = ReadCommand(new FireArm_API_Command("", "UserGroup", TmpGroup.GetBytes(), "SET"));

                TmpGroup.UserIds.Add(admin.Id);
                admin.IDGroups.Add(TmpGroup.Id);

                ReadCommand(new FireArm_API_Command(admin.Id, "User", admin.GetBytes(), "SET"));
                ReadCommand(new FireArm_API_Command(TmpGroup.Id, "UserGroup", TmpGroup.GetBytes(), "SET"));

                WorkPlace place1 = new WorkPlace("Server", "127.0.0.1", 8086);
                place1.Id = ReadCommand(new FireArm_API_Command("", "WorkPlace", place1.GetBytes(), "SET"));
                ReadCommand(new FireArm_API_Command(place1.Id, "WorkPlace", place1.GetBytes(), "SET"));

            }
        }
        public string ReadDataJson(HeaderString GetStringHeader)
        {
            byte[] dataBuffer = new byte[GetStringHeader.Size];
            FileStream.Position = GetStringHeader.startindex;
            FileStream.Read(dataBuffer, 0, GetStringHeader.Size);
            return Encoding.UTF8.GetString(dataBuffer);
        }
        public string ReadDataJson(string GetId)
        {
            for (int shag = 0; shag <= HeaderNames.Count - 1; shag++)
            {
                if (HeaderNames[shag].Id == GetId)
                {
                    HeaderString GetStringHeader = HeaderNames[shag];
                    byte[] dataBuffer = new byte[GetStringHeader.Size];
                    FileStream.Position = GetStringHeader.startindex;
                    FileStream.Read(dataBuffer, 0, GetStringHeader.Size);
                    return Encoding.UTF8.GetString(dataBuffer);
                }
            }
            return null;

        }
        public int GetNextFreePosition(int requiredSize)
        {
            if (HeaderNames.Count > 3)
            {
                for (int shag = 0; shag + 1 <= HeaderNames.Count - 1; shag++)
                {
                    if (HeaderNames[shag + 1].startindex - HeaderNames[shag].GetSizeAndPosition() >= requiredSize)
                    {
                        return HeaderNames[shag].GetSizeAndPosition();
                    }
                }
                return HeaderNames[HeaderNames.Count].GetSizeAndPosition();
            }
            else if (HeaderNames.Count == 1)
            {
                return HeaderNames[0].GetSizeAndPosition();
            }
            else if (HeaderNames.Count == 2)
            {
                return HeaderNames[1].GetSizeAndPosition();
            }
            return HeaderSize;

        }
        public byte[] WriteBaseToJson<T>(T data)
        {
            var json = JsonSerializer.Serialize(data);
            return Encoding.UTF8.GetBytes(json);
        }
        // public void WriteBase<T>(T data)
        // {
        //     switch (data)
        //     {
        //         case User user:
        //             user.Id = GenerateUniqueId();
        //             Users.Add(user);
        //             byte[] userDataBytes = WriteBaseToJson(user);
        //             int userPosition = GetNextFreePosition(userDataBytes.Length);
        //             // PositionDictionary.Add(user.Id, userPosition);
        //             var tmpheader = new HeaderString();
        //             tmpheader.Type = "User";
        //             tmpheader.Id = user.Id;
        //             tmpheader.Id = user.Id;
        //             HeaderNames.Add(new HeaderString("User" + ' ' + user.Id + ' ' + userPosition + ' ' + userDataBytes.Length + ' '));


        //             userHeader);

        //             FileStream.Position = userPosition;
        //             FileStream.Write(userDataBytes);

        //             FileStream.Position = 0;
        //             FileStream.Write(Encoding.UTF8.GetBytes(string.Join("\n", HeaderNames)));

        //             break;

        //         case UserGroup userGroup:
        //             userGroup.Id = GenerateUniqueId();
        //             UserGroups.Add(userGroup);
        //             byte[] userGroupDataBytes = WriteBaseToJson(userGroup);
        //             int userGroupPosition = GetNextFreePosition(userGroupDataBytes.Length);
        //             // PositionDictionary.Add(userGroup.Id, userGroupPosition);

        //             string groupHeader = $"{typeof(UserGroup).Name} {userGroup.Id} {userGroupPosition} {userGroupDataBytes.Length}" + ' ';

        //             HeaderNames.Add(groupHeader);

        //             FileStream.Position = userGroupPosition;
        //             FileStream.Write(userGroupDataBytes);

        //             FileStream.Position = 0;
        //             FileStream.Write(Encoding.UTF8.GetBytes(string.Join("\n", HeaderNames)));
        //             break;

        //         case Session session:
        //             session.Id = GenerateUniqueId();
        //             Sessions.Add(session);
        //             byte[] sessionDataBytes = WriteBaseToJson(session);
        //             int sessionPosition = GetNextFreePosition(sessionDataBytes.Length);
        //             // PositionDictionary.Add(session.Id, sessionPosition);

        //             string sessionHeader = $"{typeof(Session).Name} {session.Id} {sessionPosition} {sessionDataBytes.Length}" + ' ';
        //             HeaderNames.Add(sessionHeader);

        //             FileStream.Position = sessionPosition;
        //             FileStream.Write(sessionDataBytes);

        //             FileStream.Position = 0;
        //             FileStream.Write(Encoding.UTF8.GetBytes(string.Join("\n", HeaderNames)));
        //             break;

        //         case ActionHistory actionHistory:
        //             actionHistory.Id = GenerateUniqueId();
        //             ActionHistories.Add(actionHistory);
        //             byte[] actionHistoryDataBytes = WriteBaseToJson(actionHistory);
        //             int actionHistoryPosition = GetNextFreePosition(actionHistoryDataBytes.Length);
        //             // PositionDictionary.Add(actionHistory.Id, actionHistoryPosition);

        //             string actionHistoryHeader = $"{typeof(ActionHistory).Name} {actionHistory.Id} {actionHistoryPosition} {actionHistoryDataBytes.Length}" + ' ';
        //             HeaderNames.Add(actionHistoryHeader);

        //             FileStream.Position = actionHistoryPosition;
        //             FileStream.Write(actionHistoryDataBytes);

        //             FileStream.Position = 0;
        //             FileStream.Write(Encoding.UTF8.GetBytes(string.Join("\n", HeaderNames)));
        //             break;

        //         case DeviceConnection deviceConnection:

        //             DeviceConnections.Add(deviceConnection);
        //             byte[] deviceConnectionDataBytes = WriteBaseToJson(deviceConnection);
        //             int deviceConnectionPosition = GetNextFreePosition(deviceConnectionDataBytes.Length);
        //             // PositionDictionary.Add(deviceConnection.Name, deviceConnectionPosition);

        //             string deviceConnectionHeader = $"{typeof(DeviceConnection).Name} {deviceConnection.Name} {deviceConnectionPosition} {deviceConnectionDataBytes.Length}" + ' ';
        //             HeaderNames.Add(deviceConnectionHeader);

        //             FileStream.Position = deviceConnectionPosition;
        //             FileStream.Write(deviceConnectionDataBytes);

        //             FileStream.Position = 0;
        //             FileStream.Write(Encoding.UTF8.GetBytes(string.Join("\n", HeaderNames)));
        //             break;

        //         case WorkPlace workPlace:
        //             WorkPlaces.Add(workPlace);
        //             byte[] workPlaceDataBytes = WriteBaseToJson(workPlace);
        //             int workPlacePosition = GetNextFreePosition(workPlaceDataBytes.Length);
        //             // PositionDictionary.Add(workPlace.Id, workPlacePosition);

        //             string workPlaceHeader = $"{typeof(WorkPlace).Name} {workPlace.Id} {workPlacePosition} {workPlaceDataBytes.Length}" + ' ';
        //             HeaderNames.Add(workPlaceHeader);

        //             FileStream.Position = workPlacePosition;
        //             FileStream.Write(workPlaceDataBytes);

        //             FileStream.Position = 0;
        //             FileStream.Write(Encoding.UTF8.GetBytes(string.Join("\n", HeaderNames)));
        //             break;

        //         default:
        //             throw new ArgumentException("Unsupported type.");
        //     }
        //     FileStream.Flush();
        // }

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
                    if (HeaderNames[shag].Id == sb)
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
        public string ReadCommand(FireArm_API_Command GetCommnad)
        {
            switch (GetCommnad.Metode)
            {
                case "GET":
                    return ReadDataJson(GetCommnad.ID);
                case "SET":
                    var HeaderTemp = GetHeaderString(GetCommnad.ID);
                    if (HeaderTemp == null)
                    {
                        HeaderTemp = new HeaderString();
                        HeaderTemp.Id = GenerateUniqueId();
                        HeaderTemp.startindex = (int)FileStream.Length + 1;
                        HeaderTemp.Type = GetCommnad.Type;
                        HeaderTemp.Size = GetCommnad.BytesWrite.Length;
                        HeaderNames.Add(HeaderTemp);
                        SetData(HeaderTemp, GetCommnad.BytesWrite);
                        return HeaderTemp.Id;
                    }
                    // else
                    // {
                    //     HeaderTemp.Size = GetCommnad.BytesWrite.Length;
                    // }
                    return SetData(HeaderTemp, GetCommnad.BytesWrite);
                case "DEL":
                    return DelHeader(GetCommnad.ID).ToString();

            }
            return null;
        }

        public bool DelHeader(string GetId)
        {
            for (int shag = 0; shag <= HeaderNames.Count - 1; shag++)
            {
                if (HeaderNames[shag].Id == GetId)
                {
                    HeaderNames.RemoveRange(shag, 1);
                    return true;
                }
            }
            return false;
        }
        public HeaderString GetHeaderString(string GetId)
        {
            if (HeaderNames.Count == 0)
            {
                return null;
            }
            else
            {
                for (int shag = 0; shag <= HeaderNames.Count - 1; shag++)
                {
                    if (HeaderNames[shag].Id == GetId)
                    {
                        return HeaderNames[shag];
                    }
                }
            }
            return null;

        }

        public byte[] GetBytesHeader()
        {
            List<string> listStringsConver = new List<string>();
            for (int shag = 0; shag <= HeaderNames.Count - 1; shag++)
            {
                listStringsConver.Add(HeaderNames[shag].GetString());
            }
            return Encoding.UTF8.GetBytes(string.Join('\n', listStringsConver.ToArray()));
        }
        public string SetData(HeaderString GetHeader, byte[] DataWrite)
        {


            if (GetHeader.Size >= DataWrite.Length)
            {
                FileStream.Position = GetHeader.startindex;
                FileStream.Write(new ReadOnlySpan<byte>(DataWrite));
            }
            else
            {
                DelHeader(GetHeader.Id);
                GetHeader.startindex = GetNextFreePosition(DataWrite.Length);
                GetHeader.Size = DataWrite.Length;
                HeaderNames.Add(GetHeader);
                FileStream.Position = GetHeader.startindex;
                FileStream.Write(new ReadOnlySpan<byte>(DataWrite));
            }
            FileStream.Position = 0;
            FileStream.Write(new ReadOnlySpan<byte>(GetBytesHeader()));
            FileStream.Flush();
            return "ok";
        }
    }
}

