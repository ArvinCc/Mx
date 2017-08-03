using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;

namespace Mx
{
    public enum NetState
    {
        NET_WAIT_TESTING,
        NET_NORMAL,
        NET_LOST_LINKS
    }

    class NetManager
    {

        private Thread thread;

        private static volatile NetManager instance;
        private static readonly object syncRoot = new object();
        private NetManager()
        {
            thread = new Thread(new ThreadStart(Run));
            thread.IsBackground = true;
            thread.Start();
        }

        public static NetManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new NetManager();
                        }
                    }
                }
                return instance;
            }
        }

        private Socket client;

        public Socket clients
        {
            get
            {
                return client;
            }
            set
            {
                state = NetState.NET_WAIT_TESTING;
                client = value;
            }
        }

        private NetState state = NetState.NET_WAIT_TESTING;

        public NetState State
        {
            get { return state; }
        }


        private bool runState;
        public bool clientState
        {
            get { return runState; }
        }


        private string runLog;
        public string Log
        {
            get { return runLog; }
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    if (!((client.Poll(1000, SelectMode.SelectRead) && (client.Available == 0)) || !client.Connected))
                    {
                        CloseClent("一切正常~~~!!!", NetState.NET_NORMAL);
                    }
                    else
                    {
                        CloseClent("未连接服务器或已断开~~~!!!", NetState.NET_LOST_LINKS);
                    }

                }
                catch (Exception e)
                {
                    CloseClent("有错错错~~~!!!" + e, NetState.NET_LOST_LINKS);
                    Thread.Sleep(100);
                }
            }
        }

        private void CloseClent(string content, NetState states)
        {
            runLog = content;
            state = states;
        }
    }
}
