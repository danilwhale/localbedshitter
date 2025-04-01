using LocalBedShitter.Networking;

namespace LocalBedShitter;

public sealed class ChildBot(NetworkManager manager, string username, string mpPass) : Bot(manager, username, mpPass);