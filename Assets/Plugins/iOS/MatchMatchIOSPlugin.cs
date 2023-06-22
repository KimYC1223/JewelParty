using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HexaBlast.Plugin
{
#if UNITY_IOS
    public class MatchMatchIOSPlugin : MonoBehaviour
    {
        [DllImport("__Internal")]
        public static extern void _Vibrate(int _n);

        [DllImport("__Internal")]
        public static extern void _ShowToast(string message);

        public static void ShowToast(string message)
        {
            _ShowToast(message);
        }

        public static void Vibrate(long time)
        {
            _Vibrate((int)time);
        }
    }
#endif
}

