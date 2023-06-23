using MatchMatch.Common;
using MatchMatch.Common.VO;
using MatchMatch.UI.POPUP;
using MatchMatch.UI;
using System.Collections.Generic;

namespace MatchMatch.Game
{
    //===============================================================================================================================================
    // 블록 매칭 알고리즘 담당하는 클래스
    //===============================================================================================================================================
    public class BlockMatcher
    {
        #region FIELD
        //===========================================================================================================================================
        //  Public 멤버 변수
        //===========================================================================================================================================
        public static bool isLeftDropFirst = false;                     // 블록이 왼쪽부터 떨어질지, 오른쪽부터 떨어질지 결정하는 메서드

        private static int dropCount = 0;                               // 중력에 의해 떨어지는 블록 개수
        private static List<Tile> dropCountList;                        // 중력에 의해 떨어진 블록이 도착한 타일들
        #endregion

        #region PUBLIC_METHOD
        //===========================================================================================================================================
        //
        //  여러 타일들에 대해, 매칭 연산 진행
        //
        //===========================================================================================================================================
        /// <summary>
        /// 여러 타일들에 대해, 매칭 연산 진행하는 메서드.
        /// </summary>
        /// <param name="tiles">연산을 진행할 타일 배열</param>
        /// <returns>연산 결과 VO (지워진 블록 개수, 떨어진 블록 개수)</returns>
        public DeleteInfo RunBlockMatch(Tile[] tiles)
        {
            var info = new DeleteInfo();                                // 리턴 VO 생성
            var deleteSet = new HashSet<Tile>();                        // 지워야할 타일들 초기화
            dropCountList  = new List<Tile>();                          // 전역 변수 초기화
            dropCount = 0;                                              // 전역 변수 초기화

            //=======================================================================================================================================
            //  전달받은 타일들에 대해, 매칭을 진행하고 지워지는 타일 결과를 HashSet에 저장 (중복 방지)
            //=======================================================================================================================================
            foreach ( Tile tile in tiles )
            {
                info.DeletedTileNum = CalculateDeleteSet(tile, deleteSet);
            }
            // 추가로 지워지는 블록들을 계산
            info.DeletedTileNum += AdditionalDeleteSet(deleteSet);

            // MatchMatchConfig.BLOCK_DELETE_DELAY_SECOND 만큼 대기
            GameManager.Instance.DelaySecond(MatchMatchConfig.BLOCK_DELETE_DELAY_SECOND, () => {
                // 계산한 DeleteSet을 실제로 제거
                RemoveDeleteSet(deleteSet);

                // 제거 후 중력의 영향을 받아 떨어지는 블록들 계산 
                info.FalledTileNum = DropBlock(deleteSet, BlockDropChecker);
                dropCount = info.FalledTileNum;

                // 떨어지는 블록이 존재 할 경우, 초기화후 중력 적용
                if ( dropCount == 0 )
                {
                    dropCountList = new List<Tile>();
                    dropCount = 0;
                    UseGravity();
                }
            });
            return info;
        }
        #endregion

        #region PRIVATE_METHOD
        //===========================================================================================================================================
        //
        // 지워질 블록 계산
        //
        //===========================================================================================================================================
        private int CalculateDeleteSet(Tile tile, HashSet<Tile> deleteSet)
        {
            // 타일이 비어있거나, 매칭할 수 있는 블록이 아니라면, 탈출
            if( tile.CurrentBlock == null || tile.CurrentBlock.IsMatchable == false )
            {
                return 0;
            }

            //=======================================================================================================================================
            // ↙ ↗ 방향으로 캔디가 있는 지 확인
            //=======================================================================================================================================
            Match(tile, new DIRECTION[] { DIRECTION.LEFT_DOWN, DIRECTION.RIGHT_UP },
                                          MATCH_THRESHOLD.LINE_MATCH, deleteSet);

            //=======================================================================================================================================
            // ↖ ↘ 방향으로 캔디가 있는지 확인
            //=======================================================================================================================================
            Match(tile, new DIRECTION[] { DIRECTION.LEFT_UP, DIRECTION.RIGHT_DOWN },
                                          MATCH_THRESHOLD.LINE_MATCH, deleteSet);

            //=======================================================================================================================================
            // ↑ ↓ 방향으로 캔디가 있는지 확인
            //=======================================================================================================================================
            Match(tile, new DIRECTION[] { DIRECTION.UP, DIRECTION.DOWN },
                                          MATCH_THRESHOLD.LINE_MATCH, deleteSet);

            //=======================================================================================================================================
            // 4개 이상 뭉쳐있는 캔디가 있는 지확인
            //=======================================================================================================================================
            Match(tile, new DIRECTION[] { DIRECTION.UP,    DIRECTION.RIGHT_UP,     DIRECTION.RIGHT_DOWN,
                                          DIRECTION.DOWN,  DIRECTION.LEFT_DOWN,    DIRECTION.LEFT_UP,},
                                          MATCH_THRESHOLD.GROUP_MATCH, deleteSet);

            return deleteSet.Count;
        }

        //===========================================================================================================================================
        //
        // DFS를 이용한 매칭 연산
        //
        //===========================================================================================================================================
        private void Match(Tile initTile, DIRECTION[] dirs, MATCH_THRESHOLD threshold, HashSet<Tile> deleteSet)
        {
            var TempDeleteSet = new HashSet<Tile>();                                    // 임시 저장용 Delete Set
            var TileVisitCheck = new List<bool>();                                      // DFS용 방문 Check List

            // 방문 CheckList 초기화
            for ( int i = 0 ; i < GameManager.CurrentLevelInfo.PlayableTileNum ; i++ )
            {
                TileVisitCheck.Add(false);
            }
            
            // DFS용 Queue
            var CheckQueue = new Queue<Tile>();

            // Queue에 검사할 타일 하나를 넣음
            CheckQueue.Enqueue(initTile);

            //=======================================================================================================================================
            // DFS 연산
            //=======================================================================================================================================
            while ( CheckQueue.Count > 0 )
            {
                // 다음 타일 꺼내오기
                var nextTile = CheckQueue.Dequeue();

                // 다음 타일이 없거나, 다음 타일이 비어있거나, 이미 방문했거나, 블록의 타입이 다르면, 취소
                if ( nextTile == null || nextTile.CurrentBlock == null || TileVisitCheck[nextTile.Id] == true ||
                    nextTile.CurrentBlock.Type != initTile.CurrentBlock.Type )
                {
                    continue;
                }

                // 방문을 기록하고, 해당 타일을 임시 저장용 Delete Set에 저장
                TileVisitCheck[nextTile.Id] = true;
                TempDeleteSet.Add(nextTile);
                
                // 전달 받은 방향들을 기반으로 다음 타일들을 Queue에 추가
                foreach(DIRECTION dir in dirs)
                {
                    CheckQueue.Enqueue(nextTile.Neighborhoods[(int)dir]);
                }
            }

            //=======================================================================================================================================
            //  매칭 기준판단
            //=======================================================================================================================================
            // 임시 저장용 Delete Set 사이즈가 전달받은 기준값을 넘어가면, 진짜 Delete Set에 추가
            if ( TempDeleteSet.Count >= (int)threshold )
            {
                foreach ( var item in TempDeleteSet )
                {
                    // Delete Set은 Hash Set이므로, 중복일 경우 들어가지 않음
                    deleteSet.Add(item);
                }
            }

            return;
        }

        //===========================================================================================================================================
        //
        //  추가로 지워지는 타일들을 계산 (6방향)
        //
        //===========================================================================================================================================
        private int AdditionalDeleteSet(HashSet<Tile> deleteSet)
        {
            var AdditionalDeleteSet = new HashSet<Tile>();
            var VisitedTile         = new HashSet<Tile>();

            //=======================================================================================================================================
            //  모든 Delete Set을 돌면서 확인
            //=======================================================================================================================================
            foreach ( Tile t in deleteSet )
            {
                // 6방향에 대하여 모두 확인
                for ( int d = 0 ; d < (int)DIRECTION.LENGTH ; d++ )
                {
                    // 비어있거나, 해당 방향에 타일이 없다면 패스
                    // 또는, 이미 방문한 타일이라도, 패스
                    if ( t.CheckNextTileEmpty((DIRECTION)d) == true ||
                         VisitedTile.Contains(t.Neighborhoods[d]) )
                    {
                        continue;
                    }

                    // 영향을 계산하고, 만약 터지는 블록이라면 터지도록 설정
                    if ( t.Neighborhoods[d].CurrentBlock.AdditionalEffect() == true )
                    {
                        AdditionalDeleteSet.Add(t.Neighborhoods[d]);
                    }
                    // 방문 타일에 기록
                    VisitedTile.Add(t.Neighborhoods[d]);
                }
            }

            //=======================================================================================================================================
            //  추가적으로 지워지는 타일들도 Delete Set에 추가
            //=======================================================================================================================================
            foreach ( Tile additionalTile in AdditionalDeleteSet )
            {
                deleteSet.Add(additionalTile);
            }

            // 추가적으로 지워진 타일들의 개수 반환
            return AdditionalDeleteSet.Count;
        }

        //===========================================================================================================================================
        //
        //  지워야하는 타일들을 실제로 제거하는 메서드
        //
        //===========================================================================================================================================
        private int RemoveDeleteSet(HashSet<Tile> deleteSet)
        {
            // 지워야하는 타일들의 개수가 1개 이상이라면, 지워지는 효과 진행
            int deleteCount = deleteSet.Count;
            if( deleteCount > 0)
            {
                ScreenShacker.Shake();
                GameManager.SoundManagerInstance.PlaySound(SFX.POP);
                MobileVibrateManager.Vibrate(1f);
            }

            // 모든 타일을 제거
            foreach ( Tile t in deleteSet )
            {
                t.DeleteBlock();
            }

            // 애니메이션 초기화
            GameManager.TileManagerInstance.RefreshAllAnimation();
            return deleteSet.Count;
        }

        //===========================================================================================================================================
        //
        //  중력을 적용시키는 메서드
        //
        //===========================================================================================================================================
        private void UseGravity()
        {
            // 전체 타일들 목록을 가져오고, 계산해야하는 타일 목록을 초기화
            Tile[] tiles = GameManager.TileManagerInstance.Tiles;
            List<Tile> calculatedTiles = new List<Tile>();

            int moveCount = 0;
            //=======================================================================================================================================
            //  모든 타일들에 대해 조사
            //=======================================================================================================================================
            for ( int id = ( GameManager.CurrentLevelInfo.PlayableTileNum - 1 ) ; id >= 0 ; id-- )
            {
                // 타일이 비어있지 않으면 (블럭이 있으면)
                if ( tiles[id].CurrentBlock != null )
                {
                    // 타일을 움직이며, 모두 움직였을 땐 마지막 상태를 대상으로 재 매칭 연산 진행
                    bool isTileMove = tiles[id].DropBlock(BlockManager.GetGravityDirections(), (tile) => {
                        calculatedTiles.Add(tile);
                        if ( calculatedTiles.Count == moveCount )
                        {
                            DeleteInfo info = GameManager.Instance.CalculateGameStatus(calculatedTiles);
                        }
                    });

                    // 만약 타일을 움직인 경우, moveCount를 1증가시킴
                    if ( isTileMove == true )
                    {
                        moveCount++;
                    }
                }
            }

            GameManager.TileManagerInstance.RefreshAllAnimation();      // 애니메이션 초기화

            //=======================================================================================================================================
            //  만약 moveCount가 0이면, (이미 중력의 영향을 받아 안정된 상태라면,) 블럭 생성 단계 돌입
            //=======================================================================================================================================
            if ( moveCount == 0 )
            {
                CreateBlock();
            }
        }

        //===========================================================================================================================================
        //
        //  중력의 영향을 받아 떨어져야하는 타일을 계산하는 메서드
        //
        //===========================================================================================================================================
        private int DropBlock(HashSet<Tile> deleteSet, MatchMatchDelegate.GenericDelegate<Tile> deleteCallback)
        {
            // 지워진 타일들의 위쪽에 있어, 중력의 영향을 받아야하는 타일들의 Set
            HashSet<Tile> upperTiles = new HashSet<Tile>();

            //=======================================================================================================================================
            // 모든 지워진 타일들에 대해, 위쪽 타일들 계산
            //=======================================================================================================================================
            foreach ( Tile t in deleteSet )
            {
                // 위쪽 타일들을 계산
                HashSet<Tile> upper = t.CalculateUpperTile();
                foreach ( Tile upperTile in upper )
                {
                    upperTiles.Add(upperTile);
                }
            }

            //=======================================================================================================================================
            // 위쪽 타일들을, 높이 순서대로 정렬
            //=======================================================================================================================================
            List<Tile> SortedUpperList = new List<Tile>();
            foreach ( Tile tile in upperTiles )
            {
                SortedUpperList.Add(tile);
            }
            SortedUpperList.Sort((tileA, tileB) => {
                return tileB.Id.CompareTo(tileA.Id);
            });

            //=======================================================================================================================================
            // 아래쪽 타일들부터, 중력의 영향을 적용하기
            //=======================================================================================================================================
            foreach ( Tile tile in SortedUpperList )
            {
                // 실제로 떨어트리고, 도착하면 callback 실행
                tile.DropBlock(BlockManager.GetStraightDirections(), (targetTile) => {
                    deleteCallback(targetTile);
                });
            }

            // 애니메이션 초기화
            GameManager.TileManagerInstance.RefreshAllAnimation();
            return SortedUpperList.Count;
        }

        //===========================================================================================================================================
        //
        //  중력에 의해 떨어진 블럭들이 도착했을 때 실행되는 콜백 메서드
        //
        //===========================================================================================================================================
        private void BlockDropChecker(Tile tile)
        {
            // 도착한 타일 리스트에 추가
            dropCountList.Add(tile);
            
            // 만약 이 콜백함수가 마지막 콜백함수라면, (모두 중력의 영향을 다 받았다면)
            if(dropCountList.Count == dropCount )
            {
                // 이 블럭 상태들에 대해서 재 매칭 진행
                Tile[] tempList = new Tile[dropCountList.Count];
                dropCountList.CopyTo(tempList);
                GameManager.Instance.CalculateGameStatus(tempList);
            }
        }

        //===========================================================================================================================================
        //
        //  블럭을 생성하는 메서드
        //
        //===========================================================================================================================================
        private void CreateBlock()
        {
            // 비어있는 타일 개수를 세고, 다시 연산이 필요한 타일 목록을 초기화
            int emptyNum = GameManager.TileManagerInstance.GetEmptyTile();
            List<Tile> calculatedTiles = new List<Tile>();

            //=======================================================================================================================================
            // Block Manager에게 필요한 개수만큼 요청
            //=======================================================================================================================================
            GameManager.BlockManagerInstance.RequestBlock(emptyNum, (tile) => {
                // 연산이 필요한 타일에 추가
                calculatedTiles.Add(tile);

                // 모두 다 찼다면, 다시 한번 연산 진행
                if( calculatedTiles.Count == emptyNum )
                {
                    GameManager.Instance.CalculateGameStatus(calculatedTiles);
                }
            });

            // 
            GameManager.TileManagerInstance.RefreshAllAnimation();      // 애니메이션 초기화

            //=======================================================================================================================================
            //  만약 emptyNum가 0이면, (더 이상 만들 블럭이 없으면,) PlayDone 메서드 실행
            //=======================================================================================================================================
            if ( emptyNum  == 0)
            {
                PlayDone();
            }
        }

        //===========================================================================================================================================
        //
        //  플레이가 완료되었을 때, 마무리 처리를 담당하는 메서드
        //
        //===========================================================================================================================================
        private void PlayDone()
        {
            //=======================================================================================================================================
            //  게임 종료를 판단함
            //=======================================================================================================================================
            if ( GameManager.Instance.IsGameEnd() == true)
            {
                // 게임 종료 UI를 출력하는 메서드
                StageClearUIHandler clearUI = PopUpManager.Instance.WindowsDict["StageClear"] as StageClearUIHandler;
                clearUI.gameObject.SetActive(true);
                GameManager.SoundManagerInstance.PlaySound(SFX.TADA);

                // 게임 종료 UI가 끝난 후, 실행되는 callback
                clearUI.AnimationChecker(() => {
                    // 게임 결과 UI 출력 후, Star를 설정
                    GameResultUIHandler resultUI = PopUpManager.Instance.WindowsDict["GameResult"] as GameResultUIHandler;
                    resultUI.gameObject.SetActive(true);
                    resultUI.SetStarStatus(HeaderUIHandler.Instance.Score, GameManager.CurrentLevelInfo.StarThreshold);
                });
            }
            else
            {
                // 게임 종료가 아니라면, 조작이 가능하도록 변경
                GameManager.IsInteractable = true;
            }
        }
        #endregion
    }
}