namespace Microsoft.Extensions.Logging
{
    internal static class LoggingExtensions
    {
        public const string CommandsLoggerName = "EntityFramework.Commands";

        public static ILogger CreateCommandsLogger(this ILoggerFactory loggerFactory)
            => loggerFactory.CreateLogger(CommandsLoggerName);

        public static ILogger CreateCommandsLogger(this ILoggerProvider loggerProvider)
            => loggerProvider.CreateLogger(CommandsLoggerName);
    }
}
