using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace Blokee
{
    public class Board
    {
        public const int rowCount = 20;
        public const int colCount = 20;

        private int?[,] _board;

        // not using threads, not giving a crap about thread safety :)
        private static Board _instance;
        public static Board I
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Board();
                }
                return _instance;
            }
        }

        private Board()
        {
            _board = new int?[rowCount, colCount];
        }

        // Hope this works :D 
        public void RefreshBoard(string inputJson)
        {
            _board = Newtonsoft.Json.JsonConvert.DeserializeObject<int?[,]>(inputJson);
        }

        //this will help us better output/visualize the board at runtime
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            //build visual representation
            for (int i = 0; i < _board.GetLength(0); i++)
            {
                for (int j = 0; j < _board.GetLength(1); j++)
                    builder.AppendFormat("{0} ", _board[i, j] == null ? "_" : _board[i, j].ToString());
                builder.Append(Environment.NewLine);
            }   
            return builder.ToString();
        }

        public void MakeMove(Move move)
        {
            var piecePosition = move.Piece.AllVariations[move.Orientation];
            foreach (var point in piecePosition)
                _board[move.CornerRow - move.PiecePointRow + point[0], move.CornerColumn - move.PiecePointColumn + point[1]] = move.Player.Id;
        }

        private bool PointIsOutOfBounds(int row, int col)
        {
            return !(0 <= row && row < rowCount && 0 <= col && col < colCount);
        }

        private bool PointIsAdjacent(int row, int col, int playerId)
        {
            return (_board[row + 1, col] == playerId) ||
                   (_board[row - 1, col] == playerId) ||
                   (_board[row, col + 1] == playerId) ||
                   (_board[row, col - 1] == playerId);
        }

        private bool PointOverlap(int row, int col)
        {
            return _board[row, col] != null && _board[row, col] != -1;
        }

        private bool IsValidCorner(int row, int col, int playerId)
        {
            return !PointIsOutOfBounds(row, col) &&
                   !PointIsAdjacent(row, col, playerId) &&
                   !PointOverlap(row, col);
        }

        public bool PieceIsOutOfBounds(int targetRow, int targetColumn, Piece piece, int orientation)
        {
            if (!PointIsOutOfBounds(targetRow, targetColumn)) { return false; }

            return piece.AllVariations[orientation].Any(point => PointIsOutOfBounds(targetRow + point[0], targetColumn + point[1]));
        }

        public bool PieceOverlap(int targetRow, int targetColumn, Piece piece, int orientation)
        {
            return piece.AllVariations[orientation].Any(point => PointOverlap(targetRow + point[0], targetColumn + point[1]));
        }

        public bool PieceIsAdjacent(int targetRow, int targetColumn, Piece piece, int orientation, int playerId)
        {
            return piece.AllVariations[orientation].Any(point => PointIsAdjacent(targetRow + point[0], targetRow + point[1], playerId));
        }

        public bool IsLegalMove(Move move)
            /*int targetRow, int targetColumn, Piece piece, int orientation, int playerId*/
        {
            return PieceIsOutOfBounds(move.CornerRow - move.PiecePointRow, move.CornerColumn - move.PiecePointColumn, move.Piece, move.Orientation) &&
                PieceOverlap(move.CornerRow - move.PiecePointRow, move.CornerColumn - move.PiecePointColumn, move.Piece, move.Orientation) &&
                PieceIsAdjacent(move.CornerRow - move.PiecePointRow, move.CornerColumn - move.PiecePointColumn, move.Piece, move.Orientation, move.Player.Id);
        }

        public int[][] GetAllAvailableCorners(int playerId)
        {
            List<int[]> corners = new List<int[]>();

            for (int i = 0; i < rowCount; i++)
                for (int j = 0; j < colCount; j++)
                {
                    if (_board[i, j] == playerId)
                    {
                        if (IsValidCorner(i - 1, j - 1, playerId)) { corners.Add(new int[] { i - 1, j - 1 }); }
                        if (IsValidCorner(i + 1, j - 1, playerId)) { corners.Add(new int[] { i + 1, j - 1 }); }
                        if (IsValidCorner(i - 1, j + 1, playerId)) { corners.Add(new int[] { i - 1, j + 1 }); }
                        if (IsValidCorner(i + 1, j + 1, playerId)) { corners.Add(new int[] { i + 1, j + 1 }); }
                    }
                }
            return corners.ToArray();
        }

        public int[] GetCurrentScores()
        {
            int[] scores = new int[Game.playerCount];
            for (int i = 0; i < rowCount; i++)
                for (int j = 0; j < colCount; j++)
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
