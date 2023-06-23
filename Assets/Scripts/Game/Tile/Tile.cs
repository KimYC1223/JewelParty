using MatchMatch.UI;
using MatchMatch.Common;
using MatchMatch.Common.VO;
using UnityEngine;
using System.Collections.Generic;

namespace MatchMatch.Game
{
    //===============================================================================================================================================
    // 게임에서 사용되는 타일 클래스
    //===============================================================================================================================================
    public class Tile : MonoBehaviour
    {
        #region FIELD
        // 현 타일과 이웃한 타일.
        // [0] : 12시, [1] : 2시, [2] : 4시, [3] : 6시, [4] : 8시, [5] : 10시 방향 이웃 타일
        // 비어있으면 null
        public Tile[] Neighborhoods;
        public Block CurrentBlock;                                              // 이 타일이 가지고 있는 블럭
        public Vector2 Pos;                                                     // 이 타일의 위치
        public float ZoomAmount;                                                // UI상에서 확대된 정도
        public int Id;                                                          // 타일의 ID

        private float MIN_DISTANCE = MatchMatchConfig.TILE_MIN_DISTANCE;        // 드래그로 인정할 최소 거리
        private Animator TileAnimator;                                          // 타일 애니메이션
        private float ANGLE_ZERO        =   0;                                  // 0도에 해당하는 Radian 값
        private float ANGLE_PLUS_60     =   ( ( Mathf.PI ) / 3 );               // 60도에 해당하는 Radian 값
        private float ANGLE_PLUS_120    =   ( ( 2 * Mathf.PI ) / 3 );           // 120도에 해당하는 Radian 값
        private float ANGLE_PLUS_180    =   Mathf.PI;                           // 180도에 해당하는 Radian 값 
        private float ANGLE_MINUS_180   = - Mathf.PI;                           // -180도에 해당하는 Radian 값 (위와 같음)
        private float ANGLE_MINUS_120   = - ( ( 2 * Mathf.PI ) / 3 );           // -120도에 해당하는 Radian 값
        private float ANGLE_MINUS_60    = - ( ( Mathf.PI ) / 3 );               // -60도에 해당하는 Radian 값
        #endregion

        #region PUBLIC_METHOD
        //===========================================================================================================================================
        //
        //  타일 위치를 최신화 하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 타일 위치를 최신화 하는 메서드.<br />
        /// 화면의 크기에 따라 UI 크기가 달라지기 때문.<br />
        /// <br />
        /// 주기적으로 호출하는 이유는, 가로 / 세로로 디바이스 화면이 돌아갈 경우를 대비하기 위해서이다.
        /// </summary>
        public void SetTilePosition()
        {
            Pos = this.GetComponent<RectTransform>().anchoredPosition;
            ZoomAmount = UISizeHandler.Instance.Tiles.localScale.y;
            Pos *= ZoomAmount;
        }

        //===========================================================================================================================================
        //
        //  타일의 하이라이트 애니메이션 재생
        //
        //===========================================================================================================================================
        /// <summary>
        /// 타일의 하이라이트 애니메이션 재생.<br />
        /// 주로 블록이 터졌을 때 재생.
        /// </summary>
        public void PlayHighlightAnimation()
        {
            SetTilePosition();
            TileAnimator.SetTrigger("Highlight");
        }

        //===========================================================================================================================================
        //
        //  타일의 애니메이션 초기화
        //
        //===========================================================================================================================================
        /// <summary>
        /// 타일의 애니메이션 초기화
        /// </summary>
        public void PlayResetAnimation()
        {
            TileAnimator.SetTrigger("Reset");
        }

        //===========================================================================================================================================
        //
        //  타일에 불을 켜두는 애니메이션 재생.
        //
        //===========================================================================================================================================
        /// <summary>
        /// 타일에 불을 켜두는 애니메이션 재생.<br />
        /// 특수 타일 표시 용도로 사용.
        /// </summary>
        public void PlayLightOnAnimation()
        {
            TileAnimator.SetTrigger("LightOn");
        }

        //===========================================================================================================================================
        //
        //  타일에 블록을 고정시키는 메서드.
        //
        //===========================================================================================================================================
        /// <summary>
        /// 타일에 블록을 고정시키는 메서드.<br />
        /// </summary>
        /// <param name="block">고정시킬 블럭</param>
        public void SetBlock(Block block)
        {
            // CurrentBlock에 전달받은 block을 설정하고, 위치를 이 타일의 중앙으로 변경함
            this.CurrentBlock = block;
            RectTransform blockRect = block.GetComponent<RectTransform>();
            blockRect.anchoredPosition = this.GetComponent<RectTransform>().anchoredPosition;
        }

        //===========================================================================================================================================
        //
        //  이웃한 타일이 없거나 블럭을 갖고있지 않은지 확인하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 이웃한 타일이 없거나 블럭을 갖고있지 않은지 확인하는 메서드<br />
        /// 주로 타일간 블럭 스왑시 스왑이 가능한지 확인하는 용도로 사용.
        /// </summary>
        /// <param name="dir">확인하고자 하는 방향.</param>
        /// <returns>확인 결과 값.</returns>
        public bool CheckNextTileEmpty(DIRECTION dir)
        {
            return ( Neighborhoods[(int)dir] == null ||
                     Neighborhoods[(int)dir].CurrentBlock == null );
        }

        //===========================================================================================================================================
        //
        //  이웃한 타일과 블럭을 교환하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 이웃한 타일과 블럭을 교환하는 메서드.<br /><br />
        /// 스왑에 성공하면, GameManager에서 MoveCount를 하나 제거하고, 교환한 두 타일에 대해 매칭 연산 진행
        /// </summary>
        /// <param name="dir">스왑하고자 하는 방향.</param>
        public void SwapNeighborhood(DIRECTION dir)
        {
            if ( dir >= DIRECTION.UP && dir < DIRECTION.LENGTH && Neighborhoods[(int)dir] != null )
            {
                // 교환할 두 타일을 지정 (Push : 교환 신청 하는 타일 / Pull : 교환 신청 받는 타일 )
                Tile PushTile = this;                       // 이 타일
                Tile PullTile = Neighborhoods[(int)dir];    // 바꿀 타일

                // 매칭 결과를 DeleteInfo로 받고, 아무 일도 일어나지 않으면 다시 원복시킴
                DeleteInfo info = null;

                //===================================================================================================================================
                //  두 타일 스왑후, 이동 종료 후 Callback 실행
                //===================================================================================================================================
                SwapTile(PushTile, PullTile, () => {
                    // 매칭 진행
                    info = GameManager.Instance.CalculateGameStatus(new Tile[] { PushTile, PullTile });

                    //===============================================================================================================================
                    // 아무것도 지우지 못한 경우
                    //===============================================================================================================================
                    if ( info.DeletedTileNum == 0 )
                    {
                        GameManager.SoundManagerInstance.PlaySound(SFX.CANT);
                        // 다시 자리 바꿈 ( 원복시킴 )
                        SwapTile(PullTile, PushTile, () => { 
                            GameManager.TileManagerInstance.RefreshAllAnimation();
                        });
                    }
                    //===============================================================================================================================
                    // 뭐라도 지운 경우, 이동 카운트 하나 제거
                    //===============================================================================================================================
                    else
                    {
                        HeaderUIHandler.Instance.SubMoveValue();
                    }
                });
            }
        }

        //===========================================================================================================================================
        //
        //  해당 타일의 블럭을 제거하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 해당 타일의 블럭을 제거하는 메서드.<br /><br />
        /// 해당 타일의 <see cref="IBlockFunction.ReturnScore(Block)"/>를 점수로 돌려 받는다.
        /// </summary>
        public void DeleteBlock()
        {
            // 블럭이 터지는 애니메이션 재생
            PlayHighlightAnimation();

            // 점수 획득
            HeaderUIHandler.Instance.AddScoreValue(CurrentBlock.ReturnScore());

            // 실제 블럭 제거
            Destroy(CurrentBlock.gameObject);
            CurrentBlock = null;
        }

        //===========================================================================================================================================
        //
        //  해당 타일의 위쪽 타일들을 모두 찾아내고, 이를 HashSet<Tile>로 반환하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 해당 타일의 위쪽 타일들을 모두 찾아내고 반환하는 메서드.<br /><br />
        /// 이 타일을 기점으로, 막혀있거나, 비어있는 타일이 나올때 까지 위쪽을 향했을 때 나오는 타일들을 반환.
        /// </summary>
        /// <returns>위쪽에 위치한 타일들의 HashSet</returns>
        public HashSet<Tile> CalculateUpperTile()
        {
            // 현재 타일을 기준으로 함
            Tile UpTiles = this;

            // 위쪽에 위치한 타일들을 저장할 HashSet
            HashSet<Tile> moveTileSet = new HashSet<Tile>();

            // 무한 반복
            while ( true )
            {
                //===================================================================================================================================
                // 위쪽 타일이 존재하고, 비어있지 않은 경우에는
                //===================================================================================================================================
                if ( UpTiles.Neighborhoods[(int)DIRECTION.UP] != null &&
                     UpTiles.Neighborhoods[(int)DIRECTION.UP].CurrentBlock != null )
                {
                    // HashSet에 추가함
                    UpTiles = UpTiles.Neighborhoods[(int)DIRECTION.UP];
                    moveTileSet.Add(UpTiles);
                }
                //===================================================================================================================================
                // 위쪽 타일이 존재하지 않거나, 있어도 비어있을 경우에는
                //===================================================================================================================================
                else
                {
                    break;                          // Loop 종료
                }
            }

            return moveTileSet;
        }

        //===========================================================================================================================================
        //
        //  마우스 좌표를 입력 받았을 때, 현재 블럭 기준으로 6방향중 어느 방향에 위치해있는지 리턴하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 마우스 좌표를 입력 받았을 때, 현재 블럭 기준으로 6방향중, 어느 방향에 위치해있는지 리턴하는 메서드<br /><br />
        /// 마우스 좌표와 타일의 거리가 최소 인정 거리 이하라면 <see cref="DIRECTION.NOT_DEFINED"/> 리턴
        /// </summary>
        /// <param name="mousePos">현재 마우스의 위치</param>
        /// <returns>6방향 중 어느 방향에 존재하는가?</returns>
        public DIRECTION TargetingNeighborhoods(Vector2 mousePos)
        {
            // 타일에서 마우스를 바라보는 방향 벡터
            Vector2 dir = mousePos - Pos;

            // UI Zoom을 고려한 거리 측정
            float distance = dir.magnitude / ZoomAmount;

            //=======================================================================================================================================
            //  distance가 기준치 이하라면, DIRECTION.NOT_DEFINED 리턴
            //=======================================================================================================================================
            if ( distance < MIN_DISTANCE )
            {
                return DIRECTION.NOT_DEFINED;
            }

            //=======================================================================================================================================
            //  역탄젠트 함수로, 방향 벡터의 y와 x를 통해 벡터의 각도 (radian 값) 구하기
            //=======================================================================================================================================
            float angle = Mathf.Atan2(dir.y, dir.x);

            //=======================================================================================================================================
            //  각도가 두 임계값 사이일 때, 특정 방향 벡터를 리턴
            //=======================================================================================================================================
            if ( angle >= ANGLE_ZERO && angle < ANGLE_PLUS_60 )
            {
                return DIRECTION.RIGHT_UP;                                      // return 1
            }
            else if ( angle >= ANGLE_PLUS_60 && angle < ANGLE_PLUS_120 )
            {
                return DIRECTION.UP;                                            // return 0
            }
            else if ( angle >= ANGLE_PLUS_120 && angle <= ANGLE_PLUS_180 )
            {
                return DIRECTION.LEFT_UP;                                       // return 5
            }
            else if ( angle <= ANGLE_ZERO && angle > ANGLE_MINUS_60 )
            {
                return DIRECTION.RIGHT_DOWN;                                    // return 2
            }
            else if ( angle <= ANGLE_MINUS_60 && angle > ANGLE_MINUS_120 )
            {
                return DIRECTION.DOWN;                                          // return 3
            }
            else if ( angle <= ANGLE_MINUS_120 && angle >= ANGLE_MINUS_180 )
            {
                return DIRECTION.LEFT_DOWN;                                     // return 4
            }

            // 이런 일은 없지만, 기본값 설정
            return DIRECTION.NOT_DEFINED;
        }

        //===========================================================================================================================================
        //
        //  특정 방향들을 기준으로, 비어있으면 가지고 있는 블럭들을 이동시키는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 특정 방향들을 기준으로, 비어있으면 가지고 있는 블럭들을 이동시키는 메서드.<br /><br />
        /// <see cref="BlockManager.GetGravityDirections"/> 메서드 또는<br />
        /// <see cref="BlockManager.GetStraightDirections"/> 메서드를 통해 얻은<br />
        /// <see cref="DIRECTION"/> Array들을 대상으로, 해당 방향들에 타일이 비어있으면,<br />
        /// 그 위치로 블럭을 이동시킴<br /><br />
        /// Array의 인덱스가 작은 순서대로 비어있는지 확인.
        /// </summary>
        /// <param name="dirs">움직일 방향 Array</param>
        /// <param name="callback">다 움직인 후 실행할 Callback</param>
        /// <returns>타일이 움직였는지 아닌지, 움직인 결과 반환</returns>
        public bool DropBlock(DIRECTION[] dirs, MatchMatchDelegate.GenericDelegate<Tile> callback)
        {
            // 현재 블럭이 비어있으면, 움직일 수 있는 블럭이 없으므로 false 리턴
            if ( CurrentBlock == null )
            {
                return false;
            }

            Tile result = this;                                 // 최종적으로 도착할 타일
            List<Tile> path = new List<Tile>();                 // 블럭의 이동경로
            
            bool isEnd = false;                                 // Loop가 돌지 말지 결정하는 Flag 변수
            while ( isEnd == false )
            {
                isEnd = true;                                   // 일단 true로 설정

                //===================================================================================================================================
                //  전달 받은 Direction들에 대하여, 진행
                //===================================================================================================================================
                for ( int d = 0; d < dirs.Length && isEnd == true; d ++ )
                {
                    // 해당 방향의 타일이 존재하고, 해당 타일의 블럭이 비어있으면,
                    if ( result.Neighborhoods[(int)dirs[d]] != null &&
                         result.Neighborhoods[(int)dirs[d]].CurrentBlock == null )
                    {
                        // 최종 목적지를 갱신하고, 이동 경로에 추가
                        result = result.Neighborhoods[(int)dirs[d]];
                        path.Add(result);
                        isEnd = false;
                    }
                }
            }

            //=======================================================================================================================================
            //  연산이 끝났을 때, result가 이 타일이 아니라면
            //=======================================================================================================================================
            if ( result != this )
            {
                // 움직였다는 뜻이므로, result(최종 목적지 타일)에게 블럭을 넘겨줌
                result.CurrentBlock = CurrentBlock;
                CurrentBlock.MoveBlock(path, () => {
                    callback(result);
                });
                CurrentBlock = null;
            }

            //=======================================================================================================================================
            //  result가 이 타일인지 아닌지, 즉 움직였는지 아닌지 반환
            //=======================================================================================================================================
            return ( result != this );
        }
        #endregion

        #region PRIVATE_METHOD;
        //===========================================================================================================================================
        //
        //  Start 라이프 사이클에 실행되는 메서드.
        //
        //===========================================================================================================================================
        private void Start()
        {
            // Animator를 가져옴
            TileAnimator = transform.GetComponent<Animator>();
        }

        //===========================================================================================================================================
        //
        //  두 타일을 스왑하기
        //
        //===========================================================================================================================================
        private void SwapTile(Tile pushTile, Tile pullTile, MatchMatchDelegate.VoidDelegate callback)
        {
            // 2개의 Flag 변수를 선언하고, Callback함수를 통해 두 변수가 모두 true가 될 경우
            // 즉, 두 타일간 블럭의 스왑이 완료된 경우, 전달받은 callback 실행
            bool isPushTileArrived = false, isPullTileArrived = false;

            pushTile.CurrentBlock.MoveBlock(pullTile, () => {
                isPushTileArrived = true;
                if ( isPushTileArrived && isPullTileArrived && callback != null )
                {
                    callback();
                }
            });
            pullTile.CurrentBlock.MoveBlock(pushTile, () => {
                isPullTileArrived = true;
                if ( isPushTileArrived && isPullTileArrived && callback != null )
                {
                    callback();
                }
            });
        }
        #endregion
    }
}