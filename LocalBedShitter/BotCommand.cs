using LocalBedShitter.API.Players;

namespace LocalBedShitter;

public sealed class BotCommand(string name, int argumentCount, Func<RemotePlayer, string[], Task> handler)
{
    public readonly string Name = name;
    public readonly int ArgumentCount = argumentCount;

    public async Task InvokeAsync(RemotePlayer invoker, string[] args)
    {
        await handler.Invoke(invoker, args);
    }
}