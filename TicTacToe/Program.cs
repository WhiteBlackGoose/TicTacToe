using HonkSharp.Fluency;
using System;

var table = new StateTable();

var view = new NetView((x, y) => table[x, y]);

while (!Bot.ThisStateWon(table, State.O))
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

if (Bot.ThisStateWon(table, State.O))
    Console.WriteLine("The bot won.");
else
    Console.WriteLine("Draw.");

public enum State : byte
{
    Empty,
    X,
    O
}

public sealed record NetView(Func<int, int, State> CurrentState)
{
    private (int X, int Y) cursorPos = (0, 0);

    public (int X, int Y) GetUserMoveInteractive()
    {
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey();
            if (key.Key is ConsoleKey.DownArrow or ConsoleKey.UpArrow or ConsoleKey.LeftArrow or ConsoleKey.RightArrow)
            {
                var newX = (cursorPos.X + (key.Key switch
                {
                    ConsoleKey.DownArrow => 1,
                    ConsoleKey.UpArrow => -1,
                    _ => 0
                })) switch
                {
                    < 0 => 0,
                    > 2 => 2,
                    var other => other
                };

                var newY = (cursorPos.Y + (key.Key switch
                {
                    ConsoleKey.LeftArrow => -1,
                    ConsoleKey.RightArrow => 1,
                    _ => 0
                })) switch
                {
                    < 0 => 0,
                    > 2 => 2,
                    var other => other
                };

                cursorPos = (newX, newY);

                Paint();
            }
        }
        while (key.Key is not ConsoleKey.Enter and not ConsoleKey.Spacebar);

        return cursorPos;
    }

    public void Paint(bool showCursor = true)
    {
//      |X|O| |
//      |O| |X|
//      |X| | |
        Console.Clear();
        foreach (var x in 0..2)
            Console.WriteLine($"|{Read(x, 0)}|{Read(x, 1)}|{Read(x, 2)}|");


        string Read(int x, int y) 
            => (x, y) == cursorPos && showCursor
            ? "@"
            : CurrentState(x, y) switch
            {
                State.X => "X",
                State.O => "O",
                _ => " "
            };
    }
}

public unsafe struct StateTable
{
    private fixed byte states[9];

    public override int GetHashCode()
    {
        var hash = 0;
        foreach (var i in 0..8)
            hash = (hash, states[i]).GetHashCode();
        return hash;
    }

    public State this[int x, int y] => (State)states[x * 3 + y];

    public StateTable WithMove(int x, int y, State newState)
    {
        var res = this;
        res.states[x * 3 + y] = (byte)newState;
        return res;
    }

    public override string ToString()
    {
        var res = "";
        foreach (var x in 0..2)
        {
            foreach (var y in 0..2)
                res += this[x, y] switch { State.X => "X", State.O => "O", _ => "." };
            res += Environment.NewLine;
        }
        return res;
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }

    public bool IsFull
    {
        get
        {
            foreach (var i in 0..8)
                if ((State)states[i] is State.Empty)
                    return false;
            return true;
        }
    }
}

public static class Bot
{
    public static (int X, int Y) MakeMove(StateTable table)
    {
        foreach (var (x2, y2) in (0..2).AsRange().Cartesian((0..2).AsRange()))
            if (table[x2, y2] is State.Empty && NotLoseIfWeMakeThisMove(table, x2, y2))
                return (x2, y2);
        return (-1, -1);
    }

    public static bool ThisStateWon(StateTable table, State state)
        => table[0, 0] == table[0, 1] && table[0, 1] == table[0, 2] && table[0, 0] == state ||
           table[1, 0] == table[1, 1] && table[1, 1] == table[1, 2] && table[1, 0] == state ||
           table[2, 0] == table[2, 1] && table[2, 1] == table[2, 2] && table[2, 0] == state ||

           table[0, 0] == table[1, 0] && table[1, 0] == table[2, 0] && table[0, 0] == state ||
           table[0, 1] == table[1, 1] && table[1, 1] == table[2, 1] && table[0, 1] == state ||
           table[0, 2] == table[1, 2] && table[1, 2] == table[2, 2] && table[0, 2] == state ||

           table[0, 0] == table[1, 1] && table[1, 1] == table[2, 2] && table[0, 0] == state ||
           table[0, 2] == table[1, 1] && table[1, 1] == table[2, 0] && table[1, 1] == state;

    private static bool NotLoseIfWeMakeThisMove(StateTable table, int x, int y)
    {
        var newTable = table.WithMove(x, y, State.O);
        if (ThisStateWon(newTable, State.O))
            return true;
        foreach (var (x2, y2) in (0..2).AsRange().Cartesian((0..2).AsRange()))
            if (newTable[x2, y2] is State.Empty && WinIfTheyMakeThisMove(newTable, x2, y2))
                return false;
        return true;
    }

    private static bool WinIfTheyMakeThisMove(StateTable table, int x, int y)
    {
        var newTable = table.WithMove(x, y, State.X);
        if (ThisStateWon(newTable, State.X))
            return true;
        if (newTable.IsFull)
            return false;
        foreach (var (x2, y2) in (0..2).AsRange().Cartesian((0..2).AsRange()))
            if (newTable[x2, y2] is State.Empty && NotLoseIfWeMakeThisMove(newTable, x2, y2))
                return false;
        return true;
    }
}