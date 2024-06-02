using System;

namespace CSO
{
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

        public string GetString()
        {
            return Type + ' ' + Id + ' ' + startindex + ' ' + Size;
        }

    }
}
