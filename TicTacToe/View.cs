using HonkSharp.Fluency;
using System;

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