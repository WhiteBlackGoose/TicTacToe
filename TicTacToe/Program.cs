using HonkSharp.Fluency;
using System;
using System.Collections.Generic;

var table = new StateTable();

var view = new NetView((x, y) => table[x, y]);

while (!Bot.ThisStateWon(table, State.O) && !table.IsFull)
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
                    > StateTable.NLow => StateTable.NLow,
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
                    > StateTable.NLow => StateTable.NLow,
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
        foreach (var x in 0..StateTable.NLow)
        {
            Console.Write("|");
            foreach (var y in 0..StateTable.NLow)
                Console.Write($"{Read(x, y)}|");
            Console.WriteLine();
        }


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

public unsafe struct StateTable : IEquatable<StateTable>
{
    public const int N = 4;
    public const int N2 = N * N;
    public const int NLow = N - 1;
    public const int N2Low = N2 - 1;

    private fixed byte states[N * N];

    public override int GetHashCode()
    {
        var hash = 0;
        foreach (var i in 0..N2Low)
            hash = (hash, states[i]).GetHashCode();
        return hash;
    }

    public bool Equals(StateTable other)
    {
        for (int i = 0; i < N2Low; i++)
            if (other.states[i] != states[i])
                return false;
        return true;
    }

    public State this[int x, int y] => (State)states[x * N + y];

    public StateTable WithMove(int x, int y, State newState)
    {
        var res = this;
        res.states[x * N + y] = (byte)newState;
        return res;
    }

    public override string ToString()
    {
        var res = "";
        foreach (var x in 0..NLow)
        {
            foreach (var y in 0..NLow)
                res += this[x, y] switch { State.X => "X", State.O => "O", _ => "." };
            res += Environment.NewLine;
        }
        return res;
    }

    public bool IsFull
    {
        get
        {
            foreach (var i in 0..N2Low)
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
        foreach (var x2 in 0..StateTable.NLow)
            foreach (var y2 in 0..StateTable.NLow)
                if (table[x2, y2] is State.Empty && NotLoseIfWeMakeThisMove(table, x2, y2))
                    return (x2, y2);
        return (-1, -1);
    }

    public static bool ThisStateWon(StateTable table, State state)
    {
        var count = 0;
        foreach (var x in 0..StateTable.NLow)
        {
            count = 0;
            foreach (var y in 0..StateTable.NLow)
                count += table[x, y] == state ? 1 : 0;
            if (count is StateTable.N)
                return true;
        }

        foreach (var x in 0..StateTable.NLow)
        {
            count = 0;
            foreach (var y in 0..StateTable.NLow)
                count += table[y, x] == state ? 1 : 0;
            if (count is StateTable.N)
                return true;
        }

        count = 0;
        foreach (var i in 0..StateTable.NLow)
            count += table[i, i] == state ? 1 : 0;
        if (count is StateTable.N)
            return true;

        count = 0;
        foreach (var i in 0..StateTable.NLow)
            count += table[i, StateTable.NLow - i] == state ? 1 : 0;
        if (count is StateTable.N)
            return true;

        return false;
    }


    private static Dictionary<(StateTable Table, int X, int Y), bool> cache1 = new();
    private static bool NotLoseIfWeMakeThisMove(StateTable table, int x, int y)
    {
        var key = (table, x, y);
        if (cache1.TryGetValue(key, out var res))
            return res;

        var newTable = table.WithMove(x, y, State.O);
        if (ThisStateWon(newTable, State.O))
        {
            cache1[key] = true;
            return true;
        }
        foreach (var x2 in 0..StateTable.NLow)
            foreach (var y2 in 0..StateTable.NLow)
                if (newTable[x2, y2] is State.Empty && WinIfTheyMakeThisMove(newTable, x2, y2))
                {
                    cache1[key] = false;
                    return false;
                }
        cache1[key] = true;
        return true;
    }

    private static Dictionary<(StateTable Table, int X, int Y), bool> cache2 = new();
    private static bool WinIfTheyMakeThisMove(StateTable table, int x, int y)
    {
        var key = (table, x, y);
        if (cache2.TryGetValue(key, out var res))
            return res;

        var newTable = table.WithMove(x, y, State.X);
        if (ThisStateWon(newTable, State.X))
        {
            cache2[key] = true;
            return true;
        }
        if (newTable.IsFull)
        {
            cache2[key] = false;
            return false;
        }
        foreach (var x2 in 0..StateTable.NLow)
            foreach (var y2 in 0..StateTable.NLow)
                if (newTable[x2, y2] is State.Empty && NotLoseIfWeMakeThisMove(newTable, x2, y2))
                {
                    cache2[key] = false;
                    return false;
                }
        cache2[key] = true;
        return true;
    }
}