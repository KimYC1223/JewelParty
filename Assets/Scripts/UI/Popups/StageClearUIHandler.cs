using UnityEngine;
using MatchMatch.Common;
using System.Collections;

namespace MatchMatch.UI.POPUP
{
    //===============================================================================================================================================
    //  스테이지가 끝났을 때, 실행되는 UI를 제어하는 클래스
    //===============================================================================================================================================
    public class StageClearUIHandler : PopUpWindows
    {
        #region FIELD
        public Animator ShowAnimator;               // 쇼 애니메이터
        private Coroutine routine;                  // 쇼 애니메이터를 감시하는 코루틴
        #endregion

        #region PUBLIC_METHOD
        //===========================================================================================================================================
        //
        //  애니메이션이 종료되면 실행되는 콜백을 지정하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 애니메이션이 종료되면 실행되는 콜백을 지정하는 메서드.
        /// </summary>
        /// <param name="callback">애니메이션이 종료되면 실행되는 메서드</param>
        public void AnimationChecker(MatchMatchDelegate.VoidDelegate callback)
        {
            // 코루틴이 실행중이지 않은 경우, 감시하는 코루틴 실행
            if (routine == null)
            {
                routine = StartCoroutine(AnimCheck(callback));
            }
        }
        #endregion

        #region PRIVATE_ENUMERATOR
        //===========================================================================================================================================
        //
        //  애니메이터를 감시하는 코루틴
        //
        //===========================================================================================================================================
        private IEnumerator AnimCheck(MatchMatchDelegate.VoidDelegate callback)
        {
            // 무한 루프를 돌며 애니메이션 감시
            while(true)
            {
                // 애니메이션이 종료되면, 루프 탈출
                if( ShowAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f )
                {
                    break;
                }
                yield return 0;
            }

            // 초기화 후 콜백 실행
            routine = null;
            callback();
        }
        #endregion
    }
}