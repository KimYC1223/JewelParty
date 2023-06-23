//#define DEBUG_MODE

namespace MatchMatch.Common
{
    //=======================================================================================================================================
    //
    //  게임에 사용되는 여러가지 상수값을 미리 정의
    //
    //=======================================================================================================================================
    public static class MatchMatchConfig
    {
        #region STATIC_FIELD
#if !DEBUG_MODE
        public static readonly int BLOCK_SPEED = 15;                        // 한 프레임당 블럭이 움직이는 속도

        public static readonly float BLOCK_CREATE_DELAY_SECOND = 0.1f;      // 블럭을 하나 생성 한 후, 잠깐 기다리는 딜레이 (단위 : Sec)
        public static readonly float BLOCK_DELETE_DELAY_SECOND = 0.05f;     // 블럭을 하나 제거 한 후, 잠깐 기다리는 딜레이 (단위 : Sec)
        public static readonly float TILE_MIN_DISTANCE = 60f;               // 타일을 드래그했을 때, 드래그로 인정되는 드래그 길이
#else
        public static readonly int BLOCK_SPEED = 2;

        public static readonly float BLOCK_CREATE_DELAY_SECOND = 1.4f;
        public static readonly float BLOCK_DELETE_DELAY_SECOND = 2f;
        public static readonly float TILE_MIN_DISTANCE = 60f;
#endif
        #endregion
    }
}
