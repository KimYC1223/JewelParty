using UnityEngine;
using TMPro;
using HexaBlast.Game;
using HexaBlast.UI;
using HexaBlast.Common;

namespace HexaBlast.UI.POPUP
{
    public class GameResultUIHandler : PopUpWindows
    {
        private int star = 0;

        public Animator StartAnimator;
        public TMP_Text ScoreText;

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

        public void ExitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void PlayStarSound()
        {
            GameManager.SoundManagerInstance.PlaySound(SFX.STAR);
        }
    }
}