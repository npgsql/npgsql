#if NET461
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.Principal;

namespace Npgsql
{
    static class WindowsUsernameProvider
    {
        class CachedUpn
        {
            internal CachedUpn(string upn, DateTime expiryTimeUtc)
            {
                Upn = upn;
                ExpiryTimeUtc = expiryTimeUtc;
            }

            internal string Upn;
            internal DateTime ExpiryTimeUtc;
        }

        static readonly Dictionary<SecurityIdentifier, CachedUpn> CachedUpns = new Dictionary<SecurityIdentifier, CachedUpn>();

        internal static string? GetUsername(bool includeRealm)
        {
            // Side note: This maintains the hack fix mentioned before for https://github.com/npgsql/Npgsql/issues/133.
            // In a nutshell, starting with .NET 4.5 WindowsIdentity inherits from ClaimsIdentity
            // which doesn't exist in mono, and calling a WindowsIdentity method bombs.
            // The workaround is that this function that actually deals with WindowsIdentity never
            // gets called on mono, so never gets JITted and the problem goes away.

            // Gets the current user's username for integrated security purposes
            var identity = WindowsIdentity.GetCurrent();
            if (identity.User == null)
                return null;
            CachedUpn cachedUpn;
            string? upn = null;

            // Check to see if we already have this UPN cached
            lock (CachedUpns)
            {
                if (CachedUpns.TryGetValue(identity.User, out cachedUpn))
                {
                    if (cachedUpn.ExpiryTimeUtc > DateTime.UtcNow)
                        upn = cachedUpn.Upn;
                    else
                        CachedUpns.Remove(identity.User);
                }
            }

            try
            {
                if (upn == null)
                {
                    // Try to get the user's UPN in its correct case; this is what the
                    // server will need to verify against a Kerberos/SSPI ticket

                    // If the computer does not belong to a domain, returns Empty.
                    var domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                    if (domainName.Equals(string.Empty))
                        return GetWindowsIdentityUserName(includeRealm);

                    // First, find a domain server we can talk to
                    string domainHostName;

                    using (var rootDse = new DirectoryEntry("LDAP://rootDSE") { AuthenticationType = AuthenticationTypes.Secure })
                        domainHostName = (string)rootDse.Properties["dnsHostName"].Value;

                    // Query the domain server by the current user's SID
                    using var entry = new DirectoryEntry("LDAP://" + domainHostName) { AuthenticationType = AuthenticationTypes.Secure };
                    using var search = new DirectorySearcher(entry, "(objectSid=" + identity.User.Value + ")", new[] { "userPrincipalName" });

                    upn = (string)search.FindOne().Properties["userPrincipalName"][0];
                }

                if (cachedUpn == null)
                {
                    // Save this value
                    cachedUpn = new CachedUpn(upn, DateTime.UtcNow.AddHours(3.0));

                    lock (CachedUpns)
                    {
                        CachedUpns[identity.User] = cachedUpn;
                    }
                }

                var upnParts = upn.Split('@');
                return includeRealm
                    ? upnParts[0] + "@" + upnParts[1].ToUpperInvariant() // Make it Kerberos-y by uppercasing the realm part
                    : upnParts[0];
            }
            catch
            {
                // Querying the directory failed, so return the SAM name
                // (which probably won't work, but it's better than nothing)
                return GetWindowsIdentityUserName(includeRealm);
            }
        }

        static string GetWindowsIdentityUserName(bool includeRealm)
        {
            var s = WindowsIdentity.GetCurrent().Name;
            if (s == null)
                return string.Empty;
            var machineAndUser = s.Split('\\');
            return includeRealm ? $"{machineAndUser[1]}@{machineAndUser[0]}" : machineAndUser[1];
        }
    }
}
#endif
