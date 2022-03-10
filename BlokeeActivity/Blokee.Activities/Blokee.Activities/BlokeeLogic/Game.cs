using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blokee
{
    public class Game
    {
        public const int playerCount = 4;
        private int _nextPlayer;
        private Player[] _players;

        public Game(JObject gameProperties, int nextPlayer) {
            //determine who's turn is next
            if (nextPlayer >= playerCount || nextPlayer < 0)
                throw new Exception("Next player is invalid. It needs to be a number between 0 and the total number of players");
            _nextPlayer = nextPlayer;

            //init board based on the game Properties received
            Board board = Board.I;
            board.RefreshBoard(gameProperties["gameBoard"].ToString());

            //init players
            _players = new Player[playerCount];
            Enumerable.Range(0, playerCount)
                .ToList()
                .ForEach(
                index =>
                {
                    bool[] pieceAvailability = Enumerable.Repeat(true, 21).ToArray();
                    _players[index] = new Player(index, pieceAvailability);
                });
        }
    }
}
