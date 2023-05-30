using PushListenerForLinux;

public interface IWorker
{
    void DoWork();
}

public class NullWorker : IWorker
{
    public void DoWork()
    {
        // Null implementation intentionally left blank
    }
}

public class Worker : IWorker
{
    public void DoWork()
    {
        // Original implementation of the DoWork method
        // Add your bug-inducing logic here
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .UseSystemd()
            .ConfigureServices(services =>
            {
                // Register the Worker class as the service implementation
                services.AddSingleton<IWorker, Worker>();
            })
            .Build();

        await host.RunAsync();
    }
}
