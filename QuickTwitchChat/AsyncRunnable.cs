using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuickTwitchChat;

public class AsyncRunnable
{
    private CancellationToken cancellationToken;
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    public Action Finish { get; set; }

    public Task RunningTask;

    public string Name;

    public AsyncRunnable(Action action, string name = "Async Runnable")
    {
        Finish = () => Console.WriteLine($"Thread {name} Exited");
        this.Name = name;
        cancellationToken = cancellationTokenSource.Token;
        run(action, Finish);
    }

    private async void run(Action action, Action finishAction)
    {
        RunningTask = Task.Run(action, cancellationToken).ContinueWith((task =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            Finish.Invoke();
        }), cancellationToken);
        try
        {
            await RunningTask;
        }
        catch (Exception ex)
        {
            // ignored
        }

        if(!cancelFinish) finishAction?.Invoke();
    }

    private bool cancelFinish = false;
    public void Stop(bool cancelFinish = false)
    {
        {
            this.cancelFinish = cancelFinish;
            cancellationTokenSource.Cancel();
        }
    }

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        // RunningTask.Dispose();
        cancellationTokenSource.Dispose();
    }
}