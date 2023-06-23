using System.Collections;
using System.Collections.Generic;
using MatchMatch.Common;
using UnityEngine;

namespace MatchMatch.Game
{
    //===============================================================================================================================================
    //  블럭 전체를 관장하는 블럭 매니저 클래스
    //===============================================================================================================================================
    public class BlockManager : MonoBehaviour
    {
        #region FIELD
        public Sprite[] BlockIcons = new Sprite[8];         // 블록을 표현할 Sprites
        public GameObject BlockPrefab;                      // 블록 프리팹
        public Tile CreateBlockTile;                        // 블록을 만드는 프리팹
        public List<Block> Block;                           // 현재 게임에 존재하는 Block 리스트
        #endregion

        #region PUBLIC_STATIC_METHOD
        //===========================================================================================================================================
        //
        //  중력이 적용되는 방향을 리턴하는 Static 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 중력이 적용되는 방향을 리턴하는 Static 메서드.<br />
        /// <br />
        /// 중력의 영향을 받아 흘러내리는 경우, 여러 방향으로 흘러내릴 수 있으므로 <see cref="DIRECTION"/>배열을 리턴하도록 함.<br />
        /// 낮은 인덱스의 방향부터 먼저 적용하도록 함.<br />
        /// </summary>
        /// <returns>중력의 방향 배열</returns>
        public static DIRECTION[] GetGravityDirections()
        {
            // 중력의 영향을 받을 경우, 3가지 방향으로 중력을 받도록 설정. 일단 0번(우선순위 가장 높음)은 바로 아래로 설정
            DIRECTION[] dirs = new DIRECTION[3];
            dirs[0] = DIRECTION.DOWN;

            //=======================================================================================================================================
            // 왼쪽 오른쪽 우선순위를 번갈아가면서 적용
            //=======================================================================================================================================
            if ( BlockMatcher.isLeftDropFirst == true )
            {
                BlockMatcher.isLeftDropFirst = false;
                dirs[1] = DIRECTION.LEFT_DOWN;
                dirs[2] = DIRECTION.RIGHT_DOWN;
            }
            else
            {
                BlockMatcher.isLeftDropFirst = true;
                dirs[1] = DIRECTION.RIGHT_DOWN;
                dirs[2] = DIRECTION.LEFT_DOWN;
            }

            return dirs;
        }

        //===========================================================================================================================================
        //
        //  곧바로 추락하는 방향을 리턴하는 Static 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 중력이 적용되는 방향을 리턴하는 Static 메서드.<br />
        /// <br />
        /// 곧바로 추락하는 경우, 한 방향으로만 떨어짐. <see cref="DIRECTION"/>을 리턴하도록 함.<br />
        /// 추락하는 경우, 원소가 1개 들어있는 배열을 리턴하도록 함.
        /// </summary>
        /// <returns>중력의 방향 배열</returns>
        public static DIRECTION[] GetStraightDirections()
        {
            DIRECTION[] dirs = new DIRECTION[1];
            dirs[0] = DIRECTION.DOWN;
            return dirs;
        }
        #endregion

        #region PUBLIC_METHOD
        //===========================================================================================================================================
        //
        //  특정 개수만큼 블럭을 만드는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 특정 개수만큼 블럭을 만드는 메서드.<br />
        /// <br />
        /// 블럭이 하나 만들어질 때 마다, 블럭이 중력을 받아 떨어지게 되고<br />
        /// 블럭이 떨어져서 도착할 경우 도착한 타일을 전달하는 콜백을 실행함<br />
        /// </summary>
        /// <param name="num">만들 블럭의 개수</param>
        /// <param name="callback">다 만들면 실행되는 콜백</param>
        public void RequestBlock(int num, MatchMatchDelegate.GenericDelegate<Tile> callback)
        {
            StartCoroutine(CreateBlockCoroutine(num, callback));
        }

        //===========================================================================================================================================
        //
        //  게임중에 사용하는 블럭 리스트를 초기화하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 게임중에 사용하는 블럭 리스트를 초기화하는 메서드.
        /// </summary>
        public void SetBlockArray()
        {
            Block[] block = this.GetComponentsInChildren<Block>();
            Block.Clear();
            foreach ( Block c in block )
            {
                Block.Add(c);
            }
        }

        //===========================================================================================================================================
        //
        //  게임이 로드되었을 때, 초기값을 통해 블럭을 초기화하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 게임이 로드되었을 때, 초기값을 통해 블럭을 초기화하는 메서드.
        /// </summary>
        public void SetBlockInfo()
        {
            // 모든 플레이 가능한 타일에 대하여 Loop를 돌며 초기화
            for ( int i = 0 ; i < GameManager.CurrentLevelInfo.PlayableTileNum ; i++ )
            {
                Block[i].SetBlockType((BLOCK_TYPE)GameManager.CurrentLevelInfo.InitTileInfo[i]);
                Block[i].SetTile(GameManager.TileManagerInstance.Tiles[i]);
            }
        }

        //===========================================================================================================================================
        //
        //  블럭을 하나 생성하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 블럭을 하나 생성하는 메서드.
        /// </summary>
        public void CreateBlock()
        {
            SetBlockArray();                                            // 현재 관리하고있는 블럭 초기화
            GameObject go = Instantiate(BlockPrefab, this.transform);   // 프리팹 오브젝트 생성
            Block createBlock = go.GetComponent<Block>();               // 생성한 오브젝트에서 컴포넌트 가져오기

            // TO-DO : 특수한 타일들도 생성

            int RandomIndex = Random.Range(0, 6);                       // 랜덤한 숫자 생성

            createBlock.SetBlockType((BLOCK_TYPE)RandomIndex);          // 랜덤 숫자 기반으로 블럭 타입 설정
            createBlock.SetTile(CreateBlockTile);                       // 생성한 블럭의 시작 타일을 설정
            Block.Add(createBlock);                                     // 블럭 추가
            CreateBlockTile.SetBlock(createBlock);                      // 시작 타일또한 생성한 블럭을 가짐

            GameManager.SoundManagerInstance.PlaySound(SFX.CREATE);     // 사운드 재생
        }
        #endregion

        #region PRIVATE_ENUMERATOR
        //===========================================================================================================================================
        //
        //  특정 개수만큼 블럭새로 생성하는 메서드
        //
        //===========================================================================================================================================
        private IEnumerator CreateBlockCoroutine(int num, MatchMatchDelegate.GenericDelegate<Tile> callback)
        {
            for(int i = 0 ; i < num ; i ++ )
            {
                // 블럭을 생성하고, 중력을 적용. 블럭에 도착하면 실행되는 실행되는 콜백도 같이 전달
                CreateBlock();
                CreateBlockTile.DropBlock(GetGravityDirections() , callback);

                // MatchMatchConfig.BLOCK_CREATE_DELAY_SECOND 만큼 대기
                yield return new WaitForSeconds(MatchMatchConfig.BLOCK_CREATE_DELAY_SECOND);
            }
        }
        #endregion

        #region PRIVATE_METHOD
        //===========================================================================================================================================
        //
        //  Start 메서드
        //
        //===========================================================================================================================================
        private void Start()
        {
            SetBlockArray();
        }
        #endregion
    }
}
