---
layout: page
title: Documentation
---

*Note: The documentation below is for Npgsql 3.0, which is still in beta.
[See here](manual-2.2.html) for the older 2.2 documentation.*

## Intro

Npgsql aims to be fully ADO.NET-compatible, it's API should feel almost identical to other
.NET database drivers. Here's a basic code snippet to get you started.

{% highlight C# %}

using (var conn = new NpgsqlConnection("Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase"))
{
    conn.Open();
    using (var cmd = new NpgsqlCommand())
    {
        cmd.Connection = conn;

        // Insert some data
        cmd.CommandText = "INSERT INTO data (some_field) VALUES ('Hello world')";
        cmd.ExecuteNonQuery();

        // Retrieve all rows
        cmd.CommandText = "SELECT some_field FROM data";
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine(reader.GetString(0));
            }
        }
    }
}

{% endhighlight %}

You can find more info about the ADO.NET API in the [MSDN docs](https://msdn.microsoft.com/en-us/library/h43ks021(v=vs.110).aspx)
or in many tutorials on the Internet.

## Other Subjects

[Migration guide from 2.2 to 3.0](migration-3.0.html)

[Connection String Parameters](connection-string-parameters.html)

[Copy (bulk data transfer)](copy.html)

[Asynchronous notifications and Continuous Processing](continuous-processing.html)

[Logging](logging.html)

[Date and Time](datetime.html)

[Developer Resources](dev.html)

## Still to be written

[SSL (or TLS)?, Kerberos](ssl.html)

[Entity Framework](ef.html)

[Entity Framework 7](ef7.html)

