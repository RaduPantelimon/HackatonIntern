using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blokee;
using Newtonsoft.Json.Linq;

namespace Blokee.Activities
{
    public class PlayBlokeeMove : CodeActivity
    {
        public OutArgument<bool[]> AvailablePieces { get; set; }
        public InArgument<string> GameStatusJson { get; set; }
        public InArgument<int> PlayerId { get; set; }
        public InArgument<String> GameMode { get; set; } = "greedy-advanced";

        public OutArgument<bool> MoveExists { get; set; }
        public OutArgument<int> PieceId { get; set; }
        public OutArgument<int> Orientation { get; set; }
        public OutArgument<int> Row { get; set; }
        public OutArgument<int> Column { get; set; }
        

        protected override void Execute(CodeActivityContext context)
        {
            JObject gameProperties = JObject.Parse(GameStatusJson.Get(context));
            int playerId = PlayerId.Get(context);
            string gameMode = GameMode.Get(context);

            Game game = new Game(gameProperties, playerId, gameMode);
            var nextMove = game.PlayNextMove();

            if(nextMove == null)
            {
                MoveExists.Set(context, false);
                return;
            }
            else
            {
                MoveExists.Set(context, true);
                Console.WriteLine("Playing Move: " + nextMove.ToString());
            }

            PieceId.Set(context, nextMove?.PieceId ?? -1);
            Orientation.Set(context, nextMove?.Orientation ?? -1);
            Row.Set(context, nextMove?.PlacingRow);
            Column.Set(context, nextMove?.PlacingColumn);
            AvailablePieces.Set(context, game.Players[playerId].GetPieceAvailability());
        }
    }
}
