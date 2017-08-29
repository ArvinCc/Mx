using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mx
{

    namespace FSM
    {
        class StateMachine
        {

            public StateMachine()
            {
                mStateDic = new Dictionary<uint, IState>();
            }

            /// <summary>
            /// 注册
            /// </summary>
            /// <param name="state">状态</param>
            /// <returns>是否成功</returns>
            public bool RegisterState(IState state)
            {
                if (state == null)
                {
                    // Debug.LogError("StateMachine.RegisterState state=null");
                    return false;
                }

                if (mStateDic.ContainsKey(state.GetStateID()))
                {
                    //Debug.LogError("StateMachine.RegisterState mStateDic have this key,  key=" + state.GetStateID());
                    return false;
                }

                mStateDic.Add(state.GetStateID(), state);
                return true;
            }

            /// <summary>
            /// 注销状态
            /// </summary>
            /// <param name="stateId">状态id</param>
            /// <returns>是否成功</returns>
            public bool RemoveState(uint stateId)
            {
                if (!mStateDic.ContainsKey(stateId))
                {
                    return false;
                }
                if (mCurrentState != null && mCurrentState.GetStateID() == stateId)
                {
                    return false;
                }
                
                mStateDic.Remove(stateId);
                
                return true;
            }

            public bool SwitchState(uint newStateId,object param1,object param2)
            {

                if(mCurrentState !=null && mCurrentState.GetStateID()==newStateId)
                {
                    return false;
                }

                IState newState = null;
                mStateDic.TryGetValue(newStateId, out newState);

                if (newState == null)
                {
                    return false;
                }

                if (mCurrentState != null)
                {
                    mCurrentState.OnLeave(newState, param1, param2);
                }


                return true;
            }



            public IState CurrentState
            {
                get
                {
                    return mCurrentState;
                }
            }

            public uint CurrentID
            {
                get
                {
                    return mCurrentState == null ? 0 : mCurrentState.GetStateID();
                }
            }

            public IState OldState
            {
                get
                {
                    return mOldState;
                }
            }

            public uint OldID
            {
                get
                {
                    return mOldState == null ? 0 : mOldState.GetStateID();
                }
            }

            public void OnUpdate()
            {
                if (mCurrentState == null)
                    return;

                mCurrentState.OnUpdate();
            }

            public void OnFixedUpdate()
            {
                if (mCurrentState == null)
                    return;

                mCurrentState.OnFixedUpdate();
            }


            public void OnLeteUpdate()
            {
                if (mCurrentState == null)
                    return;

                mCurrentState.OnLeteUpdate();
            }


            private Dictionary<uint, IState> mStateDic = null;

            private IState mCurrentState = null;

            private IState mOldState = null;
        }
    }
}
