using UnityEngine;

namespace HexaBlast.UI.POPUP
{
    public class MenuUIHandler : PopUpWindows
    {
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}