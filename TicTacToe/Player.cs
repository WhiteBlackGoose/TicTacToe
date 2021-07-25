

using System;

public abstract class Player
{
    public State Token { get; }
    public Player(State state)
        => Token = state;
    public abstract (int X, int Y) NextMove(StateTable table);
}

public sealed class UserPlayer : Player
{
    private readonly NetView view;
    public UserPlayer(NetView view, State state) : base(state)
        => this.view = view;

    public override (int X, int Y) NextMove(StateTable table)
        => view.GetUserMoveInteractive();

    public override string ToString() => $"User {Token}";
}

public sealed class BotPlayer : Player
{
    public BotPlayer(State state) : base(state) { }
    public override (int X, int Y) NextMove(StateTable table)
        => Bot.MakeMove(table, Token);

    public override string ToString() => $"Bot {Token}";
}

public sealed class RandomPlayer : Player
{
    private readonly Random random = new();
    public RandomPlayer(State state) : base(state) { }

    public override (int X, int Y) NextMove(StateTable table)
    {
        var (x, y) = (0, 0);
        while (table[x, y] is not State.Empty)
            (x, y) = (random.Next(0, StateTable.N), random.Next(0, StateTable.N));
        return (x, y);
    }

    public override string ToString() => $"Random {Token}";
}