namespace z019.BackgroundJobs;

using Microsoft.Extensions.DependencyInjection;

public class UpdateExchangeTableService(IServiceProvider services) : IDisposable
{
    private readonly IServiceProvider services = services;
    private UpdateExchangeTableJob? jobItem;
    private Task? jobTask;

    /// <summary>
    /// The status changed on the Percentage or IsBusy.
    /// </summary>
    public event Action? OnStatusChanged;

    /// <summary>
    /// Job is busy running.
    /// </summary>
    public bool IsBusy => jobItem != null && jobItem.IsBusy;

    /// <summary>
    /// The percentage of the job completed.
    /// </summary>
    public int Percentage => jobItem == null ? 0 : jobItem.Percentage;

    public void Dispose()
    {
        RemoveJob();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Update the Exchange table from the data service.
    /// This will run in the background.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <exception cref="NullReferenceException">Exception if the <see cref="UpdateExchangeTableJob"/> is not found in DI.</exception>
    public void UpdateExchangeTable(CancellationToken cancellationToken = default)
    {
        lock (this)
        {
            if (IsBusy) return;

            using var scope = services.CreateScope();
            var job = scope.ServiceProvider.GetService<UpdateExchangeTableJob>();

            if (job == null) throw new NullReferenceException(nameof(job));

            jobItem = job;
            jobItem.OnStatusChanged += this.StatusChanged;
            jobTask = job.RunAsync(cancellationToken);
        }
    }

    private void RemoveJob()
    {
        if (jobItem != null)
        {
            jobItem.OnStatusChanged -= this.StatusChanged;
            jobItem = null;
            jobTask = null;
        }
    }

    private void StatusChanged()
    {
        OnStatusChanged?.Invoke();
        if (!IsBusy) { RemoveJob(); }
    }
}