using MatchMatch.Game;
using System.Collections.Generic;
using UnityEngine;

namespace MatchMatch.UI.POPUP
{
    //===============================================================================================================================================
    //  팝업 윈도우들을 관리하는 팝업 매니저
    //===============================================================================================================================================
    public class PopUpManager : MonoBehaviour
    {
        #region INSTANCE
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
        #endregion

        #region FIELD
        public PopUpWindows[] PopUps;                                   // 관리허고 있는 팝업들
        public Dictionary<string, PopUpWindows> WindowsDict;            // String Key로 접근 할 수 있는 팝업 목록들
        #endregion

        #region PUBLIC_METHOD
        //===========================================================================================================================================
        //
        //  모든 팝업 윈도우를 닫는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 모든 팝업 윈도우를 다는 메서드.
        /// </summary>
        public void CloseAllWindows()
        {
            CloseOtherWidnwos(null);
        }

        //===========================================================================================================================================
        //
        //  특정 팝업 윈도우를 제외하고 모든 팝업을 닫는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 특정 팝업 윈도우를 제외하고 모든 팝업을 닫는 메서드.
        /// </summary>
        /// <param name="exceptWindow">제외할 팝업 윈도우의 gameObject.name</param>
        public void CloseOtherWidnwos(string exceptWindow)
        {
            // 현재 관리되고 있는 팝업들의 딕셔너리의 ValueCollection
            Dictionary<string, PopUpWindows>.ValueCollection values = WindowsDict.Values;
            foreach ( PopUpWindows w in values )
            {
                // 특정 윈도우를 제외하고, 모두 닫음
                if( w.gameObject.name == exceptWindow )
                {
                    continue;
                }
                w.CloseWinodw();
            }
        }
        #endregion

        #region PRIVATE_METHOD
        //===========================================================================================================================================
        //
        //  Awake 라이프 사이클에 호출되는 메서드
        //
        //===========================================================================================================================================
        private void Awake()
        {
            // Dictionary를 초기화하는 작업
            GameManager.IsInteractable = false;
            WindowsDict = new Dictionary<string, PopUpWindows>();
            foreach ( PopUpWindows w in PopUps )
            {
                WindowsDict.Add(w.gameObject.name, w);
            }
        }

        //===========================================================================================================================================
        //
        //  Start 라이프 사이클에 호출되는 메서드
        //
        //===========================================================================================================================================
        private void Start()
        {
            // GameStart 팝업 오브젝트 띄우기
            WindowsDict["GameStart"].gameObject.SetActive(true);
        }
        #endregion
    }
}
