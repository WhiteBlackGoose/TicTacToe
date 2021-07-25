using System;

var table = new StateTable();

var view = new NetView((x, y) => table[x, y]);

while (!table.Won(State.O) && !table.IsFull)
{
    view.Paint();
    var (xUser, yUser) = view.GetUserMoveInteractive();
    table = table.WithMove(xUser, yUser, State.X);
    if (table.IsFull)
        break;
    var (xBot, yBot) = Bot.MakeMove(table);
    table = table.WithMove(xBot, yBot, State.O);
}

view.Paint(false);

if (table.Won(State.O))
    Console.WriteLine("The bot won.");
else if (table.Won(State.X))
    Console.WriteLine("You won.");
else
    Console.WriteLine("Draw.");
