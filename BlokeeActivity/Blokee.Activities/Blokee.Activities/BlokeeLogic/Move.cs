using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blokee
{
    public class Move
    {

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
    }
}