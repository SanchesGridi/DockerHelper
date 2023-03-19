#nullable enable

namespace DockerHelper.Core.Extensions;

public static class TaskExtensions
{
    public static async void Await(this Task @this, bool throwAnyway = false, Action<Exception>? handler = null)
    {
        try
        {
            if (@this == null)
            {
                throw new ArgumentNullException("@this");
            }
            await @this;
        }
        catch (TaskCanceledException ex)
        {
            handler?.Invoke(ex);
            if (throwAnyway)
            {
                throw;
            }
        }
        catch (OperationCanceledException ex)
        {
            handler?.Invoke(ex);
            if (throwAnyway)
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            handler?.Invoke(ex);
            if (throwAnyway)
            {
                throw;
            }
        }
    }
}
