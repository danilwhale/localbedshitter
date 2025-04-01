using System.Diagnostics;
using System.Net.Sockets;
using LocalBedShitter.API;
using LocalBedShitter.API.Players;
using LocalBedShitter.Jobs;
using LocalBedShitter.Networking;

namespace LocalBedShitter;

public sealed partial class MainBot : Bot, IAsyncDisposable
{
    public readonly Level Level;

    public readonly HashSet<BotCommand> Commands = [];

    private readonly JobPool _jobs = new();
    private readonly ChildBot[] _children;

    public MainBot(NetworkManager manager, string username, string mpPass, string ip, int port, string subUsername, string[] subMpPasses) : base(manager, username, mpPass)
    {
        Level = new Level(manager);

        // MainBot.Commands.cs
        Commands.Add(new BotCommand("tp", 1, HandleTpPlayerCommand));
        Commands.Add(new BotCommand("tp", 3, HandleTpCoordsCommand));
        Commands.Add(new BotCommand("say", -1, HandleSayCommand));
        Commands.Add(new BotCommand("setblock", 4, HandleSetBlockCommand));
        Commands.Add(new BotCommand("fill", 7, HandleFillCommand));
        Commands.Add(new BotCommand("sphere", 5, HandleSphereCommand));
        Commands.Add(new BotCommand("pyramid", 6, HandlePyramidCommand));
        Commands.Add(new BotCommand("veryeasy", 3, HandleVeryEasyCommand));
        Commands.Add(new BotCommand("jobs", 1, HandleJobsCommand));
        PlayerManager.Message += OnMessage;

        _children = new ChildBot[subMpPasses.Length];
        for (int i = 0; i < subMpPasses.Length; i++)
        {
            _children[i] = new ChildBot(new NetworkManager(new TcpClient(ip, port)), subUsername + i, subMpPasses[i]);
        }
        
        Task.Run(ExecuteJobsAsync);
    }

    private void OnMessage(RemotePlayer player, string content)
    {
        int colonIndex = content.IndexOf(':');

        if (colonIndex < 0) return;
        string actualContent = content[(colonIndex + 1)..].Trim();

        if (!actualContent.StartsWith('^')) return;
        string[] split = actualContent.Split(' ');

        if (split.Length < 1) return;
        string name = split[0][1..];
        foreach (BotCommand command in Commands
                     .Where(c => c.Name == name && (c.ArgumentCount == split.Length - 1 || c.ArgumentCount < 0)))
        {
            Task.Run(async () => await command.InvokeAsync(player, split.Length == 1 ? [] : split[1..]));
            break;
        }
    }

    private async Task ExecuteJobsAsync()
    {
        Stopwatch jobStopwatch = new();
        Stopwatch totalStopwatch = new();
        
        while (true)
        {
            if (_jobs.IsEmpty) continue;
            int jobCount = _jobs.Count;
            totalStopwatch.Restart();
            while (_jobs.TryRemove(out Job? job))
            {
                jobStopwatch.Restart();
                job.Initialize(Level);
                await Task.WhenAll(RunJobTasks(job));
                job.Dispose();
                jobStopwatch.Stop();
                LocalPlayer.SendMessage($"Finished {job} in {jobStopwatch.Elapsed:g}. {_jobs.Count} remaining jobs");
            }
            totalStopwatch.Stop();
            LocalPlayer.SendMessage($"Finished {jobCount} jobs in {totalStopwatch:g}");
        }
    }

    private IEnumerable<Task> RunJobTasks(Job job)
    {
        foreach (ChildBot child in _children)
        {
            yield return Task.Run(async () => await job.ExecuteAsync(child.LocalPlayer, Level));
        }

        yield return Task.Run(async () => await job.ExecuteAsync(LocalPlayer, Level));
    }

    public async ValueTask DisposeAsync()
    {
        foreach (ChildBot child in _children)
        {
            await child.Manager.DisposeAsync();
            child.Manager.Client.Dispose();
        }
    }
}