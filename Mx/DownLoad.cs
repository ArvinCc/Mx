using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using Mx.Json;

namespace Mx
{
    namespace Server
    {
        public class DownLoad
        {
            private Socket client;

            public string pkgId;

            private bool isStart;

            private bool isDone;

            private string path;

            private string name;

            private long fileSize;

            private long currentSize;

            private Thread thread;

            private string fileName;


            public string GetFileName() {

                return fileName;
            }
            public DownLoad(Socket client, string path, string name)
            {
                this.client = client;
                this.path = path;
                this.name = name;
                thread = new Thread(new ThreadStart(Run));
                thread.IsBackground = true;
            }

            public string GetPath()
            {
                return path;
            }

            public string GetName()
            {
                return name;
            }

            public bool GetIsDone()
            {
                return isDone;
            }

            public long GetCurrentSize()
            {
                long i = currentSize / 1024;
                return i;
            }

            public long GetFileSize()
            {
                long i = fileSize / 1024;
                return i;
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
                    try
                    {
                        if (StartRequstData())
                        {
                            WriteIn();
                        }
                        else
                        {
                            Close();
                        }
                    }
                    catch (Exception e)
                    {
                        //异常 e暂时未处理
                    }
                }
            }


            private bool StartRequstData()
            {
                Message msg = new Message(client, Encoding.UTF8.GetBytes(UserData.Instance.DownloadFile("on", name, "0")));

                msg.Start();

                while (true)
                {
                    if (msg.GetIsDone())
                    {
                        JSONArray ja = new JSONArray(msg.GetContent());
                        pkgId = ja.getJSONObject(1).getString("id");
                        //name = ja.getJSONObject(1).getString("name");
                        fileSize = Convert.ToInt32(ja.getJSONObject(1).getString("size"));
                        break;
                    }
                }

                SubFileHandler();

                if (File.Exists(fileName))
                {
                    FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);

                    if (fs.Length != 0 && fs.Length < fileSize)
                    {
                        currentSize = fs.Length;
                    }
                    else
                    {
                        return false;
                    }

                    fs.Flush();
                    fs.Close();
                }
                else
                {
                    FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                    fs.Position = fs.Length;
                    fs.Flush();
                    fs.Close();
                }

                ContinueRequstData();

                return true;
            }


            private void WriteIn()
            {
                byte[] buffer = new byte[1024 * 1024];

                while (true)
                {
                    client.Receive(buffer);
                    if (buffer[0] != 1)
                    {
                        byte[] id = new byte[6];

                        Buffer.BlockCopy(buffer, 0, id, 0, id.Length);
                        if (Encoding.UTF8.GetString(id).Equals(pkgId))
                        {
                            byte[] datalength = new byte[8];
                            Buffer.BlockCopy(buffer, id.Length, datalength, 0, datalength.Length);
                            int length = Convert.ToInt32(Encoding.UTF8.GetString(datalength));
                            byte[] data = new byte[length];

                            Buffer.BlockCopy(buffer, id.Length + datalength.Length, data, 0, data.Length);
                            if (File.Exists(fileName))
                            {
                                FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
                                fs.Position = fs.Length;
                                fs.Write(data, 0, data.Length);
                                fs.Flush();
                                fs.Close();

                                currentSize += data.Length;
                            }
                            else
                            {
                                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                                currentSize = fs.Length;
                                fs.Position = fs.Length;
                                fs.Flush();
                                fs.Close();
                            }

                            if (currentSize < fileSize)
                            {
                                ContinueRequstData();
                            }
                            else
                            {
                                Close();
                                break;
                            }
                        }
                    }
                }
            }



            private bool Abnormal() 
            {
                if (path.Trim().Equals(""))
                {
                    return false;
                }
                return false;
            }

            /// <summary>
            /// 子文件处理
            /// </summary>
            private void SubFileHandler()
            {
                StringBuilder p = new StringBuilder();
                StringBuilder n = new StringBuilder();

                if (name.IndexOf("\\") != -1||name.IndexOf("/") != -1)
                {
                    StringHandler(name,ref n);
                }

                    StringHandler(path, ref p);

                if (n.ToString().Trim().Equals(""))
                {
                    fileName = p.ToString() + "/" + name;
                }
                else
                {
                    fileName = p.ToString() + "/" + n.ToString();
                }
                string strPath = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
            }

            private void StringHandler(string st,ref StringBuilder b )
            {
                string[] o = st.Split(new char[] { '\\', '/' });
                for (int i = 0; i < o.Length; i++)
                {
                    if (i < o.Length - 1)
                    {
                        b.Append(o[i] + "/");
                    }
                    else
                    {
                        b.Append(o[i]);
                    }
                }
            }

            private void Close()
            {
                currentSize = fileSize;
                isDone = true;
            }

            private void ContinueRequstData()
            {
                byte[] buffers = Encoding.UTF8.GetBytes(UserData.Instance.DownloadFile("in", name, "" + currentSize));
                client.Send(UserData.Instance.MsgPkg(pkgId, buffers));
            }

        }
    }

}
