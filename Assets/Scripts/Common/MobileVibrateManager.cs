using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexaBlast.Plugin;

namespace HexaBlast.Common
{
    public class MobileVibrateManager
    {
        public static void Vibrate(float time)
        {
#if UNITY_ANDROID
            MatchMatchAndroidPlugin.Vibrate((long)(time * 1000));
#elif UNITY_IOS

#endif
        }
    }
}
