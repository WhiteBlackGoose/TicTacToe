using HonkSharp.Fluency;
using System;

public enum State : byte
{
    Empty,
    X,
    O
}

public unsafe struct StateTable : IEquatable<StateTable>
{
    public const int N = 4;
    public const int N2 = N * N;
    public const int NLow = N - 1;
    public const int N2Low = N2 - 1;

    public const int WinRow = 4;

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

    public bool Won(State state)
    {
        ReadOnlySpan<(int StepX, int StepY, Range XRange, Range YRange)> steps = new[] {
            (StepX: 1, StepY: 0, XRange: 0..(N - WinRow), YRange: 0..NLow),
            (StepX: 0, StepY: 1, XRange: 0..NLow, YRange: 0..(N - WinRow)),
            (StepX: 1, StepY: 1, XRange: 0..(N - WinRow), YRange: 0..(N - WinRow)),
            (StepX: 1, StepY: -1, XRange: 0..(N - WinRow), YRange: WinRow..NLow),
        };

        foreach (var (stepX, stepY, xRange, yRange) in steps)
        {
            foreach (var x in xRange)
                foreach (var y in yRange)
                    if (IsBeginOfWinRow(x, y, stepX, stepY, this, state))
                        return true;
        }

        return false;

        static bool IsBeginOfWinRow(int x, int y, int stepX, int stepY, in StateTable table, State state)
        {
            for (int i = 0; i < WinRow; i++)
            {
                if (table[x, y] != state)
                    return false;
                x += stepX;
                y += stepY;
            }
            return true;
        }
    }
}