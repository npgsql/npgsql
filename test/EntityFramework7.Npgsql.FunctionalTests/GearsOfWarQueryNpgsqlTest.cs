using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.FunctionalTests;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class GearsOfWarQueryNpgsqlTest : GearsOfWarQueryTestBase<NpgsqlTestStore, GearsOfWarQueryNpgsqlFixture>
    {
        public GearsOfWarQueryNpgsqlTest(GearsOfWarQueryNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        public override void Include_multiple_one_to_one_and_one_to_many()
        {
            base.Include_multiple_one_to_one_and_one_to_many();

            Assert.Equal(
                @"SELECT ""t"".""Id"", ""t"".""GearNickName"", ""t"".""GearSquadId"", ""t"".""Note"", ""g"".""Nickname"", ""g"".""SquadId"", ""g"".""AssignedCityName"", ""g"".""CityOrBirthName"", ""g"".""Discriminator"", ""g"".""FullName"", ""g"".""LeaderNickname"", ""g"".""LeaderSquadId"", ""g"".""Rank""
FROM ""CogTag"" AS ""t""
LEFT JOIN (
    SELECT ""g"".*
    FROM ""Gear"" AS ""g""
    WHERE (""g"".""Discriminator"" = 'Officer') OR (""g"".""Discriminator"" = 'Gear')
) AS ""g"" ON (""t"".""GearNickName"" = ""g"".""Nickname"") AND (""t"".""GearSquadId"" = ""g"".""SquadId"")
ORDER BY ""g"".""FullName""

SELECT ""w"".""Id"", ""w"".""AmmunitionType"", ""w"".""Name"", ""w"".""OwnerFullName"", ""w"".""SynergyWithId""
FROM ""Weapon"" AS ""w""
INNER JOIN (
    SELECT DISTINCT ""g"".""FullName""
    FROM ""CogTag"" AS ""t""
    LEFT JOIN (
        SELECT ""g"".*
        FROM ""Gear"" AS ""g""
        WHERE (""g"".""Discriminator"" = 'Officer') OR (""g"".""Discriminator"" = 'Gear')
    ) AS ""g"" ON (""t"".""GearNickName"" = ""g"".""Nickname"") AND (""t"".""GearSquadId"" = ""g"".""SquadId"")
) AS ""g"" ON ""w"".""OwnerFullName"" = ""g"".""FullName""
ORDER BY ""g"".""FullName""",
                Sql);
        }

        public override void Include_multiple_one_to_one_and_one_to_many_self_reference()
        {
            base.Include_multiple_one_to_one_and_one_to_many_self_reference();

            Assert.Equal(
                @"SELECT ""t"".""Id"", ""t"".""GearNickName"", ""t"".""GearSquadId"", ""t"".""Note"", ""g"".""Nickname"", ""g"".""SquadId"", ""g"".""AssignedCityName"", ""g"".""CityOrBirthName"", ""g"".""Discriminator"", ""g"".""FullName"", ""g"".""LeaderNickname"", ""g"".""LeaderSquadId"", ""g"".""Rank""
FROM ""CogTag"" AS ""t""
LEFT JOIN (
    SELECT ""g"".*
    FROM ""Gear"" AS ""g""
    WHERE (""g"".""Discriminator"" = 'Officer') OR (""g"".""Discriminator"" = 'Gear')
) AS ""g"" ON (""t"".""GearNickName"" = ""g"".""Nickname"") AND (""t"".""GearSquadId"" = ""g"".""SquadId"")
ORDER BY ""g"".""FullName""

SELECT ""w"".""Id"", ""w"".""AmmunitionType"", ""w"".""Name"", ""w"".""OwnerFullName"", ""w"".""SynergyWithId""
FROM ""Weapon"" AS ""w""
INNER JOIN (
    SELECT DISTINCT ""g"".""FullName""
    FROM ""CogTag"" AS ""t""
    LEFT JOIN (
        SELECT ""g"".*
        FROM ""Gear"" AS ""g""
        WHERE (""g"".""Discriminator"" = 'Officer') OR (""g"".""Discriminator"" = 'Gear')
    ) AS ""g"" ON (""t"".""GearNickName"" = ""g"".""Nickname"") AND (""t"".""GearSquadId"" = ""g"".""SquadId"")
) AS ""g"" ON ""w"".""OwnerFullName"" = ""g"".""FullName""
ORDER BY ""g"".""FullName""",
                Sql);
        }

        public override void Include_multiple_one_to_one_and_one_to_one_and_one_to_many()
        {
            base.Include_multiple_one_to_one_and_one_to_one_and_one_to_many();

            Assert.Equal(
                @"SELECT ""t"".""Id"", ""t"".""GearNickName"", ""t"".""GearSquadId"", ""t"".""Note"", ""g"".""Nickname"", ""g"".""SquadId"", ""g"".""AssignedCityName"", ""g"".""CityOrBirthName"", ""g"".""Discriminator"", ""g"".""FullName"", ""g"".""LeaderNickname"", ""g"".""LeaderSquadId"", ""g"".""Rank"", ""s"".""Id"", ""s"".""InternalNumber"", ""s"".""Name""
FROM ""CogTag"" AS ""t""
LEFT JOIN (
    SELECT ""g"".*
    FROM ""Gear"" AS ""g""
    WHERE (""g"".""Discriminator"" = 'Officer') OR (""g"".""Discriminator"" = 'Gear')
) AS ""g"" ON (""t"".""GearNickName"" = ""g"".""Nickname"") AND (""t"".""GearSquadId"" = ""g"".""SquadId"")
LEFT JOIN ""Squad"" AS ""s"" ON ""g"".""SquadId"" = ""s"".""Id""
ORDER BY ""s"".""Id""

SELECT ""g"".""Nickname"", ""g"".""SquadId"", ""g"".""AssignedCityName"", ""g"".""CityOrBirthName"", ""g"".""Discriminator"", ""g"".""FullName"", ""g"".""LeaderNickname"", ""g"".""LeaderSquadId"", ""g"".""Rank""
FROM ""Gear"" AS ""g""
INNER JOIN (
    SELECT DISTINCT ""s"".""Id""
    FROM ""CogTag"" AS ""t""
    LEFT JOIN (
        SELECT ""g"".*
        FROM ""Gear"" AS ""g""
        WHERE (""g"".""Discriminator"" = 'Officer') OR (""g"".""Discriminator"" = 'Gear')
    ) AS ""g"" ON (""t"".""GearNickName"" = ""g"".""Nickname"") AND (""t"".""GearSquadId"" = ""g"".""SquadId"")
    LEFT JOIN ""Squad"" AS ""s"" ON ""g"".""SquadId"" = ""s"".""Id""
) AS ""s"" ON ""g"".""SquadId"" = ""s"".""Id""
WHERE (""g"".""Discriminator"" = 'Officer') OR (""g"".""Discriminator"" = 'Gear')
ORDER BY ""s"".""Id""", Sql);
        }

        public override void Include_multiple_one_to_one_optional_and_one_to_one_required()
        {
            base.Include_multiple_one_to_one_optional_and_one_to_one_required();

            Assert.Equal(
                @"SELECT ""t"".""Id"", ""t"".""GearNickName"", ""t"".""GearSquadId"", ""t"".""Note"", ""g"".""Nickname"", ""g"".""SquadId"", ""g"".""AssignedCityName"", ""g"".""CityOrBirthName"", ""g"".""Discriminator"", ""g"".""FullName"", ""g"".""LeaderNickname"", ""g"".""LeaderSquadId"", ""g"".""Rank"", ""s"".""Id"", ""s"".""InternalNumber"", ""s"".""Name""
FROM ""CogTag"" AS ""t""
LEFT JOIN (
    SELECT ""g"".*
    FROM ""Gear"" AS ""g""
    WHERE (""g"".""Discriminator"" = 'Officer') OR (""g"".""Discriminator"" = 'Gear')
) AS ""g"" ON (""t"".""GearNickName"" = ""g"".""Nickname"") AND (""t"".""GearSquadId"" = ""g"".""SquadId"")
LEFT JOIN ""Squad"" AS ""s"" ON ""g"".""SquadId"" = ""s"".""Id""", Sql);
        }

        public override void Include_multiple_circular()
        {
            base.Include_multiple_circular();

            Assert.Equal(
                @"SELECT ""g"".""Nickname"", ""g"".""SquadId"", ""g"".""AssignedCityName"", ""g"".""CityOrBirthName"", ""g"".""Discriminator"", ""g"".""FullName"", ""g"".""LeaderNickname"", ""g"".""LeaderSquadId"", ""g"".""Rank"", ""c"".""Name"", ""c"".""Location""
FROM ""Gear"" AS ""g""
INNER JOIN ""City"" AS ""c"" ON ""g"".""CityOrBirthName"" = ""c"".""Name""
WHERE ""g"".""Discriminator"" IN ('Officer', 'Gear')
ORDER BY ""c"".""Name""

SELECT ""g"".""Nickname"", ""g"".""SquadId"", ""g"".""AssignedCityName"", ""g"".""CityOrBirthName"", ""g"".""Discriminator"", ""g"".""FullName"", ""g"".""LeaderNickname"", ""g"".""LeaderSquadId"", ""g"".""Rank""
FROM ""Gear"" AS ""g""
INNER JOIN (
    SELECT DISTINCT ""c"".""Name""
    FROM ""Gear"" AS ""g""
    INNER JOIN ""City"" AS ""c"" ON ""g"".""CityOrBirthName"" = ""c"".""Name""
    WHERE ""g"".""Discriminator"" IN ('Officer', 'Gear')
) AS ""c"" ON ""g"".""AssignedCityName"" = ""c"".""Name""
WHERE (""g"".""Discriminator"" = 'Officer') OR (""g"".""Discriminator"" = 'Gear')
ORDER BY ""c"".""Name""",
                Sql);
        }

        public override void Include_multiple_circular_with_filter()
        {
            base.Include_multiple_circular_with_filter();

            Assert.Equal(
                @"SELECT ""g"".""Nickname"", ""g"".""SquadId"", ""g"".""AssignedCityName"", ""g"".""CityOrBirthName"", ""g"".""Discriminator"", ""g"".""FullName"", ""g"".""LeaderNickname"", ""g"".""LeaderSquadId"", ""g"".""Rank"", ""c"".""Name"", ""c"".""Location""
FROM ""Gear"" AS ""g""
INNER JOIN ""City"" AS ""c"" ON ""g"".""CityOrBirthName"" = ""c"".""Name""
WHERE ""g"".""Discriminator"" IN ('Officer', 'Gear') AND (""g"".""Nickname"" = 'Marcus')
ORDER BY ""c"".""Name""

SELECT ""g"".""Nickname"", ""g"".""SquadId"", ""g"".""AssignedCityName"", ""g"".""CityOrBirthName"", ""g"".""Discriminator"", ""g"".""FullName"", ""g"".""LeaderNickname"", ""g"".""LeaderSquadId"", ""g"".""Rank""
FROM ""Gear"" AS ""g""
INNER JOIN (
    SELECT DISTINCT ""c"".""Name""
    FROM ""Gear"" AS ""g""
    INNER JOIN ""City"" AS ""c"" ON ""g"".""CityOrBirthName"" = ""c"".""Name""
    WHERE ""g"".""Discriminator"" IN ('Officer', 'Gear') AND (""g"".""Nickname"" = 'Marcus')
) AS ""c"" ON ""g"".""AssignedCityName"" = ""c"".""Name""
WHERE (""g"".""Discriminator"" = 'Officer') OR (""g"".""Discriminator"" = 'Gear')
ORDER BY ""c"".""Name""",
                Sql);
        }

        public override void Include_using_alternate_key()
        {
            base.Include_using_alternate_key();

            Assert.Equal(
                @"SELECT ""g"".""Nickname"", ""g"".""SquadId"", ""g"".""AssignedCityName"", ""g"".""CityOrBirthName"", ""g"".""Discriminator"", ""g"".""FullName"", ""g"".""LeaderNickname"", ""g"".""LeaderSquadId"", ""g"".""Rank""
FROM ""Gear"" AS ""g""
WHERE ""g"".""Discriminator"" IN ('Officer', 'Gear') AND (""g"".""Nickname"" = 'Marcus')
ORDER BY ""g"".""FullName""

SELECT ""w"".""Id"", ""w"".""AmmunitionType"", ""w"".""Name"", ""w"".""OwnerFullName"", ""w"".""SynergyWithId""
FROM ""Weapon"" AS ""w""
INNER JOIN (
    SELECT DISTINCT ""g"".""FullName""
    FROM ""Gear"" AS ""g""
    WHERE ""g"".""Discriminator"" IN ('Officer', 'Gear') AND (""g"".""Nickname"" = 'Marcus')
) AS ""g"" ON ""w"".""OwnerFullName"" = ""g"".""FullName""
ORDER BY ""g"".""FullName""",
                Sql);
        }

        public override void Where_enum()
        {
            base.Where_enum();

            Assert.Equal(
                @"SELECT ""g"".""Nickname"", ""g"".""SquadId"", ""g"".""AssignedCityName"", ""g"".""CityOrBirthName"", ""g"".""Discriminator"", ""g"".""FullName"", ""g"".""LeaderNickname"", ""g"".""LeaderSquadId"", ""g"".""Rank""
FROM ""Gear"" AS ""g""
WHERE ""g"".""Discriminator"" IN ('Officer', 'Gear') AND (""g"".""Rank"" = 2)",
                Sql);
        }

        public override void Where_nullable_enum_with_constant()
        {
            base.Where_nullable_enum_with_constant();

            Assert.Equal(
                @"SELECT ""w"".""Id"", ""w"".""AmmunitionType"", ""w"".""Name"", ""w"".""OwnerFullName"", ""w"".""SynergyWithId""
FROM ""Weapon"" AS ""w""
WHERE ""w"".""AmmunitionType"" = 1",
                Sql);
        }

        public override void Where_nullable_enum_with_null_constant()
        {
            base.Where_nullable_enum_with_null_constant();

            Assert.Equal(
                @"SELECT ""w"".""Id"", ""w"".""AmmunitionType"", ""w"".""Name"", ""w"".""OwnerFullName"", ""w"".""SynergyWithId""
FROM ""Weapon"" AS ""w""
WHERE ""w"".""AmmunitionType"" IS NULL",
                Sql);
        }

        public override void Where_nullable_enum_with_non_nullable_parameter()
        {
            base.Where_nullable_enum_with_non_nullable_parameter();

            Assert.Equal(
                @"@__p_0: 1

SELECT ""w"".""Id"", ""w"".""AmmunitionType"", ""w"".""Name"", ""w"".""OwnerFullName"", ""w"".""SynergyWithId""
FROM ""Weapon"" AS ""w""
WHERE ""w"".""AmmunitionType"" = @__p_0",
                Sql);
        }

        public override void Where_nullable_enum_with_nullable_parameter()
        {
            base.Where_nullable_enum_with_nullable_parameter();

            Assert.Equal(
                @"@__ammunitionType_0: Cartridge

SELECT ""w"".""Id"", ""w"".""AmmunitionType"", ""w"".""Name"", ""w"".""OwnerFullName"", ""w"".""SynergyWithId""
FROM ""Weapon"" AS ""w""
WHERE ""w"".""AmmunitionType"" = @__ammunitionType_0

SELECT ""w"".""Id"", ""w"".""AmmunitionType"", ""w"".""Name"", ""w"".""OwnerFullName"", ""w"".""SynergyWithId""
FROM ""Weapon"" AS ""w""
WHERE ""w"".""AmmunitionType"" IS NULL",
                Sql);
        }

        private static string Sql => TestSqlLoggerFactory.Sql;
    }
}
