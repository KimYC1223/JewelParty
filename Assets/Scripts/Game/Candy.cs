using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using HexaBlast.UI;
using HexaBlast.Common;
using System.Collections.Generic;

namespace HexaBlast.Game
{
    public class Candy : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public CANDY_TYPE Type;
        public Tile CurrentTile;

        private float moveSpeed = HexaBlastConfig.CANDY_SPEED;
        private Coroutine moveCoroutine = null;
        private RectTransform rect;
        private Image icon;

        public void SetCandyType(CANDY_TYPE type, Sprite icon)
        {
            this.Type = type;
            this.icon.sprite = icon;
        }


        public void LightOn()
        {
            if ( Type != CANDY_TYPE.OFF )
            {
                return;
            }
            Type = CANDY_TYPE.ON;
            icon.sprite = GameManager.CandyManagerInstance.CandyIcons[(int)CANDY_TYPE.ON];
        }

        public void SetTile(Tile target)
        {
            this.CurrentTile = target;
            target.CurrentCandy = this;
        }

        public void Awake()
        {
            rect = this.GetComponent<RectTransform>();
            icon = this.GetComponent<Image>();
        }

        private void OverrideCoroutine(Coroutine coroutine)
        {
            if ( moveCoroutine != null )
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }
            moveCoroutine = coroutine;
        }

        public void MoveCandyFollowingPath(List<Tile> path, HexaBlastDelegate.VoidDelegate callback)
        {
            OverrideCoroutine(StartCoroutine(MoveCandyFollowingPathyCoroutine(path, callback)));
        }

        public void MoveCandy(Tile destination, HexaBlastDelegate.VoidDelegate callback)
        {
            OverrideCoroutine(StartCoroutine(MoveCandyCoroutine(destination, callback)));
        }

        public IEnumerator MoveCandyCoroutine(Tile destination, HexaBlastDelegate.VoidDelegate callback)
        {
            CurrentTile.SetTilePosition();
            destination.SetTilePosition();
            Vector2 dir = ( destination.Pos - ( rect.anchoredPosition * CurrentTile.ZoomAmount ) ).normalized;
            
            float distance = ( destination.Pos - ( rect.anchoredPosition * CurrentTile.ZoomAmount) ).magnitude;
            float old_distance = distance;

            while ( distance >= moveSpeed )
            {
                rect.anchoredPosition = rect.anchoredPosition + ( dir * moveSpeed );
                old_distance = distance;
                distance = ( destination.Pos - ( rect.anchoredPosition * CurrentTile.ZoomAmount ) ).magnitude;

                if(old_distance <= distance)
                {
                    break;
                }
                yield return 0;
            }
            rect.anchoredPosition = destination.Pos / CurrentTile.ZoomAmount;
            yield return 0;

            moveCoroutine = null;
            if ( callback != null )
            {
                SetTile(destination);
                callback();
            }
        }

        public IEnumerator MoveCandyFollowingPathyCoroutine(List<Tile> path, HexaBlastDelegate.VoidDelegate callback)
        {
            for(int i = 0 ; i < path.Count ; i++ )
            {
                if( i < (path.Count - 1) )
                {
                    yield return StartCoroutine(MoveCandyCoroutine(path[i], null));
                }
                else
                {
                    yield return StartCoroutine(MoveCandyCoroutine(path[i], callback));
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            CurrentTile.SetTilePosition();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if ( GameManager.IsInteractable == false )
            {
                return;
            }

            if ( HeaderUIHandler.Instance.Move <= 0 )
            {
                GameManager.SoundManagerInstance.PlaySound(SFX.CANT);
                return;
            }

            GameManager.IsInteractable = false;

            Vector2 mousePos = getMousePointerToUIPos(eventData.position);
            DIRECTION dir = CurrentTile.TargetingNeighborhoods(mousePos);

            if ( dir == DIRECTION.NOT_DEFINED || CheckNextTileEmpty(dir) )
            {
                GameManager.IsInteractable = true;
                return;
            }
            CurrentTile.SwapNeighborhood(dir);
        }

        private bool CheckNextTileEmpty (DIRECTION dir)
        {
            return ( CurrentTile.Neighborhoods[(int)dir] == null || 
                     CurrentTile.Neighborhoods[(int)dir].CurrentCandy == null );
        }

        private Vector2 getMousePointerToUIPos(Vector2 mousePos)
        {
            Vector2 Offset = UISizeHandler.Instance.ScreenSize;
            Offset.x /= 2;
            Offset.y -= ( 25f + ( UISizeHandler.Instance.MainLayout.rect.height / 2 )
                              + UISizeHandler.Instance.ScoreLayout.sizeDelta.y );
            mousePos -= Offset;
            return mousePos;
        }

        public int ReturnScore()
        {
            if(Type == CANDY_TYPE.ON)
            {
                HeaderUIHandler.Instance.SubGoalValue();
                return ScoreValue.TOP;
            }
            else if ( Type == CANDY_TYPE.SUPER_0 || Type == CANDY_TYPE.SUPER_1 || Type == CANDY_TYPE.SUPER_2 )
            {
                return ScoreValue.SUPER_CANDY;
            }
            else
            {
                return ScoreValue.CANDY;
            }
        }
    }
}
