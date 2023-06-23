using MatchMatch.Common.VO;

namespace MatchMatch.Common
{
    //===============================================================================================================================================
    //  게임에서 사용되는 Delegate 모음
    //===============================================================================================================================================
    public class MatchMatchDelegate
    {
        #region DELEGATE
        public delegate void VoidDelegate();                // 아무것도 없는 델리게이트 
        public delegate void GenericDelegate<T>(T t);       // 타입 T에 대한 델리게이트
        #endregion
    }
}