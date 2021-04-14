//------------------------------------------------------------------------------
// 狀態機
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
    /// 狀態機
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// 錯誤碼
        /// </summary>
        public enum ErrorCode
        {
            /// <summary>
            /// 沒有錯誤
            /// </summary>
            NoError,
            /// <summary>
            /// 狀態移轉為 null
            /// </summary>
            TransitionNull,
            /// <summary>
            /// 找不到狀態移轉
            /// </summary>
            TransitionNotFound,
            /// <summary>
            /// 狀態移轉不被允許
            /// </summary>
            TransitionNotAllowed,
            /// <summary>
            /// 找不到狀態
            /// </summary>
            StateNotFound
        }

        private string _displayName;
        /// <summary>
        /// 狀態機顯示名稱
        /// </summary>
        public string DisplayName
        {
            get { return !string.IsNullOrEmpty(_displayName) ? _displayName : "untitled"; }
            set { _displayName = value; }
        }

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
        public StateMachine SetStates(IEnumerable<State> states)
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
        /// 狀態機目前狀態
        /// </summary>
        public State CurrentState
        {
            get
            {
                return GetCurrentState();
            }
            set
            {
                SetCurrentState(value);
            }
        }

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
        public StateMachine SetCurrentState(State currentState)
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
        public StateMachine SetCurrentStateByKey(string stateKey)
        {
            var state = this._states.Single(s => s.Key == stateKey);
            return SetCurrentState(state);
        }

        /// <summary>
        /// 建構 StateMachine 實體
        /// </summary>
        /// <param name="states">狀態集合</param>
        public StateMachine(IEnumerable<State> states = null)
        {
            SetStates(states ?? new List<State>());
        }

        /// <summary>
        /// 建構 StateMachine 實體
        /// </summary>
        /// <param name="states">狀態集合</param>
        /// <param name="initialState">啟始狀態</param>
        /// <param name="displayName">狀態機顯示名稱</param>
        public StateMachine(IEnumerable<State> states, State initialState, string displayName = null)
        {
            SetStates(states)
                .SetCurrentState(initialState);

            this.DisplayName = displayName;
        }

        /// <summary>
        /// 建構 StateMachine 實體
        /// </summary>
        /// <param name="states">狀態集合</param>
        /// <param name="initialStateKey">啟始狀態識別鍵</param>
        /// <param name="displayName">狀態機顯示名稱</param>
        public StateMachine(IEnumerable<State> states, string initialStateKey, string displayName = null)
        {
            SetStates(states)
                .SetCurrentStateByKey(initialStateKey);

            this.DisplayName = displayName;
        }

        /*/// <summary>
        /// 載入狀態集合
        /// </summary>
        /// <param name="states">狀態集合</param>
        /// <returns>狀態機實體</returns>
        public StateMachine LoadStates(IEnumerable<State> states)
        {
            foreach (var state in states)
            {
                this.AddState(state);
            }

            return this;
        }

        public StateMachine SetupTransitions(IEnumerable<StateTransitionRelation> relations,
            IEnumerable<Transition> transitions = null)
        {
            foreach(var relation in relations)
            {
                State state = FindState(relation.StateKey);

            }
        }*/

        /// <summary>
        /// 將狀態物件加入至狀態集合
        /// </summary>
        /// <param name="state">要加入的狀態物件</param>
        /// <returns>狀態集合所屬的狀態機物件</returns>
        public StateMachine AddState(State state)
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
        public StateMachine InsertState(int index, State state)
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
        public StateMachine RemoveState(State state, out bool result)
        {
            result = this._states.Remove(state);

            return this;
        }

        /// <summary>
        /// 從狀態集合中移除狀態物件
        /// </summary>
        /// <param name="state">要移除的狀態物件</param>
        /// <returns>狀態集合所屬的狀態機物件</returns>
        public StateMachine RemoveState(State state)
        {
            return RemoveState(state, out bool _);
        }

        /// <summary>
        /// 以狀態的識別鍵，從狀態集合中移除狀態物件
        /// </summary>
        /// <param name="stateKey">要移除的狀態之識別鍵</param>
        /// <param name="result">是否成功移除</param>
        /// <returns>狀態集合所屬的狀態機物件</returns>
        public StateMachine RemoveStateByKey(string stateKey, out bool result)
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
        public StateMachine RemoveStateByKey(string stateKey)
        {
            return RemoveStateByKey(stateKey, out bool _);
        }

        /// <summary>
        /// 在指定索引位置上，從狀態集合中移除狀態物件
        /// </summary>
        /// <param name="index">指定索引位置</param>
        /// <returns>狀態集合所屬的狀態機物件</returns>
        public StateMachine RemoveStateAt(int index)
        {
            this._states.RemoveAt(index);

            return this;
        }

        /// <summary>
        /// 從狀態集合中移除所有項目
        /// </summary>
        /// <returns>狀態集合所屬的狀態機物件</returns>
        public StateMachine ClearTransitions()
        {
            this._states.Clear();

            return this;
        }

        /// <summary>
        /// 在狀態集合中尋找狀態物件
        /// </summary>
        /// <param name="predicate">用來測試每個項目是否符合條件的函式</param>
        /// <returns>狀態物件</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> 為 null
        /// </exception>
        /// <exception cref="Exception">
        /// <p>測試函式條件下找不到狀態物件</p>
        /// </exception>
        public State FindState(Func<State, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var state = this._states?.FirstOrDefault(predicate);

            if (state == null)
            {
                throw new Exception($"狀態機「{DisplayName}」找不到狀態！");
            }

            return state;
        }

        /// <summary>
        /// 以狀態識別鍵在狀態集合中尋找狀態物件
        /// </summary>
        /// <param name="stateKey">狀態識別鍵</param>
        /// <returns>狀態物件</returns>
        /// <exception cref="Exception">
        /// <p>>以狀態識別鍵找不到狀態物件</p>
        /// </exception>
        public State FindState(string stateKey)
        {
            var state = this._states?.FirstOrDefault(s => s.Key == stateKey);

            if (state == null)
            {
                throw new Exception($"狀態機「{DisplayName}」找不到識別鍵為「{stateKey}」的狀態！");
            }

            return state;
        }

        /// <summary>
        /// 驗證狀態機是否有效
        /// </summary>
        /// <returns>狀態機是否有效</returns>
        public virtual bool Validate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 取得所有的狀態移轉集合
        /// </summary>
        /// <param name="predicate">過濾條件測試函式</param>
        /// <returns>所有的狀態移轉集合</returns>
        public IEnumerable<Transition> GetAllTransitions(Func<Transition, bool> predicate = null)
        {
            var transitions = States.SelectMany(s => s.Transitions);

            if (predicate != null)
            {
                transitions = transitions.Where(predicate);
            }

            return transitions;
        }

        /// <summary>
        /// 從狀態尋找狀態移轉物件
        /// </summary>
        /// <param name="statePredicate">狀態測試函式</param>
        /// <param name="transitionPredicate">狀態移轉測試函式</param>
        /// <returns>狀態移轉物件</returns>
        /// <exception cref="ArgumentNullException">
        /// <p><paramref name="statePredicate"/> 為 null</p>
        /// <p>- 或 -</p>
        /// <p><paramref name="transitionPredicate"/> 為 null</p>
        /// </exception>
        public Transition FindTransitionFromState(Func<State, bool> statePredicate, Func<Transition, bool> transitionPredicate)
        {
            if (statePredicate == null)
            {
                throw new ArgumentNullException("statePredicate");
            }

            if (transitionPredicate == null)
            {
                throw new ArgumentNullException("transitionPredicate");
            }

            return FindState(statePredicate)?.FindTransition(transitionPredicate);
        }

        /// <summary>
        /// 從狀態尋找狀態移轉物件
        /// </summary>
        /// <param name="stateKey">狀態識別鍵</param>
        /// <param name="transitionPredicate">狀態移轉測試函式</param>
        /// <returns>狀態移轉物件</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transitionPredicate"/> 為 null
        /// </exception>
        public Transition FindTransitionFromState(string stateKey, Func<Transition, bool> transitionPredicate)
        {
            if (transitionPredicate == null)
            {
                throw new ArgumentNullException("transitionPredicate");
            }

            return FindState(stateKey)?.FindTransition(transitionPredicate);
        }

        /// <summary>
        /// 從狀態尋找狀態移轉物件
        /// </summary>
        /// <param name="statePredicate">狀態測試函式</param>
        /// <param name="transitionKey">狀態移轉識別鍵</param>
        /// <returns>狀態移轉物件</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="statePredicate"/> 為 null
        /// </exception>
        public Transition FindTransitionFromState(Func<State, bool> statePredicate, string transitionKey)
        {
            if (statePredicate == null)
            {
                throw new ArgumentNullException("statePredicate");
            }

            return FindState(statePredicate)?.FindTransition(transitionKey);
        }

        /// <summary>
        /// 從狀態尋找狀態移轉物件
        /// </summary>
        /// <param name="stateKey">狀態識別鍵</param>
        /// <param name="transitionKey">狀態移轉識別鍵</param>
        /// <returns>狀態移轉物件</returns>
        public Transition FindTransitionFromState(string stateKey, string transitionKey)
        {
            return FindState(stateKey)?.FindTransition(transitionKey);
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> 為 null
        /// </exception>
        public Transition FindTransitionFromCurrentState(Func<Transition, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            return GetCurrentState()?.FindTransition(predicate);
        }

        /// <summary>
        /// 以狀態移轉識別鍵從目前狀態尋找狀態移轉物件
        /// </summary>
        /// <param name="transitionKey">狀態移轉識別鍵</param>
        /// <returns>狀態移轉物件</returns>
        public Transition FindTransitionFromCurrentState(string transitionKey)
        {
            return GetCurrentState()?.FindTransition(transitionKey);
        }

        /// <summary>
        /// 執行狀態移轉
        /// </summary>
        /// <param name="transition">要執行的狀態移轉</param>
        /// <returns>狀態移轉後的狀態</returns>
        /// <exception cref="ChangeStateException">
        /// <p>目前狀態的狀態移轉集合找不到要嘗試進行的狀態移轉</p>
        /// <p>- 或 -</p>
        /// <p>執行狀態移轉時發生未預期的異常例外！</p>
        /// </exception>
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

        /// <summary>
        /// 執行狀態移轉
        /// </summary>
        /// <param name="transitionKey">要執行的狀態移轉識別鍵</param>
        /// <returns>狀態移轉後的狀態</returns>
        public virtual State ExecuteTransition(string transitionKey)
        {
            return ExecuteTransition(FindTransitionFromCurrentState(transitionKey));
        }

        /// <summary>
        /// 取得錯誤訊息
        /// </summary>
        /// <param name="errorCode">錯誤碼</param>
        /// <returns>錯誤訊息</returns>
        public static string GetErrorMessage(ErrorCode errorCode)
        {
            switch (errorCode)
            {
                case ErrorCode.NoError:
                    return string.Empty;
                case ErrorCode.TransitionNull:
                    return "狀態移轉不得為 null！";
                case ErrorCode.TransitionNotFound:
                    return "找不到狀態移轉！";
                case ErrorCode.TransitionNotAllowed:
                    return "狀態移轉不被允許！";
                case ErrorCode.StateNotFound:
                    return "找不到狀態！";
                default:
                    return $"發生錯誤碼為「{(int)errorCode}({errorCode})」的錯誤！";
            }
        }

        /// <summary>
        /// 測試是否可以執行狀態移轉
        /// </summary>
        /// <param name="transition">嘗試要執行的狀態移轉</param>
        /// <param name="errorCode">錯誤碼</param>
        /// <returns>是否可以執行狀態移轉</returns>
        public virtual bool CanExecuteTransition(Transition transition, out ErrorCode errorCode)
        {
            errorCode = ErrorCode.NoError;

            if (transition == null)
            {
                errorCode = ErrorCode.TransitionNull;
                return false;
            }

            if (!(GetCurrentTransitions()?.Contains(transition) ?? false))
            {
                errorCode = ErrorCode.TransitionNotFound;
            }

            return true;
        }

        /// <summary>
        /// 測試是否可以執行狀態移轉
        /// </summary>
        /// <param name="transitionKey">嘗試要執行的狀態移轉識別鍵</param>
        /// <param name="errorCode">錯誤碼</param>
        /// <returns>是否可以執行狀態移轉</returns>
        public virtual bool CanExecuteTransition(string transitionKey, out ErrorCode errorCode)
        {
            errorCode = ErrorCode.NoError;

            var transition = CurrentState.Transitions.FirstOrDefault(t => t.Key == transitionKey);
            if (transition == null)
            {
                errorCode = ErrorCode.TransitionNotFound;
                return false;
            }

            return CanExecuteTransition(transition, out errorCode);
        }

        /// <summary>
        /// 測試是否可以執行狀態移轉
        /// </summary>
        /// <param name="transition">嘗試要執行的狀態移轉</param>
        /// <returns>是否可以執行狀態移轉</returns>
        public virtual bool CanExecuteTransition(Transition transition)
        {
            return CanExecuteTransition(transition, out ErrorCode _);
        }

        /// <summary>
        /// 測試是否可以執行狀態移轉
        /// </summary>
        /// <param name="transitionKey">嘗試要執行的狀態移轉識別鍵</param>
        /// <returns>是否可以執行狀態移轉</returns>
        public virtual bool CanExecuteTransition(string transitionKey)
        {
            return CanExecuteTransition(transitionKey, out ErrorCode _);
        }

        /// <summary>
        /// 以來源狀態取得可能的目的狀態集合
        /// </summary>
        /// <param name="fromState">來源狀態，未提供時使用目前狀態</param>
        /// <returns>可能的目的狀態集合</returns>
        public IEnumerable<State> GetPossibleToStates(State fromState = null)
        {
            fromState = fromState ?? CurrentState;

            return fromState.Transitions.Select(t => t.ToState);
        }

        /// <summary>
        /// 以來源狀態識別鍵取得可能的目的狀態集合
        /// </summary>
        /// <param name="fromStateKey">來源狀態識別鍵，未提供時使用目前狀態識別鍵</param>
        /// <returns>可能的目的狀態集合</returns>
        public IEnumerable<State> GetPossibleToStatesByKey(string fromStateKey = null)
        {
            fromStateKey = fromStateKey ?? CurrentState.Key;

            return GetPossibleToStates(FindState(fromStateKey));
        }

        /// <summary>
        /// 將目前狀態變更為目的狀態
        /// </summary>
        /// <param name="toState">目的狀態</param>
        /// <returns>目的狀態</returns>
        public virtual State GotoState(State toState)
        {
            var possibleToStateKeys = GetPossibleToStates().Select(s => s.Key);

            if (!possibleToStateKeys.Contains(toState.Key))
            {
                throw new Exception($"目前狀態「{CurrentState}」無法移轉至目的狀態「{toState}」！");
            }

            var transition = CurrentState.Transitions.FirstOrDefault(t => t.ToState.Key == toState.Key);

            return ExecuteTransition(transition);
        }

        /// <summary>
        /// 將目前狀態變更為目的狀態
        /// </summary>
        /// <param name="toStateKey">目的狀態識別鍵</param>
        /// <returns>目的狀態</returns>
        public virtual State GotoState(string toStateKey)
        {
            var toState = FindState(toStateKey);

            return GotoState(toState);
        }

        /// <summary>
        /// 測試是否可以將目前狀態變更為目的狀態
        /// </summary>
        /// <param name="toState">目的狀態</param>
        /// <param name="erroCode">錯誤碼</param>
        /// <returns>是否可以將目前狀態變更為目的狀態</returns>
        public virtual bool CanGotoState(State toState, out ErrorCode erroCode)
        {
            erroCode = ErrorCode.NoError;

            var possibleToStateKeys = GetPossibleToStates().Select(s => s.Key);
            if (!possibleToStateKeys.Contains(toState.Key))
            {
                erroCode = ErrorCode.TransitionNotAllowed;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 測試是否可以將目前狀態變更為目的狀態
        /// </summary>
        /// <param name="toStateKey">目的狀態識別鍵</param>
        /// <param name="errorCode">錯誤碼</param>
        /// <returns>是否可以將目前狀態變更為目的狀態</returns>
        public virtual bool CanGotoState(string toStateKey, out ErrorCode errorCode)
        {
            errorCode = ErrorCode.NoError;

            var toState = States?.FirstOrDefault(s => s.Key == toStateKey);
            if (toState == null)
            {
                errorCode = ErrorCode.StateNotFound;
                return false;
            }

            return CanGotoState(toState, out errorCode);
        }

        /// <summary>
        /// 測試是否可以將目前狀態變更為目的狀態
        /// </summary>
        /// <param name="toState">目的狀態</param>
        /// <returns>是否可以將目前狀態變更為目的狀態</returns>
        public virtual bool CanGotoState(State toState)
        {
            return CanGotoState(toState, out ErrorCode _);
        }

        /// <summary>
        /// 測試是否可以將目前狀態變更為目的狀態
        /// </summary>
        /// <param name="toStateKey">目的狀態識別鍵</param>
        /// <returns>是否可以將目前狀態變更為目的狀態</returns>
        public virtual bool CanGotoState(string toStateKey)
        {
            return CanGotoState(toStateKey, out ErrorCode _);
        }
    }
}
