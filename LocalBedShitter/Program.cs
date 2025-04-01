using System.CommandLine;
using System.Net;
using System.Net.Sockets;
using LocalBedShitter;
using LocalBedShitter.Networking;

Option<string> usernameOption = new("--username", "Username of the bot account");
Option<string> subUsernameOption = new("--sub-username", "Username prefix for sub-bot accounts");
Option<string> ipOption = new("--ip", "Target server IPv4 address");
Option<int> portOption = new("--port", "Target server port");
Option<string> mpPassOption = new("--mpPass", "Target server mppass");
Option<string[]> subMpPassesOption = new("--sub-mpPasses", "Target server mppasses for sub-bots")
{
    IsRequired = true,
    AllowMultipleArgumentsPerToken = true
};

RootCommand rootCommand = [usernameOption, subUsernameOption, ipOption, portOption, mpPassOption, subMpPassesOption];
rootCommand.SetHandler(async (username, subUsername, ip, port, mpPass, subMpPasses) =>
{
    using TcpClient client = new(AddressFamily.InterNetwork);
    await client.ConnectAsync(IPAddress.Parse(ip), port);

    await using NetworkManager manager = new(client);
    await using MainBot bot = new(manager, username, mpPass, ip, port, subUsername, subMpPasses);
    await bot.RunAsync();
}, usernameOption, subUsernameOption, ipOption, portOption, mpPassOption, subMpPassesOption);

return await rootCommand.InvokeAsync(args);