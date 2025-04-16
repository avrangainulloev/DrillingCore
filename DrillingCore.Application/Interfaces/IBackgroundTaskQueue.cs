﻿namespace DrillingCore.Application.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        Task QueueBackgroundWorkItem(Func<IServiceProvider, CancellationToken, Task> workItem);
        Task<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
