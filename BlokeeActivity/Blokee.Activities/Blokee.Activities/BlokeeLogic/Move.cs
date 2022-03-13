using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blokee
{

    

    public class Move
    {

        public static int PieceSizeConstant = 2;
        public static int ZonesOfInfluenceConstant = 1;

        public int CornerRow { get; set; }
        public int CornerColumn { get; set; }

        public int PiecePointRow { get; set; }
        public int PiecePointColumn { get; set; }

        public int PlacingRow
        {
            get
            {
                return CornerRow - PiecePointRow;
            }
        }

        public int PlacingColumn
        {
            get
            {
                return CornerColumn - PiecePointColumn;
            }
        }

        public int PieceId { get; set; }
        public int Orientation { get; set; }

        public Player Player { get; set; }
        public int Score { get; set; }

        public Move(Player _player, Piece _piece, int _orientation, int _cornerRow, int _cornerColumn, int _piecePointRow, int _piecePointColumn)
        {
            Player = _player;
            CornerRow = _cornerRow;
            CornerColumn = _cornerColumn;
            PiecePointRow = _piecePointRow;
            PiecePointColumn = _piecePointColumn;
            PieceId = _piece.Id;
            Orientation = _orientation;
            Score = 0;
        }

        public Move(Player _player, int _pieceId, int _orientation, int _cornerRow, int _cornerColumn, int _piecePointRow, int _piecePointColumn)
        {
            Player = _player;
            CornerRow = _cornerRow;
            CornerColumn = _cornerColumn;
            PiecePointRow = _piecePointRow;
            PiecePointColumn = _piecePointColumn;
            PieceId = _pieceId;
            Orientation = _orientation;
            Score = 0;
        }

        //this will help us with the debugging 
        public override string ToString()
        {
            return String.Format("CornerRow:{0}; CornerColumn:{1}; Piece:{2}; Orientation:{3}; PiecePointRow: {4}; PiecePointColumn: {5}; PlayerID: {6}",
                CornerRow,
                CornerColumn,
                PieceId,
                Orientation,
                PiecePointRow,
                PiecePointColumn,
                Player.Id);
        }

        public double GetMoveScore(Game game)
        {
            double MoveScore;

            MoveScore = PieceSizeConstant * this.Player.Pieces[this.PieceId].Points.Length;

            //we will compose a score that should roughly tell us how good a move is
            int piecesAvailable = this.Player.Pieces.Where(piece => piece.IsAvailable == true).Count();
            int[] zonesOfInFluence = GetZonesOfInfluence(game);
            MoveScore += piecesAvailable / 2 * ZonesOfInfluenceConstant * ((double)zonesOfInFluence[game.NextPlayer] / zonesOfInFluence.Sum());
               
            return MoveScore;
        }

        public int[] GetZonesOfInfluence(Game game)
        {
            //first we play the move
            //to Asses how it would divide the board into "areas of influence" for the players
            game.PlayMove(this);
            File.WriteAllText("testBoard.txt", game.Board.ToString());

            try
            {
                int?[,] tempBoard = (int?[,])game.Board._board.Clone();
                int currentPlayer = game.NextPlayer;
                //adding all corners in a queue, from there we will expand the influence zone for each player
                Queue<int[]> pointsQueue = new Queue<int[]>();
                do{
                    int[][] corners = game.Board.GetAllAvailableCorners(currentPlayer);
                    foreach(var corner in corners)
                        //ignore a corner if a player before already "claimed" it
                        if (tempBoard[corner[0], corner[1]] == null || tempBoard[corner[0], corner[1]] < Game.playerCount)
                        {
                            tempBoard[corner[0], corner[1]] = currentPlayer + Game.playerCount;
                            pointsQueue.Enqueue(new int[] { corner[0], corner[1], currentPlayer + Game.playerCount });
                        }
                    //next, do the same for the next player;
                    currentPlayer = (currentPlayer + 1) % Game.playerCount;
                }while (currentPlayer != game.NextPlayer);

                while (pointsQueue.Count > 0)
                {
                    int[] point = pointsQueue.Dequeue();
                    foreach (var direction in Board.directions)
                    {
                        //check if the current player can advance in the following direction
                        int[] newPoint = new int[] { point[0] + direction[0], point[1] + direction[1], point[2] };
                        if (game.Board.IsValidCorner(newPoint[0], newPoint[1], newPoint[2] % Game.playerCount) &&
                            (tempBoard[newPoint[0], newPoint[1]] == -1 || tempBoard[newPoint[0], newPoint[1]] == null))
                        {
                            tempBoard[newPoint[0], newPoint[1]] = newPoint[2];
                            pointsQueue.Enqueue(newPoint);
                        }
                    }
                }
                File.WriteAllText("testTempBoard.txt", new Board(tempBoard).ToString());
                return GetZonesOfInfluenceScore(tempBoard);
            }
            catch(Exception e)
            {

                Console.WriteLine("Found exception while playing game: " + e.ToString());
                throw;
            }
            finally
            {
                game.UndoMove(this);
            }
        }

        public int[] GetZonesOfInfluenceScore(int?[,] _board = null)
        {
           int[] scores = new int[Game.playerCount];
            for (int i = 0; i < Board.rowCount; i++)
                for (int j = 0; j < Board.colCount; j++)
                {
                    if (_board[i, j] != null && _board[i, j] >= Game.playerCount)
                    {
                        scores[_board[i, j].Value % Game.playerCount]++;
                    }
                }
            return scores;
        }

    }
}