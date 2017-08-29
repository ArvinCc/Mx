using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mx
{

    namespace FSM 
    {

        interface IState
        {
            /// <summary>
            /// 获取状态机的ID
            /// </summary>
            /// <returns></returns>
            uint GetStateID();


            /// <summary>
            /// 进入这个状态
            /// </summary>
            /// <param name="machine">状态机</param>
            /// <param name="prevState">上一个状态</param>
            /// <param name="param1">参数1</param>
            /// <param name="param2">参数2</param>
            void OnEnter(StateMachine machine,IState prevState,object param1,object param2);


            /// <summary>
            /// 离开当前这状态
            /// </summary>
            /// <param name="nextState">下一个要进入的状态</param>
            /// <param name="param1">参数1</param>
            /// <param name="param2">参数2</param>
            void OnLeave(IState nextState,object param1,object param2);


            void OnUpdate();


            void OnFixedUpdate();


            void OnLeteUpdate();

        }
    }
}
