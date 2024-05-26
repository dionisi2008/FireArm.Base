// See https://aka.ms/new-console-template for more information
using CSO;

Console.WriteLine("Hello, World!");
var test1 = new Storage();
test1.FileStream.Close();
File.Delete("default_filename.txt");

