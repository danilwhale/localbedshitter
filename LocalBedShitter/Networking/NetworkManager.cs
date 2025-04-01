using System.Collections.Concurrent;
using System.Net.Sockets;
using LocalBedShitter.Networking.Packets;

namespace LocalBedShitter.Networking;

public sealed class NetworkManager(TcpClient client) : IAsyncDisposable
{
    public readonly List<IPacketProcessor> Processors = [];

    private readonly ConcurrentQueue<IPacket> _readQueue = [];

    public readonly TcpClient Client = client;
    private readonly NetworkStream _stream = client.GetStream();

    public async Task PollAsync()
    {
        while (_stream.DataAvailable)
        {
            try
            {
                if (PacketManager.TryRead(_stream, out IPacket? packet))
                {
                    _readQueue.Enqueue(packet);
                }
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"Failed to read packet:\n{ex}");
            }
        }

        while (_readQueue.TryDequeue(out IPacket? packet))
        {
            for (int i = 0; i < Processors.Count; i++)
            {
                IPacketProcessor processor = Processors[i];
                processor.ProcessPacket(in packet);
            }
        }
    }

    public void SendPacket<T>(T packet) where T : IPacket
    {
        try
        {
            PacketManager.Write(_stream, packet);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to write packet:\n{ex}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _stream.DisposeAsync();
    }
}