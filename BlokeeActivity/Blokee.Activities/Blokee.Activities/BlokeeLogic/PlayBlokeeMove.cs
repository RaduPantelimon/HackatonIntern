using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blokee;

namespace Blokee.Activities
{
    public class PlayBlokeeMove : CodeActivity
    {
        public InOutArgument<bool[]> AvailablePieces { get; set; }
        public InArgument<string> BoardJson { get; set; }
        public InArgument<int> PlayerId { get; set; }

        public OutArgument<int> PieceId { get; set; }
        public OutArgument<int> Orientation { get; set; }
        public OutArgument<int> Row { get; set; }
        public OutArgument<int> Column { get; set; }
        

        protected override void Execute(CodeActivityContext context)
        {
            Board.I.RefreshBoard(BoardJson.Get(context));
            var player = new Player(PlayerId.Get(context), AvailablePieces.Get(context));
            var nextMove = player.Play();

            PieceId.Set(context, nextMove[0]);
            Orientation.Set(context, nextMove[1]);
            Row.Set(context, nextMove[2]);
            Column.Set(context, nextMove[3]);

            AvailablePieces.Set(context, player.GetPieceAvailability());
        }
    }
}
