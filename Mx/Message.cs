using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Timers;
using System.Threading;

namespace Mx
{

    namespace Server
    {
        public class Message : EventTask
        {

            private string pkgId;

            private byte[] senContent;

            private string refContent;

            private bool isDone;

            private System.Timers.Timer timer;

            public Message(Socket client, byte[] content)
            {
                senContent = content;
                if (NetManager.Instance.State != NetState.NET_NORMAL)
                {
                    NetManager.Instance.clients = client;
                }

                MessageManager.Instance.AddTask(this);
            }


            public string GetSenContent()
            {
                return Encoding.UTF8.GetString(senContent);
            }

            public string GetContent()
            {
                return refContent;
            }
            public void Init()
            {
                timer = new System.Timers.Timer(1000);
                timer.Elapsed += new ElapsedEventHandler(PacketLossHandler);
                timer.Enabled = true;
            }

            private bool isPause;

            public void Pause()
            {
                isPause = true;
                timer.Stop();
            }

            public void Resume()
            {
                isPause = true;
                timer.Start();
            }

            public void Stop()
            {
                Close("信息发送失败,强制停止~!!!");
            }

            public bool GetIsDone()
            {
                return isDone;
            }

            public void Run()
            {
                while(true)
                {
                    if (NetManager.Instance.State == NetState.NET_WAIT_TESTING)
                    {
                        Thread.Sleep(100);
                    }
                    else 
                    {
                        break;
                    }
                }

                SendRequest();
                Receive();
            }

            public string lg()
            {
                return NetManager.Instance.Log + "||状态:？？？？？？";
            }

            private void SendRequest()
            {
                while (true)
                {
                    string id = Utils.Tool.GetRandomId(6);
                    if (!MessageManager.Instance.CtnId(id))
                    {
                        pkgId = id;
                        MessageManager.Instance.AddId(id);
                        break;
                    }
                }
                Send();
            }

            private void Receive()
            {
                while (!isDone)
                {
                    if (!isPause)
                    {
                        if (NetManager.Instance.State == NetState.NET_LOST_LINKS)
                        {
                            Close(NetManager.Instance.Log);
                            break;
                        }
                        else
                        {
                            if (MessageManager.Instance.CtnSucPkg(pkgId))
                            {
                                Close(Encoding.UTF8.GetString(MessageManager.Instance.GetSucPkg(pkgId)));
                                MessageManager.Instance.RemSucPkg(pkgId);
                                MessageManager.Instance.RemId(pkgId);
                                break;
                            }
                            else if (MessageManager.Instance.CtnFailPkgId(pkgId))
                            {
                                timer.Stop();
                                MessageManager.Instance.RemFailPkgId(pkgId);
                                Send();
                            }
                        }
                    }
                }
            }

            private void PacketLossHandler(object source, ElapsedEventArgs e)
            {
                if (!pkgId.Equals(""))
                {
                    Send();
                }
            }

            private void Send()
            {

                if (NetManager.Instance.State == NetState.NET_LOST_LINKS)
                {
                    Close(NetManager.Instance.Log);
                    return;
                }

                try
                {

                    NetManager.Instance.clients.Send(UserData.Instance.MsgPkg(pkgId, senContent));

                    timer.Start();

                }
                catch (Exception e)
                {
                    Close("发送失败,网络存在异常!!!");
                }

            }

            private void Close(string content)
            {
                timer.Stop();
                refContent = content;
                isDone = true;
            }
        }
    }
}
