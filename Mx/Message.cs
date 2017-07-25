using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Timers;

namespace Mx
{

    namespace Server
    {
        public class Message
        {
            private Socket client;

            private string pkgId;

            private byte[] senContent;

            private string refContent;

            private bool isDone;

            private Thread thread;

            private System.Timers.Timer timer;

            public Message(Socket client, byte[] content)
            {
                this.client = client;
                this.senContent = content;
                thread = new Thread(new ThreadStart(Run));
                thread.IsBackground = true;
                timer = new System.Timers.Timer(2000);
                timer.Elapsed += new ElapsedEventHandler(PacketLossHandler);
                timer.Enabled = true;
            }

            public string GetContent()
            {
                return refContent;
            }

            public bool GetIsDone()
            {
                return isDone;
            }

            public void Start()
            {
                if (thread != null)
                {
                    thread.Start();
                }
            }

            public void Pause()
            {
                if (thread != null && thread.ThreadState.ToString().Equals("Running") && !isDone)
                {
                    thread.Suspend();
                }
            }

            public void Resume()
            {
                if (thread != null && thread.ThreadState.ToString().Equals("Suspended") && !isDone)
                {
                    thread.Resume();
                }
            }

            public void Stop()
            {
                isDone = true;
            }


            public void Run()
            {
                if (client != null)
                {
                    SendRequest();

                    Receive();
                }
            }


            public void SendRequest()
            {
                timer.Start();
                pkgId = Utils.Tool.GetRandomId(6);
                client.Send(UserData.Instance.MsgPkg(pkgId, senContent));
            }

            public string Log()
            {

                return msg;
            }

            private string msg;

            private void Receive()
            {
                byte[] buffer = new byte[1024 * 1024];
                try
                {
                    int len = 0;
                    while (!isDone)
                    {
                        len = client.Receive(buffer);
                        if (buffer[0] != 1)
                        {
                            msg = "采集到多少字节";
                            byte[] id = new byte[6];
                            Buffer.BlockCopy(buffer, 0, id, 0, id.Length);
                            if (Encoding.UTF8.GetString(id).Equals(pkgId))
                            {
                                byte[] bytes = new byte[8];
                                Buffer.BlockCopy(buffer, 6, bytes, 0, bytes.Length);
                                int length = Convert.ToInt32(Encoding.UTF8.GetString(bytes));
                                byte[] bytes1 = new byte[length];
                                Buffer.BlockCopy(buffer, id.Length + bytes.Length, bytes1, 0, bytes1.Length);
                                Close(Utils.Tool.StringHandler(Encoding.UTF8.GetString(bytes1)));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Close("msgerror:" + e);
                    //异常未处理 e
                }

                //byte[] buffer = new byte[1024 * 1024];
                //try
                //{
                //    while (!isDone)
                //    {
                //        client.Receive(buffer);

                //        if (buffer[0] != 1)
                //        {
                //            try
                //            {
                //                byte[] id = new byte[6];
                //                Buffer.BlockCopy(buffer, 0, id, 0, id.Length);

                //                if (Encoding.UTF8.GetString(id).Equals(pkgId))
                //                {
                //                    byte[] bytes = new byte[8];
                //                    Buffer.BlockCopy(buffer, 6, bytes, 0, bytes.Length);
                //                    int length = Convert.ToInt32(Encoding.UTF8.GetString(bytes));
                //                    byte[] tail = new byte[2];
                //                    Buffer.BlockCopy(buffer, id.Length + bytes.Length + length, tail, 0, tail.Length);
                //                    if (Encoding.UTF8.GetString(tail).Equals("mx"))
                //                    {
                //                        byte[] bytes1 = new byte[length];
                //                        Buffer.BlockCopy(buffer, 6 + bytes.Length, bytes1, 0, bytes1.Length);
                //                        Close(Utils.Tool.StringHandler(Encoding.UTF8.GetString(bytes1)));
                //                    }
                //                }
                //            }
                //            catch (Exception e)
                //            {
                //                Close("msgerror:" + e);
                //            }
                //        }
                //    }
                //}
                //catch (Exception e)
                //{
                //    Close("msgerror:" + e);
                //    //异常未处理 e
                //}
            }
            private void PacketLossHandler(object source, ElapsedEventArgs e)
            {
                timer.Stop();
                Close("msgget:fail!");
            }

            private void Close(string content)
            {
                refContent = content;
                isDone = true;
            }
        }
    }
}
