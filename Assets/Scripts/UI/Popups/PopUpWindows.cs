using UnityEngine;

namespace HexaBlast.UI.POPUP
{
    public abstract class PopUpWindows : MonoBehaviour
    {
        private void OnEnable()
        {
            OnOpenWindow();
        }

        public virtual void OnOpenWindow()
        {
            PopUpManager.Instance.CloseOtherWidnwos(this.gameObject.name);
        }

        public virtual void CloseWinodw()
        {
            this.gameObject.SetActive(false);
        }
    }
}
