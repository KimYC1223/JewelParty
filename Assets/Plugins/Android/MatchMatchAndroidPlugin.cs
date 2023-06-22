using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

namespace HexaBlast.Plugin
{
#if UNITY_ANDROID
    public class MatchMatchAndroidPlugin
    {

        public static void ShowToast(string message)
        {
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
        }

        public static void Vibrate(int time)
        {
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
        }

        private static void CheckAndroidPermissionAndDo(string permission, Action actionIfPermissionGranted)
        {
             ???? ?????? ???? ????
            if ( Permission.HasUserAuthorizedPermission(permission) == false )
            {
                 ???? ???? ?????? ???? ???? ????
                PermissionCallbacks pCallbacks = new PermissionCallbacks();
                pCallbacks.PermissionGranted += str => Debug.Log($"{str} ????");
                pCallbacks.PermissionGranted += _ => actionIfPermissionGranted();  ???? ?? ???? ????

                pCallbacks.PermissionDenied += str => Debug.Log($"{str} ????");

                pCallbacks.PermissionDeniedAndDontAskAgain += str => Debug.Log($"{str} ?????? ????(???? ???? ????)");

                 ???? ????
                Permission.RequestUserPermission(permission, pCallbacks);
            }
             ?????? ???? ???? ???? ????
            else
            {
                actionIfPermissionGranted();  ???? ???? ????
            }
        }

    }
    #endif
}
