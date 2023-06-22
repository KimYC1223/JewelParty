using UnityEngine;
using HexaBlast.Common;
using HexaBlast.Game;
using System.Collections;

namespace HexaBlast.UI.POPUP
{
    public class GameStartUIHandler : PopUpWindows
    {
        public Animator ShowAnimator;

        public void SoundPlay()
        {
            GameManager.SoundManagerInstance.PlayBackgroundMusic(BGM.GAME);
            GameManager.SoundManagerInstance.PlaySound(SFX.READY_GO);
        }

        public void AnimationEndStep()
        {
            GameManager.IsInteractable = true;
            CloseWinodw();
        }
    }
}