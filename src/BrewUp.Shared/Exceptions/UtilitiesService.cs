using Microsoft.Extensions.Logging;

namespace BrewUp.Shared.Exceptions;

public static class UtilitiesService
{
    public static string GetErrorMessage(Exception ex)
    {
        while (true)
        {
            if (ex.InnerException == null)
                return
                    $"Source: {ex.Source} StackTrace: {ex.StackTrace} Message: {ex.Message}, Source: {ex.Source}, Trace: {ex.StackTrace};";

            ex = ex.InnerException;
        }
    }

    public static void LogError(Exception ex, ILogger logger)
    {
        logger.LogError(GetErrorMessage(ex));
    }
}