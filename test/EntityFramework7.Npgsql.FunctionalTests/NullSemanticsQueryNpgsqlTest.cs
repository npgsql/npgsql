﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.FunctionalTests;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class NullSemanticsQueryNpgsqlTest : NullSemanticsQueryTestBase<NpgsqlTestStore, NullSemanticsQueryNpgsqlFixture>
    {
        public NullSemanticsQueryNpgsqlTest(NullSemanticsQueryNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        public override void Compare_bool_with_bool_equal()
        {
            base.Compare_bool_with_bool_equal();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" = ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" = ""e"".""NullableBoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableBoolA"" = ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" = ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL AND ""e"".""NullableBoolB"" IS NULL)",
                Sql);
        }

        public override void Compare_negated_bool_with_bool_equal()
        {
            base.Compare_negated_bool_with_bool_equal();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" <> ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""BoolA"" <> ""e"".""NullableBoolB"") AND ""e"".""NullableBoolB"" IS NOT NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" <> ""e"".""BoolB"") AND ""e"".""NullableBoolA"" IS NOT NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableBoolA"" <> ""e"".""NullableBoolB"") AND (""e"".""NullableBoolA"" IS NOT NULL AND ""e"".""NullableBoolB"" IS NOT NULL)) OR (""e"".""NullableBoolA"" IS NULL AND ""e"".""NullableBoolB"" IS NULL)",
                Sql);
        }

        public override void Compare_bool_with_negated_bool_equal()
        {
            base.Compare_bool_with_negated_bool_equal();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" <> ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""BoolA"" <> ""e"".""NullableBoolB"") AND ""e"".""NullableBoolB"" IS NOT NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" <> ""e"".""BoolB"") AND ""e"".""NullableBoolA"" IS NOT NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableBoolA"" <> ""e"".""NullableBoolB"") AND (""e"".""NullableBoolA"" IS NOT NULL AND ""e"".""NullableBoolB"" IS NOT NULL)) OR (""e"".""NullableBoolA"" IS NULL AND ""e"".""NullableBoolB"" IS NULL)",
                Sql);
        }

        public override void Compare_negated_bool_with_negated_bool_equal()
        {
            base.Compare_negated_bool_with_negated_bool_equal();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" = ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""BoolA"" = ""e"".""NullableBoolB"") AND ""e"".""NullableBoolB"" IS NOT NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" = ""e"".""BoolB"") AND ""e"".""NullableBoolA"" IS NOT NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableBoolA"" = ""e"".""NullableBoolB"") AND (""e"".""NullableBoolA"" IS NOT NULL AND ""e"".""NullableBoolB"" IS NOT NULL)) OR (""e"".""NullableBoolA"" IS NULL AND ""e"".""NullableBoolB"" IS NULL)",
                Sql);
        }

        public override void Compare_bool_with_bool_equal_negated()
        {
            base.Compare_bool_with_bool_equal_negated();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" <> ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""BoolA"" <> ""e"".""NullableBoolB"") OR ""e"".""NullableBoolB"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" <> ""e"".""BoolB"") OR ""e"".""NullableBoolA"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableBoolA"" <> ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL OR ""e"".""NullableBoolB"" IS NULL)) AND (""e"".""NullableBoolA"" IS NOT NULL OR ""e"".""NullableBoolB"" IS NOT NULL)",
                Sql);
        }

        public override void Compare_negated_bool_with_bool_equal_negated()
        {
            base.Compare_negated_bool_with_bool_equal_negated();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" = ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""BoolA"" = ""e"".""NullableBoolB"") OR ""e"".""NullableBoolB"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" = ""e"".""BoolB"") OR ""e"".""NullableBoolA"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableBoolA"" = ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL OR ""e"".""NullableBoolB"" IS NULL)) AND (""e"".""NullableBoolA"" IS NOT NULL OR ""e"".""NullableBoolB"" IS NOT NULL)",
                Sql);
        }

        public override void Compare_bool_with_negated_bool_equal_negated()
        {
            base.Compare_bool_with_negated_bool_equal_negated();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" = ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""BoolA"" = ""e"".""NullableBoolB"") OR ""e"".""NullableBoolB"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" = ""e"".""BoolB"") OR ""e"".""NullableBoolA"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableBoolA"" = ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL OR ""e"".""NullableBoolB"" IS NULL)) AND (""e"".""NullableBoolA"" IS NOT NULL OR ""e"".""NullableBoolB"" IS NOT NULL)",
                Sql);
        }

        public override void Compare_negated_bool_with_negated_bool_equal_negated()
        {
            base.Compare_negated_bool_with_negated_bool_equal_negated();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" <> ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""BoolA"" <> ""e"".""NullableBoolB"") OR ""e"".""NullableBoolB"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" <> ""e"".""BoolB"") OR ""e"".""NullableBoolA"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableBoolA"" <> ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL OR ""e"".""NullableBoolB"" IS NULL)) AND (""e"".""NullableBoolA"" IS NOT NULL OR ""e"".""NullableBoolB"" IS NOT NULL)",
                Sql);
        }

        public override void Compare_bool_with_bool_not_equal()
        {
            base.Compare_bool_with_bool_not_equal();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" <> ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""BoolA"" <> ""e"".""NullableBoolB"") OR ""e"".""NullableBoolB"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" <> ""e"".""BoolB"") OR ""e"".""NullableBoolA"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableBoolA"" <> ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL OR ""e"".""NullableBoolB"" IS NULL)) AND (""e"".""NullableBoolA"" IS NOT NULL OR ""e"".""NullableBoolB"" IS NOT NULL)",
                Sql);
        }

        public override void Compare_negated_bool_with_bool_not_equal()
        {
            base.Compare_negated_bool_with_bool_not_equal();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" = ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""BoolA"" = ""e"".""NullableBoolB"") OR ""e"".""NullableBoolB"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" = ""e"".""BoolB"") OR ""e"".""NullableBoolA"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableBoolA"" = ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL OR ""e"".""NullableBoolB"" IS NULL)) AND (""e"".""NullableBoolA"" IS NOT NULL OR ""e"".""NullableBoolB"" IS NOT NULL)",
                Sql);
        }

        public override void Compare_bool_with_negated_bool_not_equal()
        {
            base.Compare_bool_with_negated_bool_not_equal();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" = ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""BoolA"" = ""e"".""NullableBoolB"") OR ""e"".""NullableBoolB"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" = ""e"".""BoolB"") OR ""e"".""NullableBoolA"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableBoolA"" = ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL OR ""e"".""NullableBoolB"" IS NULL)) AND (""e"".""NullableBoolA"" IS NOT NULL OR ""e"".""NullableBoolB"" IS NOT NULL)",
                Sql);
        }

        public override void Compare_negated_bool_with_negated_bool_not_equal()
        {
            base.Compare_negated_bool_with_negated_bool_not_equal();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" <> ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""BoolA"" <> ""e"".""NullableBoolB"") OR ""e"".""NullableBoolB"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" <> ""e"".""BoolB"") OR ""e"".""NullableBoolA"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableBoolA"" <> ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL OR ""e"".""NullableBoolB"" IS NULL)) AND (""e"".""NullableBoolA"" IS NOT NULL OR ""e"".""NullableBoolB"" IS NOT NULL)",
                Sql);
        }

        public override void Compare_bool_with_bool_not_equal_negated()
        {
            base.Compare_bool_with_bool_not_equal_negated();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" = ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" = ""e"".""NullableBoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableBoolA"" = ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" = ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL AND ""e"".""NullableBoolB"" IS NULL)",
                Sql);
        }

        public override void Compare_negated_bool_with_bool_not_equal_negated()
        {
            base.Compare_negated_bool_with_bool_not_equal_negated();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" <> ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""BoolA"" <> ""e"".""NullableBoolB"") AND ""e"".""NullableBoolB"" IS NOT NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" <> ""e"".""BoolB"") AND ""e"".""NullableBoolA"" IS NOT NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableBoolA"" <> ""e"".""NullableBoolB"") AND (""e"".""NullableBoolA"" IS NOT NULL AND ""e"".""NullableBoolB"" IS NOT NULL)) OR (""e"".""NullableBoolA"" IS NULL AND ""e"".""NullableBoolB"" IS NULL)",
                Sql);
        }

        public override void Compare_bool_with_negated_bool_not_equal_negated()
        {
            base.Compare_bool_with_negated_bool_not_equal_negated();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" <> ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""BoolA"" <> ""e"".""NullableBoolB"") AND ""e"".""NullableBoolB"" IS NOT NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" <> ""e"".""BoolB"") AND ""e"".""NullableBoolA"" IS NOT NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableBoolA"" <> ""e"".""NullableBoolB"") AND (""e"".""NullableBoolA"" IS NOT NULL AND ""e"".""NullableBoolB"" IS NOT NULL)) OR (""e"".""NullableBoolA"" IS NULL AND ""e"".""NullableBoolB"" IS NULL)",
                Sql);
        }

        public override void Compare_negated_bool_with_negated_bool_not_equal_negated()
        {
            base.Compare_negated_bool_with_negated_bool_not_equal_negated();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" = ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""BoolA"" = ""e"".""NullableBoolB"") AND ""e"".""NullableBoolB"" IS NOT NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" = ""e"".""BoolB"") AND ""e"".""NullableBoolA"" IS NOT NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableBoolA"" = ""e"".""NullableBoolB"") AND (""e"".""NullableBoolA"" IS NOT NULL AND ""e"".""NullableBoolB"" IS NOT NULL)) OR (""e"".""NullableBoolA"" IS NULL AND ""e"".""NullableBoolB"" IS NULL)",
                Sql);
        }

        public override void Compare_equals_method()
        {
            base.Compare_equals_method();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" = ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" = ""e"".""NullableBoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableBoolA"" = ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" = ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL AND ""e"".""NullableBoolB"" IS NULL)",
                Sql);
        }

        public override void Compare_equals_method_negated()
        {
            base.Compare_equals_method_negated();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""BoolA"" <> ""e"".""BoolB""

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""BoolA"" <> ""e"".""NullableBoolB"") OR ""e"".""NullableBoolB"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" <> ""e"".""BoolB"") OR ""e"".""NullableBoolA"" IS NULL

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableBoolA"" <> ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL OR ""e"".""NullableBoolB"" IS NULL)) AND (""e"".""NullableBoolA"" IS NOT NULL OR ""e"".""NullableBoolB"" IS NOT NULL)",
                Sql);
        }

        public override void Compare_complex_equal_equal_equal()
        {
            base.Compare_complex_equal_equal_equal();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN ""e"".""BoolA"" = ""e"".""BoolB""
    THEN TRUE::bool ELSE FALSE::bool
END = CASE
    WHEN ""e"".""IntA"" = ""e"".""IntB""
    THEN TRUE::bool ELSE FALSE::bool
END

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN ""e"".""NullableBoolA"" = ""e"".""BoolB""
    THEN TRUE::bool ELSE FALSE::bool
END = CASE
    WHEN ""e"".""IntA"" = ""e"".""NullableIntB""
    THEN TRUE::bool ELSE FALSE::bool
END

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN (""e"".""NullableBoolA"" = ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL AND ""e"".""NullableBoolB"" IS NULL)
    THEN TRUE::bool ELSE FALSE::bool
END = CASE
    WHEN (""e"".""NullableIntA"" = ""e"".""NullableIntB"") OR (""e"".""NullableIntA"" IS NULL AND ""e"".""NullableIntB"" IS NULL)
    THEN TRUE::bool ELSE FALSE::bool
END",
                Sql);
        }

        public override void Compare_complex_equal_not_equal_equal()
        {
            base.Compare_complex_equal_not_equal_equal();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN ""e"".""BoolA"" = ""e"".""BoolB""
    THEN TRUE::bool ELSE FALSE::bool
END <> CASE
    WHEN ""e"".""IntA"" = ""e"".""IntB""
    THEN TRUE::bool ELSE FALSE::bool
END

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN ""e"".""NullableBoolA"" = ""e"".""BoolB""
    THEN TRUE::bool ELSE FALSE::bool
END <> CASE
    WHEN ""e"".""IntA"" = ""e"".""NullableIntB""
    THEN TRUE::bool ELSE FALSE::bool
END

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN (""e"".""NullableBoolA"" = ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL AND ""e"".""NullableBoolB"" IS NULL)
    THEN TRUE::bool ELSE FALSE::bool
END <> CASE
    WHEN (""e"".""NullableIntA"" = ""e"".""NullableIntB"") OR (""e"".""NullableIntA"" IS NULL AND ""e"".""NullableIntB"" IS NULL)
    THEN TRUE::bool ELSE FALSE::bool
END",
                Sql);
        }

        public override void Compare_complex_not_equal_equal_equal()
        {
            base.Compare_complex_not_equal_equal_equal();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN ""e"".""BoolA"" <> ""e"".""BoolB""
    THEN TRUE::bool ELSE FALSE::bool
END = CASE
    WHEN ""e"".""IntA"" = ""e"".""IntB""
    THEN TRUE::bool ELSE FALSE::bool
END

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN (""e"".""NullableBoolA"" <> ""e"".""BoolB"") OR ""e"".""NullableBoolA"" IS NULL
    THEN TRUE::bool ELSE FALSE::bool
END = CASE
    WHEN (""e"".""IntA"" = ""e"".""NullableIntB"") AND ""e"".""NullableIntB"" IS NOT NULL
    THEN TRUE::bool ELSE FALSE::bool
END

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN ((""e"".""NullableBoolA"" <> ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL OR ""e"".""NullableBoolB"" IS NULL)) AND (""e"".""NullableBoolA"" IS NOT NULL OR ""e"".""NullableBoolB"" IS NOT NULL)
    THEN TRUE::bool ELSE FALSE::bool
END = CASE
    WHEN ((""e"".""NullableIntA"" = ""e"".""NullableIntB"") AND (""e"".""NullableIntA"" IS NOT NULL AND ""e"".""NullableIntB"" IS NOT NULL)) OR (""e"".""NullableIntA"" IS NULL AND ""e"".""NullableIntB"" IS NULL)
    THEN TRUE::bool ELSE FALSE::bool
END",
                Sql);
        }

        public override void Compare_complex_not_equal_not_equal_equal()
        {
            base.Compare_complex_not_equal_not_equal_equal();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN ""e"".""BoolA"" <> ""e"".""BoolB""
    THEN TRUE::bool ELSE FALSE::bool
END <> CASE
    WHEN ""e"".""IntA"" = ""e"".""IntB""
    THEN TRUE::bool ELSE FALSE::bool
END

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN (""e"".""NullableBoolA"" <> ""e"".""BoolB"") OR ""e"".""NullableBoolA"" IS NULL
    THEN TRUE::bool ELSE FALSE::bool
END <> CASE
    WHEN (""e"".""IntA"" = ""e"".""NullableIntB"") AND ""e"".""NullableIntB"" IS NOT NULL
    THEN TRUE::bool ELSE FALSE::bool
END

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN ((""e"".""NullableBoolA"" <> ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL OR ""e"".""NullableBoolB"" IS NULL)) AND (""e"".""NullableBoolA"" IS NOT NULL OR ""e"".""NullableBoolB"" IS NOT NULL)
    THEN TRUE::bool ELSE FALSE::bool
END <> CASE
    WHEN ((""e"".""NullableIntA"" = ""e"".""NullableIntB"") AND (""e"".""NullableIntA"" IS NOT NULL AND ""e"".""NullableIntB"" IS NOT NULL)) OR (""e"".""NullableIntA"" IS NULL AND ""e"".""NullableIntB"" IS NULL)
    THEN TRUE::bool ELSE FALSE::bool
END",
                Sql);
        }

        public override void Compare_complex_not_equal_equal_not_equal()
        {
            base.Compare_complex_not_equal_equal_not_equal();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN ""e"".""BoolA"" <> ""e"".""BoolB""
    THEN TRUE::bool ELSE FALSE::bool
END = CASE
    WHEN ""e"".""IntA"" <> ""e"".""IntB""
    THEN TRUE::bool ELSE FALSE::bool
END

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN (""e"".""NullableBoolA"" <> ""e"".""BoolB"") OR ""e"".""NullableBoolA"" IS NULL
    THEN TRUE::bool ELSE FALSE::bool
END = CASE
    WHEN (""e"".""IntA"" <> ""e"".""NullableIntB"") OR ""e"".""NullableIntB"" IS NULL
    THEN TRUE::bool ELSE FALSE::bool
END

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN ((""e"".""NullableBoolA"" <> ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL OR ""e"".""NullableBoolB"" IS NULL)) AND (""e"".""NullableBoolA"" IS NOT NULL OR ""e"".""NullableBoolB"" IS NOT NULL)
    THEN TRUE::bool ELSE FALSE::bool
END = CASE
    WHEN ((""e"".""NullableIntA"" <> ""e"".""NullableIntB"") OR (""e"".""NullableIntA"" IS NULL OR ""e"".""NullableIntB"" IS NULL)) AND (""e"".""NullableIntA"" IS NOT NULL OR ""e"".""NullableIntB"" IS NOT NULL)
    THEN TRUE::bool ELSE FALSE::bool
END",
                Sql);
        }

        public override void Compare_complex_not_equal_not_equal_not_equal()
        {
            base.Compare_complex_not_equal_not_equal_not_equal();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN ""e"".""BoolA"" <> ""e"".""BoolB""
    THEN TRUE::bool ELSE FALSE::bool
END <> CASE
    WHEN ""e"".""IntA"" <> ""e"".""IntB""
    THEN TRUE::bool ELSE FALSE::bool
END

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN (""e"".""NullableBoolA"" <> ""e"".""BoolB"") OR ""e"".""NullableBoolA"" IS NULL
    THEN TRUE::bool ELSE FALSE::bool
END <> CASE
    WHEN (""e"".""IntA"" <> ""e"".""NullableIntB"") OR ""e"".""NullableIntB"" IS NULL
    THEN TRUE::bool ELSE FALSE::bool
END

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE CASE
    WHEN ((""e"".""NullableBoolA"" <> ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL OR ""e"".""NullableBoolB"" IS NULL)) AND (""e"".""NullableBoolA"" IS NOT NULL OR ""e"".""NullableBoolB"" IS NOT NULL)
    THEN TRUE::bool ELSE FALSE::bool
END <> CASE
    WHEN ((""e"".""NullableIntA"" <> ""e"".""NullableIntB"") OR (""e"".""NullableIntA"" IS NULL OR ""e"".""NullableIntB"" IS NULL)) AND (""e"".""NullableIntA"" IS NOT NULL OR ""e"".""NullableIntB"" IS NOT NULL)
    THEN TRUE::bool ELSE FALSE::bool
END",
                Sql);
        }

        public override void Compare_nullable_with_null_parameter_equal()
        {
            base.Compare_nullable_with_null_parameter_equal();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableStringA"" IS NULL",
                Sql);
        }

        public override void Compare_nullable_with_non_null_parameter_not_equal()
        {
            base.Compare_nullable_with_non_null_parameter_not_equal();

            Assert.Equal(
                @"@__prm_0: Foo

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableStringA"" = @__prm_0",
                Sql);
        }

        public override void Join_uses_database_semantics()
        {
            base.Join_uses_database_semantics();

            Assert.Equal(
                @"SELECT ""e1"".""Id"", ""e2"".""Id"", ""e1"".""NullableIntA"", ""e2"".""NullableIntB""
FROM ""NullSemanticsEntity1"" AS ""e1""
INNER JOIN ""NullSemanticsEntity2"" AS ""e2"" ON ""e1"".""NullableIntA"" = ""e2"".""NullableIntB""",
                Sql);
        }

        public override void Contains_with_local_array_closure_with_null()
        {
            base.Contains_with_local_array_closure_with_null();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableStringA"" IN ('Foo') OR ""e"".""NullableStringA"" IS NULL",
                Sql);
        }

        public override void Contains_with_local_array_closure_false_with_null()
        {
            base.Contains_with_local_array_closure_false_with_null();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableStringA"" NOT IN ('Foo') AND ""e"".""NullableStringA"" IS NOT NULL",
                Sql);
        }

        public override void Contains_with_local_array_closure_with_multiple_nulls()
        {
            base.Contains_with_local_array_closure_with_multiple_nulls();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableStringA"" IN ('Foo') OR ""e"".""NullableStringA"" IS NULL",
                Sql);
        }

        public override void Where_multiple_ors_with_null()
        {
            base.Where_multiple_ors_with_null();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableStringA"" IN ('Foo', 'Blah') OR ""e"".""NullableStringA"" IS NULL",
                Sql);
        }

        public override void Where_multiple_ands_with_null()
        {
            base.Where_multiple_ands_with_null();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableStringA"" NOT IN ('Foo', 'Blah') AND ""e"".""NullableStringA"" IS NOT NULL",
                Sql);
        }

        public override void Where_multiple_ors_with_nullable_parameter()
        {
            base.Where_multiple_ors_with_nullable_parameter();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableStringA"" IN ('Foo') OR ""e"".""NullableStringA"" IS NULL",
                Sql);
        }

        public override void Where_multiple_ands_with_nullable_parameter_and_constant()
        {
            base.Where_multiple_ands_with_nullable_parameter_and_constant();

            Assert.Equal(
                @"@__prm3_2: Blah

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableStringA"" NOT IN ('Foo', @__prm3_2) AND ""e"".""NullableStringA"" IS NOT NULL",
                Sql);
        }

        public override void Where_multiple_ands_with_nullable_parameter_and_constant_not_optimized()
        {
            base.Where_multiple_ands_with_nullable_parameter_and_constant_not_optimized();

            Assert.Equal(
                @"@__prm3_2: Blah

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (((""e"".""NullableStringB"" IS NOT NULL AND ((""e"".""NullableStringA"" <> 'Foo') OR ""e"".""NullableStringA"" IS NULL)) AND ""e"".""NullableStringA"" IS NOT NULL) AND ""e"".""NullableStringA"" IS NOT NULL) AND ((""e"".""NullableStringA"" <> @__prm3_2) OR ""e"".""NullableStringA"" IS NULL)",
                Sql);
        }

        public override void Where_equal_nullable_with_null_value_parameter()
        {
            base.Where_equal_nullable_with_null_value_parameter();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableStringA"" IS NULL",
                Sql);
        }

        public override void Where_not_equal_nullable_with_null_value_parameter()
        {
            base.Where_not_equal_nullable_with_null_value_parameter();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableStringA"" IS NOT NULL",
                Sql);
        }

        public override void Where_equal_with_coalesce()
        {
            base.Where_equal_with_coalesce();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (COALESCE(""e"".""NullableStringA"", ""e"".""NullableStringB"") = ""e"".""NullableStringC"") OR ((""e"".""NullableStringA"" IS NULL AND ""e"".""NullableStringB"" IS NULL) AND ""e"".""NullableStringC"" IS NULL)",
                Sql);
        }

        public override void Where_not_equal_with_coalesce()
        {
            base.Where_not_equal_with_coalesce();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((COALESCE(""e"".""NullableStringA"", ""e"".""NullableStringB"") <> ""e"".""NullableStringC"") OR ((""e"".""NullableStringA"" IS NULL AND ""e"".""NullableStringB"" IS NULL) OR ""e"".""NullableStringC"" IS NULL)) AND ((""e"".""NullableStringA"" IS NOT NULL OR ""e"".""NullableStringB"" IS NOT NULL) OR ""e"".""NullableStringC"" IS NOT NULL)",
                Sql);
        }

        public override void Where_equal_with_coalesce_both_sides()
        {
            base.Where_equal_with_coalesce_both_sides();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE COALESCE(""e"".""NullableStringA"", ""e"".""NullableStringB"") = COALESCE(""e"".""StringA"", ""e"".""StringB"")",
                Sql);
        }

        public override void Where_not_equal_with_coalesce_both_sides()
        {
            base.Where_not_equal_with_coalesce_both_sides();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((COALESCE(""e"".""NullableIntA"", ""e"".""NullableIntB"") <> COALESCE(""e"".""NullableIntC"", ""e"".""NullableIntB"")) OR ((""e"".""NullableIntA"" IS NULL AND ""e"".""NullableIntB"" IS NULL) OR (""e"".""NullableIntC"" IS NULL AND ""e"".""NullableIntB"" IS NULL))) AND ((""e"".""NullableIntA"" IS NOT NULL OR ""e"".""NullableIntB"" IS NOT NULL) OR (""e"".""NullableIntC"" IS NOT NULL OR ""e"".""NullableIntB"" IS NOT NULL))",
                Sql);
        }

        public override void Where_equal_with_conditional()
        {
            base.Where_equal_with_conditional();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (CASE
    WHEN (""e"".""NullableStringA"" = ""e"".""NullableStringB"") OR (""e"".""NullableStringA"" IS NULL AND ""e"".""NullableStringB"" IS NULL)
    THEN ""e"".""NullableStringA"" ELSE ""e"".""NullableStringB""
END = ""e"".""NullableStringC"") OR (CASE
    WHEN (""e"".""NullableStringA"" = ""e"".""NullableStringB"") OR (""e"".""NullableStringA"" IS NULL AND ""e"".""NullableStringB"" IS NULL)
    THEN ""e"".""NullableStringA"" ELSE ""e"".""NullableStringB""
END IS NULL AND ""e"".""NullableStringC"" IS NULL)",
                Sql);
        }

        public override void Where_not_equal_with_conditional()
        {
            base.Where_not_equal_with_conditional();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ((""e"".""NullableStringC"" <> CASE
    WHEN ((""e"".""NullableStringA"" = ""e"".""NullableStringB"") AND (""e"".""NullableStringA"" IS NOT NULL AND ""e"".""NullableStringB"" IS NOT NULL)) OR (""e"".""NullableStringA"" IS NULL AND ""e"".""NullableStringB"" IS NULL)
    THEN ""e"".""NullableStringA"" ELSE ""e"".""NullableStringB""
END) OR (""e"".""NullableStringC"" IS NULL OR CASE
    WHEN ((""e"".""NullableStringA"" = ""e"".""NullableStringB"") AND (""e"".""NullableStringA"" IS NOT NULL AND ""e"".""NullableStringB"" IS NOT NULL)) OR (""e"".""NullableStringA"" IS NULL AND ""e"".""NullableStringB"" IS NULL)
    THEN ""e"".""NullableStringA"" ELSE ""e"".""NullableStringB""
END IS NULL)) AND (""e"".""NullableStringC"" IS NOT NULL OR CASE
    WHEN ((""e"".""NullableStringA"" = ""e"".""NullableStringB"") AND (""e"".""NullableStringA"" IS NOT NULL AND ""e"".""NullableStringB"" IS NOT NULL)) OR (""e"".""NullableStringA"" IS NULL AND ""e"".""NullableStringB"" IS NULL)
    THEN ""e"".""NullableStringA"" ELSE ""e"".""NullableStringB""
END IS NOT NULL)",
                Sql);
        }

        public override void Where_equal_with_conditional_non_nullable()
        {
            base.Where_equal_with_conditional_non_nullable();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableStringC"" <> CASE
    WHEN ((""e"".""NullableStringA"" = ""e"".""NullableStringB"") AND (""e"".""NullableStringA"" IS NOT NULL AND ""e"".""NullableStringB"" IS NOT NULL)) OR (""e"".""NullableStringA"" IS NULL AND ""e"".""NullableStringB"" IS NULL)
    THEN ""e"".""StringA"" ELSE ""e"".""StringB""
END) OR ""e"".""NullableStringC"" IS NULL",
                Sql);
        }

        public override void Where_equal_with_and_and_contains()
        {
            base.Where_equal_with_and_and_contains();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableStringA"" LIKE ((('%' || ""e"".""NullableStringB"")) || '%') AND ""e"".""BoolA"" = TRUE",
                Sql);
        }

        public override void Where_equal_using_relational_null_semantics()
        {
            base.Where_equal_using_relational_null_semantics();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableBoolA"" = ""e"".""NullableBoolB""",
                Sql);
        }

        public override void Where_equal_using_relational_null_semantics_with_parameter()
        {
            base.Where_equal_using_relational_null_semantics_with_parameter();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableBoolA"" IS NULL",
                Sql);
        }

        public override void Where_equal_using_relational_null_semantics_complex_with_parameter()
        {
            base.Where_equal_using_relational_null_semantics_complex_with_parameter();

            Assert.Equal(
                @"@__prm_0: False

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" = ""e"".""NullableBoolB"") OR @__prm_0 = TRUE",
                Sql);
        }

        public override void Where_not_equal_using_relational_null_semantics()
        {
            base.Where_not_equal_using_relational_null_semantics();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableBoolA"" <> ""e"".""NullableBoolB""",
                Sql);
        }

        public override void Where_not_equal_using_relational_null_semantics_with_parameter()
        {
            base.Where_not_equal_using_relational_null_semantics_with_parameter();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableBoolA"" IS NOT NULL",
                Sql);
        }

        public override void Where_not_equal_using_relational_null_semantics_complex_with_parameter()
        {
            base.Where_not_equal_using_relational_null_semantics_complex_with_parameter();

            Assert.Equal(
                @"@__prm_0: False

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" <> ""e"".""NullableBoolB"") OR @__prm_0 = TRUE",
                Sql);
        }

        public override void Switching_null_semantics_produces_different_cache_entry()
        {
            base.Switching_null_semantics_produces_different_cache_entry();

            Assert.Equal(
                @"SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE (""e"".""NullableBoolA"" = ""e"".""NullableBoolB"") OR (""e"".""NullableBoolA"" IS NULL AND ""e"".""NullableBoolB"" IS NULL)

SELECT ""e"".""Id""
FROM ""NullSemanticsEntity1"" AS ""e""
WHERE ""e"".""NullableBoolA"" = ""e"".""NullableBoolB""",
                Sql);
        }

        private static string Sql => TestSqlLoggerFactory.Sql;
    }
}
