# Diagnostics overview

Npgsql provides several ways to analyze what's going on inside Npgsql and to diagnose performance issues. Each has its own dedicated doc page:

* [**Tracing**](tracing.md) allows collecting information on which queries are executed, including precise timing information on start, end and duration. These events can be collected in a database, searched, graphically explored and otherwise analyzed.
* [**Logging**](logging.md) generates textual information on various events within Npgsql; log levels can be adjusted to collect low-level information, helpful for diagnosing errors.
* [**Metrics**](metrics.md) generates aggregated quantitative data, useful for tracking the performance of your application in realtime and over time (e.g. how many queries are currently being executed in a particular moment).

For information on the exceptions thrown by Npgsql, and on notices produced by PostgreSQL, [see this page](exceptions_notices.md).
