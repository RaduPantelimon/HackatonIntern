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
                return PiecePointColumn - PiecePointColumn;
            }
        }

        public Piece Piece { get; set; }
        public int Orientation { get; set; }

        public Player Player { get; set; }

        public Move(Player _player, Piece _piece, int _orientation, int _cornerRow, int _cornerColumn, int _piecePointRow, int _piecePointColumn)
        {
            Player = _player;
            CornerRow = _cornerRow;
            CornerColumn = _cornerColumn;
            PiecePointRow = _piecePointRow;
            PiecePointColumn = _piecePointColumn;
            Piece = _piece;
            Orientation = _orientation;
        }

    }
}
