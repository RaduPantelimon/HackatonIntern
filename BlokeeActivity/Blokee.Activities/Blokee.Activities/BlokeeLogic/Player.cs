using System;
using System.Collections.Generic;
using System.Linq;

namespace Blokee
{
    public class Player
    {
        public int Id { get; set; }
        public Piece[] Pieces { get; set; }
        public int score = 0;
        public int currentMoveCount = 0;
        public Player(int Id, bool[] availability)
        {
            this.Id = Id;
            availability = availability ?? Enumerable.Repeat(true,20).ToArray();
            this.Pieces = new Piece[]
            {
                new Piece0(availability[0]),
                new Piece1(availability[1]),
                new Piece2(availability[2]),
                new Piece3(availability[3]),
                new Piece4(availability[4]),
                new Piece5(availability[5]),
                new Piece6(availability[6]),
                new Piece7(availability[7]),
                new Piece8(availability[8]),
                new Piece9(availability[9]),
                new Piece10(availability[10]),
                new Piece11(availability[11]),
                new Piece12(availability[12]),
                new Piece13(availability[13]),
                new Piece14(availability[14]),
                new Piece15(availability[15]),
                new Piece16(availability[16]),
                new Piece17(availability[17]),
                new Piece18(availability[18]),
                new Piece19(availability[19]),
                new Piece20(availability[20])
            };
        }

        private Move PlayBarasona()
        {
            int pieceIndex, orientation, row, col, piecePointRow = 0, piecePointColumn = 0;
            if (currentMoveCount == 1)
            {
                pieceIndex = 12;
                switch (Id)
                {
                    case 0:
                        orientation = 0;
                        row = col = 0;
                        piecePointRow = 0;
                        piecePointColumn = 0;
                        break;
                    case 1:
                        orientation = 3;
                        row = 19;
                        col = 0;
                        piecePointRow = 2;
                        piecePointColumn = 0;
                        break;
                    case 2:
                        orientation = 2;
                        row = 0;
                        col = 19;
                        piecePointRow = 2;
                        piecePointColumn = 2;
                        break;
                    default:
                        orientation = 1;
                        row = 19;
                        col = 19;
                        piecePointRow = 0;
                        piecePointColumn = 2;
                        break;
                }
            }
            else if (currentMoveCount == 2)
            {
                pieceIndex = 7;
                orientation = 1;
                switch (Id)
                {
                    case 0:
                        row = 3;
                        col = 2;
                        piecePointRow = 1;
                        piecePointColumn = 0;
                        break;
                    case 1:
                        row = 17;
                        col = 3;
                        piecePointRow = 1;
                        piecePointColumn = 2;
                        break;
                    case 2:
                        row = 17;
                        col = 16;
                        piecePointRow = 1;
                        piecePointColumn = 2;
                        break;
                    default:
                        row = 3;
                        col = 17;
                        piecePointRow = 1;
                        piecePointColumn = 2;
                        break;
                }
            }
            else if (currentMoveCount == 3)
            {
                pieceIndex = 11;
                switch (Id)
                {
                    case 0:
                        orientation = 1;
                        row = col = 5;
                        break;
                    case 1:
                        orientation = 2;
                        row = 5;
                        col = 14;
                        break;
                    case 2:
                        orientation = 3;
                        row = 14;
                        col = 5;
                        break;
                    default:
                        orientation = 4;
                        row = 14;
                        col = 14;
                        break;
                }
            }
            else
            {
                switch (Id)
                {
                    case 0:
                        pieceIndex = 3;
                        orientation = 1;
                        row = 6;
                        col = 7;
                        break;
                    case 1:
                        pieceIndex = 14;
                        orientation = 2;
                        row = 5;
                        col = 12;
                        break;
                    case 2:
                        pieceIndex = 14;
                        orientation = 3;
                        row = 12;
                        col = 5;
                        break;
                    default:
                        pieceIndex = 3;
                        orientation = 4;
                        row = 12;
                        col = 13;
                        break;
                }
            }
            this.Pieces[pieceIndex].IsAvailable = false;
            return new Move(this, this.Pieces[pieceIndex], orientation, row, col, piecePointRow, piecePointColumn); //{ this.Pieces[pieceIndex].Id, orientation, row, col };
        }

        private List<Move> GetValidMoves(Piece piece, int[][] corners)
        {
            List<Move> validMoves = new List<Move>();

            foreach (var orientation in piece.Orientations)
            {
                foreach (var corner in corners)
                {
                    foreach (var point in piece.AllVariations[orientation])
                    {
                        Move currentMove = new Move(this, piece, orientation, corner[0], corner[1], point[0], point[1]);
                        if (Board.I.IsLegalMove(currentMove/*corner[0], corner[1], piece, orientation, this.Id*/)) { validMoves.Add(currentMove); }

                    }
                }
            }

            return validMoves;
        }

        private Move PlayGreedy()
        {
            var availablePieces = this.Pieces.Where(piece => piece.IsAvailable).OrderByDescending(piece => piece.Weight);
            var availableCorners = Board.I.GetAllAvailableCorners(this.Id);
            List<Move> moves = new List<Move>();

            foreach (var piece in availablePieces)
            {
                moves = GetValidMoves(piece, availableCorners);
                if (moves.Any()) { piece.IsAvailable = false; break; }
            }

            return moves.Any() ? moves[new Random().Next(moves.Count)] : null;
        }

        //private int PlayGreedyAdvanced()
        //{
        //    var availablePieces = this.Pieces.Where(piece => piece.IsAvailable).OrderByDescending(piece => piece.Weight);
        //    var availableCorners = Board.I.GetAllAvailableCorners(this.Id);
        //    List<int[]> moves = new List<int[]>();

        //    foreach (var piece in availablePieces)
        //    {
        //        moves = GetValidMoves(piece, availableCorners);
        //        if (moves.Any()) { piece.IsAvailable = false; break; }
        //    }
        //}

        public Move Play()
        {
            currentMoveCount++;
            if (currentMoveCount <= 4)
            {
                return PlayBarasona();
            }

            return PlayGreedy();
        }

        public bool[] GetPieceAvailability()
        {
            return this.Pieces.Select(piece => piece.IsAvailable).ToArray();
        }
    }
}
