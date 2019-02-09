using System;

namespace Npgsql.CrateDb.Tests
{
    class CrateIntegrationTest : TestBase
    {

        protected void CreateTestTable()
        {
            using (var con = OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"create table if not exists test(
                        id integer primary key,
                        string_field string,
                        boolean_field boolean,
                        byte_field byte,
                        short_field short,
                        integer_field integer,
                        long_field long,
                        float_field float,
                        double_field double,
                        timestamp_field timestamp,
                        object_field object(strict) as (""inner"" string),
                        ip_field ip,
                        geo_point_field geo_point,
                        geo_shape_field geo_shape
                      ) clustered by (id) into 1 shards with (number_of_replicas=0)";

                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void InsertIntoTestTable()
        {
            using (var con = OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"insert into test 
                       (id, string_field, boolean_field, byte_field, 
                        short_field, integer_field, long_field, float_field, double_field, object_field, 
                        timestamp_field, ip_field, geo_point_field, geo_shape_field) values 
                       (@id, @string_field, @boolean_field, @byte_field, 
                        @short_field, @integer_field, @long_field, @float_field, @double_field, @object_field, 
                        @timestamp_field, @ip_field, @geo_point_field, @geo_shape_field)";

                    cmd.Parameters.AddWithValue("@id", 1);
                    cmd.Parameters.AddWithValue("@string_field", "Youri");
                    cmd.Parameters.AddWithValue("@boolean_field", true);
                    cmd.Parameters.AddWithValue("@byte_field", (byte)120);
                    cmd.Parameters.AddWithValue("@byte_field", DBNull.Value);
                    cmd.Parameters.AddWithValue("@short_field", (short)1000);
                    cmd.Parameters.AddWithValue("@integer_field", 1200000);
                    cmd.Parameters.AddWithValue("@long_field", 120000000000L);
                    cmd.Parameters.AddWithValue("@float_field", 1.4f);
                    cmd.Parameters.AddWithValue("@double_field", 3.456789);

                    cmd.Parameters.AddWithValue("@object_field", NpgsqlTypes.NpgsqlDbType.Json, "{ \"inner\": \"Zoon\" }");
                    //cmd.Parameters.AddWithValue("@object_field", NpgsqlTypes.NpgsqlDbType.Json, new { inner = "Zoon" });

                    cmd.Parameters.AddWithValue("@timestamp_field", new DateTime(2018, 10, 11).ToUniversalTime());
                    cmd.Parameters.AddWithValue("@ip_field", "127.0.0.1");
                    cmd.Parameters.AddWithValue("@geo_point_field", new double[] { 9.7419021d, 47.4048045d });
                    cmd.Parameters.AddWithValue("@geo_shape_field", "POLYGON ((30 10, 40 40, 20 40, 10 20, 30 10))");
                    cmd.ExecuteNonQuery();



                    cmd.CommandText = "refresh table test";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void TearDownTestTable()
        {
            using (var con = OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"drop table if exists test";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void CreateArrayTestTable()
        {
            using (var con = OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"create table if not exists arrayTest (
                        id integer primary key,
                        str_array array(string),
                        bool_array array(boolean),
                        byte_array array(byte),
                        short_array array(short),
                        integer_array array(integer),
                        long_array array(long),
                        float_array array(float),
                        double_array array(double),
                        timestamp_array array(timestamp),
                        obj_array array(object),
                        geo_shape_array array(geo_shape)
                      ) clustered by (id) into 1 shards with (number_of_replicas=0)";

                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void InsertIntoArrayTestTable()
        {
            using (var con = OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"insert into arrayTest (id, str_array, bool_array, byte_array, 
                        short_array, integer_array, long_array, float_array, double_array, timestamp_array, 
                        ip_array, obj_array, geo_shape_array) values 
                       (@id, @str_array, @bool_array, @byte_array, 
                        @short_array, @integer_array, @long_array, @float_array, @double_array, @timestamp_array, 
                        @ip_array, @obj_array, @geo_shape_array)";

                    cmd.Parameters.AddWithValue("@id", 1);
                    cmd.Parameters.AddWithValue("@str_array", new string[] { "a", "b", "c", "d" });
                    cmd.Parameters.AddWithValue("@bool_array", new bool[] { true, false });
                    cmd.Parameters.AddWithValue("@byte_array", new byte[] { 120, 100 });
                    cmd.Parameters.AddWithValue("@short_array", new short[] { 1300, 1200 });
                    cmd.Parameters.AddWithValue("@integer_array", new int[] { 2147483647, 234583 });
                    cmd.Parameters.AddWithValue("@long_array", new long[] { 9223372036854775807L, 4L });
                    cmd.Parameters.AddWithValue("@float_array", new float[] { 3.402f, 3.403f, 1.4f });
                    cmd.Parameters.AddWithValue("@double_array", new double[] { 1.79769313486231570e+308, 1.69769313486231570e+308 });
                    cmd.Parameters.AddWithValue("@timestamp_array", new DateTime[] { new DateTime(2000, 1, 1).ToUniversalTime(), new DateTime(1970, 1, 1).ToUniversalTime() });
                    cmd.Parameters.AddWithValue("@ip_array", new string[] { "127.142.132.9", "127.0.0.1" });
                    cmd.Parameters.AddWithValue("@obj_array", NpgsqlTypes.NpgsqlDbType.Json, "[ { \"inner\": \"Zoon1\" }, { \"inner\": \"Zoon2\" } ]");
                    //cmd.Parameters.AddWithValue("@obj_array", NpgsqlTypes.NpgsqlDbType.Json, new object[] { new { inner = "Zoon1" }, new { inner = "Zoon2" } });
                    cmd.Parameters.AddWithValue("@geo_shape_array", new string[] { "POLYGON ((30 10, 40 40, 20 40, 10 20, 30 10))", "POLYGON ((40 20, 50 50, 30 50, 20 30, 40 20))" });
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "refresh table arrayTest";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void TearDownArrayTestTable()
        {
            using (var con = OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"drop table if exists arrayTest";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void CreateBulkInsertTestTable()
        {
            using (var con = OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"create table if not exists authors(
                        id integer primary key,
                        author string                        
                      ) clustered by (id) into 1 shards with (number_of_replicas=0)";

                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void TearDownBulkInsertTestTable()
        {
            using (var con = OpenConnection())
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"drop table if exists authors";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
