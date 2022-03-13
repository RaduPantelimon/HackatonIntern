using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blokee
{
    public class Minimax
    {
        private Player currentPlayer;
        private Game game;
        private Board board;

        public Minimax(Player p, Game g)
        {
            this.currentPlayer = p;
            this.game = g;
            this.board = g.Board;
        }

        public Move GetMinimax(Board b)
        {
            this.board = b;
            return GetMinimax();
        }

        private Move GetMinimax()
        {
            var availableCorners = board.GetAllAvailableCorners(this.currentPlayer.Id);
            //get a copy of the current pieces
            var availablePieces = this.currentPlayer.Pieces.Where(piece => piece.IsAvailable).OrderByDescending(piece => piece.Weight);

            List<Move> allPossibleMoves = new List<Move>();

            //calculate the moves
            foreach (var piece in availablePieces)
            {
                allPossibleMoves.AddRange(this.currentPlayer.GetValidMoves(this.board, piece, availableCorners));
            }

            //calculate the scores
            foreach(var move in allPossibleMoves)
            {
                move.Score = this.evalMove(move, currentPlayer, this.board);
            }

            List<Move> finalMoves = new List<Move>();

            if (allPossibleMoves.Any())
            {
                //top 5 highest scores
                var top_by_score = allPossibleMoves.OrderByDescending(p => p.Score).Take(5);

                foreach (var move in top_by_score)
                {
                    var boardCopy = new Board(this.board);
                    var gameCopy = JsonConvert.DeserializeObject<Game>(JsonConvert.SerializeObject(this.game));
                    var playersCopy = gameCopy.Players;
                    var opponents = playersCopy.Where(p => p.Id != currentPlayer.Id);
                    var currentTestPlayer = new Player(this.currentPlayer);

                    boardCopy.MakeMove(move);
                    //remove the used piece from the options
                    currentTestPlayer.Pieces[move.PieceId].IsAvailable = false;

                    //update corners
                    var currentTestPlayeravailableCorners = boardCopy.GetAllAvailableCorners(currentTestPlayer.Id);

                    //update corners for opponents
                    var opponentCorners = new List<int[][]>();
                    foreach (var opponent in opponents)
                    {
                        opponentCorners.Add(boardCopy.GetAllAvailableCorners(opponent.Id));
                    }

                    //create a copy of the pieces that the current player has
                    var availableLocalPieces = currentTestPlayer.Pieces.Where(localPiece => localPiece.IsAvailable ).OrderByDescending(localPiece => localPiece.Weight);

                    //ponenent's turn to place piece
                    foreach (var opponent in opponents)
                    {
                        //create list of opponents pieces and sort them by size
                        //! I used weight
                        var availableLocalOpponentPieces = opponent.Pieces.Where(localPiece => localPiece.IsAvailable).OrderByDescending(localPiece => localPiece.Weight);

                        //create list of all possible moves
                        var allPossibleOpponentMoves = new List<Move>();
                        foreach (var opponentPiece in availableLocalOpponentPieces)
                        {
                            allPossibleOpponentMoves.AddRange(currentTestPlayer.GetValidMoves(boardCopy, opponentPiece, boardCopy.GetAllAvailableCorners(opponent.Id)));
                        }
                        /*
                        //calculate the scores
                        foreach (var opponentMove in allPossibleOpponentMoves)
                        {
                            opponentMove.Score = this.evalMove(opponentMove, currentPlayer, boardCopy);
                        }
                        */
                        allPossibleOpponentMoves.OrderByDescending(localOpponentPiece => localOpponentPiece.Score);

                        if (allPossibleOpponentMoves.Any())
                        {

                            var final_moves_op = new List<Move>();
                            // evaluate each move
                            foreach(var opponentMove in final_moves_op)
                            {
                                move.Score = this.evalMove(opponentMove, currentPlayer, boardCopy);
                            }

                            //sort moves by score
                            final_moves_op.OrderByDescending(m => m.Score);

                            //take the highest scoring move
                            var topMove = final_moves_op.First();
                            if(topMove != null)
                                //update the board with the highest scoring move
                                boardCopy.MakeMove(topMove);
                            //create a list of the other opponents
                            var other_opponents = opponents.Where(op => op.Id != opponent.Id);
                            //update the corners of the other opponents
                            foreach(var op in other_opponents)
                            {
                                opponentCorners[op.Id] = boardCopy.GetAllAvailableCorners(op.Id);
                            }
                            currentTestPlayeravailableCorners = boardCopy.GetAllAvailableCorners(currentTestPlayer.Id);
                        }
                        else
                        {
                            //return pieace of no possible moves
                            return move; //?
                        }
                    }
                    //finished opponents turns

                    var possibleMoves = new List<Move>();
                    //calculate the moves
                    foreach (var localPiece in availableLocalPieces)
                    {
                        possibleMoves.AddRange(this.currentPlayer.GetValidMoves(boardCopy, localPiece, currentTestPlayeravailableCorners));
                    }

                    if (possibleMoves.Any())
                    {
                        //calculate all final moves 2 and sort them by score
                        foreach (var localMove in possibleMoves)
                        {
                            localMove.Score = this.evalMove(localMove, currentPlayer, boardCopy);
                        }
                        possibleMoves.OrderByDescending(m => m.Score);

                        //# calculate the best score for each initial piece (can be weighted differently)
                        //best_score = weights[3] * by_score_2[0][1] + weights[4] * score
                        //# append initial piece plus potential score to final_choices
                        //final_choices.append((piece, best_score))
                        finalMoves.Add(possibleMoves.First());//+score for this
                    }
                    else
                    {
                        //add the first played piece to the final choices list
                        return null;
                    }


                }

                //sort by the highest score
                return finalMoves.First();

            }

            return null;
        }

        //evaluate score of a move
        //can be used by both Greedy and Minimax
        private int evalMove(Move move, Player currentPlayer, Board board)
        {
            var availableCorners = board.GetAllAvailableCorners(currentPlayer.Id);

            //backup the players
            var players = new Player[4];
            Array.Copy(game.Players, players,4);

            var opponents = new List<Player>();
            opponents = players.Where(p => p.Id != currentPlayer.Id).ToList();

            var test_board = new Board(board);
            
            //update the board with the move
            test_board.MakeMove(move);
            //copy the current player
            var test_player = new Player(currentPlayer);

            var availableCornersForCurrentTestPlayer = test_board.GetAllAvailableCorners(test_player.Id);

            var oponnentsCorners = new List<int[][]>();

            foreach(var opponent in opponents)
            {
                oponnentsCorners.Add(test_board.GetAllAvailableCorners(opponent.Id));
            }

            //  calculate the corners of the opponents
            // find the difference between the number of corners the current player has and and the 
            // mean number of corners the opponents have
            var corner_difference = 0;
            foreach (var oponnentCorners in oponnentsCorners)
            {
                    corner_difference += (availableCornersForCurrentTestPlayer.Length - oponnentCorners.Length);
            }
            corner_difference /= 3;

            // # return the score = size + difference in the number of corners
            // return (piece, weights[0] * piece.size + weights[1] * corner_difference)
            return currentPlayer.Pieces.Where(piece => piece.Id == move.PieceId).First().Weight * corner_difference;
        }
    }
}