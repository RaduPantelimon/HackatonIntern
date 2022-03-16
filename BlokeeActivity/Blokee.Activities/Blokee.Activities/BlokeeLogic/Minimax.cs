using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blokee
{
    public class Minimax
    {
        private Player currentPlayer { get; set; }
        private Game game { get; set; }
        private Board board { get; set; }
        public static double SecondMoveConstant = 0.5;
        public static double CornerScoreConstant = 2;

        public Minimax() { }

        public Minimax(Player p, Game g)
        {
            this.currentPlayer = p;
            this.game = g;
            this.board = g.Board;
        }
        public Minimax(Player p)
        {
            this.currentPlayer = p;
        }

        public Move GetMinimaxMove(Board b)
        {
            this.board = b;
            
            return GetMinimaxMove();
        }

        public Move GetMinimaxMove(Player p,Game g)
        {
            this.board = new Board((int?[,])g.Board._board.Clone());
            this.game = g;
            this.currentPlayer = p;

            return GetMinimaxMove();
        }

        private Move GetMinimaxMove()
        {
            var availableCorners = board.GetAllAvailableCorners(this.currentPlayer.Id);
            //get a copy of the current pieces
            var availablePieces = this.currentPlayer.Pieces.Where(piece => piece.IsAvailable);

            List<Move> allPossibleMoves = new List<Move>();

            Console.WriteLine("Calculating all possible moves for the current player");
            //calculate the moves
            foreach (var piece in availablePieces)
            {
                allPossibleMoves.AddRange(this.currentPlayer.GetValidMoves(this.board, piece, availableCorners));
            }
            

            if (allPossibleMoves.Any())
            {   //calculate the scores
                foreach (var move in allPossibleMoves)
                {
                    move.Score = move.GetMoveScore(game);// this.evalMove(move, currentPlayer, this.board);
                }

                List<Move> finalMoves = new List<Move>();
                //top 5 highest scores
                var top_by_score = allPossibleMoves.OrderByDescending(p => p.Score).Take(5);

                foreach (var move in top_by_score)
                {
                    Console.WriteLine("Creating a copy of the whole game");

                    var boardCopy = new Board((int?[,])this.game.Board._board.Clone());
                    if(boardCopy == null )
                    {
                        throw new Exception("Couldn't create a copy of the board");
                    }
                    var gameCopy = new Game(boardCopy, currentPlayer.Id, this.game.Players );
                    var playersCopy = gameCopy.Players;

                    var opponents = playersCopy.Where(p => p.Id != currentPlayer.Id).ToList();//.ForEach(o => opponents.Add(o) );

                    Console.WriteLine("Placing a move in the mocked game");
                    Console.WriteLine(move.ToString());

                    gameCopy.PlayMove(move);

                    //update corners
                    Console.WriteLine("Updating the corners");
                    var currentTestPlayeravailableCorners = boardCopy.GetAllAvailableCorners(currentPlayer.Id);

                    //create a copy of the pieces that the current player has
                    var availableLocalPieces = currentPlayer.Pieces.Where(localPiece => localPiece.IsAvailable ).OrderByDescending(localPiece => localPiece.Weight).ToList();

                    var movedByOpponents = new List<Move>();
                    //opponents's turn to place piece
                    foreach (var opponent in opponents)
                    {
                        //create list of opponents pieces and sort them by size
                        //! I used weight
                        var availableLocalOpponentPieces = opponent.Pieces.Where(localPiece => localPiece.IsAvailable).OrderByDescending(localPiece => localPiece.Weight).ToList();
                        var availableLocalOponentCorners = boardCopy.GetAllAvailableCorners(opponent.Id);

                        //create list of all possible moves
                        Console.WriteLine("Creating a list with the opponent's moves");
                        var allPossibleOpponentMoves = new List<Move>();
                        foreach (var opponentPiece in availableLocalOpponentPieces)
                        {
                            allPossibleOpponentMoves.AddRange(opponent.GetValidMoves(boardCopy, opponentPiece, availableLocalOponentCorners));
                        }
                        
                        //calculate the scores
                        Console.WriteLine("Calculating the score for the moves");
                        foreach (var opponentMove in allPossibleOpponentMoves)
                        {
                            gameCopy.PlayMove(opponentMove);
                            opponentMove.Score = Move.GetCornersScores(gameCopy, opponent) - Move.GetCornersScores(gameCopy, currentPlayer);// this.evalMove(opponentMove, currentPlayer, boardCopy);
                            gameCopy.UndoMove(opponentMove);
                        }
                        
                        allPossibleOpponentMoves = allPossibleOpponentMoves.OrderByDescending(localOpponentPiece => localOpponentPiece.Score).ToList();

                        if (allPossibleOpponentMoves.Any())
                        {
                            //take the highest scoring move
                            var topMove = allPossibleOpponentMoves.First();
                            if (topMove != null)
                            {
                                //update the board with the highest scoring move
                                boardCopy.MakeMove(topMove);
                                movedByOpponents.Add(topMove);
                            }
                        }
                    }
                    
                    if (movedByOpponents.Count == 0)
                        return move;

                    currentTestPlayeravailableCorners = boardCopy.GetAllAvailableCorners(currentPlayer.Id);
                    //finished opponents turns
                    //MUTAREA 2 in FATA
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
                            //localMove.Score = this.evalMove(localMove, currentPlayer, boardCopy);
                            localMove.Score = this.evalMoveAdvanced(localMove, currentPlayer, gameCopy);
                        }
                        var final_moves2 = possibleMoves.OrderByDescending(m => m.Score).First();

                        move.Score += final_moves2.Score * SecondMoveConstant;
                        finalMoves.Add(move);//+score for this
                    }
                    else
                    {
                        //no more possible moves left
                        //add the first played piece to the final choices list
                        finalMoves.Add(move);
                    }

                    
                    foreach(var opMove in movedByOpponents)
                    {
                        gameCopy.UndoMove(opMove);
                    }
                    gameCopy.UndoMove(move);
                }

                //sort by the highest score
                var greedyMove = finalMoves[0];
                var minimaxMove = finalMoves.OrderByDescending(p => p.Score).First();

                //VERY IMPORTANT FOR DEBUGGING - DO NOT DELETE THIS LINE
                if(minimaxMove != greedyMove)
                {
                    Console.WriteLine("ATTENTION!!!!! MINIMAX considered that this move < {0} >would be better than < {1} > despite the second one having a better score.",
                        minimaxMove.ToString(), 
                        greedyMove.ToString());
                }

                return minimaxMove;
            }

            return null;
        }


        //evaluate score of a move
        //can be used by both Greedy and Minimax
        private double evalMoveAdvanced(Move move, Player currentPlayer, Game game)
        {
            double playerCornerScore = 0, enemyCornerScore = 0;
            game.PlayMove(move);
            foreach (Player p in game.Players)
            {
                if (p.Id == currentPlayer.Id)
                    playerCornerScore = Move.GetCornersScores(game, p);
                else
                {
                    enemyCornerScore += Move.GetCornersScores(game, p);
                }
            }
            game.UndoMove(move);
            return move.Player.Pieces[move.PieceId].Weight + (playerCornerScore * 3 - enemyCornerScore) * CornerScoreConstant;
        }


        //evaluate score of a move
        //can be used by both Greedy and Minimax
        private double evalMove(Move move, Player currentPlayer, Board board)
        {
            var availableCorners = board.GetAllAvailableCorners(currentPlayer.Id);

            //backup the players
            var players = new Player[4];
            Array.Copy(game.Players, players,4);

            var opponents = players.Where(p => p.Id != currentPlayer.Id).ToList();

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
            double corner_difference = 0;
            foreach (var oponnentCorners in oponnentsCorners)
            {
                    corner_difference += (availableCornersForCurrentTestPlayer.Length - oponnentCorners.Length);
            }
            corner_difference /= 3;

            // # return the score = size + difference in the number of corners
            // return (piece, weights[0] * piece.size + weights[1] * corner_difference)
            return currentPlayer.Pieces.Where(piece => piece.Id == move.PieceId).First().Weight + corner_difference;
        }
    }
}