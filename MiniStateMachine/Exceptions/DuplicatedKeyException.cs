//------------------------------------------------------------------------------
// MiniStateMachine - 識別鍵重複例外
// 組織：國立東華大學 / Organization: National Dong Hwa University
// 作者：王建中 / Author: Wang, Jian-Zhong
//------------------------------------------------------------------------------

using System;

namespace MiniStateMachine.Exceptions
{
    /// <summary>
    /// 識別鍵重複例外
    /// </summary>
    [Serializable]
    public class DuplicatedKeyException : Exception
    {
        /// <summary>
        /// 重複的識別鍵
        /// </summary>
        public string DuplicatedKey { get; private set; }

        /// <summary>
        /// 建構 DuplicatedKeyException 實體
        /// </summary>
        /// <param name="message">錯誤訊息</param>
        /// <param name="duplicatedKey">重複的識別鍵</param>
        public DuplicatedKeyException(string message, string duplicatedKey)
            : base(!string.IsNullOrEmpty(message) ? message : string.Format("識別鍵「{0}」已存在，識別鍵不得重複！", duplicatedKey))
        {
            this.DuplicatedKey = duplicatedKey;
        }

        public DuplicatedKeyException() { }
        public DuplicatedKeyException(string message) : base(message) { }
        public DuplicatedKeyException(string message, Exception inner) : base(message, inner) { }
        protected DuplicatedKeyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
