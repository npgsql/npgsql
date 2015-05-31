---
layout: page
title: Asynchronous Notifications, Continuous Processing, Asynchronous Messages and Keepalives
---

## Asynchronous PostgreSQL messages

PostgreSQL has a feature whereby arbitrary notification messages can be sent between clients. For example, one client may wait until it is
notified by another client of a task that it is supposed to perform. Notifications are, by their nature, asynchronous - they can arrive
at any point. For more detail about this feature, see the PostgreSQL [NOTIFY command](http://www.postgresql.org/docs/current/static/sql-notify.html).
Some other asynchronous message types are notices (e.g. database shutdown imminent) and parameter changes, see the
[PostgreSQL protocol docs](http://www.postgresql.org/docs/current/static/protocol-flow.html#PROTOCOL-ASYNC) for more details.

## Processing of Asynchronous Messages

Npgsql exposes notification messages via the Notification event on NpgsqlConnection.
Since asynchronous notifications are rarely used and processing them is complex, by default Npgsql only processes notification messages as
part of regular (synchronous) query interaction. That is, if an asynchronous notification is sent, Npgsql will only process it and emit an
event to the user the next time a command is sent and processed. To make Npgsql process messages at any time, even if no query is in
progress, enable the [Continuous Processing](connection-string-parameters.html#continuous-processing) flag in your connection string. 

## Keepalive

Clients in continuous processing mode can sometimes wait for hours, even days before getting a single notification. In this scenario,
how can the client know the connection is still up, and hasn't been broken by a server or network outage? For this purpose, Npgsql
has a keepalive feature, which makes it send periodic `SELECT 1` queries. This feature is by default disabled, and must be enabled via
the [Keepalive](connection-string-parameter.html#keepalive) connection string parameter, setting the number of seconds between each
keepalive. Note that Npgsql does not turn on TCP keepalive because that feature isn't universally reliable across all network
equipment.

