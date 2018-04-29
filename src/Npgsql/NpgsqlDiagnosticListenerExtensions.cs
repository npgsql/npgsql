using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Npgsql
{
    internal static class NpgsqlDiagnosticListenerExtensions
    {
        public const string DiagnosticListenerName = "NpgsqlDiagnosticListener";

        private const string NpgsqlClientPrefix = "Npgsql.";
        
        public const string NpgsqlBeforeExecuteCommand = NpgsqlClientPrefix + nameof(WriteCommandBefore);
        public const string NpgsqlAfterExecuteCommand = NpgsqlClientPrefix + nameof(WriteCommandAfter);
        public const string NpgsqlErrorExecuteCommand = NpgsqlClientPrefix + nameof(WriteCommandError);
        
        public static Guid WriteCommandBefore(this DiagnosticListener @this, NpgsqlCommand command, [CallerMemberName] string operation = "")
        {
            if (!@this.IsEnabled(NpgsqlBeforeExecuteCommand))
            {
                return Guid.Empty;
            }
            
            var operationId = Guid.NewGuid();

            @this.Write(
                NpgsqlBeforeExecuteCommand,
                new
                {
                    OperationId = operationId,
                    Operation = operation,
                    ConnectionId = command.Connection?.Connector?.Id,
                    Command = command
                });

            return operationId;
        }
        
        public static void WriteCommandAfter(this DiagnosticListener @this, Guid operationId, NpgsqlCommand command, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlAfterExecuteCommand))
            {
                @this.Write(
                    NpgsqlAfterExecuteCommand,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        ConnectionId = command.Connection?.Connector?.Id,
                        Command = command,
                        Timestamp = Stopwatch.GetTimestamp()
                    });
            }
        }
        
        public static void WriteCommandError(this DiagnosticListener @this, Guid operationId, NpgsqlCommand command, Exception ex, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlErrorExecuteCommand))
            {
                @this.Write(
                    NpgsqlErrorExecuteCommand,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        ConnectionId = command.Connection?.Connector?.Id,
                        Command = command,
                        Exception = ex,
                        Timestamp = Stopwatch.GetTimestamp()
                    });
            }
        }
    }
}
