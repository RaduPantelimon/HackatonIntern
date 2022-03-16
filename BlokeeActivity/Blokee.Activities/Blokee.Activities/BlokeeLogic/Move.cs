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

        public static double PieceSizeConstant = 1.75;
        public static double ZonesOfInfluenceConstant = 0.6;
        public static double CornerConstant = 2.2;
        
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
        public double Score { get; set; }

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
            int playerId = game.NextPlayer;
            int[] currentScores = game.GetCurrentScores();
            int enemyScoreTotal = currentScores.Sum() - currentScores[playerId];
            game.PlayMove(this);
            //File.WriteAllText("testBoard.txt", game.Board.ToString());

            double MoveScore = PieceSizeConstant * this.Player.Pieces[this.PieceId].Points.Length;
            try
            {
                //we will compose a score that should roughly tell us how good a move is
                int piecesAvailable = this.Player.Pieces.Where(piece => piece.IsAvailable == true).Count();
                int[] zonesOfInFluence = GetZonesOfInfluence(game);
                MoveScore += piecesAvailable * ZonesOfInfluenceConstant * ((double)zonesOfInFluence[playerId] / zonesOfInFluence.Sum());

                //next, we will compose a score based on the situation of the corners on the board
                double playerCornerScore = 0, enemyCornerScore = 0;
                foreach (Player p in game.Players)
                {
                    if(p.Id == playerId)
                        playerCornerScore = GetCornersScores(game, p);
                    else
                    {
                        enemyCornerScore += GetCornersScores(game, p) * currentScores[p.Id] / enemyScoreTotal;
                    }
                }
                MoveScore += (playerCornerScore - enemyCornerScore) * CornerConstant;

            }
            catch (Exception e)
            {

                Console.WriteLine("Found exception while playing game: " + e.ToString());
                throw;
            }
            finally
            {
                game.UndoMove(this);
            }
            return MoveScore;
        }

        public static double GetCornersScores(Game game, Player player)
        {
            //first we play the move
            //to Asses how it would divide the board into "areas of influence" for the players
            double score = 0;
            Board densityBoard = new Board(); //(int?[,])game.Board._board.Clone();
            densityBoard._board.FillArray(0);
            int[][] corners = game.Board.GetAllAvailableCorners(player.Id);
            Piece[] availablePieces = player.Pieces.Where(piece => piece.IsAvailable == true).ToArray();

            //let's see exactly how many pieces we can fit in each workflow
            foreach (var corner in corners)
            {
                int piecesCounter = 0;
                //create a new board that will keep track how this corner can spread
                Board dummyBoard = new Board();

                foreach (var piece in availablePieces)
                {
                    bool pieceFits = false;
                    foreach (var orientation in piece.Orientations)
                    {
                        foreach (var point in piece.AllVariations[orientation])
                        {
                            Move currentMove = new Move(player, piece, orientation, corner[0], corner[1], point[0], point[1]);
                            if (game.Board.IsLegalMove(currentMove))
                            {
                                //validMoves.Add(currentMove);
                                pieceFits = true;
                                dummyBoard.MakeMove(currentMove);
                                piecesCounter++;
                                break;
                            }
                        }
                        if (pieceFits) break;
                    }
                }

                //take all of the points on the dummy board and use them to fill the density matrix
                int spread = 0; double density = 0;
                for(int i=0; i<Board.rowCount; i++)
                {
                    for (int j = 0; j < Board.colCount; j++)
                    {
                        if (dummyBoard._board[i, j] != null)
                        {
                            densityBoard._board[i, j]++;
                            density += densityBoard._board[i, j].Value;
                            spread++;
                        }
                    }
                }
                if(piecesCounter>0)
                    score += (piecesCounter * spread) / (availablePieces.Length * density * density);
            }
            //File.WriteAllText("testCornersTempBoard.txt", densityBoard.ToString());
            return score;
        }


        public int[] GetZonesOfInfluence(Game game)
        {
            //first we play the move
            //to Asses how it would divide the board into "areas of influence" for the players
           
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
                //File.WriteAllText("testTempBoard.txt", new Board(tempBoard).ToString());
                return GetZonesOfInfluenceScore(tempBoard);
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