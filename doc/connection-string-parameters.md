---
layout: page
title: Connection String Parameters
---

Parameter keywords are case-insensitive.

<table border="1">
  <thead>
    <tr>
      <th>Keyword</th>
      <th>Description</th>
    </tr>
  </thead>

  <tbody>
    <tr><td>Connection</td></tr>

    <tr>
      <td>Host</td>
      <td>The hostname or IP address of the PostgreSQL server to connect to.</td>
    <tr>

    <tr>
      <td>Port</td>
      <td>The TCP port of the PostgreSQL server.</td>
    <tr>

    <tr>
      <td>Database</td>
      <td>The PostgreSQL database to connect to.</td>
    </tr>

    <tr>
      <td>Username</td>
      <td>The username to connect with. Not required if using IntegratedSecurity.</td>
    </tr>

    <tr>
      <td>Password</td>
      <td>The password to connect with. Not required if using IntegratedSecurity.</td>
    </tr>

    <tr>
      <td>Application Name</td>
      <td>The optional application name parameter to be sent to the backend during connection initiation.</td>
    </tr>

    <tr>
      <td>Enlist</td>
      <td>Whether to enlist in an ambient TransactionScope.</td>
    </tr>

    <tr>
      <td>SearchPath</td>
      <td>Gets or sets the schema search path.</td>
    </tr>

  </tbody>

  <tbody>
    <tr><td>Security</td></tr>

    <tr>
      <td>SSLMode</td>
      <td>Controls whether SSL is required, disabled or preferred, depending on server support.</td>
    </tr>

    <tr id="trust-server-certificate">
      <td>Trust Server Certificate</td>
      <td>Whether to trust the server certificate without validating it.</td>
    </tr>

    <tr>
      <td>Use SSL Stream</td>
      <td>Npgsql uses its own internal implementation of TLS/SSL. Turn this on to use .NET SslStream instead.</td>
    </tr>

    <tr>
      <td>Integrated Security</td>
      <td>Whether to use Windows integrated security to log in.</td>
    </tr>

    <tr>
      <td>Kerberos Service Name</td>
      <td>The Kerberos service name to be used for authentication.</td>
    </tr>

    <tr>
      <td>Include Realm</td>
      <td>The Kerberos realm to be used for authentication.</td>
    </tr>

  </tbody>

  <tbody>
    <tr><td>Pooling</td></tr>

    <tr>
      <td>Pooling</td>
      <td>Whether connection pooling should be used.</td>
    </tr>

    <tr>
      <td>Minimum Pool Size</td>
      <td>The minimum connection pool size.</td>
    </tr>

    <tr>
      <td>Maximum Pool Size</td>
      <td>The maximum connection pool size.</td>
    </tr>

    <tr>
      <td>Connection Lifetime</td>
      <td>The time to wait before closing unused connections in the pool if the count of all connections exeeds MinPoolSize.</td>
    </tr>

  </tbody>

  <tbody>
    <tr><td>Entity Framework</td></tr>

    <tr>
      <td>EF Template Database</td>
      <td>The database template to specify when creating a database in Entity Framework. If not specified, PostgreSQL defaults to template1.</td>
    </tr>

  </tbody>

  <tbody>
    <tr><td>Advanced</td></tr>

    <tr id="continuous-processing">
      <td>Continuous Processing</td>
      <td>Whether to process messages that arrive between command activity.</td>
    </tr>

    <tr id="keepalive">
      <td>Keepalive</td>
      <td>The number of seconds of connection inactivity before Npgsql sends a keepalive query.</td>
    </tr>

    <tr>
      <td>Buffer Size</td>
      <td>Determines the size of the internal buffer Npgsql uses when reading or writing. Increasing may improve performance if transferring large values from the database.</td>
    </tr>

  </tbody>

  <tbody>
    <tr><td>Compatibility</td></tr>

    <tr>
      <td>Server Compatibility Mode</td>
      <td>A compatibility mode for special PostgreSQL server types.</td>
    </tr>

    <tr>
      <td>Convert Infinity DateTime</td>
      <td>Makes MaxValue and MinValue timestamps and dates readable as infinity and negative infinity.</td>
    </tr>
  </tbody>

<table>
