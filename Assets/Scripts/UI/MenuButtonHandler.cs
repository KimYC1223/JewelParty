using HexaBlast.UI.POPUP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexaBlast.UI
{
    public class MenuButtonHandler : MonoBehaviour
    {
        // Start is called before the first frame update
        public void OpenMenu()
        {
            Debug.Log(PopUpManager.Instance.WindowsDict["Menu"] == null);
            PopUpManager.Instance.WindowsDict["Menu"].gameObject.SetActive(true);
        }
    }
}
