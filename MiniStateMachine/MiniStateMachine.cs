//------------------------------------------------------------------------------
// MiniStateMachine 狀態機
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
    /// MiniStateMachine 狀態機
    /// </summary>
    public class MiniStateMachine
    {
        /// <summary>
        /// 狀態機顯示名稱
        /// </summary>
        public string DisplayName { get; set; }

        private List<State> _states;
        /// <summary>
        /// 狀態集合
        /// </summary>
        public IEnumerable<State> States
        {
            get
            {
                if (this._states == null)
                {
                    this._states = new List<State>();
                }
                return this._states;
            }
            private set
            {
                this._states = value?.ToList() ?? new List<State>();
            }
        }

        /// <summary>
        /// 設定狀態集合
        /// </summary>
        /// <param name="states">狀態集合</param>
        /// <returns>狀態機實體</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="states"/> 為 null
        /// </exception>
        /// <exception cref="DuplicatedKeyException">識別鍵重複例外</exception>
        public MiniStateMachine SetStates(IEnumerable<State> states)
        {
            if (states == null)
            {
                throw new ArgumentNullException("states");
            }

            var stateGroups = states.GroupBy(s => s.Key);

            if (states.Count() != stateGroups.Count())
            {
                string duplicatedKey = stateGroups.Where(g => g.Count() > 1).FirstOrDefault()?.Key;
                throw new DuplicatedKeyException($"狀態集合中有狀態識別鍵重複「{ duplicatedKey ?? "n/a" }」！", duplicatedKey);
            }

            this.States = states;

            return this;
        }

        // 狀態機目前狀態
        private State _currentState;

        /// <summary>
        /// 取得狀態機目前狀態，預設為狀態集合中的第一個狀態項目
        /// </summary>
        /// <returns>狀態機目前狀態</returns>
        public State GetCurrentState()
        {
            return this._currentState ?? this._states?.FirstOrDefault();
        }

        /// <summary>
        /// 設定狀態機目前狀態
        /// </summary>
        /// <param name="currentState">要設定的目前狀態</param>
        /// <returns>狀態機實體</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="currentState"/> 為 null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// 在狀態集合裡找不到 <paramref name="currentState"/>
        /// </exception>
        public MiniStateMachine SetCurrentState(State currentState)
        {
            if (currentState is null)
            {
                throw new ArgumentNullException("currentState");
            }

            if (!(this._states?.Contains(currentState) ?? false))
            {
                throw new ArgumentException($"狀態集合裡找不到要設定的目前狀態「{currentState.Key}」！", "currentState");
            }

            this._currentState = currentState;

            return this;
        }

        /// <summary>
        /// 以狀態識別項設定狀態機目前狀態
        /// </summary>
        /// <param name="stateKey">狀態識別項</param>
        /// <returns>狀態機實體</returns>
        public MiniStateMachine SetCurrentStateByKey(string stateKey)
        {
            var state = this._states.Single(s => s.Key == stateKey);
            return SetCurrentState(state);
        }

        /// <summary>
        /// 建構 MiniStateMachine 實體
        /// </summary>
        /// <param name="states">狀態集合</param>
        public MiniStateMachine(IEnumerable<State> states = null)
        {
            SetStates(states ?? new List<State>());
        }

        /// <summary>
        /// 建構 MiniStateMachine 實體
        /// </summary>
        /// <param name="initialState">啟始狀態</param>
        /// <param name="states">狀態集合</param>
        /// <param name="displayName">狀態機顯示名稱</param>
        public MiniStateMachine(State initialState, IEnumerable<State> states, string displayName = null)
        {
            SetStates(states)
                .SetCurrentState(initialState);

            this.DisplayName = displayName;
        }

        /// <summary>
        /// 將狀態物件加入至狀態集合
        /// </summary>
        /// <param name="state">要加入的狀態物件</param>
        /// <returns>狀態集合所屬的狀態機物件</returns>
        public MiniStateMachine AddState(State state)
        {
            if (this._states.Any(s => s.Key == state.Key))
            {
                throw new DuplicatedKeyException(null, state.Key);
            }

            this._states.Add(state);

            return this;
        }

        /// <summary>
        /// 在指定索引位置上，將狀態物件插入到狀態集合中
        /// </summary>
        /// <param name="index">指定索引位置</param>
        /// <param name="state">要插入的狀態物件</param>
        /// <returns>狀態集合所屬的狀態機物件</returns>
        /// <exception cref="DuplicatedKeyException">識別鍵重複例外</exception>
        public MiniStateMachine InsertState(int index, State state)
        {
            if (this._states.Any(s => s.Key == state.Key))
            {
                throw new DuplicatedKeyException(null, state.Key);
            }

            this._states.Insert(index, state);

            return this;
        }

        /// <summary>
        /// 從狀態集合中移除狀態物件
        /// </summary>
        /// <param name="state">要移除的狀態物件</param>
        /// <param name="result">是否成功移除</param>
        /// <returns>狀態集合所屬的狀態機物件</returns>
        public MiniStateMachine RemoveState(State state, out bool result)
        {
            result = this._states.Remove(state);

            return this;
        }

        /// <summary>
        /// 從狀態集合中移除狀態物件
        /// </summary>
        /// <param name="state">要移除的狀態物件</param>
        /// <returns>狀態集合所屬的狀態機物件</returns>
        public MiniStateMachine RemoveState(State state)
        {
            return RemoveState(state, out bool _);
        }

        /// <summary>
        /// 以狀態的識別鍵，從狀態集合中移除狀態物件
        /// </summary>
        /// <param name="stateKey">要移除的狀態之識別鍵</param>
        /// <param name="result">是否成功移除</param>
        /// <returns>狀態集合所屬的狀態機物件</returns>
        public MiniStateMachine RemoveStateByKey(string stateKey, out bool result)
        {
            var state = this._states.SingleOrDefault(s => s.Key == stateKey);

            if (state == null)
            {
                result = false;
                return this;
            }

            return RemoveState(state, out result);
        }

        /// <summary>
        /// 以狀態的識別鍵，從狀態集合中移除狀態物件
        /// </summary>
        /// <param name="stateKey">要移除的狀態之識別鍵</param>
        /// <returns>狀態集合所屬的狀態機物件</returns>
        public MiniStateMachine RemoveStateByKey(string stateKey)
        {
            return RemoveStateByKey(stateKey, out bool _);
        }

        /// <summary>
        /// 在指定索引位置上，從狀態集合中移除狀態物件
        /// </summary>
        /// <param name="index">指定索引位置</param>
        /// <returns>狀態集合所屬的狀態機物件</returns>
        public MiniStateMachine RemoveStateAt(int index)
        {
            this._states.RemoveAt(index);

            return this;
        }

        /// <summary>
        /// 從狀態集合中移除所有項目
        /// </summary>
        /// <returns>狀態集合所屬的狀態機物件</returns>
        public MiniStateMachine ClearTransitions()
        {
            this._states.Clear();

            return this;
        }

        /// <summary>
        /// 在狀態集合中尋找狀態物件
        /// </summary>
        /// <param name="predicate">用來測試每個項目是否符合條件的函式</param>
        /// <returns>狀態物件</returns>
        public State FindState(Func<State, bool> predicate)
        {
            return this._states?.FirstOrDefault(predicate);
        }

        /// <summary>
        /// 以狀態識別鍵在狀態集合中尋找狀態物件
        /// </summary>
        /// <param name="stateKey">狀態識別鍵</param>
        /// <returns>狀態物件</returns>
        public State FindState(string stateKey)
        {
            return FindState(s => s.Key == stateKey);
        }

        /// <summary>
        /// 驗證狀態機是否有效
        /// </summary>
        /// <returns>狀態機是否有效</returns>
        public bool Validate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 取得目前狀態的狀態移轉集合
        /// </summary>
        /// <returns>目前狀態的狀態移轉集合</returns>
        public IEnumerable<Transition> GetCurrentTransitions()
        {
            return GetCurrentState()?.Transitions;
        }

        /// <summary>
        /// 從目前狀態尋找狀態移轉物件
        /// </summary>
        /// <param name="predicate">用來測試每個項目是否符合條件的函式</param>
        /// <returns>狀態移轉物件</returns>
        public Transition FindTransitionFromCurrentState(Func<Transition, bool> predicate)
        {
            return GetCurrentTransitions()?.FirstOrDefault(predicate);
        }

        /// <summary>
        /// 以狀態移轉識別鍵從目前狀態尋找狀態移轉物件
        /// </summary>
        /// <param name="transitionKey">狀態移轉識別鍵</param>
        /// <returns>狀態移轉物件</returns>
        public Transition FindTransitionFromCurrentState(string transitionKey)
        {
            return FindTransitionFromCurrentState(t => t.Key == transitionKey);
        }

        /// <summary>
        /// 執行狀態移轉
        /// </summary>
        /// <param name="transition">要執行的狀態移轉</param>
        /// <returns>狀態移轉後的狀態</returns>
        public virtual State ExecuteTransition(Transition transition)
        {
            if (transition == null)
            {
                throw new ArgumentNullException("transition");
            }

            if (!(GetCurrentTransitions()?.Contains(transition) ?? false))
            {
                throw new ChangeStateException("目前狀態的狀態移轉集合找不到要嘗試進行的狀態移轉", null, null, transition);
            }

            State from = GetCurrentState();
            State to = transition.ToState;

            try
            {
                from.Exit(to);
                transition.Execute();
                to.Enter(from);
            }
            catch (Exception ex)
            {
                throw new ChangeStateException($"執行狀態移轉從狀態「{from.Key}」到狀態「{to.Key}」時發生異常例外！",
                    from, to, transition, ex);
            }

            SetCurrentState(to);

            return to;
        }
    }
}
