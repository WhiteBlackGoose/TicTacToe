using System;

var table = new StateTable();

var view = new NetView((x, y) => table[x, y]);

(Player player1, Player player2) = (new UserPlayer(view, State.X), new BotPlayer(State.O));
// (Player player2, Player player1) = (new BotPlayer(State.X), new BotPlayer(State.O));
// (Player player2, Player player1) = (new BotPlayer(State.X), new RandomPlayer(State.O));

while (!table.IsFull && !table.Won(State.O) && !table.Won(State.X))
{
    view.Paint();
    var (x, y) = player1.NextMove(table);
    table = table.WithMove(x, y, player1.Token);
    (player1, player2) = (player2, player1);
}

view.Paint(false);

if (table.Won(player1.Token))
    Console.WriteLine($"The first player ({player1}) won.");
else if (table.Won(player2.Token))
    Console.WriteLine($"The second player ({player2}) won.");
else
    Console.WriteLine("Draw.");
