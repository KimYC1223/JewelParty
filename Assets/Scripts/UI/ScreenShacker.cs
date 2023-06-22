using UnityEngine;

namespace HexaBlast.UI.POPUP
{
    public class ScreenShacker : MonoBehaviour
    {
        private static bool isShake;
        public Animator ShakerAnimator;

        private void Update()
        {
            if( isShake )
            {
                isShake = false;
                ShakerAnimator.SetTrigger("Shake");
            }
        }

        public static void Shake()
        {
            isShake = true;
        }
    }
}