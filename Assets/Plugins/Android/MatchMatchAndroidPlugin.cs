using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

namespace MatchMatch.Plugin
{
    //===============================================================================================================================================
    //  Match Match에서 사용하는 Android Native SDK Plugin
    //===============================================================================================================================================
    public class MatchMatchAndroidPlugin
    {
        #region PUBLIC_STATIC_METHOD
        //===========================================================================================================================================
        //
        //  토스트 팝업 메세지 출력하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 토스트 팝업 메세지 출력하는 메서드.
        /// </summary>
        /// <param name="message">출력할 메세지</param>
        public static void ShowToast(string message)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using ( var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer") )
            {
                var unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                using ( var MatchMatchSDKClass = new AndroidJavaClass("com.kimyc1223.matchmatch.MatchMatchAndroidSDK") )
                {
                    AndroidJavaObject MatchMatchSDKInstance = MatchMatchSDKClass.CallStatic<AndroidJavaObject>("instance");
                    MatchMatchSDKInstance.Call("setContext", unityActivity);

                    unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                        MatchMatchSDKInstance.Call("ShowToast", message);
                    }));
                }
            }
#endif
        }

        //===========================================================================================================================================
        //
        //  일정 시간동안 진동을 재생하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 일정 시간동안 진동을 재생하는 메서드.
        /// </summary>
        /// <param name="time">재생할 시간</param>
        public static void Vibrate(long time)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            CheckAndroidPermissionAndDo("android.permission.VIBRATE", () => {
                using ( var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer") )
                {
                    var unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                    using ( var MatchMatchSDKClass = new AndroidJavaClass("com.kimyc1223.matchmatch.MatchMatchAndroidSDK") )
                    {
                        AndroidJavaObject MatchMatchSDKInstance = MatchMatchSDKClass.CallStatic<AndroidJavaObject>("instance");
                        MatchMatchSDKInstance.Call("setContext", unityActivity);

                        MatchMatchSDKInstance.Call("Vibrate", time);
                    }
                }
            });
#endif
        }

        //===========================================================================================================================================
        //
        //  안드로이드 사용자의 퍼미션을 받는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 안드로이드 사용자의 퍼미션을 받는 메서드.
        /// </summary>
        /// <param name="permission">안드로이드용 퍼미션 종류. <see cref="Permission"/>을 참고하거나, 직접 String을 입력해야 함</param>
        /// <param name="actionIfPermissionGranted">퍼미션을 수락했을 때 실행되는 콜백</param>
        private static void CheckAndroidPermissionAndDo(string permission, Action actionIfPermissionGranted)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if ( Permission.HasUserAuthorizedPermission(permission) == false )
            {
                PermissionCallbacks pCallbacks = new PermissionCallbacks();
                pCallbacks.PermissionGranted += str => { };
                pCallbacks.PermissionGranted += _ => actionIfPermissionGranted();

                pCallbacks.PermissionDenied += str => { };

                pCallbacks.PermissionDeniedAndDontAskAgain += str => { };

                Permission.RequestUserPermission(permission, pCallbacks);
            }
            else
            {
                actionIfPermissionGranted();
            }
#endif
        }
        #endregion
    }
}
