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
        public bool IsOver;
        public Player[] Players { get; }
        public int NextPlayer;
        public Board Board { get; }

        private int _consecutiveNoValidMoves;

        
        //private Minimax minimaxStrategy;

        public Game(JObject gameProperties, int nextPlayer) {
            //determine who's turn is next
            if (nextPlayer >= playerCount || nextPlayer < 0)
                throw new Exception("Next player is invalid. It needs to be a number between 0 and the total number of players");
            NextPlayer = nextPlayer;

            //init board based on the game Properties received
            this.Board = new Board();
            Board.RefreshBoard(gameProperties["gameBoard"].ToString());

            //init players
            Players = new Player[playerCount];
            Enumerable.Range(0, playerCount).ToList()
                .ForEach(
                index =>
                {   
                    bool[] pieceAvailability = Enumerable.Repeat(true, 21).ToArray();
                    gameProperties["enemyMoves"][index].ToList().ForEach(id => pieceAvailability[(int)id] = false);
                    Players[index] = new Player(index, pieceAvailability);
                });

            //minimaxStrategy = new Minimax(Players(0), this);
        }

        public Move PlayNextMove()
        {
            if (this.IsOver) throw new Exception("Game is over, no further moves possible");
            Move move = Players[NextPlayer].Play(this.Board);

            //if move is valid update board and player
            if(move != null)
            {
                this.Board.MakeMove(move);// move[2], move[3], Players[NextPlayer].Pieces[move[0]], move[1], Players[NextPlayer].Id);
                _consecutiveNoValidMoves = 0;
            }
            else
            {
                _consecutiveNoValidMoves++;
                if (_consecutiveNoValidMoves == playerCount) this.IsOver = true;
            }
            

            NextPlayer = (NextPlayer + 1) % playerCount;
            return move;
        }

    }
}
