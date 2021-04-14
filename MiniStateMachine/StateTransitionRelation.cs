//------------------------------------------------------------------------------
// 狀態對狀態移轉之關聯
// 組織：國立東華大學 / Organization: National Dong Hwa University
// 作者：王建中 / Author: Wang, Jian-Zhong
//------------------------------------------------------------------------------

namespace MiniStateMachine
{
    /// <summary>
    /// 狀態對狀態移轉之關聯
    /// </summary>
    public class StateTransitionRelation
    {
        /// <summary>
        /// 狀態識別鍵
        /// </summary>
        public string StateKey { get; set; }

        /// <summary>
        /// 狀態移轉識別鍵
        /// </summary>
        public string TransitionKey { get; set; }
    }
}
