using UnityEngine;
using HexaBlast.Common;
using System.Collections;

namespace HexaBlast.UI.POPUP
{
    public class StageClearUIHandler : PopUpWindows
    {
        public Animator ShowAnimator;

        private Coroutine routine;

        public void AnimationChecker(HexaBlastDelegate.VoidDelegate callback)
        {
            if (routine == null)
            {
                routine = StartCoroutine(AnimCheck(callback));
            }
        }

        private IEnumerator AnimCheck(HexaBlastDelegate.VoidDelegate callback)
        {
            while(true)
            {
                if( ShowAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f )
                {
                    break;
                }
                yield return 0;
            }

            routine = null;
            callback();
        }

    }
}