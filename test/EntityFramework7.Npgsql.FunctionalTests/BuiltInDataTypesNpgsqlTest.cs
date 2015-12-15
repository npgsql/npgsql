using System;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;
using NpgsqlTypes;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class BuiltInDataTypesNpgsqlTest : BuiltInDataTypesTestBase<BuiltInDataTypesNpgsqlFixture>
    {
        public BuiltInDataTypesNpgsqlTest(BuiltInDataTypesNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public virtual void Can_query_using_any_mapped_data_type()
        {
            using (var context = CreateContext())
            {
                context.Set<MappedNullableDataTypes>().Add(
                    new MappedNullableDataTypes
                    {
                        Tinyint = 80,
                        Smallint = 79,
                        Int = 999,
                        Bigint = 78L,
                        Real = 84.4f,
                        Double_precision = 85.5,
                        Decimal = 101.7m,
                        Numeric = 103.9m,

                        Text = "Gumball Rules!",
                        Bytea = new byte[] { 86 },

                        Timestamp = new DateTime(2015, 1, 2, 10, 11, 12),
                        //Timestamptz = new DateTime(2016, 1, 2, 11, 11, 12, DateTimeKind.Utc),
                        Date = new DateTime(2015, 1, 2, 0, 0, 0),
                        Time = new TimeSpan(11, 15, 12),
                        //Timetz = new DateTimeOffset(0, 0, 0, 12, 0, 0, TimeSpan.FromHours(2)),
                        Interval = new TimeSpan(11, 15, 12),

                        Uuid = new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11"),
                        Bool = true,

                        Macaddr = PhysicalAddress.Parse("08-00-2B-01-02-03"),
                        Point = new NpgsqlPoint(5.2, 3.3),

                        SomeComposite = new SomeComposite { SomeNumber = 8, SomeText = "foo" }
                    });

                Assert.Equal(1, context.SaveChanges());
            }

            using (var context = CreateContext())
            {
                var entity = context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999);

                byte? param1 = 80;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Tinyint == param1));

                short? param2 = 79;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Smallint == param2));

                long? param3 = 78L;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Bigint == param3));

                float? param4 = 84.4f;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Real == param4));

                double? param5 = 85.5;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Double_precision == param5));

                decimal? param6 = 101.7m;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Decimal == param6));

                decimal? param7 = 103.9m;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Numeric == param7));

                var param8 = "Gumball Rules!";
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Text == param8));

                var param9 = new byte[] { 86 };
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Bytea == param9));

                DateTime? param10 = new DateTime(2015, 1, 2, 10, 11, 12);
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Timestamp == param10));

                //DateTime? param11 = new DateTime(2019, 1, 2, 14, 11, 12, DateTimeKind.Utc);
                //Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Timestamptz == param11));

                DateTime? param12 = new DateTime(2015, 1, 2, 0, 0, 0);
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Date == param12));

                TimeSpan? param13 = new TimeSpan(11, 15, 12);
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Time == param13));

                //DateTimeOffset? param14 = new DateTimeOffset(0, 0, 0, 12, 0, 0, TimeSpan.FromHours(2));
                //Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Timetz == param14));

                TimeSpan? param15 = new TimeSpan(11, 15, 12);
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Interval == param15));

                Guid? param16 = new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11");
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Uuid == param16));

                bool? param17 = true;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Bool == param17));

                PhysicalAddress param18 = PhysicalAddress.Parse("08-00-2B-01-02-03");
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Macaddr.Equals(param18)));

                NpgsqlPoint? param19 = new NpgsqlPoint(5.2, 3.3);
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.Point == param19));

                SomeComposite param20 = new SomeComposite { SomeNumber = 8, SomeText = "foo" };
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.SomeComposite.Equals(param20)));
            }
        }

        [Fact]
        public virtual void Can_query_using_any_mapped_data_types_with_nulls()
        {
            using (var context = CreateContext())
            {
                context.Set<MappedNullableDataTypes>().Add(
                    new MappedNullableDataTypes
                    {
                        Int = 911,
                    });

                Assert.Equal(1, context.SaveChanges());
            }

            using (var context = CreateContext())
            {
                var entity = context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911);

                byte? param1 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Bigint == param1));

                short? param2 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Smallint == param2));

                long? param3 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Bigint == param3));

                float? param4 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Real == param4));

                double? param5 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Double_precision == param5));

                decimal? param6 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Decimal == param6));

                decimal? param7 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Numeric == param7));

                string param8 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Text == param8));

                byte[] param9 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Bytea == param9));

                DateTime? param10 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Timestamp == param10));

                DateTime? param11 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Timestamptz == param11));

                DateTime? param12 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Date == param12));

                TimeSpan? param13 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Time == param13));

                DateTimeOffset? param14 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Timetz == param14));

                TimeSpan? param15 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Interval == param15));

                Guid? param16 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Uuid == param16));

                bool? param17 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Bool == param17));

                PhysicalAddress param18 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Macaddr == param18));

                NpgsqlPoint? param19 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.Point == param19));

                SomeComposite param20 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.SomeComposite == param20));
            }
        }

        [Fact]
        public virtual void Can_insert_and_read_back_all_mapped_data_types()
        {
            using (var context = CreateContext())
            {
                context.Set<MappedDataTypes>().Add(
                    new MappedDataTypes
                    {
                        Tinyint = 80,
                        Smallint = 79,
                        Int = 77,
                        Bigint = 78L,
                        Real = 84.4f,
                        Double_precision = 85.5,
                        Decimal = 101.1m,
                        Numeric = 103.3m,

                        Text = "Gumball Rules!",
                        Bytea = new byte[] { 86 },

                        Timestamp = new DateTime(2016, 1, 2, 11, 11, 12),
                        //Timestamptz = new DateTime(2016, 1, 2, 11, 11, 12, DateTimeKind.Utc),
                        Date = new DateTime(2015, 1, 2, 10, 11, 12),
                        Time = new TimeSpan(11, 15, 12),
                        //Timetz = new DateTimeOffset(0, 0, 0, 12, 0, 0, TimeSpan.FromHours(2)),
                        Interval = new TimeSpan(11, 15, 12),

                        Uuid = new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11"),
                        Bool = true,

                        Macaddr = PhysicalAddress.Parse("08-00-2B-01-02-03"),
                        Point = new NpgsqlPoint(5.2, 3.3),

                        SomeComposite = new SomeComposite { SomeNumber = 8, SomeText = "foo" }
                    });

                Assert.Equal(1, context.SaveChanges());
            }

            using (var context = CreateContext())
            {
                var entity = context.Set<MappedDataTypes>().Single(e => e.Int == 77);

                Assert.Equal(80, entity.Tinyint);
                Assert.Equal(79, entity.Smallint);
                Assert.Equal(77, entity.Int);
                Assert.Equal(78, entity.Bigint);
                Assert.Equal(84.4f, entity.Real);
                Assert.Equal(85.5, entity.Double_precision);
                Assert.Equal(101.1m, entity.Decimal);
                Assert.Equal(103.3m, entity.Numeric);

                Assert.Equal("Gumball Rules!", entity.Text);
                Assert.Equal(new byte[] { 86 }, entity.Bytea);

                Assert.Equal(new DateTime(2016, 1, 2, 11, 11, 12), entity.Timestamp);
                //Assert.Equal(new DateTime(2016, 1, 2, 11, 11, 12), entity.Timestamptz);
                Assert.Equal(new DateTime(2015, 1, 2, 0, 0, 0), entity.Date);
                Assert.Equal(new TimeSpan(11, 15, 12), entity.Time);
                //Assert.Equal(new DateTimeOffset(0, 0, 0, 12, 0, 0, TimeSpan.FromHours(2)), entity.Timetz);
                Assert.Equal(new TimeSpan(11, 15, 12), entity.Interval);

                Assert.Equal(new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11"), entity.Uuid);
                Assert.Equal(true, entity.Bool);

                Assert.Equal(PhysicalAddress.Parse("08-00-2B-01-02-03"), entity.Macaddr);
                Assert.Equal(new NpgsqlPoint(5.2, 3.3), entity.Point);

                Assert.Equal(new SomeComposite { SomeNumber = 8, SomeText = "foo" }, entity.SomeComposite);

            }
        }

        [Fact]
        public virtual void Can_insert_and_read_back_all_mapped_nullable_data_types()
        {
            using (var context = CreateContext())
            {
                context.Set<MappedNullableDataTypes>().Add(
                    new MappedNullableDataTypes
                    {
                        Tinyint = 80,
                        Smallint = 79,
                        Int = 77,
                        Bigint = 78L,
                        Real = 84.4f,
                        Double_precision = 85.5,
                        Decimal = 101.1m,
                        Numeric = 103.3m,

                        Text = "Gumball Rules!",
                        Bytea = new byte[] { 86 },

                        Timestamp = new DateTime(2016, 1, 2, 11, 11, 12),
                        //Timestamptz = new DateTime(2016, 1, 2, 11, 11, 12, DateTimeKind.Utc),
                        Date = new DateTime(2015, 1, 2, 10, 11, 12),
                        Time = new TimeSpan(11, 15, 12),
                        //Timetz = new DateTimeOffset(0, 0, 0, 12, 0, 0, TimeSpan.FromHours(2)),
                        Interval = new TimeSpan(11, 15, 12),

                        Uuid = new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11"),
                        Bool = true,

                        Macaddr = PhysicalAddress.Parse("08-00-2B-01-02-03"),
                        Point = new NpgsqlPoint(5.2, 3.3),

                        SomeComposite = new SomeComposite { SomeNumber = 8, SomeText = "foo" }
                    });

                Assert.Equal(1, context.SaveChanges());
            }

            using (var context = CreateContext())
            {
                var entity = context.Set<MappedNullableDataTypes>().Single(e => e.Int == 77);

                Assert.Equal(80, entity.Tinyint.Value);
                Assert.Equal(79, entity.Smallint.Value);
                Assert.Equal(77, entity.Int);
                Assert.Equal(78, entity.Bigint);
                Assert.Equal(84.4f, entity.Real);
                Assert.Equal(85.5, entity.Double_precision);
                Assert.Equal(101.1m, entity.Decimal);
                Assert.Equal(103.3m, entity.Numeric);

                Assert.Equal("Gumball Rules!", entity.Text);
                Assert.Equal(new byte[] { 86 }, entity.Bytea);

                Assert.Equal(new DateTime(2016, 1, 2, 11, 11, 12), entity.Timestamp);
                //Assert.Equal(new DateTime(2016, 1, 2, 11, 11, 12), entity.Timestamptz);
                Assert.Equal(new DateTime(2015, 1, 2, 0, 0, 0), entity.Date);
                Assert.Equal(new TimeSpan(11, 15, 12), entity.Time);
                //Assert.Equal(new DateTimeOffset(0, 0, 0, 12, 0, 0, TimeSpan.FromHours(2)), entity.Timetz);
                Assert.Equal(new TimeSpan(11, 15, 12), entity.Interval);

                Assert.Equal(new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11"), entity.Uuid);
                Assert.Equal(true, entity.Bool);

                Assert.Equal(PhysicalAddress.Parse("08-00-2B-01-02-03"), entity.Macaddr);
                Assert.Equal(new NpgsqlPoint(5.2, 3.3), entity.Point);

                Assert.Equal(new SomeComposite { SomeNumber = 8, SomeText = "foo" }, entity.SomeComposite);
            }
        }

        [Fact]
        public virtual void Can_insert_and_read_back_all_mapped_data_types_set_to_null()
        {
            using (var context = CreateContext())
            {
                context.Set<MappedNullableDataTypes>().Add(
                    new MappedNullableDataTypes
                    {
                        Int = 78
                    });

                Assert.Equal(1, context.SaveChanges());
            }

            using (var context = CreateContext())
            {
                var entity = context.Set<MappedNullableDataTypes>().Single(e => e.Int == 78);

                Assert.Null(entity.Tinyint);
                Assert.Null(entity.Smallint);
                Assert.Null(entity.Bigint);
                Assert.Null(entity.Real);
                Assert.Null(entity.Double_precision);
                Assert.Null(entity.Decimal);
                Assert.Null(entity.Numeric);

                Assert.Null(entity.Text);
                Assert.Null(entity.Bytea);

                Assert.Null(entity.Timestamp);
                Assert.Null(entity.Timestamptz);
                Assert.Null(entity.Date);
                Assert.Null(entity.Time);
                Assert.Null(entity.Timetz);
                Assert.Null(entity.Interval);

                Assert.Null(entity.Uuid);
                Assert.Null(entity.Bool);

                Assert.Null(entity.Macaddr);
                Assert.Null(entity.Point);

                Assert.Null(entity.SomeComposite);
            }
        }

        // TODO: Other tests from SqlServerBuiltInDataTypesSqlServerTest?
    }
}
