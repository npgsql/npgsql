---
layout: doc
title: Connection String Parameters
---

Parameter keywords are case-insensitive.

<table border="1">
  <thead>
    <tr>
      <th>Keyword</th>
      <th>Description</th>
      <th>Default</th>
    </tr>
  </thead>

  <tbody>

    <tr>
      <td>Host</td>
      <td>The hostname or IP address of the PostgreSQL server to connect to.</td>
      <td><strong>Required<strong></td>
    </tr>

    <tr>
      <td>Port</td>
      <td>The TCP port of the PostgreSQL server.</td>
      <td>5432</td>
    </tr>

    <tr>
      <td>Database</td>
      <td>The PostgreSQL database to connect to.</td>
      <td><strong>Required</strong></td>
    </tr>

    <tr>
      <td>Username</td>
      <td>The username to connect with. Not required if using IntegratedSecurity.</td>
      <td></td>
    </tr>

    <tr>
      <td>Password</td>
      <td>The password to connect with. Not required if using IntegratedSecurity.</td>
      <td></td>
    </tr>

    <tr>
      <td>Application Name</td>
      <td>The optional application name parameter to be sent to the backend during connection initiation.</td>
      <td></td>
    </tr>

    <tr>
      <td>Enlist</td>
      <td>Whether to enlist in an ambient TransactionScope.</td>
      <td>false</td>
    </tr>

    <tr>
      <td>Search Path</td>
      <td>Gets or sets the schema search path.</td>
      <td></td>
    </tr>

    <tr>
      <td>SSL Mode</td>
      <td>Controls whether SSL is required, disabled or preferred, depending on server support.</td>
      <td>Disabled</td>
    </tr>

    <tr id="trust-server-certificate">
      <td>Trust Server Certificate</td>
      <td>Whether to trust the server certificate without validating it.</td>
      <td>false</td>
    </tr>

    <tr>
      <td>Use SSL Stream</td>
      <td>Npgsql uses its own internal implementation of TLS/SSL. Turn this on to use .NET SslStream instead.</td>
      <td>false</td>
    </tr>

    <tr>
      <td>Integrated Security</td>
      <td>Whether to use Windows integrated security to log in.</td>
      <td>false</td>
    </tr>

    <tr>
      <td>Kerberos Service Name</td>
      <td>The Kerberos service name to be used for authentication.</td>
      <td></td>
    </tr>

    <tr>
      <td>Include Realm</td>
      <td>The Kerberos realm to be used for authentication.</td>
      <td></td>
    </tr>

    <tr>
      <td>Pooling</td>
      <td>Whether connection pooling should be used.</td>
      <td>true</td>
    </tr>

    <tr>
      <td>Minimum Pool Size</td>
      <td>The minimum connection pool size.</td>
      <td>1</td>
    </tr>

    <tr>
      <td>Maximum Pool Size</td>
      <td>The maximum connection pool size.</td>
      <td>20</td>
    </tr>

    <tr>
      <td>Connection Lifetime</td>
      <td>The time to wait before closing unused connections in the pool if the count of all connections exeeds MinPoolSize.</td>
      <td>15</td>
    </tr>

    <tr>
      <td>EF Template Database</td>
      <td>The database template to specify when creating a database in Entity Framework. If not specified, PostgreSQL defaults to template1.</td>
      <td>template1</td>
    </tr>

    <tr id="continuous-processing">
      <td>Continuous Processing</td>
      <td>Whether to process messages that arrive between command activity.</td>
      <td>false</td>
    </tr>

    <tr id="keepalive">
      <td>Keepalive</td>
      <td>The number of seconds of connection inactivity before Npgsql sends a keepalive query.</td>
      <td>disabled</td>
    </tr>

    <tr>
      <td>Timeout</td>
      <td>The time to wait (in seconds) while trying to establish a connection before terminating the attempt and generating an error.</td>
      <td>15</td>
    </tr>

    <tr>
      <td>Command Timeout</td>
      <td>The time to wait (in seconds) while trying to execute a command before terminating the attempt and generating an error. Set to zero for infinity.</td>
      <td>30</td>
    </tr>

    <tr>
      <td>Internal Command Timeout</td>
      <td>The time to wait (in seconds) while trying to execute a an internal command before terminating the attempt and generating an error. -1 uses CommandTimeout, 0 means no timeout.</td>
      <td>-1</td>
    </tr>

    <tr>
      <td>Backend Timeouts</td>
      <td>Whether to have the backend enforce CommandTimeout and InternalCommandTimeout via the statement_timeout variable.</td>
      <td>true</td>
    </tr>

    <tr>
      <td>Buffer Size</td>
      <td>Determines the size of the internal buffer Npgsql uses when reading or writing. Increasing may improve performance if transferring large values from the database.</td>
      <td>8192</td>
    </tr>

    <tr>
      <td>Server Compatibility Mode</td>
      <td>A compatibility mode for special PostgreSQL server types.</td>
      <td>off</td>
    </tr>

    <tr>
      <td>Convert Infinity DateTime</td>
      <td>Makes MaxValue and MinValue timestamps and dates readable as infinity and negative infinity.</td>
      <td>false</td>
    </tr>

  </tbody>

<table>
