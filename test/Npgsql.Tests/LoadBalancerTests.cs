using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Npgsql.Tests;

public class LoadBalancerTests
{
    [Test]
    public void TestLoadBalance()
        {
            var connStringBuilder = "host=127.0.0.3;port=5433;database=yugabyte;userid=yugabyte;password=yugsbyte;Load Balance Hosts=true;Timeout=0;Topology Keys=c1.r1.z1";
            List<NpgsqlConnection> conns = new List<NpgsqlConnection>();

            try
            {
                for (var i = 1; i <= 6; i++)
                {
                    NpgsqlConnection conn = new NpgsqlConnection(connStringBuilder);
                    Console.WriteLine("Connection created");
                    conn.Open();
                    Console.WriteLine("Connection open");
                    conns.Add(conn);
                }

                foreach (var conn1 in conns )
                {
                    Console.WriteLine("Connection host:{0}",conn1.Host );
                }
                // NpgsqlCommand empDropCmd = new NpgsqlCommand("DROP TABLE IF EXISTS employee;", conn);
                // empDropCmd.ExecuteNonQuery();
                // Console.WriteLine("Dropped table Employee");
                //
                // NpgsqlCommand empCreateCmd = new NpgsqlCommand("CREATE TABLE employee (id int PRIMARY KEY, name varchar, age int, language varchar);", conn);
                // empCreateCmd.ExecuteNonQuery();
                // Console.WriteLine("Created table Employee");
                //
                // NpgsqlCommand empInsertCmd = new NpgsqlCommand("INSERT INTO employee (id, name, age, language) VALUES (1, 'John', 35, 'CSharp');", conn);
                // int numRows = empInsertCmd.ExecuteNonQuery();
                // Console.WriteLine("Inserted data (1, 'John', 35, 'CSharp')");
                //
                // NpgsqlCommand empPrepCmd = new NpgsqlCommand("SELECT name, age, language FROM employee WHERE id = @EmployeeId", conn);
                // empPrepCmd.Parameters.Add("@EmployeeId", NpgsqlTypes.NpgsqlDbType.Integer);
                //
                // empPrepCmd.Parameters["@EmployeeId"].Value = 1;
                // NpgsqlDataReader reader = empPrepCmd.ExecuteReader();
                //
                // Console.WriteLine("Query returned:\nName\tAge\tLanguage");
                // while (reader.Read())
                // {
                //     Console.WriteLine("{0}\t{1}\t{2}", reader.GetString(0), reader.GetInt32(1), reader.GetString(2));
                // }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failure:" + ex.Message);
                Console.WriteLine("Failure stacktrace: " + ex.StackTrace);
            }
            finally
            {
                foreach (var conn in conns)
                {
                    if (conn.State != System.Data.ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                }
            }
        }
}
