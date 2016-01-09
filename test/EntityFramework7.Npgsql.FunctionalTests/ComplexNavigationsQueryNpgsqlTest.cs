﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.FunctionalTests;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class ComplexNavigationsQueryNpgsqlTest : ComplexNavigationsQueryTestBase<NpgsqlTestStore, ComplexNavigationsQueryNpgsqlFixture>
    {
        public ComplexNavigationsQueryNpgsqlTest(ComplexNavigationsQueryNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        public override void Multi_level_include_one_to_many_optional_and_one_to_many_optional_produces_valid_sql()
        {
            base.Multi_level_include_one_to_many_optional_and_one_to_many_optional_produces_valid_sql();

            Assert.Equal(
                @"SELECT ""e"".""Id"", ""e"".""Name"", ""e"".""OneToMany_Optional_Self_InverseId"", ""e"".""OneToMany_Required_Self_InverseId"", ""e"".""OneToOne_Optional_SelfId""
FROM ""Level1"" AS ""e""
ORDER BY ""e"".""Id""

SELECT ""l"".""Id"", ""l"".""Level1_Optional_Id"", ""l"".""Level1_Required_Id"", ""l"".""Name"", ""l"".""OneToMany_Optional_InverseId"", ""l"".""OneToMany_Optional_Self_InverseId"", ""l"".""OneToMany_Required_InverseId"", ""l"".""OneToMany_Required_Self_InverseId"", ""l"".""OneToOne_Optional_PK_InverseId"", ""l"".""OneToOne_Optional_SelfId""
FROM ""Level2"" AS ""l""
INNER JOIN (
    SELECT DISTINCT ""e"".""Id""
    FROM ""Level1"" AS ""e""
) AS ""e"" ON ""l"".""OneToMany_Optional_InverseId"" = ""e"".""Id""
ORDER BY ""e"".""Id"", ""l"".""Id""

SELECT ""l"".""Id"", ""l"".""Level2_Optional_Id"", ""l"".""Level2_Required_Id"", ""l"".""Name"", ""l"".""OneToMany_Optional_InverseId"", ""l"".""OneToMany_Optional_Self_InverseId"", ""l"".""OneToMany_Required_InverseId"", ""l"".""OneToMany_Required_Self_InverseId"", ""l"".""OneToOne_Optional_PK_InverseId"", ""l"".""OneToOne_Optional_SelfId""
FROM ""Level3"" AS ""l""
INNER JOIN (
    SELECT DISTINCT ""e"".""Id"", ""l"".""Id"" AS ""Id0""
    FROM ""Level2"" AS ""l""
    INNER JOIN (
        SELECT DISTINCT ""e"".""Id""
        FROM ""Level1"" AS ""e""
    ) AS ""e"" ON ""l"".""OneToMany_Optional_InverseId"" = ""e"".""Id""
) AS ""l0"" ON ""l"".""OneToMany_Optional_InverseId"" = ""l0"".""Id0""
ORDER BY ""l0"".""Id"", ""l0"".""Id0""", Sql);
        }

        public override void Multi_level_include_correct_PK_is_chosen_as_the_join_predicate_for_queries_that_join_same_table_multiple_times()
        {
            base.Multi_level_include_correct_PK_is_chosen_as_the_join_predicate_for_queries_that_join_same_table_multiple_times();

            Assert.Equal(
                @"SELECT ""e"".""Id"", ""e"".""Name"", ""e"".""OneToMany_Optional_Self_InverseId"", ""e"".""OneToMany_Required_Self_InverseId"", ""e"".""OneToOne_Optional_SelfId""
FROM ""Level1"" AS ""e""
ORDER BY ""e"".""Id""

SELECT ""l"".""Id"", ""l"".""Level1_Optional_Id"", ""l"".""Level1_Required_Id"", ""l"".""Name"", ""l"".""OneToMany_Optional_InverseId"", ""l"".""OneToMany_Optional_Self_InverseId"", ""l"".""OneToMany_Required_InverseId"", ""l"".""OneToMany_Required_Self_InverseId"", ""l"".""OneToOne_Optional_PK_InverseId"", ""l"".""OneToOne_Optional_SelfId""
FROM ""Level2"" AS ""l""
INNER JOIN (
    SELECT DISTINCT ""e"".""Id""
    FROM ""Level1"" AS ""e""
) AS ""e"" ON ""l"".""OneToMany_Optional_InverseId"" = ""e"".""Id""
ORDER BY ""e"".""Id"", ""l"".""Id""

SELECT ""l"".""Id"", ""l"".""Level2_Optional_Id"", ""l"".""Level2_Required_Id"", ""l"".""Name"", ""l"".""OneToMany_Optional_InverseId"", ""l"".""OneToMany_Optional_Self_InverseId"", ""l"".""OneToMany_Required_InverseId"", ""l"".""OneToMany_Required_Self_InverseId"", ""l"".""OneToOne_Optional_PK_InverseId"", ""l"".""OneToOne_Optional_SelfId"", ""l1"".""Id"", ""l1"".""Level1_Optional_Id"", ""l1"".""Level1_Required_Id"", ""l1"".""Name"", ""l1"".""OneToMany_Optional_InverseId"", ""l1"".""OneToMany_Optional_Self_InverseId"", ""l1"".""OneToMany_Required_InverseId"", ""l1"".""OneToMany_Required_Self_InverseId"", ""l1"".""OneToOne_Optional_PK_InverseId"", ""l1"".""OneToOne_Optional_SelfId""
FROM ""Level3"" AS ""l""
INNER JOIN (
    SELECT DISTINCT ""e"".""Id"", ""l"".""Id"" AS ""Id0""
    FROM ""Level2"" AS ""l""
    INNER JOIN (
        SELECT DISTINCT ""e"".""Id""
        FROM ""Level1"" AS ""e""
    ) AS ""e"" ON ""l"".""OneToMany_Optional_InverseId"" = ""e"".""Id""
) AS ""l0"" ON ""l"".""OneToMany_Optional_InverseId"" = ""l0"".""Id0""
INNER JOIN ""Level2"" AS ""l1"" ON ""l"".""OneToMany_Required_InverseId"" = ""l1"".""Id""
ORDER BY ""l0"".""Id"", ""l0"".""Id0"", ""l1"".""Id""

SELECT ""l"".""Id"", ""l"".""Level2_Optional_Id"", ""l"".""Level2_Required_Id"", ""l"".""Name"", ""l"".""OneToMany_Optional_InverseId"", ""l"".""OneToMany_Optional_Self_InverseId"", ""l"".""OneToMany_Required_InverseId"", ""l"".""OneToMany_Required_Self_InverseId"", ""l"".""OneToOne_Optional_PK_InverseId"", ""l"".""OneToOne_Optional_SelfId""
FROM ""Level3"" AS ""l""
INNER JOIN (
    SELECT DISTINCT ""l0"".""Id"", ""l0"".""Id0"", ""l1"".""Id"" AS ""Id1""
    FROM ""Level3"" AS ""l""
    INNER JOIN (
        SELECT DISTINCT ""e"".""Id"", ""l"".""Id"" AS ""Id0""
        FROM ""Level2"" AS ""l""
        INNER JOIN (
            SELECT DISTINCT ""e"".""Id""
            FROM ""Level1"" AS ""e""
        ) AS ""e"" ON ""l"".""OneToMany_Optional_InverseId"" = ""e"".""Id""
    ) AS ""l0"" ON ""l"".""OneToMany_Optional_InverseId"" = ""l0"".""Id0""
    INNER JOIN ""Level2"" AS ""l1"" ON ""l"".""OneToMany_Required_InverseId"" = ""l1"".""Id""
) AS ""l1"" ON ""l"".""OneToMany_Optional_InverseId"" = ""l1"".""Id1""
ORDER BY ""l1"".""Id"", ""l1"".""Id0"", ""l1"".""Id1""", Sql);
        }

        private static string Sql => TestSqlLoggerFactory.Sql;
    }
}
