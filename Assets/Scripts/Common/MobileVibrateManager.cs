using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchMatch.Plugin;

namespace MatchMatch.Common
{
    //===============================================================================================================================================
    //  모바일 환경에서, 진동을 울리게하는 클래스
    //===============================================================================================================================================
    public class MobileVibrateManager
    {
        #region PUBLIC_METHOD
        //===========================================================================================================================================
        //
        //  정해진 시간동안 진동을 울리게하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 모바일 환경에서, 정해진 시간동안 진동을 울리는 메서드.<br />
        /// <br />
        /// 모바일 환경이 아닐 경우, 아무 일도 일어나지 않음<br />
        /// 단위는 밀리세컨드 (1초 : 1000)<br />
        /// </summary>
        /// <param name="time">진동을 울릴 시간 (단위 : 밀리세컨드)</param>
        public static void Vibrate(float time)
        {
#if UNITY_ANDROID
            MatchMatchAndroidPlugin.Vibrate((long)(time * 1000));
#elif UNITY_IOS
            MatchMatchIOSPlugin.Vibrate((long)(time * 1000));
#endif
        }
        #endregion
    }
}
