using HexaBlast.Common.VO;
using System.Collections;
using UnityEngine;

namespace HexaBlast.Common
{
    public class NetworkManager
    {
        public IEnumerator LoadGameLevel(string serverAddress, HexaBlastDelegate.LevelInfoDelegate callback)
        {
            LevelInfo result = new LevelInfo();

            // ============================================================================================================
            // TO-DO : ������ ����ؼ� �� Json �����͸� �޾ƿ��� ���
            // ============================================================================================================
            yield return 0;
            try
            {
                string jsonString = "{ \"LevelName\" : \"HexaBlast 21 Level\", " +
                                                        "\"Level\" : 21, " +
                                                        "\"Goal\" : 20, " +
                                                        "\"InitMove\" : 20, " +
                                                        "\"StarThreshold\" : [4500,26000,35000], " +
                                                        "\"PlayableTileNum\" : 30, " +
                                    "\"InitTileInfo\" : [0,7,7,7,1,7,5,1,0,5,3,4,3,1,4,1,1,3,7,3,0,5,4,0,7,3,7,7,7,7] }";
                result = JsonUtility.FromJson<LevelInfo>(jsonString);
                callback(result);
            } catch(System.Exception e)
            {
                Debug.LogError(e);
            }

        }
    }
}
