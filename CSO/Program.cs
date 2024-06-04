using System;
using System.Text;

namespace CSO // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString());
            Console.WriteLine("Hello, World!");
            var test1 = new Storage();
            
            var t = "sd";
            
            test1.FileStream.Close();
            
            //File.Delete("default_filename.txt");
        }
    }
}



