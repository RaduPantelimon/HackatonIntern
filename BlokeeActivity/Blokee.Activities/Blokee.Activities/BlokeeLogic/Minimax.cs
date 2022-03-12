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

        public Minimax(Player p, Game g)
        {
            this.currentPlayer = p;
            this.game = g;
        }

        private Move PlayMinimax()
        {
            var availableCorners = Board.I.GetAllAvailableCorners(this.currentPlayer.Id);
            var availablePieces = this.currentPlayer.Pieces.Where(piece => piece.IsAvailable).OrderByDescending(piece => piece.Weight);

            List<Move> allPossibleMoves = new List<Move>();

            //calculate the moves
            foreach (var piece in availablePieces)
            {
                allPossibleMoves.AddRange(this.currentPlayer.GetValidMoves(piece, availableCorners));
            }

            //calculate the scores
            foreach(var move in allPossibleMoves)
            {
                move.Score = this.evalMove(move.Piece, currentPlayer, this.game);
            }

            List<Move> finalMoves = new List<Move>();

            if (allPossibleMoves.Any())
            {
                //top 3 highest scores
                var top_by_score = allPossibleMoves.OrderBy(p => p.Score).Take(3);

                foreach (var piece in top_by_score)
                {
                    var boardCopy = JsonConvert.DeserializeObject<Board>(JsonConvert.SerializeObject(this.game.board));
                    var playersCopy = this.game.Players;
                    var opponents = playersCopy.Where(p => p.Id != currentPlayer.Id);
                    var currentTestPlayer = JsonConvert.DeserializeObject<Player>(JsonConvert.SerializeObject(this.currentPlayer));


                    boardCopy.MakeMove(piece);
                    currentTestPlayer.availableCorners = boardCopy.GetAllAvailableCorners(this.currentPlayer.Id);

                    //update corners for opponents
                    foreach (var opponent in opponents)
                    {
                        opponent.availableCorners = boardCopy.GetAllAvailableCorners(opponent.Id);
                    }

                    //create a copy of the pieces that the current player has
                    var availableLocalPieces = currentTestPlayer.Pieces.Where(localPiece => localPiece.IsAvailable).OrderByDescending(localPiece => localPiece.Weight);

                    //remove the used piece from the board

                    //#ponenent's turn to place piece
                    foreach (var opponent in opponents)
                    {
                        //create list of opponents pieaces and sort them by size/weight
                        var availableLocalOpponentPieces = opponent.Pieces.Where(localPiece => localPiece.IsAvailable).OrderByDescending(localPiece => localPiece.Weight);

                        var allPossibleOponnentMoves = new List<Move>();//create list of all possible moves
                        foreach (var opponentPiece in availableLocalOpponentPieces)
                        {
                            allPossibleOponnentMoves.AddRange(this.currentPlayer.GetValidMoves(opponentPiece, opponent.availableCorners));
                        }
                        //calculate the scores
                        foreach (var move in allPossibleOponnentMoves)
                        {
                            move.Score = this.evalMove(move.Piece, currentPlayer, this.game);
                        }

                        allPossibleOponnentMoves.OrderByDescending(localOpponentPiece => localOpponentPiece.Score);

                        if (allPossibleOponnentMoves.Any())
                        {

                            var final_moves_op = new List<Move>();
                            // evaluate each move
                            //sort moves by score
                            //take the highest scoring move
                            //update the board with the highest scoring move

                            //create a list of the other opponents
                            //update the corners of the other opponents

                        }
                        else
                        {
                            //return pieace of no possible moves
                            return piece; //?
                        }
                    }
                    //#finished opponents turns

                    var possibleMoves2 = new List<Move>();
                    //calculate the moves
                    foreach (var localPiece in availableLocalPieces)
                    {
                        allPossibleMoves.AddRange(this.currentPlayer.GetValidMoves(localPiece, currentTestPlayer.availableCorners));
                    }

                    if (possibleMoves2.Any())
                    {
                        var final_moves_2 = new List<Move>();
                        //calculate all final moves 2 and sort them by score

                        //# calculate the best score for each initial piece (can be weighted differently)
                        //best_score = weights[3] * by_score_2[0][1] + weights[4] * score
                        //# append initial piece plus potential score to final_choices
                        //final_choices.append((piece, best_score))
                    }
                    else
                    {
                        //add the first played piece to the final choices list
                    }


                }
                //sort by the highest score
                //return that move

            }

            return finalMoves.First();
        }

        //evaluate score of a move
        //can be used by both Greedy and Minimax
        private int evalMove(Piece piece, Player currentPlayer, Game game)
        {
            var availableCorners = game.board.GetAllAvailableCorners(currentPlayer.Id);
            var board = game.board;

            //backup the players
            var players = new Player[4];
            Array.Copy(game.Players, players,4);

            var opponents = players.Where(p => p.Id != currentPlayer.Id);

           // var test_board = deepcopy(board)
           // test_board = copy.deepcopy(board)
            //update the map with the piece
            //test_player = create a copy of the current player
            //update the corners of the current player
            //update the corner of all opponents

            // # calculate the mean of the corners of the opponents
            // opponent_corners = [len(opponent.corners) for opponent in opponents]
            //// # find the difference between the number of corners the current player has and and the 
            // # mean number of corners the opponents have
            // corner_difference = np.mean([my_corners - opponent_corner for opponent_corner in opponent_corners])
            // # return the score = size + difference in the number of corners
            // return (piece, weights[0] * piece.size + weights[1] * corner_difference)
            return 0;
        }
    }
}