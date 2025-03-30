using System.CommandLine;
using System.Net;
using System.Net.Sockets;
using LocalBedShitter;
using LocalBedShitter.Networking;

Option<string> usernameOption = new("--username", "Username for the bot");
Option<string> ipOption = new("--ip", "Target server IPv4 address");
Option<int> portOption = new("--port", "Target server port");
Option<string> mpPassOption = new("--mpPass", "Target server mppass");

RootCommand rootCommand = [usernameOption, ipOption, portOption, mpPassOption];
rootCommand.SetHandler(async (username, ip, port, mpPass) =>
{
    using TcpClient client = new(AddressFamily.InterNetwork);
    await client.ConnectAsync(IPAddress.Parse(ip), port);

    await using NetworkManager manager = new(client);
    using Bot bot = new(manager, username, mpPass);
    await bot.RunAsync();
}, usernameOption, ipOption, portOption, mpPassOption);

return await rootCommand.InvokeAsync(args);