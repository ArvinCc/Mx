using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mx
{
    interface EventTask
    {
        void Init();

        void Pause();

        void Resume();

        void Stop();

        void Run();

        bool GetIsDone();

        string GetSenContent();

    }
}
