using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MatchMatch.Plugin
{
    //===============================================================================================================================================
    //  Match Match에서 사용하는 IOS Native SDK Plugin
    //===============================================================================================================================================
    public class MatchMatchIOSPlugin : MonoBehaviour
    {
        #region EXTERNAL_METHOD
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void _Vibrate(int _n);

        [DllImport("__Internal")]
        public static extern void _ShowToast(string message);
#endif
        #endregion

        #region PUBLIC_STATIC_METHOD
        //===============================================================================================================================================
        //
        //  토스트 팝업 메세지 출력하는 메서드
        //
        //===============================================================================================================================================
        /// <summary>
        /// 토스트 팝업 메세지 출력하는 메서드.
        /// </summary>
        /// <param name="message">출력할 메세지</param>
        public static void ShowToast(string message)
        {
#if UNITY_IOS && !UNITY_EDITOR
            _ShowToast(message);
#endif
        }

        //===============================================================================================================================================
        //
        //  일정 시간동안 진동을 재생하는 메서드
        //
        //===============================================================================================================================================
        /// <summary>
        /// 일정 시간동안 진동을 재생하는 메서드.
        /// </summary>
        /// <param name="time">재생할 시간</param>
        public static void Vibrate(long time)
        {
#if UNITY_IOS && !UNITY_EDITOR
            _Vibrate((int)time);
#endif
        }
        #endregion
    }
}

