using System.Diagnostics.CodeAnalysis;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public sealed class JobPool
{
    public int Count => _jobs.Count;
    public bool IsEmpty => _jobs.Count == 0;
    
    private readonly Queue<Job> _jobs = [];

    public void Add<T>(T job) where T : Job
    {
        _jobs.Enqueue(job);
    }

    public void Add<T>(IEnumerable<T> jobs) where T : Job
    {
        foreach (T job in jobs)
        {
            _jobs.Enqueue(job);
        }
    }

    public Job Remove()
    {
        return _jobs.Dequeue();
    }

    public bool TryRemove([NotNullWhen(true)] out Job? job)
    {
        return _jobs.TryDequeue(out job);
    }

    public void Clear()
    {
        _jobs.Clear();
    }
}