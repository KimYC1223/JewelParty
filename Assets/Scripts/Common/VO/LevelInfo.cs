using System;

namespace MatchMatch.Common.VO
{
    //===============================================================================================================================================
    //  MatchMatch 게임에서 사용하는 Level 정보의 VO
    //===============================================================================================================================================
    [Serializable]
    public class LevelInfo
    {
        public string LevelName;            // 이번 레벨의 이름
        public int Level;                   // 레벨의 ID
        public string GoalType;             // 목표 타입
        public int GoalNum_1;               // 목표 숫자 1
        public int GoalNum_2;               // 목표 숫자 2
        public int InitMove;                // 움직일 수 있는 횟수
        public int[] StarThreshold;         // Star 획득 Threshold
        public int PlayableTileNum;         // 플레이 가능한 타일 개수
        public int[] InitTileInfo;          // 초기 타일 정보
    }
}
