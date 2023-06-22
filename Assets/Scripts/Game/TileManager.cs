using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexaBlast.Common;

namespace HexaBlast.Game
{
    public class TileManager : MonoBehaviour
    {
        public Tile[] Tiles = new Tile[] { };

        public void SetTileArray()
        {
            Tiles = transform.GetComponentsInChildren<Tile>();
            for(int id = 0 ; id < GameManager.CurrentLevelInfo.PlayableTileNum ; id ++)
            {
                Tiles[id].Id = id;
            }
        }

        public int GetEmptyTile()
        {
            int count = 0;

            if(Tiles.Length == 0)
            {
                SetTileArray();
            }

            for ( int id = 0 ; id < GameManager.CurrentLevelInfo.PlayableTileNum ; id++ )
            {
                if (Tiles[id].CurrentCandy == null )
                {
                    count++;
                }
            }

            return count;
        }

        public void StopAllAnimation()
        {
            for ( int i = 0 ; i < GameManager.CurrentLevelInfo.PlayableTileNum ; i++ )
            {
                Tiles[i].PlayResetAnimation();
            }
        }

        public void RefreshAllAnimation()
        {
            for ( int i = 0 ; i < GameManager.CurrentLevelInfo.PlayableTileNum ; i++ )
            {
                if ( Tiles[i].CurrentCandy == null || Tiles[i].CurrentCandy.Type != CANDY_TYPE.ON )
                {
                    Tiles[i].PlayResetAnimation();
                }
                else
                {
                    Tiles[i].PlayLightOnAnimation();
                }
            }
        }
    }
}
