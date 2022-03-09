using System.Collections.Generic;
using System.Linq;

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

        public void PlayPiece(int targetRow, int targetColumn, Piece piece, int translation, int playerId)
        {
            var piecePosition = piece.AllVariations[translation];
            foreach (var point in piecePosition) { _board[targetRow + point[0], targetColumn + point[1]] = playerId; };
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
            return piece.AllVariations[orientation].Any(point => PointIsAdjacent(point[0], point[1], playerId));
        }

        public bool IsLegalMove(int targetRow, int targetColumn, Piece piece, int orientation, int playerId)
        {
            return PieceIsOutOfBounds(targetRow, targetColumn, piece, orientation) &&
                PieceOverlap(targetRow, targetColumn, piece, orientation) &&
                PieceIsAdjacent(targetRow, targetColumn, piece, orientation, playerId);
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
