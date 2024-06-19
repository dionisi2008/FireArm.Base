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
            //var test1 = new Storage();
            var test2 = new FireArm_API_Server("127.0.0.1", 8085);
            var test3 = new FireArm_API_Client("ws://127.0.0.1:8085/");
            //test1.FileStream.Close();
            
            //File.Delete("default_filename.txt");
        }
    }
}



