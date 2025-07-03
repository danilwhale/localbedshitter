using System.Collections.Concurrent;
using LocalBedShitter.API.Players;

namespace LocalBedShitter;

public sealed class PlayerPool
{
    private readonly LocalPlayer[] _players;
    private readonly Task?[] _tasks;

    public PlayerPool(IEnumerable<LocalPlayer> players)
    {
        _players = players.ToArray();
        _tasks = new Task?[_players.Length];
    }

    public async Task RunOrWaitAsync(Func<LocalPlayer, Task> task)
    {
        while (true)
        {
            for (int i = 0; i < _tasks.Length; i++)
            {
                ref Task? t = ref _tasks[i];
                
                if (t is { IsCompleted: false }) continue;
                int ii = i;
                t = Task.Run(async () =>
                {
                    await task(_players[ii]);
                });
                return;
            }
        }
    }
}