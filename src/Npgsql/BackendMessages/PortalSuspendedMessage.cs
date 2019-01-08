namespace Npgsql.BackendMessages
{
    class PortalSuspendedMessage : IBackendMessage
    {
        public BackendMessageCode Code => BackendMessageCode.PortalSuspended;
        internal static readonly PortalSuspendedMessage Instance = new PortalSuspendedMessage();
        PortalSuspendedMessage() { }
    }
}
