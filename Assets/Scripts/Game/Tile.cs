using HexaBlast.UI;
using HexaBlast.Common;
using HexaBlast.Common.VO;
using UnityEngine;
using System.Collections.Generic;

namespace HexaBlast.Game
{
    //========================================================================================================================
    // ���� Ÿ�� Class (�̿� ������ ������ ����)
    //========================================================================================================================
    public class Tile : MonoBehaviour
    {

        private float MIN_DISTANCE = HexaBlastConfig.TILE_MIN_DISTANCE;

        // �� Ÿ�ϰ� �̿��� Ÿ��.
        // [0] : 12��, [1] : 2��, [2] : 4��, [3] : 6��, [4] : 8��, [5] : 10�� ���� �̿� Ÿ��
        // ��������� null
        public Tile[] Neighborhoods;
        public Candy CurrentCandy;
        public Vector2 Pos;
        public float ZoomAmount;
        public int Id;

        private Animator TileAnimator;

        private float ANGLE_ZERO        =   0;
        private float ANGLE_PLUS_60     =   ( ( Mathf.PI ) / 3 );
        private float ANGLE_PLUS_120    =   ( ( 2 * Mathf.PI ) / 3 );
        private float ANGLE_PLUS_180    =   Mathf.PI;
        private float ANGLE_MINUS_180   = - Mathf.PI;
        private float ANGLE_MINUS_120   = - ( ( 2 * Mathf.PI ) / 3 );
        private float ANGLE_MINUS_60    = - ( ( Mathf.PI ) / 3 );

        public void Start()
        {
            TileAnimator = transform.GetComponent<Animator>();
        }

        public void SetTilePosition()
        {
            Pos = this.GetComponent<RectTransform>().anchoredPosition;
            ZoomAmount = UISizeHandler.Instance.Tiles.localScale.y;
            Pos *= ZoomAmount;
        }

        public void PlayHighlightAnimation()
        {
            SetTilePosition();
            TileAnimator.SetTrigger("Highlight");
        }

        public void PlayResetAnimation()
        {
            TileAnimator.SetTrigger("Reset");
        }

        public void PlayLightOnAnimation()
        {
            TileAnimator.SetTrigger("LightOn");
        }

        public void SetCandy(Candy candy)
        {
            this.CurrentCandy = candy;
            RectTransform candyRect = candy.GetComponent<RectTransform>();
            candyRect.anchoredPosition = this.GetComponent<RectTransform>().anchoredPosition;
        }


        public void SwapNeighborhood(DIRECTION dir)
        {
            if ( dir >= DIRECTION.UP && dir < DIRECTION.LENGTH && Neighborhoods[(int)dir] != null )
            {
                Tile PushTile = this;
                Tile PullTile = Neighborhoods[(int)dir];

                DeleteInfo info = null;
                SwapTile(PushTile, PullTile, () => {
                    info = GameManager.Instance.CalculateGameStatus(new Tile[] { PushTile, PullTile });

                    if ( info.DeletedTileNum == 0 )
                    {
                        GameManager.SoundManagerInstance.PlaySound(SFX.CANT);
                        SwapTile(PullTile, PushTile, () => { 
                            GameManager.TileManagerInstance.RefreshAllAnimation();
                        });
                    }
                    else
                    {
                        HeaderUIHandler.Instance.SubMoveValue();
                    }
                });
            }
        }

        public void SwapTile(Tile pushTile, Tile pullTile, HexaBlastDelegate.VoidDelegate callback)
        {
            bool isPushTileArrived = false, isPullTileArrived = false;
            pushTile.CurrentCandy.MoveCandy(pullTile, () => {
                isPushTileArrived = true;
                if( isPushTileArrived && isPullTileArrived && callback != null )
                {
                    callback();
                }
            });
            pullTile.CurrentCandy.MoveCandy(pushTile, () => {
                isPullTileArrived = true;
                if ( isPushTileArrived && isPullTileArrived && callback != null )
                {
                    callback();
                }
            });
        }

        public void DeleteCandy()
        {
            PlayHighlightAnimation();
            HeaderUIHandler.Instance.AddScoreValue(CurrentCandy.ReturnScore());
            Destroy(CurrentCandy.gameObject);
            CurrentCandy = null;
        }

        public HashSet<Tile> CalculateUpperTile()
        {
            Tile UpTiles = this;

            HashSet<Tile> moveTileSet = new HashSet<Tile>();

            while ( true )
            {
                if ( UpTiles.Neighborhoods[(int)DIRECTION.UP] != null &&
                     UpTiles.Neighborhoods[(int)DIRECTION.UP].CurrentCandy != null )
                {
                    UpTiles = UpTiles.Neighborhoods[(int)DIRECTION.UP];
                    moveTileSet.Add(UpTiles);
                }
                else
                {
                    break;
                }
            }

            return moveTileSet;
        }

        public DIRECTION TargetingNeighborhoods(Vector2 mousePos)
        {
            Vector2 dir = mousePos - Pos;

            float distance = dir.magnitude / ZoomAmount;
            if ( distance < MIN_DISTANCE )
            {
                return DIRECTION.NOT_DEFINED;
            }

            float angle = Mathf.Atan2(dir.y, dir.x);

            if ( angle >= ANGLE_ZERO && angle < ANGLE_PLUS_60 )
            {
                return DIRECTION.RIGHT_UP;
            }
            else if ( angle >= ANGLE_PLUS_60 && angle < ANGLE_PLUS_120 )
            {
                return DIRECTION.UP;
            }
            else if ( angle >= ANGLE_PLUS_120 && angle <= ANGLE_PLUS_180 )
            {
                return DIRECTION.LEFT_UP;
            }
            else if ( angle <= ANGLE_ZERO && angle > ANGLE_MINUS_60 )
            {
                return DIRECTION.RIGHT_DOWN;
            }
            else if ( angle <= ANGLE_MINUS_60 && angle > ANGLE_MINUS_120 )
            {
                return DIRECTION.DOWN;
            }
            else if ( angle <= ANGLE_MINUS_120 && angle >= ANGLE_MINUS_180 )
            {
                return DIRECTION.LEFT_DOWN;
            }
            return DIRECTION.NOT_DEFINED;
        }

        public bool DropCandy(DIRECTION[] dirs, HexaBlastDelegate.GenericDelegate<Tile> callback)
        {
            if ( CurrentCandy == null )
            {
                return false;
            }

            Tile result = this;
            List<Tile> path = new List<Tile>();
            

            bool isEnd = false;
            while ( isEnd == false )
            {
                isEnd = true;
                for ( int d = 0; d < dirs.Length && isEnd == true; d ++) 
                {
                    if ( result.Neighborhoods[(int)dirs[d]] != null &&
                         result.Neighborhoods[(int)dirs[d]].CurrentCandy == null )
                    {
                        result = result.Neighborhoods[(int)dirs[d]];
                        path.Add(result);
                        isEnd = false;
                    }
                }
            }

            if ( result != this )
            {
                result.CurrentCandy = CurrentCandy;
                CurrentCandy.MoveCandyFollowingPath(path, () => {
                    callback(result);
                });
                CurrentCandy = null;
            }
            return ( result != this );
        }
    }
}