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

            //for debuging
            int moveIdx= 0;

            //init a new game
            Game game = new Game(gameProperties, 0, "greedy-advanced");//"minimax");
            //play moves until game is over
            while(!game.IsOver)
            {
                Move move = game.PlayNextMove();
                //this is just to start debuging when you're fairly late in the game
                if (moveIdx>16)
                {
                    Console.WriteLine("Barasona Over!!!");
                }
                if(move!= null) Console.WriteLine("Playing Move: " + move.ToString());
                Console.WriteLine(Environment.NewLine + game.Board.ToString());
                /*
                if (move!= null)
                {
                    ///TEST
                    //game.UndoMove(move);
                    //Console.WriteLine(Environment.NewLine + game.Board.ToString());
                    //game.PlayMove(move);
                    //Console.WriteLine(Environment.NewLine + game.Board.ToString());
                    ///TEST
                }
                */

                moveIdx++;
            }
        }
    }
}
