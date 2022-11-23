using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.Util;

namespace Npgsql;

/// <summary>
/// 
/// </summary>
public sealed class TopologyAwareDataSource: ClusterAwareDataSource
{
    internal TopologyAwareDataSource(NpgsqlConnectionStringBuilder settings, NpgsqlDataSourceConfiguration dataSourceConfig) : base(settings, dataSourceConfig)
    {
        Console.WriteLine("Inside TopologyAwareDatasource");
        try
        {
            NpgsqlDataSource control = new UnpooledDataSource(settings, dataSourceConfig);
            NpgsqlConnection controlConnection = NpgsqlConnection.FromDataSource(control);
            controlConnection.Open();
            GetCurrentServers(settings, controlConnection);
            controlConnection.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    internal void GetCurrentServers(NpgsqlConnectionStringBuilder settings, NpgsqlConnection conn)
    {
        NpgsqlCommand QUERY_SERVER = new NpgsqlCommand("Select * from yb_servers()",conn);
        NpgsqlDataReader reader = QUERY_SERVER.ExecuteReader();
        var topology = settings.TopologyKeys;
        Console.WriteLine(@"Topology in url:{0}",topology);
        while (reader.Read())
        {
            // Console.WriteLine("Hosts : {0}", reader.GetString(0));
            var cloud = reader.GetString(4);
            var region = reader.GetString(5);
            var zone = reader.GetString(6);
            
            Console.WriteLine(@"Topologies : {0}.{1}.{2}",cloud,region,zone);
            
            _hosts?.Add(reader.GetString(0));
        }

        if (_hosts != null)
            foreach (var host in _hosts)
            {
                Console.WriteLine("Hosts: {0}", host);
            }
    }
}
