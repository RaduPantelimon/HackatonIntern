using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blokee;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read the file as one string.
            string text = System.IO.File.ReadAllText(@"test resources\latestValues.txt");
            System.Console.WriteLine("Contents of WriteText.txt = {0}", text);
            JObject gameProperties = JObject.Parse(text);

            //init a new game
            Game game = new Game(gameProperties,0);

        }
    }
}
