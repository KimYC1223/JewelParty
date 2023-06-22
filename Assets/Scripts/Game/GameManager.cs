using HexaBlast.Common;
using HexaBlast.UI;
using HexaBlast.Common.VO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HexaBlast.Game
{
    public class GameManager : MonoBehaviour
    {
        public static bool IsInteractable = true;
        public static LevelInfo CurrentLevelInfo;

        private NetworkManager networkManager;
        private CandyMatcher   candyMatcher;

        private static TileManager _TileManagerInstance;
        public static TileManager TileManagerInstance
        {
            get
            {
                if ( _TileManagerInstance == null )
                {
                    _TileManagerInstance = GameObject.Find("Tile Manager Object").GetComponent<TileManager>();
                }
                return _TileManagerInstance;
            }

            private set { }
        }

        private static CandyManager _CandyManagerInstance;
        public static CandyManager CandyManagerInstance
        {
            get
            {
                if ( _CandyManagerInstance == null )
                {
                    _CandyManagerInstance = GameObject.Find("Candy Manager Object").GetComponent<CandyManager>();
                }
                return _CandyManagerInstance;
            }

            private set { }
        }

        private static UISizeHandler _UISizeHandlerInstance;
        public static UISizeHandler UISizeHandlerInstance
        {
            get
            {
                if ( _UISizeHandlerInstance == null )
                {
                    _UISizeHandlerInstance = GameObject.Find("Main Canvas").GetComponent<UISizeHandler>();
                }
                return _UISizeHandlerInstance;
            }

            private set { }
        }

        private static SoundManager _SoundManagerInstance;
        public static SoundManager SoundManagerInstance
        {
            get
            {
                if ( _SoundManagerInstance == null )
                {
                    _SoundManagerInstance = GameObject.Find("Main Camera").GetComponent<SoundManager>();
                }
                return _SoundManagerInstance;
            }

            private set { }
        }

        private static GameManager _Instance;
        public static GameManager Instance
        {
            get
            {
                if ( _Instance == null )
                {
                    _Instance = GameObject.Find("Game Manager").GetComponent<GameManager>();
                }
                return _Instance;
            }

            private set { }
        }

        private void Start()
        {
            networkManager = new NetworkManager();
            candyMatcher   = new CandyMatcher();

            StartCoroutine(networkManager.LoadGameLevel("Data_Server_IP", (levelInfo) => {
                CurrentLevelInfo = levelInfo;
                TileManagerInstance.SetTileArray();
                CandyManagerInstance.SetCandyInfo();
                Debug.Log("HexaBlast::GameManager::Start::Load game level complete.");

                HeaderUIHandler.Instance.SetGoalValue(CurrentLevelInfo.Goal);
                HeaderUIHandler.Instance.SetMoveValue(CurrentLevelInfo.InitMove);
                HeaderUIHandler.Instance.SetStarThreshold(CurrentLevelInfo.StarThreshold);
            }));

            MobileVibrateManager.Vibrate(1f);
        }

        public DeleteInfo CalculateGameStatus(List<Tile> tileList)
        {
            Tile[] tiles = new Tile[tileList.Count];
            for(int i = 0 ; i < tileList.Count ; i++ )
            {
                tiles[i] = tileList[i];
            }
            return CalculateGameStatus(tiles);
        }

        public DeleteInfo CalculateGameStatus(Tile[] tiles)
        {
            return candyMatcher.CalculateGameStatus(tiles);
        }

        public void DelaySecond(float seconds, HexaBlastDelegate.VoidDelegate callback)
        {
            StartCoroutine(DelaySecondCoroutine(seconds, callback));
        }

        private IEnumerator DelaySecondCoroutine (float seconds, HexaBlastDelegate.VoidDelegate callback)
        {
            yield return new WaitForSeconds(seconds);
            if(callback != null)
            {
                callback();
            }
        }

        public bool IsGameEnd()
        {
           return ( HeaderUIHandler.Instance.Goal == 0 );
        }
    }
}
