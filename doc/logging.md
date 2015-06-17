---
layout: page
title: Logging
---

Npgsql includes a built-in feature for outputting logging events which can help debug issues.

Npgsql logging is disabled by default and must be turned on. Logging can be turned on by setting `NpgsqlLogManager.Provider` to a class implementing the `INpgsqlLoggingProvider` interface. Npgsql comes with a console implementation which can be set up as follows:

{% highlight C# %}

NpgsqlLogManager.Provider = new ???

{% endhighlight %}

*Note: you must set the logging provider *before* invoking any other Npgsql method, at the very start of your program.*

It's trivial to create a logging provider that passes log messages to whatever logging framework you use. You can find such an adaptor for NLog [here](http://ni).

*Note:* the logging API is a first implementation and will probably improve/change - don't treat it as a stable part of the Npgsql API. Let us know if you think there are any missing messages or features!
