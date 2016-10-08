using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlToPox;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("XML: ");
                string userInput = ReadLine();
                if (String.IsNullOrEmpty(userInput)) continue;
                Console.WriteLine(Tools.Convert(userInput));
            }
        }

        private static string ReadLine()
        {
            Stream inputStream = Console.OpenStandardInput(65535);
            byte[] bytes = new byte[65535];
            int outputLength = inputStream.Read(bytes, 0, 65535);
            //Console.WriteLine(outputLength);
            char[] chars = Encoding.UTF7.GetChars(bytes, 0, outputLength);
            return new string(chars);
        }
    }
}
