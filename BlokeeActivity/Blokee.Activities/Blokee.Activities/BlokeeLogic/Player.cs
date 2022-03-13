﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Blokee
{
    public enum DifficultyLevel
    {
        Basic,
        Intermediate,
    }

    public class Player
    {
        public int Id { get; set; }
        public Piece[] Pieces { get; set; }
        public int score = 0;
        public int currentMoveCount = 0;
        private readonly DifficultyLevel Difficulty;

        public Player(int Id, bool[] availability, DifficultyLevel difficulty = DifficultyLevel.Basic)
        {
            this.Id = Id;
            this.Difficulty = difficulty;
            availability = availability ?? Enumerable.Repeat(true,21).ToArray();
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

        //copy constructor
        public Player(Player originalPlayer)
        {
            this.Id = originalPlayer.Id;
            this.Difficulty = originalPlayer.Difficulty;
        }

        private Move GetBarosanaMove(bool getAndPLay = true)
        {
            int pieceIndex, orientation, row, col, piecePointRow = 0, piecePointColumn = 0;
            if (currentMoveCount == 0)
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
                        row = 19;
                        col = 19;
                        piecePointRow = 2;
                        piecePointColumn = 2;
                        break;
                    default:
                        orientation = 1;
                        row = 0;
                        col = 19;
                        piecePointRow = 0;
                        piecePointColumn = 2;
                        break;
                }
            }
            else if (currentMoveCount == 1)
            {
                pieceIndex = 7;
                orientation = 0;
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
                        piecePointRow = 2;
                        piecePointColumn = 1;
                        break;
                    case 2:
                        row = 17;
                        col = 16;
                        piecePointRow = 2;
                        piecePointColumn = 1;
                        break;
                    default:
                        row = 3;
                        col = 17;
                        piecePointRow = 1;
                        piecePointColumn = 2;
                        break;
                }
            }
            else if (currentMoveCount == 2)
            {
                pieceIndex = 10;
                switch (Id)
                {
                    case 0:
                        orientation = 1;
                        row = 4; col = 5;
                        piecePointRow = 0;
                        piecePointColumn = 1;
                        break;
                    case 1:
                        orientation = 0;
                        row = 15; col = 5;
                        piecePointRow = 2;
                        piecePointColumn = 1;
                        break;
                    case 2:
                        orientation = 3;
                        row = 15;
                        col = 14;
                        piecePointRow = 2;
                        piecePointColumn = 1;
                        break;
                    default:
                        orientation = 2;
                        row = 4;
                        col = 14;
                        piecePointRow = 0;
                        piecePointColumn = 1;
                        break;
                }
            }
            else
            {
                switch (Id)
                {
                    case 0:
                        pieceIndex = 2;
                        orientation = 7;
                        row = 5;
                        col = 7;
                        piecePointRow = 0;
                        piecePointColumn = 1;
                        break;
                    case 1:
                        pieceIndex = 13;
                        orientation = 5;
                        row = 13;
                        col = 6;
                        piecePointRow = 3;
                        piecePointColumn = 1;
                        break;
                    case 2:
                        pieceIndex = 2;
                        orientation = 0;
                        row = 13;
                        col = 13;
                        piecePointRow = 1;
                        piecePointColumn = 2;
                        break;
                    default:
                        pieceIndex = 13;
                        orientation = 0;
                        row = 5;
                        col = 12;
                        piecePointRow = 0;
                        piecePointColumn = 2;
                        break;

                }
            }
            //if we also want to play the move asap
            if(getAndPLay)this.Pieces[pieceIndex].IsAvailable = false;
            return new Move(this, this.Pieces[pieceIndex], orientation, row, col, piecePointRow, piecePointColumn); //{ this.Pieces[pieceIndex].Id, orientation, row, col };
        }

        public List<Move> GetValidMoves(Board board, Piece piece, int[][] corners)
        {
            List<Move> validMoves = new List<Move>();

            foreach (var orientation in piece.Orientations)
            {
                foreach (var corner in corners)
                {
                    foreach (var point in piece.AllVariations[orientation])
                    {
                        Move currentMove = new Move(this, piece, orientation, corner[0], corner[1], point[0], point[1]);
                        if (board.IsLegalMove(currentMove)) { validMoves.Add(currentMove); }

                    }
                }
            }

            return validMoves;
        }

        private Move GetGreedyMove(Board board, bool getAndPLay = true)
        {
            var availablePieces = this.Pieces.Where(piece => piece.IsAvailable).OrderByDescending(piece => piece.Weight);
            var availableCorners = board.GetAllAvailableCorners(this.Id);
            List<Move> moves = new List<Move>();

            foreach (var piece in availablePieces)
            {
                moves = GetValidMoves(board, piece, availableCorners);
                if (moves.Any()) { break; }
            }

            if (moves.Any())
            {
                Move selectedMove = moves[new Random().Next(moves.Count)];
                if(getAndPLay) selectedMove.Player.Pieces[selectedMove.PieceId].IsAvailable = false;
                return selectedMove;
            }
            return null;
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

        public Move GetMove(Board board, bool getAndPLay = true)
        {
            currentMoveCount = Pieces.Where(piece => piece.IsAvailable ==false).Count();
            if (currentMoveCount < 4)
            {
                return GetBarosanaMove();
            }

            if (Difficulty == DifficultyLevel.Basic) return GetGreedyMove(board, getAndPLay);
            return null;
        }

        public bool[] GetPieceAvailability()
        {
            return this.Pieces.Select(piece => piece.IsAvailable).ToArray();
        }
    }
}
