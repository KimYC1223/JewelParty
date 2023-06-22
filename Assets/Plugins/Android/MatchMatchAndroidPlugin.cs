using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

namespace HexaBlast.Plugin
{
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

        //public static void Vibrate(int time)
        //{
        //    CheckAndroidPermissionAndDo("android.permission.VIBRATE", () => {
        //        using ( var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer") )
        //        {
        //            var unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        //            using ( var MatchMatchSDKClass = new AndroidJavaClass("com.kimyc1223.matchmatch.MatchMatchAndroidSDK") )
        //            {
        //                AndroidJavaObject MatchMatchSDKInstance = MatchMatchSDKClass.CallStatic<AndroidJavaObject>("instance");
        //                MatchMatchSDKInstance.Call("setContext", unityActivity);

        //                MatchMatchSDKInstance.Call("Vibrate", time);
        //            }
        //        }
        //    });
        //}

        //private static void CheckAndroidPermissionAndDo(string permission, Action actionIfPermissionGranted)
        //{
        //    // ���� ������ �ȵ� ����
        //    if ( Permission.HasUserAuthorizedPermission(permission) == false )
        //    {
        //        // ���� ��û ���信 ���� ���� �ݹ�
        //        PermissionCallbacks pCallbacks = new PermissionCallbacks();
        //        pCallbacks.PermissionGranted += str => Debug.Log($"{str} ����");
        //        pCallbacks.PermissionGranted += _ => actionIfPermissionGranted(); // ���� �� ��� ����

        //        pCallbacks.PermissionDenied += str => Debug.Log($"{str} ����");

        //        pCallbacks.PermissionDeniedAndDontAskAgain += str => Debug.Log($"{str} ���ϰ� ����(�ٽ� ���� ����)");

        //        // ���� ��û
        //        Permission.RequestUserPermission(permission, pCallbacks);
        //    }
        //    // ������ ���� �Ǿ� �ִ� ���
        //    else
        //    {
        //        actionIfPermissionGranted(); // �ٷ� ��� ����
        //    }
        //}

#if UNITY_ANDROID && !UNITY_EDITOR
        public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
        public static AndroidJavaClass unityPlayer;
        public static AndroidJavaObject currentActivity;
        public static AndroidJavaObject vibrator;
#endif

        public static void Vibrate()
        {
            if ( isAndroid() )
            {
                vibrator.Call("vibrate");
            }
            else
            {
#if UNITY_EDITOR
                Handheld.Vibrate();
#endif
            }
        }

        public static void Vibrate(long milliseconds)
        {
            if ( isAndroid() )
            {
                vibrator.Call("vibrate", milliseconds);
            }
            else
            {
#if UNITY_EDITOR
                Handheld.Vibrate();
#endif
            }
        }

        public static bool HasVibrator()
        {
            return isAndroid();
        }

        public static void Cancel()
        {
            if ( isAndroid() )
                vibrator.Call("cancel");
        }

        private static bool isAndroid()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
			return true;
#else
            return false;
#endif
        }
    }
}
