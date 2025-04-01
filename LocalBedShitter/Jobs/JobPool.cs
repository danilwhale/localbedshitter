using System.Diagnostics.CodeAnalysis;

namespace LocalBedShitter.Jobs;

public sealed class JobPool
{
    public int Count => Queue.Count;
    public bool IsEmpty => Queue.Count == 0;
    
    public readonly Queue<Job> Queue = [];

    public void Add<T>(T job) where T : Job
    {
        Queue.Enqueue(job);
    }

    public void Add<T>(IEnumerable<T> jobs) where T : Job
    {
        foreach (T job in jobs)
        {
            Queue.Enqueue(job);
        }
    }

    public Job Remove()
    {
        return Queue.Dequeue();
    }

    public bool TryRemove([NotNullWhen(true)] out Job? job)
    {
        return Queue.TryDequeue(out job);
    }

    public void Clear()
    {
        Queue.Clear();
    }
}