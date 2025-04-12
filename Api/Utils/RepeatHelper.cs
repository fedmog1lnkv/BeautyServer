using System.Diagnostics;

namespace Api.Utils;

public static class RepeatHelper
{
    public static async Task RepeatAction(
        Func<Task> action,
        TimeSpan interval,
        TimeSpan intervalOnError,
        CancellationToken cancellationToken)
    {
        try
        {
            Stopwatch stopwatch = new();
            while (!cancellationToken.IsCancellationRequested)
            {
                stopwatch.Restart();

                TimeSpan delay;
                try
                {
                    await action();
                    delay = interval - stopwatch.Elapsed;
                }
                catch (Exception)
                {
                    delay = intervalOnError - stopwatch.Elapsed;
                }

                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException) { }
    }
}