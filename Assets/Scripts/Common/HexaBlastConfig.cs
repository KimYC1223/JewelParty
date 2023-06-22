//#define DEBUG_MODE

namespace HexaBlast.Common
{
    public static class HexaBlastConfig
    {
#if DEBUG_MODE
        public const int CANDY_SPEED = 12;

        public const float CANDY_CREATE_DELAY_SECOND = 1.4f;
        public const float CANDY_DELETE_DELAY_SECOND = 2f;
        public const float TILE_MIN_DISTANCE = 60f;
#else
        public const int CANDY_SPEED = 15;

        public const float CANDY_CREATE_DELAY_SECOND = 0.1f;
        public const float CANDY_DELETE_DELAY_SECOND = 0.05f;
        public const float TILE_MIN_DISTANCE = 60f;
#endif
    }
}
