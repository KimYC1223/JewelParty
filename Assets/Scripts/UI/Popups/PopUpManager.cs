using HexaBlast.Game;
using System.Collections.Generic;
using UnityEngine;

namespace HexaBlast.UI.POPUP
{
    public class PopUpManager : MonoBehaviour
    {
        private static PopUpManager _Instance;
        public static PopUpManager Instance
        {
            get
            {
                if( _Instance == null )
                {
                    _Instance = GameObject.Find("Pop Up Canvas").transform.GetComponent<PopUpManager>();
                }
                return _Instance;
            }

            private set { }
        }

        public PopUpWindows[] PopUps;
        public Dictionary<string, PopUpWindows> WindowsDict;

        private void Awake()
        {
            GameManager.IsInteractable = false;
            WindowsDict = new Dictionary<string, PopUpWindows>();
            foreach ( PopUpWindows w in PopUps )
            {
                WindowsDict.Add(w.gameObject.name, w);
            }
        }

        private void Start()
        {
            WindowsDict["GameStart"].gameObject.SetActive(true);
        }

        public void CloseAllWindows()
        {
            CloseOtherWidnwos(null);
        }

        public void CloseOtherWidnwos(string exceptWindow)
        {
            Dictionary<string, PopUpWindows>.ValueCollection values = WindowsDict.Values;
            foreach ( PopUpWindows w in values )
            {
                if( w.gameObject.name == exceptWindow )
                {
                    continue;
                }
                w.CloseWinodw();
            }
        }
    }
}
