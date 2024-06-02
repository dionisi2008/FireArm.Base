// See https://aka.ms/new-console-template for more information
using CSO;

Console.WriteLine(DateTime.Now.ToString());
Console.WriteLine("Hello, World!");
var test1 = new Storage(@"D:\default_filename.txt");
for (int i = 0; i <= 10; i++)
{
    test1.ReadCommand("GET" + '\n' + "oHhAfqFdqMcr3ao8");
    Console.WriteLine(i + " " + DateTime.Now.ToString());
}
test1.ReadCommand("GET" + '\n' + "oHhAfqFdqMcr3ao8");
Console.WriteLine(DateTime.Now.ToString());

test1.FileStream.Close();
File.Delete("default_filename.txt");

