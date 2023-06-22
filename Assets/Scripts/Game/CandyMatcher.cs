using HexaBlast.Common;
using HexaBlast.Common.VO;
using HexaBlast.UI.POPUP;
using HexaBlast.UI;
using UnityEngine;
using System.Collections.Generic;

namespace HexaBlast.Game
{
    public class CandyMatcher
    {
        public static bool isLeftDropFirst = false;

        private static int dropCount = 0;
        private static List<Tile> dropCountList;

        public DeleteInfo CalculateGameStatus(Tile[] tiles)
        {
            var info = new DeleteInfo();
            var deleteSet = new HashSet<Tile>();
            dropCountList  = new List<Tile>();
            dropCount = 0;

            foreach ( Tile tile in tiles )
            {
                info.DeletedTileNum = CalculateDeleteSet(tile, ref deleteSet);
            }
            info.DeletedTileNum += LightControl(ref deleteSet);

            GameManager.Instance.DelaySecond(HexaBlastConfig.CANDY_DELETE_DELAY_SECOND, () => {
                RemoveDeleteSet(deleteSet);

                info.FalledTileNum = DropCandy(deleteSet, CandyDropChecker);
                dropCount = info.FalledTileNum;

                if ( dropCount == 0 )
                {
                    dropCountList = new List<Tile>();
                    dropCount = 0;
                    UseGravity();
                }
            });
            return info;
        }

        private int CalculateDeleteSet(Tile tile, ref HashSet<Tile> deleteSet)
        {
            var result = new DeleteInfo();
            if( tile.CurrentCandy == null || tile.CurrentCandy.Type == CANDY_TYPE.OFF || tile.CurrentCandy.Type == CANDY_TYPE.ON)
            {
                return 0;
            }

            //=======================================================================================================================
            // ↙ ↗ 방향으로 캔디가 있는 지 확인
            //=======================================================================================================================
            Match(tile, new DIRECTION[] { DIRECTION.LEFT_DOWN, DIRECTION.RIGHT_UP },
                                          MATCH_THRESHOLD.LINE_MATCH, ref deleteSet);

            //=======================================================================================================================
            // ↖ ↘ 방향으로 캔디가 있는지 확인
            //=======================================================================================================================
            Match(tile, new DIRECTION[] { DIRECTION.LEFT_UP, DIRECTION.RIGHT_DOWN },
                                          MATCH_THRESHOLD.LINE_MATCH, ref deleteSet);

            //=======================================================================================================================
            // ↑ ↓ 방향으로 캔디가 있는지 확인
            //=======================================================================================================================
            Match(tile, new DIRECTION[] { DIRECTION.UP, DIRECTION.DOWN },
                                          MATCH_THRESHOLD.LINE_MATCH, ref deleteSet);

            //=======================================================================================================================
            // 4개 이상 뭉쳐있는 캔디가 있는 지확인
            //=======================================================================================================================
            Match(tile, new DIRECTION[] { DIRECTION.UP,    DIRECTION.RIGHT_UP,     DIRECTION.RIGHT_DOWN,
                                          DIRECTION.DOWN,  DIRECTION.LEFT_DOWN,    DIRECTION.LEFT_UP,},
                                          MATCH_THRESHOLD.GROUP_MATCH, ref deleteSet);

            result.DeletedTileNum = deleteSet.Count;

            return deleteSet.Count;
        }

        private void Match(Tile initTile, DIRECTION[] dirs, MATCH_THRESHOLD threshold, ref HashSet<Tile> deleteSet)
        {
            var TempDeleteSet = new HashSet<Tile>();
            var TileVisitCheck = new List<bool>();
            for ( int i = 0 ; i < GameManager.CurrentLevelInfo.PlayableTileNum ; i++ )
            {
                TileVisitCheck.Add(false);
            }
            var CheckQueue = new Queue<Tile>();

            CheckQueue.Enqueue(initTile);
            while ( CheckQueue.Count > 0 )
            {
                var nextTile = CheckQueue.Dequeue();
                if ( nextTile == null || nextTile.CurrentCandy == null || TileVisitCheck[nextTile.Id] == true ||
                    nextTile.CurrentCandy.Type != initTile.CurrentCandy.Type )
                {
                    continue;
                }
                TileVisitCheck[nextTile.Id] = true;
                TempDeleteSet.Add(nextTile);
                
                foreach(DIRECTION dir in dirs)
                {
                    CheckQueue.Enqueue(nextTile.Neighborhoods[(int)dir]);
                }
            }

            if ( TempDeleteSet.Count >= (int)threshold )
            {
                foreach ( var item in TempDeleteSet )
                {
                    deleteSet.Add(item);
                }
            }

            return;
        }
        private int LightControl(ref HashSet<Tile> deleteSet)
        {
            var LightOffSet = new HashSet<Tile>();
            var LightOnSet  = new HashSet<Tile>();

            foreach (Tile t in deleteSet)
            {
                for ( int d = 0 ; d < (int)DIRECTION.LENGTH ; d++ )
                {
                    if ( t.Neighborhoods[d] == null || t.Neighborhoods[d].CurrentCandy == null)
                    {
                        continue;
                    }
                    CANDY_TYPE type = t.Neighborhoods[d].CurrentCandy.Type;
                    if(type == CANDY_TYPE.OFF)
                    {
                        LightOffSet.Add(t.Neighborhoods[d]);
                    }
                    else if (type == CANDY_TYPE.ON)
                    {
                        LightOnSet.Add(t.Neighborhoods[d]);
                    }
                }
            }

            foreach(Tile offTile in LightOffSet)
            {
                offTile.CurrentCandy.LightOn();
            }

            foreach ( Tile onTile in LightOnSet )
            {
                deleteSet.Add(onTile);
            }

            return LightOnSet.Count;
        }

        private int RemoveDeleteSet(HashSet<Tile> deleteSet)
        {
            int deleteCount = deleteSet.Count;
            if( deleteCount > 0)
            {
                ScreenShacker.Shake();
                GameManager.SoundManagerInstance.PlaySound(SFX.POP);
                MobileVibrateManager.Vibrate(1f);
            }

            foreach ( Tile t in deleteSet )
            {
                t.DeleteCandy();
            }

            GameManager.TileManagerInstance.RefreshAllAnimation();
            return deleteSet.Count;
        }



        public int DropCandy(HashSet<Tile> deleteSet, HexaBlastDelegate.GenericDelegate<Tile> deleteCallback)
        {
            HashSet<Tile> upperTiles = new HashSet<Tile>();
            foreach ( Tile t in deleteSet )
            {
                HashSet<Tile> upper = t.CalculateUpperTile();
                foreach ( Tile upperTile in upper )
                {
                    upperTiles.Add(upperTile);
                }
            }

            List<Tile> SortedUpperList = new List<Tile>();
            foreach ( Tile tile in upperTiles )
            {
                SortedUpperList.Add(tile);
            }
            SortedUpperList.Sort((tileA, tileB) => {
                return tileB.Id.CompareTo(tileA.Id);
            });

            foreach ( Tile tile in SortedUpperList )
            {
                tile.DropCandy(CandyManager.GetStraightDirections(), (targetTile) => {
                    deleteCallback(targetTile);
                });
            }
            GameManager.TileManagerInstance.RefreshAllAnimation();
            return SortedUpperList.Count;
        }

        private void CandyDropChecker(Tile tile)
        {
            dropCountList.Add(tile);
            if(dropCountList.Count == dropCount )
            {
                Tile[] tempList = new Tile[dropCountList.Count];
                dropCountList.CopyTo(tempList);
                GameManager.Instance.CalculateGameStatus(tempList);
            }
        }

        private void UseGravity()
        {
            Tile[] tiles = GameManager.TileManagerInstance.Tiles;
            List<Tile> calculatedTiles = new List<Tile>();

            int moveCount = 0;
            for ( int id = ( GameManager.CurrentLevelInfo.PlayableTileNum - 1 ) ; id >= 0 ; id-- )
            {
                if ( tiles[id].CurrentCandy != null )
                {
                    bool isTileMove = tiles[id].DropCandy(CandyManager.GetGravityDirections(), (tile) => {
                        calculatedTiles.Add(tile);
                        if( calculatedTiles.Count == moveCount )
                        {
                            DeleteInfo info = GameManager.Instance.CalculateGameStatus(calculatedTiles);
                        }
                    });

                    if ( isTileMove == true )
                    {
                        moveCount++;
                    }
                }
            }

            GameManager.TileManagerInstance.RefreshAllAnimation();
            if ( moveCount == 0 )
            {
                CreateCandy();
            }
        }

        private void CreateCandy()
        {
            int emptyNum = GameManager.TileManagerInstance.GetEmptyTile();
            List<Tile> calculatedTiles = new List<Tile>();
            GameManager.CandyManagerInstance.RequestCandy(emptyNum, (tile) => {
                calculatedTiles.Add(tile);
                if( calculatedTiles.Count == emptyNum )
                {
                    DeleteInfo info = GameManager.Instance.CalculateGameStatus(calculatedTiles);
                }
            });

            GameManager.TileManagerInstance.RefreshAllAnimation();
            if ( emptyNum  == 0)
            {
                PlayDone();
            }
        }

        private void PlayDone()
        {
            if(GameManager.Instance.IsGameEnd() == true)
            {
                StageClearUIHandler clearUI = PopUpManager.Instance.WindowsDict["StageClear"] as StageClearUIHandler;
                clearUI.gameObject.SetActive(true);
                GameManager.SoundManagerInstance.PlaySound(SFX.TADA);
                clearUI.AnimationChecker(() => {
                    GameResultUIHandler resultUI = PopUpManager.Instance.WindowsDict["GameResult"] as GameResultUIHandler;
                    resultUI.gameObject.SetActive(true);
                    resultUI.SetStarStatus(HeaderUIHandler.Instance.Score, GameManager.CurrentLevelInfo.StarThreshold);
                });
            }
            else
            {
                GameManager.IsInteractable = true;
            }
        }
    }
}