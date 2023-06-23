using MatchMatch.Common.VO;
using System.Collections;
using UnityEngine;

namespace MatchMatch.Common
{
    //===============================================================================================================================================
    //  서버 또는 로컬 파일과 통신하기 위한 네트워크 매니저
    //===============================================================================================================================================
    public class NetworkManager
    {
        #region PUBLIC_ENUMERATOR
        //===========================================================================================================================================
        //
        //  게임 레벨 정보를 받아올 수 있는 Enumerator
        //
        //===========================================================================================================================================
        /// <summary>
        /// 게임 레벨 정보를 받아올 수 있는 Enumerator.<br />
        /// 작업 완료 후 <see cref="LevelInfo"/> 콜백을 실행함.<br />
        /// </summary>
        /// <param name="serverAddress">받아올 서버 주소</param>
        /// <param name="callback">받아온 후 실행시킬 callback</param>
        public IEnumerator LoadGameLevel(string serverAddress, MatchMatchDelegate.GenericDelegate<LevelInfo> callback)
        {
            LevelInfo result = new LevelInfo();

            // ============================================================================================================
            // TO-DO : 서버와 통신해서 맵 Json 데이터를 받아오는 기능
            // ============================================================================================================
            yield return 0;
            try
            {
                string jsonString = "{ \"LevelName\" : \"MatchMatch 21 Level\", " +
                                                        "\"Level\" : 21, " +
                                                        "\"GoalType\" : \"GOAL_BLOCK_COUNT\", " +
                                                        "\"GoalNum_1\" : 10, " +
                                                        "\"GoalNum_2\" : 0, " +
                                                        "\"InitMove\" : 20, " +
                                                        "\"StarThreshold\" : [4500,26000,35000], " +
                                                        "\"PlayableTileNum\" : 30, " +
                                    "\"InitTileInfo\" : [0,7,7,7,1,7,5,1,0,5,3,4,3,1,4,1,1,3,7,3,0,5,4,0,7,3,7,7,7,7] }";

                // Json string 파싱
                result = JsonUtility.FromJson<LevelInfo>(jsonString);
                callback(result);
            } catch(System.Exception e)
            {
                Debug.LogError(e);
            }

        }
        #endregion
    }
}
