using Microsoft.Extensions.Logging;
using DotNetEnv; 

namespace Codemy.BuildingBlocks.Core.Extensions
{
    public static class LogExtensions
    {
        public static void LoadEnvFile(ILogger? logger = null)
        {
            var currentDir = AppContext.BaseDirectory;
            string? envPath = null;

            while (currentDir != null)
            {
                var possiblePath = Path.Combine(currentDir, ".env");
                if (File.Exists(possiblePath))
                {
                    envPath = possiblePath;
                    break;
                }
                currentDir = Directory.GetParent(currentDir)?.FullName;
            }

            if (envPath != null)
            {
                DotNetEnv.Env.Load(envPath);
                logger?.LogInformation($".env loaded from: {envPath}");
            }
            else
            {
                logger?.LogWarning(".env file not found in any parent directory.");
            }
        }

    }
}
