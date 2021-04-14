//------------------------------------------------------------------------------
// 狀態移轉資料模型
// 組織：國立東華大學 / Organization: National Dong Hwa University
// 作者：王建中 / Author: Wang, Jian-Zhong
//------------------------------------------------------------------------------

using System;

namespace MiniStateMachine
{
    /// <summary>
    /// 狀態移轉資料模型
    /// </summary>
    public class Transition
    {
        /// <summary>
        /// 狀態移轉識別鍵
        /// </summary>
        public string Key { get; private set; }

        private string _displayName;
        /// <summary>
        /// 狀態移轉顯示名稱
        /// </summary>
        public string DisplayName
        {
            get { return !string.IsNullOrEmpty(_displayName) ? _displayName : Key; }
            set { _displayName = value; }
        }

        /// <summary>
        /// 移轉完成後的目的狀態
        /// </summary>
        public State ToState { get; set; }

        /// <summary>
        /// 執行移轉時的動作
        /// </summary>
        public Action ExecutingAction { get; set; }

        /// <summary>
        /// 執行移轉
        /// </summary>
        internal virtual void Execute()
        {
            if (this.ExecutingAction != null)
            {
                ExecutingAction();
            }
        }

        /// <summary>
        /// 建構 Transition 實體
        /// </summary>
        /// <param name="key">狀態移轉識別鍵</param>
        /// <param name="toState">移轉完成後的目的狀態</param>
        /// <param name="displayName">狀態移轉顯示名稱</param>
        /// <param name="executingAction">執行移轉時的動作</param>
        public Transition(string key, State toState = null, string displayName = null,
            Action executingAction = null)
        {
            this.Key = key;
            this.ToState = toState;
            this.DisplayName = displayName;
            this.ExecutingAction = executingAction;
        }

        /// <summary>
        /// 傳回代表目前物件的字串
        /// </summary>
        /// <returns>表示目前物件的字串</returns>
        public override string ToString()
        {
            return $"Key: {Key}, DisplayName: {DisplayName}";
        }

        /// <summary>
        /// 設定移轉完成後的目的狀態
        /// </summary>
        /// <param name="toState">移轉完成後的目的狀態</param>
        /// <returns>轉態移轉實體</returns>
        public Transition SetToState(State toState)
        {
            this.ToState = toState;

            return this;
        }

        /// <summary>
        /// 設定執行移轉時的動作
        /// </summary>
        /// <param name="executingAction">執行移轉時的動作</param>
        /// <returns>轉態移轉實體</returns>
        public Transition SetExecutingAction(Action executingAction)
        {
            this.ExecutingAction = executingAction;

            return this;
        }
    }
}
