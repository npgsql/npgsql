---
layout: doc
title: Internal Implementation Notes
---

A single command can contain multiple SQL queries in its CommandText.
Npgsql implements this by splitting the CommandText on semicolons and sending the queries via the PostgreSQL extended protocol.
The message chain for a non-prepared multiple-query command is as follows:

P/D/B/P/D/B/E/C/E/C/S

The reason we first prepare and bind all queries, and only then start executing them, is to avoid a deadlock
(big thanks to Emil Lenngren for pointing this out). Imagine two queries which are sent with the following message chain:

P/D/B/E/P/D/B/E/S

The backend will start writing the first query's resultset to us after the first execute. If that resultset is large, the
backend will block since we're not reading any data yet. On the other hand, if the second query itself happens to be big,
we might block, waiting for the backend to read it - deadlock. Sending all the queries before starting the execute any
ensures that the backend reads them all before starting to write anything.


