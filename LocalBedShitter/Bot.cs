using LocalBedShitter.API.Players;
using LocalBedShitter.Networking;
using LocalBedShitter.Networking.Packets;

namespace LocalBedShitter;

public sealed class Bot : IPacketProcessor, IDisposable
{
    public readonly NetworkManager Manager;
    public readonly PlayerManager PlayerManager;
    public readonly LocalPlayer Player;

    public Bot(NetworkManager manager, string username, string mpPass)
    {
        Manager = manager;
        manager.Processors.Add(this);
        
        PlayerManager = new PlayerManager(manager);
        PlayerManager.Message += OnMessage;
        
        Player = new LocalPlayer(manager, username);
        Player.Authenticate(mpPass);
    }

    private void OnMessage(RemotePlayer player, string content)
    {
        int colonIndex = content.IndexOf(':');
            
        if (colonIndex < 0) return;
        string actualContent = content[(colonIndex + 1)..].Trim();
            
        if (!actualContent.StartsWith('^')) return;
        Player.SendMessage($"{player.Username} invoked '{actualContent[1..]}'");    
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Manager.Poll();
            await Task.Delay(20);
        }
    }
    
    public void ProcessPacket(ref readonly IPacket packet)
    {
    }
    
    public void Dispose()
    {
        Manager.Processors.Remove(this);
        PlayerManager.Dispose();
        Player.Dispose();
    }
}