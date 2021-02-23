//------------------------------------------------------------------------------
// MiniStateMachine - 變更狀態異常例外
// 組織：國立東華大學 / Organization: National Dong Hwa University
// 作者：王建中 / Author: Wang, Jian-Zhong
//------------------------------------------------------------------------------

using System;

namespace MiniStateMachine.Exceptions
{
    /// <summary>
    /// 變更狀態異常例外
    /// </summary>
    [Serializable]
    public class ChangeStateException : Exception
    {
        /// <summary>
        /// 嘗試進行狀態移轉的來源狀態
        /// </summary>
        public State FromState { get; private set; }

        /// <summary>
        /// 嘗試進行狀態移轉的目的狀態
        /// </summary>
        public State ToState { get; private set; }

        /// <summary>
        /// 嘗試進行的狀態移轉
        /// </summary>
        public Transition Transition { get; private set; }

        /// <summary>
        /// 建構 ChangeStateException 實體
        /// </summary>
        /// <param name="message">錯誤訊息</param>
        /// <param name="fromState">嘗試進行狀態移轉的來源狀態</param>
        /// <param name="toState">嘗試進行狀態移轉的目的狀態</param>
        /// <param name="transition">嘗試進行的狀態移轉</param>
        /// <param name="inner">內部例外</param>
        public ChangeStateException(string message,
            State fromState, State toState, Transition transition, Exception inner = null) : base(message, inner)
        {
            this.FromState = fromState;
            this.ToState = toState;
            this.Transition = transition;
        }

        public ChangeStateException() { }
        public ChangeStateException(string message) : base(message) { }
        public ChangeStateException(string message, Exception inner) : base(message, inner) { }
        protected ChangeStateException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
