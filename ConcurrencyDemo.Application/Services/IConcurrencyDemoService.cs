namespace ConcurrencyDemo.Application.Services
{
    public interface IConcurrencyDemoService
    {
        Task RunAsync(CancellationToken cancellationToken = default);
    }
}
