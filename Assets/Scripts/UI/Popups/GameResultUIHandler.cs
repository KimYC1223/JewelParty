using UnityEngine;
using TMPro;
using MatchMatch.Game;
using MatchMatch.UI;
using MatchMatch.Common;

namespace MatchMatch.UI.POPUP
{
    //===============================================================================================================================================
    //  게임 결과 UI를 제어하는 핸들러
    //===============================================================================================================================================
    public class GameResultUIHandler : PopUpWindows
    {
        #region FIELD
        public Animator StartAnimator;          // 쇼 애니메이터
        public TMP_Text ScoreText;              // 얻은 점수를 표시하는GUI Text

        private int star = 0;                   // 얻은 별의 개수
        #endregion

        #region PUBLIC_METHOD
        //===========================================================================================================================================
        //
        //  얻은 스타를 결정하고 표시하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 얻은 스타를 결정하고 표시하는 메서드.
        /// </summary>
        /// <param name="score">플레이어가 얻은 점수</param>
        /// <param name="thresholds">스타를 얻을 수 있는 조건 배열</param>
        public void SetStarStatus(int score, int[] thresholds)
        {
            star = 0;

            for(int i = 0 ; i < thresholds.Length ; i++ )
            {
                if(score >= thresholds[i] )
                {
                    star++;
                }
            }

            ScoreText.text = score.ToString();
            StartAnimator.SetInteger("Star", star);
        }

        //===========================================================================================================================================
        //
        //  게임 종료 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 게임을 종료하는 메서드.
        /// </summary>
        public void ExitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        //===========================================================================================================================================
        //  사운드를 재생하는 메서드 (애니메이터에서 사용)
        //===========================================================================================================================================
        /// <summary>
        /// 사운드를 재생하는 메서드.
        /// </summary>
        public void PlayStarSound()
        {
            GameManager.SoundManagerInstance.PlaySound(SFX.STAR);
        }
        #endregion
    }
}