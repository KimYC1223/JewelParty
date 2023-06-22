using HexaBlast.Common.VO;

namespace HexaBlast.Common
{
     public class HexaBlastDelegate
    {
        public delegate void VoidDelegate();
        public delegate void LevelInfoDelegate(LevelInfo levelInfo);
        public delegate void GenericDelegate<T>(T t);
    }
}