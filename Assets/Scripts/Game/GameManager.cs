using MatchMatch.Common;
using MatchMatch.UI;
using MatchMatch.Common.VO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MatchMatch.Game
{
    //===============================================================================================================================================
    //  게임 전체를 관장하는 토탈 메니저
    //===============================================================================================================================================
    public class GameManager : MonoBehaviour
    {
        #region FIELD
        public static bool IsInteractable = true;           // 현재 블럭의 조작이 가능한지 저장하는 변수
        public static LevelInfo CurrentLevelInfo;           // 현재 플레이중인 레벨의 정보

        private NetworkManager networkManager;              // 네트워크 매니저
        private BlockMatcher   blockMatcher;                // 블록 매칭 알고리즘 담당하는 클래스
        #endregion

        #region INSTANCE
        //===========================================================================================================================================
        //  타일 매니저 인스턴스
        //===========================================================================================================================================
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

        //===========================================================================================================================================
        //  블록 매니저 인스턴스
        //===========================================================================================================================================
        private static BlockManager _BlockManagerInstance;
        public static BlockManager BlockManagerInstance
        {
            get
            {
                if ( _BlockManagerInstance == null )
                {
                    _BlockManagerInstance = GameObject.Find("Block Manager Object").GetComponent<BlockManager>();
                }
                return _BlockManagerInstance;
            }

            private set { }
        }

        //===========================================================================================================================================
        //  UI Size 핸들러 인스턴스
        //===========================================================================================================================================
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

        //===========================================================================================================================================
        //  사운드 매니저 인스턴스
        //===========================================================================================================================================
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

        //===========================================================================================================================================
        //  게임 매니저 인스턴스
        //===========================================================================================================================================
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
        #endregion

        #region PUBLIC_METHOD
        //===========================================================================================================================================
        //
        //  주어진 타일들에 대해서, 타일 매칭 진행 ( List<Tile>로 전달 )
        //
        //===========================================================================================================================================
        /// <summary>
        /// 주어진 타일들에 대해서, 타일 매칭 진행하는 메서드.<br />
        /// 지워지는 타일과 떨어지는 타일들의 개수를 <see cref="DeleteInfo"/> VO를 통해 리턴.<br />
        /// </summary>
        /// <param name="tileList">매칭을 진행할 타일 List</param>
        /// <returns>지워지는 타일과 떨어지는 타일들의 개수</returns>
        public DeleteInfo CalculateGameStatus(List<Tile> tileList)
        {
            // List를 Array로 변환 후 넘겨줌
            Tile[] tiles = new Tile[tileList.Count];
            for(int i = 0 ; i < tileList.Count ; i++ )
            {
                tiles[i] = tileList[i];
            }
            return CalculateGameStatus(tiles);
        }

        //===========================================================================================================================================
        //
        //  주어진 타일들에 대해서, 타일 매칭 진행 ( Tile[]로 전달 )
        //
        //===========================================================================================================================================
        /// <summary>
        /// 주어진 타일들에 대해서, 타일 매칭 진행하는 메서드.<br />
        /// 지워지는 타일과 떨어지는 타일들의 개수를 <see cref="DeleteInfo"/> VO를 통해 리턴.<br />
        /// </summary>
        /// <param name="tiles">매칭을 진행할 타일 Array</param>
        /// <returns>지워지는 타일과 떨어지는 타일들의 개수</returns>
        public DeleteInfo CalculateGameStatus(Tile[] tiles)
        {
            return blockMatcher.RunBlockMatch(tiles);
        }

        //===========================================================================================================================================
        //
        //  일정 시간 후 임의의 콜백을 실행시키는 메서드.
        //
        //===========================================================================================================================================
        /// <summary>
        /// 일정 시간 후 임의의 콜백을 실행시키는 메서드.
        /// </summary>
        /// <param name="seconds">대기 시간. ( 단위 :Sec )</param>
        /// <param name="callback">실행시킬 콜백</param>
        public void DelaySecond(float seconds, MatchMatchDelegate.VoidDelegate callback)
        {
            StartCoroutine(DelaySecondCoroutine(seconds, callback));
        }

        //===========================================================================================================================================
        //
        //  게임이 종료되었는지 판단하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 게임이 종료되었는지 판단하는 메서드.
        /// </summary>
        /// <returns>게임 종료 여부</returns>
        public bool IsGameEnd()
        {
            return ( HeaderUIHandler.Instance.Goal == 0 );
        }
        #endregion

        #region PRIVATE_METHOD
        //===========================================================================================================================================
        //
        //  게임이 로드 될 때 실행하는 메서드
        //
        //===========================================================================================================================================
        private void Start()
        {
            // Private 인스턴스 생성
            networkManager = new NetworkManager();
            blockMatcher = new BlockMatcher();

            //=======================================================================================================================================
            // 게임 정보 로드
            //=======================================================================================================================================
            StartCoroutine(networkManager.LoadGameLevel("Data_Server_IP", (levelInfo) => {
                // 받아온 정보를 통해 각종 어레이 추가
                CurrentLevelInfo = levelInfo;
                TileManagerInstance.SetTileArray();
                BlockManagerInstance.SetBlockInfo();
                Debug.Log("MatchMatch::GameManager::Start::Load game level complete.");

                HeaderUIHandler.Instance.SetGoalValue(10);
                HeaderUIHandler.Instance.SetMoveValue(CurrentLevelInfo.InitMove);
                HeaderUIHandler.Instance.SetStarThreshold(CurrentLevelInfo.StarThreshold);
            }));
        }
        #endregion

        #region PRIVATE_ENUMERATOR
        //===========================================================================================================================================
        //
        //  일정 시간 대기하는 Enumerator
        //
        //===========================================================================================================================================
        private IEnumerator DelaySecondCoroutine (float seconds, MatchMatchDelegate.VoidDelegate callback)
        {
            // 일정 시간 대기 후, 정해진 callback 실행
            yield return new WaitForSeconds(seconds);
            if(callback != null)
            {
                callback();
            }
        }
        #endregion
    }
}
