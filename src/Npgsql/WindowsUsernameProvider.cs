#if NET45 || NET451
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.Principal;
using JetBrains.Annotations;

namespace Npgsql
{
    static class WindowsUsernameProvider
    {
        class CachedUpn
        {
            internal string Upn;
            internal DateTime ExpiryTimeUtc;
        }

        static readonly Dictionary<SecurityIdentifier, CachedUpn> CachedUpns = new Dictionary<SecurityIdentifier, CachedUpn>();

        [CanBeNull]
        internal static string GetUsername(bool includeRealm)
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
            string upn = null;

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
                    string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                    if (domainName.Equals(string.Empty))
                    {
                        return GetWindowsIdentityUserName(includeRealm);
                    }

                    // First, find a domain server we can talk to
                    string domainHostName;

                    using (DirectoryEntry rootDse = new DirectoryEntry("LDAP://rootDSE") { AuthenticationType = AuthenticationTypes.Secure })
                    {
                        domainHostName = (string)rootDse.Properties["dnsHostName"].Value;
                    }

                    // Query the domain server by the current user's SID
                    using (DirectoryEntry entry = new DirectoryEntry("LDAP://" + domainHostName) { AuthenticationType = AuthenticationTypes.Secure })
                    {
                        DirectorySearcher search = new DirectorySearcher(entry,
                            "(objectSid=" + identity.User.Value + ")", new[] { "userPrincipalName" });

                        SearchResult result = search.FindOne();

                        upn = (string)result.Properties["userPrincipalName"][0];
                    }
                }

                if (cachedUpn == null)
                {
                    // Save this value
                    cachedUpn = new CachedUpn() { Upn = upn, ExpiryTimeUtc = DateTime.UtcNow.AddHours(3.0) };

                    lock (CachedUpns)
                    {
                        CachedUpns[identity.User] = cachedUpn;
                    }
                }

                string[] upnParts = upn.Split('@');

                if (includeRealm)
                {
                    // Make it Kerberos-y by uppercasing the realm part
                    return upnParts[0] + "@" + upnParts[1].ToUpperInvariant();
                }
                else
                {
                    return upnParts[0];
                }
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
