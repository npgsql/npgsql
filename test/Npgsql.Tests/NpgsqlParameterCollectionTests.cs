using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests;

// This test class has global effects on case sensitive matching in param collection.
[NonParallelizable]
[TestFixture(CompatMode.OnePass)]
[TestFixture(CompatMode.TwoPass)]
public class NpgsqlParameterCollectionTests
{
    readonly CompatMode _compatMode;
    const int LookupThreshold = NpgsqlParameterCollection.LookupThreshold;

    [Test]
    public void Can_only_add_NpgsqlParameter()
    {
        using var command = new NpgsqlCommand();
        Assert.That(() => command.Parameters.Add("hello"), Throws.Exception.TypeOf<InvalidCastException>());
        Assert.That(() => command.Parameters.Add(new SomeOtherDbParameter()), Throws.Exception.TypeOf<InvalidCastException>());
        Assert.That(() => command.Parameters.Add(null!), Throws.Exception.TypeOf<ArgumentNullException>());
    }

    /// <summary>
    /// Test which validates that Clear() indeed cleans up the parameters in a command so they can be added to other commands safely.
    /// </summary>
    [Test]
    public void Clear()
    {
        var p = new NpgsqlParameter();
        var c1 = new NpgsqlCommand();
        var c2 = new NpgsqlCommand();
        c1.Parameters.Add(p);
        Assert.AreEqual(1, c1.Parameters.Count);
        Assert.AreEqual(0, c2.Parameters.Count);
        c1.Parameters.Clear();
        Assert.AreEqual(0, c1.Parameters.Count);
        c2.Parameters.Add(p);
        Assert.AreEqual(0, c1.Parameters.Count);
        Assert.AreEqual(1, c2.Parameters.Count);
    }

    [Test]
    public void Hash_lookup_parameter_rename_bug()
    {
        using var command = new NpgsqlCommand();
        // Put plenty of parameters in the collection to turn on hash lookup functionality.
        for (var i = 0; i < LookupThreshold; i++)
        {
            command.Parameters.AddWithValue(string.Format("p{0:00}", i + 1), NpgsqlDbType.Text, string.Format("String parameter value {0}", i + 1));
        }

        // Make sure hash lookup is generated.
        Assert.AreEqual(command.Parameters["p03"].ParameterName, "p03");

        // Rename the target parameter.
        command.Parameters["p03"].ParameterName = "a_new_name";

        try
        {
            // Try to exploit the hash lookup bug.
            // If the bug exists, the hash lookups will be out of sync with the list, and be unable
            // to find the parameter by its new name.
            Assert.IsTrue(command.Parameters.IndexOf("a_new_name") >= 0);
        }
        catch (Exception e)
        {
            throw new Exception("NpgsqlParameterCollection hash lookup/parameter rename bug detected", e);
        }
    }

    [Test]
    public void Remove_duplicate_parameter([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        if (_compatMode == CompatMode.OnePass)
            return;

        using var command = new NpgsqlCommand();
        // Put plenty of parameters in the collection to turn on hash lookup functionality.
        for (var i = 0; i < count; i++)
        {
            command.Parameters.AddWithValue(string.Format("p{0:00}", i + 1), NpgsqlDbType.Text,
                string.Format("String parameter value {0}", i + 1));
        }

        // Make sure lookup is generated.
        Assert.AreEqual(command.Parameters["p02"].ParameterName, "p02");

        // Add uppercased version causing a list to be created.
        command.Parameters.AddWithValue("P02", NpgsqlDbType.Text, "String parameter value 2");

        // Remove the original parameter by its name causing the multivalue to use a single value again.
        command.Parameters.Remove(command.Parameters["p02"]);

        // Test whether we can still find the last added parameter, and if its index is correctly shifted in the lookup.
        Assert.IsTrue(command.Parameters.IndexOf("p02") == count - 1);
        Assert.IsTrue(command.Parameters.IndexOf("P02") == count - 1);
        // And finally test whether other parameters were also correctly shifted.
        Assert.IsTrue(command.Parameters.IndexOf("p03") == 1);
    }

    [Test]
    public void Remove_parameter([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        using var command = new NpgsqlCommand();
        // Put plenty of parameters in the collection to turn on hash lookup functionality.
        for (var i = 0; i < count; i++)
        {
            command.Parameters.AddWithValue(string.Format("p{0:00}", i + 1), NpgsqlDbType.Text,
                string.Format("String parameter value {0}", i + 1));
        }

        // Make sure lookup is generated.
        Assert.AreEqual(command.Parameters["p02"].ParameterName, "p02");

        // Remove the parameter by its name
        command.Parameters.Remove(command.Parameters["p02"]);

        // Make sure we cannot find it, also not case insensitively.
        Assert.IsTrue(command.Parameters.IndexOf("p02") == -1);
        Assert.IsTrue(command.Parameters.IndexOf("P02") == -1);
    }

    [Test]
    public void Correct_index_returned_for_duplicate_ParameterName([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        if (_compatMode == CompatMode.OnePass)
            return;

        using var command = new NpgsqlCommand();
        // Put plenty of parameters in the collection to turn on hash lookup functionality.
        for (var i = 0; i < count; i++)
        {
            command.Parameters.AddWithValue(string.Format("parameter{0:00}", i + 1), NpgsqlDbType.Text, string.Format("String parameter value {0}", i + 1));
        }

        // Make sure lookup is generated.
        Assert.AreEqual(command.Parameters["parameter02"].ParameterName, "parameter02");

        // Add uppercased version.
        command.Parameters.AddWithValue("Parameter02", NpgsqlDbType.Text, "String parameter value 2");

        // Insert another case insensitive before the original.
        command.Parameters.Insert(0, new NpgsqlParameter("ParameteR02", NpgsqlDbType.Text) { Value = "String parameter value 2" });

        // Try to find the exact index.
        Assert.IsTrue(command.Parameters.IndexOf("parameter02") == 2);
        Assert.IsTrue(command.Parameters.IndexOf("Parameter02") == command.Parameters.Count - 1);
        Assert.IsTrue(command.Parameters.IndexOf("ParameteR02") == 0);
        // This name does not exist so we expect the first case insensitive match to be returned.
        Assert.IsTrue(command.Parameters.IndexOf("ParaMeteR02") == 0);

        // And finally test whether other parameters were also correctly shifted.
        Assert.IsTrue(command.Parameters.IndexOf("parameter03") == 3);
    }

    [Test]
    public void One_pass_finds_case_insensitive_lookups([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        if (_compatMode == CompatMode.TwoPass)
            return;

        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;
        for (var i = 0; i < count; i++)
            parameters.Add(new NpgsqlParameter($"p{i}", i));

        Assert.That(command.Parameters.IndexOf("P1"), Is.EqualTo(1));
    }

    [Test]
    public void One_pass_finds_case_sensitive_lookups([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        if (_compatMode == CompatMode.TwoPass)
            return;

        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;
        for (var i = 0; i < count; i++)
            parameters.Add(new NpgsqlParameter($"p{i}", i));

        Assert.That(command.Parameters.IndexOf("p1"), Is.EqualTo(1));
    }

    [Test]
    public void One_pass_add_does_not_throw_on_positional_duplicate([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        if (_compatMode == CompatMode.TwoPass)
            return;

        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;
        for (var i = 0; i < count; i++)
            Assert.DoesNotThrow(() =>
            {
                parameters.Add(new NpgsqlParameter(NpgsqlParameter.PositionalName, i));
            });
    }

    [Test]
    public void One_pass_insert_does_not_throw_on_positional_duplicate([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        if (_compatMode == CompatMode.TwoPass)
            return;

        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;
        for (var i = 0; i < count; i++)
            Assert.DoesNotThrow(() =>
            {
                parameters.Insert(i, new NpgsqlParameter(NpgsqlParameter.PositionalName, i));
            });
    }

    [Test]
    public void One_pass_name_change_does_not_throw_on_positional_duplicate([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        if (_compatMode == CompatMode.TwoPass)
            return;

        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;
        for (var i = 0; i < count; i++)
            parameters.Add(new NpgsqlParameter("", i));

        parameters.Add(new NpgsqlParameter("p", count));
        parameters[count].ParameterName = NpgsqlParameter.PositionalName;
    }

    [Test]
    public void One_pass_add_throws_on_duplicate([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        if (_compatMode == CompatMode.TwoPass)
            return;

        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;
        for (var i = 0; i < count; i++)
            parameters.Add(new NpgsqlParameter($"p{i}", i));

        Assert.Throws<ArgumentException>(() =>
        {
            command.Parameters.Add(new NpgsqlParameter("p1", 1));
        });
    }

    [Test]
    public void One_pass_insert_throws_on_duplicate([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        if (_compatMode == CompatMode.TwoPass)
            return;

        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;
        for (var i = 0; i < count; i++)
            parameters.Add(new NpgsqlParameter($"p{i}", i));

        Assert.Throws<ArgumentException>(() =>
        {
            command.Parameters.Insert(1, new NpgsqlParameter("p1", 1));
        });
    }

    [Test]
    public void One_pass_name_change_throws_on_duplicate([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        if (_compatMode == CompatMode.TwoPass)
            return;

        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;
        for (var i = 0; i < count; i++)
            parameters.Add(new NpgsqlParameter($"p{i}", i));

        // Same instance, changing its name to what it already is.
        Assert.DoesNotThrow(() =>
        {
            command.Parameters[1].ParameterName = "p1";
        });

        Assert.Throws<ArgumentException>(() =>
        {
            command.Parameters[1].ParameterName = "p2";
        });
    }

    [Test]
    public void Two_pass_add_does_not_throw_on_duplicate([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        if (_compatMode == CompatMode.OnePass)
            return;

        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;
        for (var i = 0; i < count; i++)
            parameters.Add(new NpgsqlParameter($"p{i}", i));

        Assert.DoesNotThrow(() =>
        {
            command.Parameters.Add(new NpgsqlParameter("p1", 1));
        });

        // Positional
        command.Parameters.Clear();

        for (var i = 0; i < count; i++)
            Assert.DoesNotThrow(() =>
            {
                command.Parameters.Add(new NpgsqlParameter(NpgsqlParameter.PositionalName, i));
            });
    }

    [Test]
    public void Two_pass_insert_does_not_throw_on_duplicate([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        if (_compatMode == CompatMode.OnePass)
            return;

        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;
        for (var i = 0; i < count; i++)
            parameters.Add(new NpgsqlParameter($"p{i}", i));

        Assert.DoesNotThrow(() =>
        {
            command.Parameters.Insert(1, new NpgsqlParameter("p1", 1));
        });

        // Positional
        command.Parameters.Clear();

        for (var i = 0; i < count; i++)
            Assert.DoesNotThrow(() =>
            {
                command.Parameters.Insert(i, new NpgsqlParameter(NpgsqlParameter.PositionalName, i));
            });
    }

    [Test]
    public void Two_pass_name_change_does_not_throw_on_duplicate([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        if (_compatMode == CompatMode.OnePass)
            return;

        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;
        for (var i = 0; i < count; i++)
            parameters.Add(new NpgsqlParameter($"p{i}", i));

        // Same instance, changing its name to what it already is.
        Assert.DoesNotThrow(() =>
        {
            command.Parameters[1].ParameterName = "p1";
        });

        Assert.DoesNotThrow(() =>
        {
            command.Parameters[1].ParameterName = "p2";
        });

        // Positional
        command.Parameters.Clear();

        for (var i = 0; i < count; i++)
            command.Parameters.Add(new NpgsqlParameter(NpgsqlParameter.PositionalName, i));

        command.Parameters.Add(new NpgsqlParameter("p", count));
        command.Parameters[count].ParameterName = NpgsqlParameter.PositionalName;
    }


    [Test]
    public void Throws_on_indexer_mismatch([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;
        for (var i = 0; i < count; i++)
            parameters.Add(new NpgsqlParameter($"p{i}", i));

        Assert.DoesNotThrow(() =>
        {
            command.Parameters["p1"] = new NpgsqlParameter("p1", 1);
            command.Parameters["p1"] = new NpgsqlParameter("P1", 1);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            command.Parameters["p1"] = new NpgsqlParameter("p2", 1);
        });
    }

    [Test]
    public void Positional_parameter_lookup_returns_first_match([Values(LookupThreshold, LookupThreshold - 2)] int count)
    {
        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;
        for (var i = 0; i < count; i++)
            parameters.Add(new NpgsqlParameter(NpgsqlParameter.PositionalName, i));

        Assert.That(command.Parameters.IndexOf(""), Is.EqualTo(0));
    }

    [Test]
    public void IndexOf_falls_back_to_first_insensitive_match([Values] bool manyParams)
    {
        if (_compatMode == CompatMode.OnePass)
            return;

        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;

        parameters.Add(new NpgsqlParameter("foo", 8));
        parameters.Add(new NpgsqlParameter("bar", 8));
        parameters.Add(new NpgsqlParameter("BAR", 8));
        Assert.That(parameters, Has.Count.LessThan(LookupThreshold));

        if (manyParams)
            for (var i = 0; i < LookupThreshold; i++)
                parameters.Add(new NpgsqlParameter($"p{i}", i));

        Assert.That(parameters.IndexOf("Bar"), Is.EqualTo(1));
    }

    [Test]
    public void IndexOf_prefers_case_sensitive_match([Values] bool manyParams)
    {
        if (_compatMode == CompatMode.OnePass)
            return;

        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;

        parameters.Add(new NpgsqlParameter("FOO", 8));
        parameters.Add(new NpgsqlParameter("foo", 8));
        Assert.That(parameters, Has.Count.LessThan(LookupThreshold));

        if (manyParams)
            for (var i = 0; i < LookupThreshold; i++)
                parameters.Add(new NpgsqlParameter($"p{i}", i));

        Assert.That(parameters.IndexOf("foo"), Is.EqualTo(1));
    }

    [Test]
    public void IndexOf_matches_all_parameter_syntaxes()
    {
        using var command = new NpgsqlCommand();
        var parameters = command.Parameters;

        parameters.Add(new NpgsqlParameter("@foo0", 8));
        parameters.Add(new NpgsqlParameter(":foo1", 8));
        parameters.Add(new NpgsqlParameter("foo2", 8));

        for (var i = 0; i < parameters.Count; i++)
        {
            Assert.That(parameters.IndexOf("foo" + i), Is.EqualTo(i));
            Assert.That(parameters.IndexOf("@foo" + i), Is.EqualTo(i));
            Assert.That(parameters.IndexOf(":foo" + i), Is.EqualTo(i));
        }
    }

    [Test]
    public void Clean_name()
    {
        var param = new NpgsqlParameter();
        var command = new NpgsqlCommand();
        command.Parameters.Add(param);

        param.ParameterName = null;

        // These should not throw exceptions
        Assert.AreEqual(0, command.Parameters.IndexOf(param.ParameterName));
        Assert.AreEqual(NpgsqlParameter.PositionalName, param.ParameterName);
    }

    public NpgsqlParameterCollectionTests(CompatMode compatMode)
    {
        _compatMode = compatMode;

#if DEBUG
        NpgsqlParameterCollection.TwoPassCompatMode = compatMode == CompatMode.TwoPass;
#else
            if (compatMode == CompatMode.TwoPass)
                Assert.Ignore("Cannot test case-insensitive NpgsqlParameterCollection behavior in RELEASE");
#endif
    }

    class SomeOtherDbParameter : DbParameter
    {
        public override void ResetDbType() {}

        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }
        [AllowNull] public override string ParameterName { get; set; } = "";
        [AllowNull] public override string SourceColumn { get; set; } = "";
        public override object? Value { get; set; }
        public override bool SourceColumnNullMapping { get; set; }
        public override int Size { get; set; }
    }
}

public enum CompatMode
{
    TwoPass,
    OnePass
}
