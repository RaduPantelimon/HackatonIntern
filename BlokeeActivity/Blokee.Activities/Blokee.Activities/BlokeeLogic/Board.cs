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
        public static readonly int[][] directions = new int[][] {
            new int[] {0, 1},
            new int[] {0, -1},
            new int[] {1, 0},
            new int[] {-1, 0},
            new int[] {-1, -1},
            new int[] {-1, 1},
            new int[] {1, -1},
            new int[] {1, 1}};

        public int?[,] _board;

        public Board()
        {
            _board = new int?[rowCount, colCount];
        }

        public Board(string inputJson)
        {
            RefreshBoard(inputJson);
        }
        public Board(int?[,] board)
        {
            _board = (int?[,])board.Clone();
        }

        // Copy Constructor
        public Board(Board originalBoard)
        {
            this._board = (int?[,])originalBoard._board.Clone();
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
            //Console.WriteLine("Playing Move: " + move.ToString());
            var piecePosition = move.Player.Pieces[move.PieceId].AllVariations[move.Orientation];
            foreach (var point in piecePosition)
                _board[move.PlacingRow + point[0], move.PlacingColumn + point[1]] = move.Player.Id;
        }
        public void UndoMove(Move move)
        {
            //Console.WriteLine("Undoing Move: " + move.ToString());
            var piecePosition = move.Player.Pieces[move.PieceId].AllVariations[move.Orientation];
            foreach (var point in piecePosition)
                _board[move.PlacingRow + point[0], move.PlacingColumn + point[1]] = null;
        }


        public bool PointIsOutOfBounds(int row, int col)
        {
            return !(0 <= row && row < rowCount && 0 <= col && col < colCount);
        }

        public bool PointIsAdjacent(int row, int col, int playerId)
        {
            return (row + 1 < rowCount && _board[row + 1, col] == playerId) ||
                   (row - 1 >= 0 && _board[row - 1, col] == playerId) ||
                   (col + 1 < colCount && _board[row, col + 1] == playerId) ||
                   (col - 1 >= 0 && _board[row, col - 1] == playerId);
        }

        private bool PointOverlap(int row, int col)
        {
            return //!(0<= row && row<rowCount && 0< col && col<= colCount) ||
                (_board[row, col] != null && _board[row, col] != -1);
        }

        public bool IsValidCorner(int row, int col, int playerId)
        {
            return !PointIsOutOfBounds(row, col) &&
                   !PointIsAdjacent(row, col, playerId) &&
                   !PointOverlap(row, col);
        }

        public bool PieceIsOutOfBounds(int targetRow, int targetColumn, Piece piece, int orientation)
        {
            //Console.WriteLine("Piece out of bounds check. target row: {0}; target col: {1}; piece: {2}; orientation: {3}", targetRow, targetColumn, piece.Id, orientation);
            if (PointIsOutOfBounds(targetRow, targetColumn)) { return true; }

            return piece.AllVariations[orientation].Any(point => PointIsOutOfBounds(targetRow + point[0], targetColumn + point[1]));
        }

        public bool PieceOverlap(int targetRow, int targetColumn, Piece piece, int orientation)
        {
            //Console.WriteLine("Piece overlap check. target row: {0}; target col: {1}; piece: {2}; orientation: {3}", targetRow, targetColumn, piece.Id, orientation);
            return piece.AllVariations[orientation].Any(point => PointOverlap(targetRow + point[0], targetColumn + point[1]));
        }

        public bool PieceIsAdjacent(int targetRow, int targetColumn, Piece piece, int orientation, int playerId)
        {
            //Console.WriteLine("Piece is adjacent check. target row: {0}; target col: {1}; piece: {2}; orientation: {3}", targetRow, targetColumn, piece.Id, orientation);
            return piece.AllVariations[orientation].Any(point => PointIsAdjacent(targetRow + point[0], targetColumn + point[1], playerId));
        }

        public bool IsLegalMove(Move move)
            /*int targetRow, int targetColumn, Piece piece, int orientation, int playerId*/
        {
            //Console.WriteLine("Checking Move: " + move.ToString());
            /*bool oob = PieceIsOutOfBounds(move.PlacingRow, move.PlacingColumn, move.Piece, move.Orientation);
            Console.WriteLine("PieceIsOutOfBounds: {0}", oob);
            if (oob == false) return false; 
            bool overlap = PieceOverlap(move.PlacingRow, move.PlacingColumn, move.Piece, move.Orientation);
            Console.WriteLine("PieceOverlap: {0}", overlap);
            if (overlap == false) return false;
            bool adjacent = PieceIsAdjacent(move.PlacingRow, move.PlacingColumn, move.Piece, move.Orientation, move.Player.Id);
            Console.WriteLine("PieceIsAdjacent: {0}", adjacent);
            if (adjacent == false) return false;
            return true;*/
            return !PieceIsOutOfBounds(move.PlacingRow, move.PlacingColumn, move.Player.Pieces[move.PieceId], move.Orientation) &&
                !PieceOverlap(move.PlacingRow, move.PlacingColumn, move.Player.Pieces[move.PieceId], move.Orientation) &&
                !PieceIsAdjacent(move.PlacingRow, move.PlacingColumn, move.Player.Pieces[move.PieceId], move.Orientation, move.Player.Id);
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

    }
}
