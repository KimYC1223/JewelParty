using System;

namespace HexaBlast.Common.VO
{
    [Serializable]
    public class LevelInfo
    {
        public string LevelName;
        public int Level;
        public int Goal;
        public int InitMove;
        public int[] StarThreshold;
        public int PlayableTileNum;
        public int[] InitTileInfo;
    }
}
