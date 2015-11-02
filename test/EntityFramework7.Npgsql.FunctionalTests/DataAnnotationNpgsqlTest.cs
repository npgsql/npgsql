using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.Metadata;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class DataAnnotationNpgsqlTest : DataAnnotationTestBase<NpgsqlTestStore, DataAnnotationNpgsqlFixture>
    {
        public DataAnnotationNpgsqlTest(DataAnnotationNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        public override void ConcurrencyCheckAttribute_throws_if_value_in_database_changed()
        {
            base.ConcurrencyCheckAttribute_throws_if_value_in_database_changed();

            Assert.Equal(@"SELECT ""r"".""UniqueNo"", ""r"".""MaxLengthProperty"", ""r"".""Name"", ""r"".""RowVersion""
FROM ""Sample"" AS ""r""
WHERE ""r"".""UniqueNo"" = 1
LIMIT 1

SELECT ""r"".""UniqueNo"", ""r"".""MaxLengthProperty"", ""r"".""Name"", ""r"".""RowVersion""
FROM ""Sample"" AS ""r""
WHERE ""r"".""UniqueNo"" = 1
LIMIT 1

@p2: 1
@p0: ModifiedData
@p1: 00000000-0000-0000-0003-000000000001
@p3: 00000001-0000-0000-0000-000000000001

UPDATE ""Sample"" SET ""Name"" = @p0, ""RowVersion"" = @p1
WHERE ""UniqueNo"" IS NOT DISTINCT FROM @p2 AND ""RowVersion"" IS NOT DISTINCT FROM @p3;

@p2: 1
@p0: ChangedData
@p1: 00000000-0000-0000-0002-000000000001
@p3: 00000001-0000-0000-0000-000000000001

UPDATE ""Sample"" SET ""Name"" = @p0, ""RowVersion"" = @p1
WHERE ""UniqueNo"" IS NOT DISTINCT FROM @p2 AND ""RowVersion"" IS NOT DISTINCT FROM @p3;",
                Sql);
        }

        public override void DatabaseGeneratedAttribute_autogenerates_values_when_set_to_identity()
        {
            base.DatabaseGeneratedAttribute_autogenerates_values_when_set_to_identity();

            Assert.Equal(@"@p0: 
@p1: Third
@p2: 00000000-0000-0000-0000-000000000003

INSERT INTO ""Sample"" (""MaxLengthProperty"", ""Name"", ""RowVersion"")
VALUES (@p0, @p1, @p2)
RETURNING ""UniqueNo"";",
                Sql);
        }

        [Fact(Skip="Npgsql does not support length")]
        public override void MaxLengthAttribute_throws_while_inserting_value_longer_than_max_length() {}

        public override void RequiredAttribute_for_navigation_throws_while_inserting_null_value()
        {
            base.RequiredAttribute_for_navigation_throws_while_inserting_null_value();

            Assert.Equal(@"@p0: 0
@p1: Book1

INSERT INTO ""BookDetail"" (""Id"", ""BookId"")
VALUES (@p0, @p1);

@p0: 0
@p1: 

INSERT INTO ""BookDetail"" (""Id"", ""BookId"")
VALUES (@p0, @p1);",
                Sql);
        }

        public override void RequiredAttribute_for_property_throws_while_inserting_null_value()
        {
            base.RequiredAttribute_for_property_throws_while_inserting_null_value();

            Assert.Equal(@"@p0: 
@p1: ValidString
@p2: 00000000-0000-0000-0000-000000000001

INSERT INTO ""Sample"" (""MaxLengthProperty"", ""Name"", ""RowVersion"")
VALUES (@p0, @p1, @p2)
RETURNING ""UniqueNo"";

@p0: 
@p1: 
@p2: 00000000-0000-0000-0000-000000000002

INSERT INTO ""Sample"" (""MaxLengthProperty"", ""Name"", ""RowVersion"")
VALUES (@p0, @p1, @p2)
RETURNING ""UniqueNo"";",
                Sql);
        }

        [Fact(Skip="Npgsql does not support length")]
        public override void StringLengthAttribute_throws_while_inserting_value_longer_than_max_length() {}

        [Fact(Skip="Npgsql does not support length")]
        public override void TimestampAttribute_throws_if_value_in_database_changed() {}

        private static string Sql => TestSqlLoggerFactory.Sql;
    }
}
