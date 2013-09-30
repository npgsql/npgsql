using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NpgsqlTests
{
    [Explicit]
    public class SpeedTests : TestBase
    {
        [Test, Description("A normal insert command with one parameter")]
        public void ParameterizedInsert()
        {
            using (var command = Conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "INSERT INTO data (field_text) values (:data)";

                IDbDataParameter dataParameter = command.CreateParameter();
                dataParameter.Direction = ParameterDirection.Input;
                dataParameter.DbType = DbType.String;
                dataParameter.ParameterName = "data";

                command.Parameters.Add(dataParameter);
                var sw = new Stopwatch();
                sw.Start();
                for (var i = 0; i < 10000; i++)
                {
                    dataParameter.Value = "yo";
                    command.ExecuteScalar();
                }
                Console.WriteLine("Insert time: {0}ms", sw.ElapsedMilliseconds);
            }
        }

        [Test, Description("A prepared insert command with one parameter")]
        public void ParameterizedPreparedInsert()
        {
            using (var command = Conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "INSERT INTO data (field_text) values (:data)";

                IDbDataParameter dataParameter = command.CreateParameter();
                dataParameter.Direction = ParameterDirection.Input;
                dataParameter.DbType = DbType.String;
                dataParameter.ParameterName = "data";

                command.Parameters.Add(dataParameter);
                command.Prepare();
                var sw = new Stopwatch();
                sw.Start();
                for (var i = 0; i < 10000; i++)
                {
                    dataParameter.Value = "yo";
                    command.ExecuteScalar();
                }
                Console.WriteLine("Insert time: {0}ms", sw.ElapsedMilliseconds);
            }
        }

        #region Setup / Teardown / Utils

        private Stopwatch _watch;

        [SetUp]
        public void Setup()
        {
            _watch = new Stopwatch();
            _watch.Start();
        }

        [TearDown]
        public void Teardown()
        {
            _watch.Stop();
            Console.WriteLine("Total test running time: {0}ms",  _watch.ElapsedMilliseconds);
        }

        #endregion
    }
}