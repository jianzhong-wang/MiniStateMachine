//------------------------------------------------------------------------------
// 狀態資料模型
// 組織：國立東華大學 / Organization: National Dong Hwa University
// 作者：王建中 / Author: Wang, Jian-Zhong
//------------------------------------------------------------------------------

using MiniStateMachine.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniStateMachine
{
    /// <summary>
    /// 狀態資料模型
    /// </summary>
    public class State
    {
        /// <summary>
        /// 狀態識別鍵
        /// </summary>
        public string Key { get; private set; }

        private string _displayName;
        /// <summary>
        /// 狀態顯示名稱
        /// </summary>
        public string DisplayName
        {
            get { return !string.IsNullOrEmpty(_displayName) ? _displayName : Key; }
            set { _displayName = value; }
        }

        /// <summary>
        /// 進入狀態動作委派
        /// </summary>
        /// <param name="fromState">來源狀態</param>
        public delegate void EnterAction(State fromState);
        /// <summary>
        /// 進入狀態時進行的動作
        /// </summary>
        public EnterAction OnEnter { get; set; }

        /// <summary>
        /// 進入狀態
        /// </summary>
        /// <param name="fromState">來源狀態</param>
        internal virtual void Enter(State fromState)
        {
            if (this.OnEnter != null)
            {
                this.OnEnter.Invoke(fromState);
            }
        }

        /// <summary>
        /// 離開狀態動作委派
        /// </summary>
        /// <param name="toState">目的狀態</param>
        public delegate void ExitAction(State toState);
        /// <summary>
        /// 離開狀態時進行的動作
        /// </summary>
        public ExitAction OnExit { get; set; }

        /// <summary>
        /// 離開狀態
        /// </summary>
        /// <param name="toState">目的狀態</param>
        internal virtual void Exit(State toState)
        {
            if (this.OnExit != null)
            {
                this.OnExit.Invoke(toState);
            }
        }

        /// <summary>
        /// 是否為結束狀態
        /// </summary>
        public bool IsEndState { get; set; }

        private List<Transition> _transitions;
        /// <summary>
        /// 狀態移轉集合
        /// </summary>
        public IEnumerable<Transition> Transitions
        {
            get
            {
                if (this._transitions == null)
                {
                    this._transitions = new List<Transition>();
                }
                return this._transitions;
            }
            private set
            {
                this._transitions = value?.ToList() ?? new List<Transition>();
            }
        }

        /// <summary>
        /// 設定狀態移轉集合
        /// </summary>
        /// <param name="transitions">狀態移轉集合</param>
        /// <returns>狀態實體</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transitions"/> 為 null
        /// </exception>
        /// <exception cref="DuplicatedKeyException">識別鍵重複例外</exception>
        public State SetTransitions(IEnumerable<Transition> transitions)
        {
            if (transitions == null)
            {
                throw new ArgumentNullException("transitions");
            }

            var transitionGroups = transitions.GroupBy(t => t.Key);

            if (transitions.Count() != transitionGroups.Count())
            {
                string duplicatedKey = transitionGroups.Where(g => g.Count() > 1).FirstOrDefault()?.Key;
                throw new DuplicatedKeyException($"狀態移轉集合中有狀態移轉識別鍵重複「{ duplicatedKey ?? "n/a" }」！", duplicatedKey);
            }

            this.Transitions = transitions;

            return this;
        }

        /// <summary>
        /// 建構 State 實體
        /// </summary>
        /// <param name="key">狀態識別鍵</param>
        /// <param name="transitions">狀態移轉集合</param>
        /// <param name="displayName">狀態顯示名稱</param>
        /// <param name="onEnter">進入狀態時進行的動作</param>
        /// <param name="onExit">離開狀態時進行的動作</param>
        /// <param name="isEndState">是否為結束狀態</param>
        public State(string key, IEnumerable<Transition> transitions = null, string displayName = null,
            EnterAction onEnter = null, ExitAction onExit = null, bool isEndState = false)
        {
            this.Key = key;
            this.DisplayName = displayName;
            this.OnEnter = onEnter;
            this.OnExit = onExit;
            this.IsEndState = isEndState;

            if (isEndState || transitions == null)
            {
                transitions = new List<Transition>();
            }

            SetTransitions(transitions);
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
        /// 尋找狀態移轉物件
        /// </summary>
        /// <param name="predicate">用來測試每個項目是否符合條件的函式</param>
        /// <returns>狀態移轉物件</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> 為 null
        /// </exception>
        /// <exception cref="Exception">
        /// <p>測試函式條件下找不到狀態移轉物件</p>
        /// </exception>
        public Transition FindTransition(Func<Transition, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var transition = Transitions.FirstOrDefault(predicate);

            if (transition == null)
            {
                throw new Exception($"狀態「{ToString()}」找不到狀態移轉！");
            }

            return transition;
        }

        /// <summary>
        /// 以狀態移轉識別鍵尋找狀態移轉物件
        /// </summary>
        /// <param name="transitionKey">狀態移轉識別鍵</param>
        /// <returns>狀態移轉物件</returns>
        /// <exception cref="Exception">
        /// <p>以狀態移轉識別鍵找不到狀態移轉物件</p>
        /// </exception>
        public Transition FindTransition(string transitionKey)
        {
            var transition = Transitions.FirstOrDefault(t => t.Key == transitionKey);

            if (transition == null)
            {
                throw new Exception($"狀態「{ToString()}」找不到識別鍵為「{transitionKey}」的狀態移轉！");
            }

            return transition;
        }

        /// <summary>
        /// 將狀態移轉物件加入至狀態移轉集合
        /// </summary>
        /// <param name="transition">要加入的狀態移轉物件</param>
        /// <returns>狀態移轉集合所屬的狀態物件</returns>
        /// <exception cref="DuplicatedKeyException">識別鍵重複例外</exception>
        public State AddTransition(Transition transition)
        {
            if (this._transitions.Any(t => t.Key == transition.Key))
            {
                throw new DuplicatedKeyException(null, transition.Key);
            }

            this._transitions.Add(transition);

            return this;
        }

        /// <summary>
        /// 在指定索引位置上，將狀態移轉物件插入到狀態移轉集合中
        /// </summary>
        /// <param name="index">指定索引位置</param>
        /// <param name="transition">要插入的狀態移轉物件</param>
        /// <returns>狀態移轉集合所屬的狀態物件</returns>
        /// <exception cref="DuplicatedKeyException">識別鍵重複例外</exception>
        public State InsertTransition(int index, Transition transition)
        {
            if (this._transitions.Any(t => t.Key == transition.Key))
            {
                throw new DuplicatedKeyException(null, transition.Key);
            }

            this._transitions.Insert(index, transition);

            return this;
        }

        /// <summary>
        /// 從狀態移轉集合中移除狀態移轉物件
        /// </summary>
        /// <param name="transition">要移除的狀態移轉物件</param>
        /// <param name="result">是否成功移除</param>
        /// <returns>狀態移轉集合所屬的狀態物件</returns>
        public State RemoveTransition(Transition transition, out bool result)
        {
            result = this._transitions.Remove(transition);

            return this;
        }

        /// <summary>
        /// 從狀態移轉集合中移除狀態移轉物件
        /// </summary>
        /// <param name="transition">要移除的狀態移轉物件</param>
        /// <returns>狀態移轉集合所屬的狀態物件</returns>
        public State RemoveTransition(Transition transition)
        {
            return RemoveTransition(transition, out bool _);
        }

        /// <summary>
        /// 以狀態移轉的識別鍵，從狀態移轉集合中移除狀態移轉物件
        /// </summary>
        /// <param name="transitionKey">要移除的狀態移轉之識別鍵</param>
        /// <param name="result">是否成功移除</param>
        /// <returns>狀態移轉集合所屬的狀態物件</returns>
        public State RemoveTransitionByKey(string transitionKey, out bool result)
        {
            var transition = this._transitions.SingleOrDefault(t => t.Key == transitionKey);

            if (transition == null)
            {
                result = false;
                return this;
            }

            return RemoveTransition(transition, out result);
        }

        /// <summary>
        /// 以狀態移轉的識別鍵，從狀態移轉集合中移除狀態移轉物件
        /// </summary>
        /// <param name="transitionKey">要移除的狀態移轉之識別鍵</param>
        /// <returns>狀態移轉集合所屬的狀態物件</returns>
        public State RemoveTransitionByKey(string transitionKey)
        {
            return RemoveTransitionByKey(transitionKey, out bool _);
        }

        /// <summary>
        /// 在指定索引位置上，從狀態移轉集合中移除狀態移轉物件
        /// </summary>
        /// <param name="index">指定索引位置</param>
        /// <returns>狀態移轉集合所屬的狀態物件</returns>
        public State RemoveTransitionAt(int index)
        {
            this._transitions.RemoveAt(index);

            return this;
        }

        /// <summary>
        /// 從狀態移轉集合中移除所有項目
        /// </summary>
        /// <returns>狀態移轉集合所屬的狀態物件</returns>
        public State ClearTransitions()
        {
            this._transitions.Clear();

            return this;
        }
    }
}
