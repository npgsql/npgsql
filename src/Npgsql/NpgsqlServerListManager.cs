#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Resources;
using System.Reflection;

namespace Npgsql
{
    internal class ServerListManager
    {
        private static Dictionary<string, ServerPair> Servers = new Dictionary<string, ServerPair>();

        /// <summary>
        /// Parse host property. And move the prefer host to the head of resultServerList for performance.
        /// Then get current connection info (host and port) from memory.
        /// </summary>
        /// <param name="context"></param>

        public static ServerPair[] getServerInfo(NpgsqlConnector context)
        {
            var serverList = analyzeConnectionString(context.Settings.Host, context.Settings.Port);
            var resultServerList = new List<ServerPair>(2);

            lock (Servers)
            {
                foreach (ServerPair sp in serverList)
                {
                    var key = sp.toString();

                    if (!Servers.ContainsKey(key))
                    {
                        Servers.Add(key, sp);
                        resultServerList.Add(sp);
                    }
                    else
                    {
                        resultServerList.Add(Servers[key]);
                    }
                }
             }
            
            NpgsqlServerListManager preferStatus;

            if (context.Settings.TargetServer == TargetServer.any)
            {
                return resultServerList.ToArray();
            }
            else if (context.Settings.TargetServer == TargetServer.master)
            {
                preferStatus = NpgsqlServerListManager.Master;
            }
            else
            {
                preferStatus = NpgsqlServerListManager.Slave;
            }
            
            //move the prefer host to the head of resultServerList for performance.
            for (var i = 0; i < resultServerList.Count; i++)
            {
                ServerPair sp = resultServerList[i];
                
                if (sp.HostStatus == preferStatus)
                {
                    if(i != 0)
                    {
                        ServerPair tmpSp = resultServerList[0];
                        resultServerList[0] = sp;
                        resultServerList[i] = tmpSp;
                     }
                     break;
                }
            }

            /*
             * if targetServer is set to preferSlave, move the master host to the last of the result.
             * Server to avoid connet to master host twice
            */

            if (context.Settings.TargetServer == TargetServer.preferSlave)
            {
                if (resultServerList[0].HostStatus == NpgsqlServerListManager.Master)
                {
                    ServerPair tmpSp = resultServerList[resultServerList.Count-1];
                    resultServerList[resultServerList.Count-1] = resultServerList[0];
                    resultServerList[0] = tmpSp;
                }
            }
            return resultServerList.ToArray();
        }

        /// <summary>
        /// Parse Host property.
        /// example:   Host = [2002:23:45:44]:4567,193.168.5.3:56
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>

        private static List<ServerPair> analyzeConnectionString(string host, int port)
        {
            var tmpServerList = new List<ServerPair>();
            string[] serverList = host.Split(',');        

            for (var i=0; i < serverList.Length; i++)
            {
                serverList[i] = serverList[i].Trim();

                if (serverList[i].Length == 0)
                {
                    throw new ArgumentException("Connection string argument missing!");              
                }
               
                var portIdx = serverList[i].LastIndexOf(':');

                if (portIdx != -1 && serverList[i].LastIndexOf(']') < portIdx)
                {
                    var portStr = serverList[i].Substring(portIdx + 1);
                    var hostStr = serverList[i].Substring(0, portIdx).Trim();           

                    if (hostStr.Length == 0)
                    {                     
                        throw new ArgumentException("Connection string argument missing!");
                    }

                   try
                    {
                        tmpServerList.Add(new ServerPair(hostStr, Convert.ToInt32(portStr)));
                    }
                    catch (FormatException exception)
                    {
                        throw new ArgumentException("Connection string argument missing!", exception);
                    }
                    catch (OverflowException exception)
                    {
                        throw new ArgumentException("Connection string argument missing!", exception);
                    }
                }
                else
                {
                    tmpServerList.Add(new ServerPair(serverList[i],port));
                }
            }
            return tmpServerList;           
      }
  }
    
    internal enum NpgsqlServerListManager
    {
        Unknow,
        Down,
        Master,
        Slave
    }

    internal class ServerPair
    {
        private string displayedHost;
        private int displayedPort;
        private volatile NpgsqlServerListManager status;

        public ServerPair(string value1, int value2)
        {
            DisplayedHost = value1;
            DisplayedPort = value2;
        }

        public string Host
        {
            get { return DisplayedHost; }
        }

        public int Port
        {
            get { return DisplayedPort; }
        }

        public NpgsqlServerListManager HostStatus
        {
            get { return status; }
        }

        public string DisplayedHost { get => displayedHost; set => displayedHost = value; }
        public int DisplayedPort { get => displayedPort; set => displayedPort = value; }

        public void updateHostStatus(NpgsqlServerListManager status)
        {
            this.status = status;
        }

        public string toString()
        {
            return DisplayedHost + ":" + DisplayedPort;
        }
    }
}

