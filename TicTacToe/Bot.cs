using HonkSharp.Fluency;
using System.Collections.Generic;
using System;

public static class Bot
{
    public static (int X, int Y) MakeMove(StateTable table)
    {
        foreach (var x2 in 0..StateTable.NLow)
            foreach (var y2 in 0..StateTable.NLow)
                if (table[x2, y2] is State.Empty && WinIfMakeThisMove(table, x2, y2, State.O))
                    return (x2, y2);

        foreach (var x2 in 0..StateTable.NLow)
            foreach (var y2 in 0..StateTable.NLow)
                if (table[x2, y2] is State.Empty && NotLoseIfMakeThisMove(table, x2, y2, State.O))
                    return (x2, y2);

        return (-1, -1);
    }

    private static State NextTurn(State current)
        => current switch
        {
            State.X => State.O,
            State.O => State.X,
            _ => throw new()
        };

    private static bool NotLoseIfMakeThisMove(StateTable table, int x, int y, State player)
    => Memoized<(StateTable, int, int, State), bool>.Get((table, x, y, player), 1, static arg =>
    {
        var (table, x, y, player) = arg;
        var newTable = table.WithMove(x, y, player);
        if (newTable.Won(player) || newTable.IsFull)
            return true;
        foreach (var x2 in 0..StateTable.NLow)
            foreach (var y2 in 0..StateTable.NLow)
                if (newTable[x2, y2] is State.Empty && WinIfMakeThisMove(newTable, x2, y2, NextTurn(player)))
                    return false;
        return true;
    });


    private static bool WinIfMakeThisMove(StateTable table, int x, int y, State player)
    => Memoized<(StateTable, int, int, State), bool>.Get((table, x, y, player), 0, static arg =>
    {
        var (table, x, y, player) = arg;
        var newTable = table.WithMove(x, y, player);
        if (newTable.Won(player))
            return true;
        if (newTable.IsFull)
            return false;
        foreach (var x2 in 0..StateTable.NLow)
            foreach (var y2 in 0..StateTable.NLow)
                if (newTable[x2, y2] is State.Empty && NotLoseIfMakeThisMove(newTable, x2, y2, NextTurn(player)))
                    return false;
        return true;
    });

    private static class Memoized<TIn, TOut>
    {
        private readonly static Dictionary<(TIn, int MethodTag), TOut> cache = new();
        public static TOut Get(TIn arg, int methodTag, Func<TIn, TOut> factory)
        {
            if (cache.TryGetValue((arg, methodTag), out var res))
                return res;
            res = factory(arg);
            cache[(arg, methodTag)] = res;
            return res;
        }
    }
}