using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Mx.Server;
using System.Net.Sockets;

namespace Mx
{
    class MessageManager : DataHandler
    {

        private static volatile MessageManager instance;
        private static readonly object syncRoot = new object();
        private MessageManager()
        {
            Init();
        }

        public static MessageManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new MessageManager();
                        }
                    }
                }
                return instance;
            }
        }

        private void Init()
        {
            threadRed = new Thread(new ThreadStart(ReadRun));
            threadRed.IsBackground = true;
            threadRed.Start();

            threadTask = new Thread(new ThreadStart(TaskRun));
            threadTask.IsBackground = true;
            threadTask.Start();
        }

        #region 共享数据

        #region 数据包
        private Thread threadRed;

        private Dictionary<string, byte[]> sucesDataPkg = new Dictionary<string, byte[]>();

        private List<string> failPkgId = new List<string>();

        private List<string> idlist = new List<string>();

        public void AddSucPkg(string id, byte[] content)
        {
            lock (sucesDataPkg)
            {
                sucesDataPkg.Add(id, content);
            }
        }

        public void RemSucPkg(string id)
        {
            lock (sucesDataPkg)
            {
                sucesDataPkg.Remove(id);
            }
        }

        public bool CtnSucPkg(string id)
        {
            lock (sucesDataPkg)
            {
                if (sucesDataPkg.ContainsKey(id))
                {
                    return true;
                }
                return false;
            }
        }

        public byte[] GetSucPkg(string id)
        {
            lock (sucesDataPkg)
            {
                return sucesDataPkg[id];
            }
        }

        public void AddFailPkgId(string id)
        {
            lock (failPkgId)
            {
                failPkgId.Add(id);
            }
        }

        public void RemFailPkgId(string id)
        {
            lock (failPkgId)
            {
                failPkgId.Remove(id);
            }
        }

        public bool CtnFailPkgId(string id)
        {
            lock (failPkgId)
            {
                if (failPkgId.Contains(id))
                {
                    return true;
                }
                return false;
            }
        }

        public void AddId(string id)
        {
            lock (idlist)
            {
                idlist.Add(id);
            }
        }

        public void RemId(string id)
        {
            lock (idlist)
            {
                idlist.Remove(id);
            }
        }

        public bool CtnId(string id)
        {
            lock (idlist)
            {
                if (idlist.Contains(id))
                {
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region 任务列
        private Dictionary<string, EventTask> taskList = new Dictionary<string, EventTask>();
        public void AddTask(EventTask task)
        {
            lock (taskList)
            {
                string content = task.GetSenContent();
                if (!CtnTask(content))
                {
                    taskList.Add(content, task);
                }
            }
        }

        public void RemTask(EventTask task)
        {
            lock (taskList)
            {
                string content = task.GetSenContent();

                if (CtnTask(content))
                {
                    taskList.Remove(content);
                }
            }
        }

        public bool CtnTask(string content)
        {
            lock (taskList)
            {
                if (taskList.ContainsKey(content))
                {
                    return true;
                }
                return false;
            }
        }

        public EventTask GetTask()
        {
            lock (taskList)
            {
                if (taskList.Count > 0)
                {
                    return taskList[taskList.Keys.First()];
                }

                return null;
            }
        }

        #endregion

        #endregion

        #region 发送任务

        private Thread threadTask;

        private void TaskRun()
        {
            while (true)
            {
                EventTask currentTask = GetTask();

                if (currentTask != null)
                {
                    currentTask.Init();

                    currentTask.Run();

                    while (!currentTask.GetIsDone())
                    {
                        Thread.Sleep(100);
                    }

                    RemTask(currentTask);
                }
            }
        }

        #endregion

        #region 读取数据

        private void ReadRun()
        {
            byte[] buffer = new byte[1024 * 1024];
            int len = 0;
            while (true)
            {
                try
                {
                    len = NetManager.Instance.clients.Receive(buffer);
                    //区分是客户端来了，还是消息来了  
                    if (buffer[0] != 1)//客户端  
                    {
                        byte[] id = new byte[6];
                        Buffer.BlockCopy(buffer, 0, id, 0, id.Length);
                        string ids = Encoding.UTF8.GetString(id);

                        if (idlist.Contains(ids))
                        {
                            byte[] bytes = new byte[8];
                            Buffer.BlockCopy(buffer, 6, bytes, 0, bytes.Length);
                            int length = Convert.ToInt32(Encoding.UTF8.GetString(bytes));

                            if (len >= length + 14)
                            {
                                byte[] bytes1 = new byte[length];
                                Buffer.BlockCopy(buffer, id.Length + bytes.Length, bytes1, 0, bytes1.Length);
                                AddSucPkg(ids, bytes1);
                            }
                            else
                            {
                                AddFailPkgId(ids);
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
        }
        #endregion
    }
}
