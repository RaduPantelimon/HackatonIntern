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

        public Game(Board b, int nextPlayer, Player[] players)
        {
            this.Board = b;
            this.NextPlayer = nextPlayer;
            this.Players = players;
        }

        public Game(JObject gameProperties, int nextPlayer, String gameMode) {
            //determine who's turn is next
            if (nextPlayer >= playerCount || nextPlayer < 0)
                throw new Exception("Next player is invalid. It needs to be a number between 0 and the total number of players");
            NextPlayer = nextPlayer;

            //init board based on the game Properties received
            this.Board = new Board();
            Board.RefreshBoard(gameProperties["gameBoard"].ToString());
            
            DifficultyLevel dl = DifficultyLevel.Advanced;
            switch(gameMode)
            {
                case "greedy":
                    dl = DifficultyLevel.Basic;
                    break;
                case "minimax" :
                    dl = DifficultyLevel.Advanced;
                    break;
                default:
                    dl = DifficultyLevel.Intermediate;
                    break;
            }
            Console.WriteLine("Difficulty: " + gameMode);

            //init players
            Players = new Player[playerCount];
            Enumerable.Range(0, playerCount).ToList()
                .ForEach(
                index =>
                {   
                    bool[] pieceAvailability = Enumerable.Repeat(true, 21).ToArray();
                    gameProperties["enemyMoves"][index].ToList().ForEach(id => pieceAvailability[(int)id] = false);
                    Players[index] = new Player(index, pieceAvailability, dl);
                    
                });
        }

        public void PlayMove(Move move)
        {
            this.Board.MakeMove(move);
            move.Player.Pieces[move.PieceId].IsAvailable = false;
            NextPlayer = (NextPlayer + 1) % playerCount;
        }

        public void UndoMove(Move move)
        {
            this.Board.UndoMove(move);
            move.Player.Pieces[move.PieceId].IsAvailable = true;
            NextPlayer = (NextPlayer - 1 + playerCount) % playerCount;
        }

        public Move PlayNextMove()
        {
            if (this.IsOver) throw new Exception("Game is over, no further moves possible");
            Move move = Players[NextPlayer].GetMove(this, false);

            //if move is valid update board and player
            if(move != null)
            {
                PlayMove(move);
                _consecutiveNoValidMoves = 0;
            }
            else
            {
                NextPlayer++;
                _consecutiveNoValidMoves++;
                if (_consecutiveNoValidMoves == playerCount) this.IsOver = true;
            }
            return move;
        }

        public int[] GetCurrentScores(int?[,] _board = null)
        {
            _board = (_board == null) ? Board._board : _board;
            int[] scores = new int[Game.playerCount];
            for (int i = 0; i < Board.rowCount; i++)
                for (int j = 0; j < Board.colCount; j++)
                {
                    if (_board[i, j] != null && _board[i, j] != -1)
                    {
                        scores[_board[i, j].Value]++;
                    }
                }
            return scores;
        }
    }
}
