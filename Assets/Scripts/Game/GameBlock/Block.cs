using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using MatchMatch.UI;
using MatchMatch.Common;
using System.Collections.Generic;

namespace MatchMatch.Game
{
    //===============================================================================================================================================
    //  게임에서 사용되는 블럭 클래스
    //===============================================================================================================================================
    public class Block : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        #region FIELD
        public BLOCK_TYPE Type;                                     // 블럭의 타입
        public Tile CurrentTile;                                    // 현재 블럭이 위치한 타일
        public bool IsMatchable;                                    // 블럭이 스스로 매칭이 될 수 있는지 여부

        private IBlockFunction blockFuntion;                        // 블럭의 특수한 기능들을 정의한 인터페이스
        private float moveSpeed = MatchMatchConfig.BLOCK_SPEED;     // 블럭 움직이는 스피드
        private Coroutine moveCoroutine = null;                     // 블럭을 움직이게하는 코루틴
        private RectTransform rect;                                 // 블럭의 위치 정보를 저장하는 RectTransform
        private Image icon;                                         // 블럭의 아이콘
        #endregion

        #region PUBLIC_METHOD
        //===========================================================================================================================================
        //
        //  Block의 타입을 결정하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// Block의 타입을 결정하는 메서드.<br />
        /// 블럭의 타입을 결정하면서, 블럭의 sprite 및 blockFunction을 결정함. <see cref="BLOCK_TYPE"/>를 입력으로 받음<br />
        /// </summary>
        /// <param name="type">어떤 블럭으로 설정 할 것인지 결정하는 </param>
        public void SetBlockType(BLOCK_TYPE type)
        {
            // 타입을 결정
            this.Type = type;

            // block Function을 결정
            switch(type)
            {
                case BLOCK_TYPE.ON: case BLOCK_TYPE.OFF:
                    blockFuntion = new GoalBlock();    break;
                default:
                    blockFuntion = new NormalBlock();  break;
            }

            // 블럭의 아이콘과 특성을 결정
            SetBlockIcon(type);
            SetMatchableBlock();
        }

        //===========================================================================================================================================
        //
        //  Block Type에 따라 블록의 sprite를 결정
        //
        //===========================================================================================================================================
        /// <summary>
        /// Block Type에 따라 블록의 sprite를 결정하는 메서드.<br />
        /// <see cref="BLOCK_TYPE"/>를 입력으로 받음.<br />
        /// </summary>
        /// <param name="type">어떤 블럭으로 설정 할 것인지 결정하는 </param>
        public void SetBlockIcon(BLOCK_TYPE type)
        {
            this.icon.sprite = GameManager.BlockManagerInstance.BlockIcons[(int)type];
        }

        //===========================================================================================================================================
        //
        //  주변에서 블럭이 터졌을 때, 실행되는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 주변에서 블럭이 터졌을 때, 실행되는 메서드. <br />
        /// 또한 이 블럭또한 터지는지 아닌지 리턴함.<br />
        /// </summary>
        /// <returns>이 블럭도 터지는지 아닌지 리턴</returns>
        public bool AdditionalEffect()
        {
            return blockFuntion.AdditionalEffect(this);
        }

        //===========================================================================================================================================
        //
        //  이 캔디가 위치한 타일을 설정하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 이 캔디가 위치한 타일을 설정하는 메서드. <br />
        /// 설정하고자하는 <see cref="Tile"/>을 인자로 넘겨준다.<br />
        /// </summary>
        /// <param name="target"></param>
        public void SetTile(Tile target)
        {
            // 위치한 타일을 설정하고, 그 타일의 블럭 또한 이 블럭으로 설정
            this.CurrentTile = target;
            target.CurrentBlock = this;
        }

        //===========================================================================================================================================
        //
        //  블럭이 터졌을 때 얻을 수 있는 스코어 전달
        //
        //===========================================================================================================================================
        /// <summary>
        /// 블럭이 터졌을 때 얻을 수 있는 스코어를 가져오는 메서드.<br />
        /// </summary>
        /// <returns>점수 값</returns>
        public int ReturnScore()
        {
            return blockFuntion.ReturnScore(this);
        }

        //===========================================================================================================================================
        //
        //  블럭을 다른 타일로 움직이는 메서드 (여러 타일을 거쳐서 움직임)
        //
        //===========================================================================================================================================
        /// <summary>
        /// 블럭을 다른 타일들로 움직이는 메서드. (여러 타일을 통해 움직이는 메서드)<br />
        /// </summary>
        /// <param name="path">블럭이 움직일 타일 경로</param>
        /// <param name="callback">마지막 타일에 도착했을 때 실행될 <see cref="MatchMatchDelegate.VoidDelegate"/> callback</param>
        public void MoveBlock(List<Tile> path, MatchMatchDelegate.VoidDelegate callback)
        {
            OverrideCoroutine(StartCoroutine(MoveBlockFollowingPathyCoroutine(path, callback)));
        }

        //===========================================================================================================================================
        //
        //  블럭을 다른 타일로 움직이는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 블럭을 다른 타일로 움직이는 메서드.<br />
        /// </summary>
        /// <param name="path">블럭을 이동시킬 도착지 타일</param>
        /// <param name="callback">타일에 도착했을 때 실행될 <see cref="MatchMatchDelegate.VoidDelegate"/> callback</param>
        public void MoveBlock(Tile destination, MatchMatchDelegate.VoidDelegate callback)
        {
            OverrideCoroutine(StartCoroutine(MoveBlockCoroutine(destination, callback)));
        }
        #endregion

        #region PRIVATE_IENUMERATOR
        //===========================================================================================================================================
        //
        //  블럭을 움직이게 하는 코루틴
        //
        //===========================================================================================================================================
        private IEnumerator MoveBlockCoroutine(Tile destination, MatchMatchDelegate.VoidDelegate callback)
        {
            CurrentTile.SetTilePosition();                      // 현재 타일의 위치 정보 최신화
            destination.SetTilePosition();                      // 목적지 타일의 위치 정보 최신화

            // 목적지를 향한 방향 단위 벡터 계산
            Vector2 dir = ( destination.Pos - ( rect.anchoredPosition * CurrentTile.ZoomAmount ) ).normalized;
            
            // 목적지까지에 대한 거리 계산
            float distance = ( destination.Pos - ( rect.anchoredPosition * CurrentTile.ZoomAmount) ).magnitude;
            float old_distance = distance;

            //=======================================================================================================================================
            // 거리가 줄어드는 동안 계속, 방향 단위벡터를 향해 속도만큼 이동
            //=======================================================================================================================================
            while ( distance >= moveSpeed )
            {
                // 방향벡터 * 속도 만큼 위치 업데이트
                rect.anchoredPosition = rect.anchoredPosition + ( dir * moveSpeed );

                // 거리를 계산
                old_distance = distance;
                distance = ( destination.Pos - ( rect.anchoredPosition * CurrentTile.ZoomAmount ) ).magnitude;

                // 거리가 줄어들 때 동안만 업데이트 (블럭이 목적지를 지나칠 경우 대비)
                if(old_distance <= distance)
                {
                    break;
                }
                yield return 0;
            }

            // 종료되었을 때, 목적지의 위치로 업데이트
            rect.anchoredPosition = destination.Pos / CurrentTile.ZoomAmount;
            yield return 0;

            moveCoroutine = null;                               // 실행중인 코루틴을 제거
            if ( callback != null )                             // Callback 함수가 존재할 경우 (경유지가 아닌경우)
            {
                SetTile(destination);                           // 현재 블럭이 위치한 타일을 설정
                callback();                                     // 콜백함수 실행
            }
        }

        //===========================================================================================================================================
        //
        //  블럭을 움직이게 하는 코루틴 (여러 타일들을 거쳐 이동)
        //
        //===========================================================================================================================================
        private IEnumerator MoveBlockFollowingPathyCoroutine(List<Tile> path, MatchMatchDelegate.VoidDelegate callback)
        {
            // 전체 타일들에 대해 Loop 진행
            for(int i = 0 ; i < path.Count ; i++ )
            {
                //===================================================================================================================================
                // 마지막 타일이 아닐경우, callback을 null을 넣고 진행
                //===================================================================================================================================
                if ( i < (path.Count - 1) )
                {
                    yield return StartCoroutine(MoveBlockCoroutine(path[i], null));
                }
                //===================================================================================================================================
                // 마지막 타일일 경우, callback을 넘겨줌
                //===================================================================================================================================
                else
                {
                    yield return StartCoroutine(MoveBlockCoroutine(path[i], callback));
                }
            }
        }
        #endregion

        #region PRIVATE_METHOD
        //===========================================================================================================================================
        //
        //  Awake 라이프사이클에서 실행되는 메서드 정의
        //
        //===========================================================================================================================================
        private void Awake()
        {
            rect = this.GetComponent<RectTransform>();
            icon = this.GetComponent<Image>();
        }

        //===========================================================================================================================================
        //
        //  이 블럭으로 매칭 연산을 실행할 수 있는지 판단하는 메서드
        //
        //===========================================================================================================================================
        private void SetMatchableBlock()
        {
            blockFuntion.SetMatchableBlock(this);
        }

        //===========================================================================================================================================
        //
        //  코루틴을 실행하는 메서드. 이미 실행중이라면 취소해버림
        //
        //===========================================================================================================================================
        private void OverrideCoroutine(Coroutine coroutine)
        {
            if ( moveCoroutine != null )
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }
            moveCoroutine = coroutine;
        }

        //===========================================================================================================================================
        //
        //  MousePos 정보를 Center Center RectTransform 좌표계로 변경하는 메서드
        //
        //===========================================================================================================================================
        private Vector2 getMousePointerToUIPos(Vector2 mousePos)
        {
            // 현재 화면 사이즈를 가져옴
            Vector2 Offset = UISizeHandler.Instance.ScreenSize;
            Offset.x /= 2;
            Offset.y -= ( 25f + ( UISizeHandler.Instance.MainLayout.rect.height / 2 )
                              + UISizeHandler.Instance.ScoreLayout.sizeDelta.y );

            // MousePos 오프셋 조정
            mousePos -= Offset;
            return mousePos;
        }
        #endregion

        #region POINTER_HANDLER
        //===========================================================================================================================================
        //
        //  블럭을 손으로 눌렀을 때 실행되는 메서드
        //
        //===========================================================================================================================================
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            // 누른 타일의 위치 정보 최신화
            CurrentTile.SetTilePosition();
        }

        //===========================================================================================================================================
        //
        //  블럭을 손으로 눌렀다 땠을 때 실행되는 메서드
        //
        //===========================================================================================================================================
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            //=======================================================================================================================================
            //  조작을 할 수 없을 경우, 아무 일도 일어나지 않음
            //=======================================================================================================================================
            if ( GameManager.IsInteractable == false )
            {
                return;
            }

            //=======================================================================================================================================
            //  해당 레벨의 Move Count를 모두 사용 했을 경우, 소리 재생 후 아무 일도 일어나지 않음
            //=======================================================================================================================================
            if ( HeaderUIHandler.Instance.Move <= 0 )
            {
                GameManager.SoundManagerInstance.PlaySound(SFX.CANT);
                return;
            }

            GameManager.IsInteractable = false;                             // 작업을 진행하기 전, 조작을 할 수 없도록 변경
            Vector2 mousePos = getMousePointerToUIPos(eventData.position);  // 마우스 위치를 UI 좌표계로 변경
            DIRECTION dir = CurrentTile.TargetingNeighborhoods(mousePos);   // 어느 방향으로 드래그했는지 확인

            // 드래그를 했는지, 했다면 그 방향으로 할 수 있는지 확인
            if ( dir == DIRECTION.NOT_DEFINED || CurrentTile.CheckNextTileEmpty(dir) )
            {
                // 조작을 할 수 있도록 변경
                GameManager.IsInteractable = true;
                return;
            }
            // 할 수 있다면, 두 타일과 자리 교체
            CurrentTile.SwapNeighborhood(dir);
        }
        #endregion
    }
}
