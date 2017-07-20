using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

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

            public Message(Socket client, byte[] content)
            {
                this.client = client;
                this.senContent = content;
                thread = new Thread(new ThreadStart(Run));
                thread.IsBackground = true;
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
                pkgId = Utils.Tool.GetRandomId(6);

                client.Send(UserData.Instance.MsgPkg(pkgId, senContent));
            }

            private void Receive()
            {
                byte[] buffer = new byte[1024 * 1024];

                try
                {
                    while (true)
                    {
                        client.Receive(buffer);
                        if (buffer[0] != 1)
                        {
                            byte[] id = new byte[6];
                            Buffer.BlockCopy(buffer, 0, id, 0, id.Length);

                            if (Encoding.UTF8.GetString(id).Equals(pkgId))
                            {

                                byte[] bytes = new byte[8];

                                Buffer.BlockCopy(buffer, 6, bytes, 0, bytes.Length);

                                int length = Convert.ToInt32(Encoding.UTF8.GetString(bytes));

                                byte[] bytes1 = new byte[length];

                                Buffer.BlockCopy(buffer, 6 + bytes.Length, bytes1, 0, bytes1.Length);

                                refContent = Utils.Tool.StringHandler(Encoding.UTF8.GetString(bytes1)); 
                                isDone = true;
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //异常未处理 e
                }
            }
        }
    }
}
