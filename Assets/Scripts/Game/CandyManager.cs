using System.Collections;
using System.Collections.Generic;
using HexaBlast.Common;
using UnityEngine;

namespace HexaBlast.Game
{
    public class CandyManager : MonoBehaviour
    {
        public Sprite[] CandyIcons = new Sprite[8];
        public GameObject CandyPrefab;
        public Tile CreateCandyTile;
        public List<Candy> Candy;

        public static DIRECTION[] GetGravityDirections()
        {
            DIRECTION[] dirs = new DIRECTION[3];
            dirs[0] = DIRECTION.DOWN;
            if ( CandyMatcher.isLeftDropFirst == true )
            {
                CandyMatcher.isLeftDropFirst = false;
                dirs[1] = DIRECTION.LEFT_DOWN;
                dirs[2] = DIRECTION.RIGHT_DOWN;
            }
            else
            {
                CandyMatcher.isLeftDropFirst = true;
                dirs[1] = DIRECTION.RIGHT_DOWN;
                dirs[2] = DIRECTION.LEFT_DOWN;
            }

            return dirs;
        }

        public static DIRECTION[] GetStraightDirections()
        {
            DIRECTION[] dirs = new DIRECTION[1];
            dirs[0] = DIRECTION.DOWN;
            return dirs;
        }

        public void RequestCandy(int num, HexaBlastDelegate.GenericDelegate<Tile> callback)
        {
            StartCoroutine(CreateCandyCoroutine(num, callback));
        }

        private IEnumerator CreateCandyCoroutine(int num, HexaBlastDelegate.GenericDelegate<Tile> callback)
        {
            for(int i = 0 ; i < num ; i ++ )
            {
                CreateCandy();
                CreateCandyTile.DropCandy(GetGravityDirections() , callback);

                yield return new WaitForSeconds(HexaBlastConfig.CANDY_CREATE_DELAY_SECOND);
            }
        }

        private void Start()
        {
            SetCandyArray();
        }

        public void SetCandyArray()
        {
            Candy[] candy = this.GetComponentsInChildren<Candy>();
            Candy.Clear();
            foreach ( Candy c in candy )
            {
                Candy.Add(c);
            }
        }

        public void SetCandyInfo()
        {
            for ( int i = 0 ; i < GameManager.CurrentLevelInfo.PlayableTileNum ; i++ )
            {
                Candy[i].SetCandyType( (CANDY_TYPE) GameManager.CurrentLevelInfo.InitTileInfo[i], 
                                        CandyIcons[GameManager.CurrentLevelInfo.InitTileInfo[i]]);
                Candy[i].SetTile(GameManager.TileManagerInstance.Tiles[i]);
            }
        }

        public void CreateCandy()
        {
            SetCandyArray();
            GameObject go = Instantiate(CandyPrefab, this.transform);
            Candy createCandy = go.GetComponent<Candy>();

            int RandomIndex = Random.Range(0, 6);
            createCandy.SetCandyType((CANDY_TYPE)RandomIndex, CandyIcons[RandomIndex]);
            createCandy.SetTile(CreateCandyTile);
            Candy.Add(createCandy);
            CreateCandyTile.SetCandy(createCandy);

            GameManager.SoundManagerInstance.PlaySound(SFX.CREATE);
        }
    }
}
