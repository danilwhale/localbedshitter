// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using LocalBedShitter.API;
using LocalBedShitter.API.Players;
using LocalBedShitter.Networking;

internal class Program
{
    public static async Task Main(string[] args)
    {
        using TcpClient client = new(AddressFamily.InterNetwork);
        await client.ConnectAsync(IPAddress.Parse("92.119.126.203"), 65535);

        await using NetworkManager manager = new(client);
        using CancellationTokenSource cancelSource = new();

        using PlayerManager playerManager = new(manager);
        using LocalPlayer localPlayer = new(manager, "localbedshitter");

        playerManager.Messaged += (player, content) =>
        {
            int colonIndex = content.IndexOf(':');
            
            if (colonIndex < 0) return;
            string actualContent = content[(colonIndex + 1)..].Trim();
            
            if (!actualContent.StartsWith('^')) return;
            localPlayer.SendMessage($"{player.Username} invoked '{actualContent[1..]}'");    
        };
        
        localPlayer.Authenticate("9be213a6bd85dd02ce7dd56bec0d9e20");
        
        while (true)
        {
            manager.Poll();
            await Task.Delay(20);
        }
    }
}