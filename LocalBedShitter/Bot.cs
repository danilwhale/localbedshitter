using System.Diagnostics;
using System.Numerics;
using System.Text;
using LocalBedShitter.API;
using LocalBedShitter.API.Players;
using LocalBedShitter.Jobs;
using LocalBedShitter.Networking;
using LocalBedShitter.Networking.Packets;

namespace LocalBedShitter;

public abstract class Bot
{
    public readonly NetworkManager Manager;
    public readonly PlayerManager PlayerManager;
    public readonly LocalPlayer LocalPlayer;

    public Bot(NetworkManager manager, string username, string mpPass)
    {
        Manager = manager;

        PlayerManager = new PlayerManager(manager);

        LocalPlayer = new LocalPlayer(manager, username);
        LocalPlayer.Authenticate(mpPass);
    }

    public virtual async Task RunAsync()
    {
        while (true)
        {
            await Manager.PollAsync();
        }
    }
}