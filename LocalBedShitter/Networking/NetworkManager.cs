using System.Collections.Concurrent;
using System.Net.Sockets;
using LocalBedShitter.Networking.Packets;

namespace LocalBedShitter.Networking;

public sealed class NetworkManager : IAsyncDisposable
{
    public bool DataAvailable => !_readQueue.IsEmpty;
    
    public readonly HashSet<IPacketProcessor> Processors = [];

    private readonly ConcurrentQueue<IPacket> _readQueue = [];
    private readonly ConcurrentQueue<IPacket> _writeQueue = [];

    private readonly NetworkStream _stream;

    private readonly CancellationTokenSource _cancelSource = new();
    private readonly Task _readTask;
    private readonly Task _writeTask;

    public NetworkManager(TcpClient client)
    {
        _stream = client.GetStream();

        _readTask = Task.Run(async () => await ReadPacketsAsync(_cancelSource.Token), _cancelSource.Token);
        _writeTask = Task.Run(async () => await WritePacketsAsync(_cancelSource.Token), _cancelSource.Token);
    }

    private async Task ReadPacketsAsync(CancellationToken cancel)
    {
        while (!cancel.IsCancellationRequested)
        {
            while (!_stream.DataAvailable) await Task.Delay(10, cancel);
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
        }
    }

    private async Task WritePacketsAsync(CancellationToken cancel)
    {
        while (!cancel.IsCancellationRequested)
        {
            try
            {
                while (_writeQueue.IsEmpty) await Task.Delay(10, cancel);
                while (_writeQueue.TryDequeue(out IPacket? packet))
                {
                    PacketManager.Write(_stream, packet);
                }
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"Failed to write packet:\n{ex}");
            }
        }
    }

    public void Poll()
    {
        while (_readQueue.TryDequeue(out IPacket? packet))
        {
            foreach (IPacketProcessor processor in Processors.ToHashSet())
            {
                processor.ProcessPacket(in packet);
            }
        }
    }

    public void SendPacket<T>(T packet) where T : IPacket
    {
        _writeQueue.Enqueue(packet);
    }

    public async ValueTask DisposeAsync()
    {
        await _cancelSource.CancelAsync();
        await Task.WhenAll(_readTask, _writeTask);
        
        await _stream.DisposeAsync();
    }
}