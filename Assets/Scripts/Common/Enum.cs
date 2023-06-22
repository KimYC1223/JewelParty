namespace HexaBlast.Common
{
    public enum DIRECTION
    {
        UP          = 0,
        RIGHT_UP    = 1,
        RIGHT_DOWN  = 2,
        DOWN        = 3,
        LEFT_DOWN   = 4,
        LEFT_UP     = 5,
        LENGTH      = 6,
        NOT_DEFINED = -1
    }

    public enum CANDY_TYPE
    {
        BLUE        = 0, 
        GREEN       = 1, 
        ORANGE      = 2, 
        PURPLE      = 3, 
        RED         = 4, 
        YELLOW      = 5, 
        ON          = 6, 
        OFF         = 7,
        SUPER_0     = 8,
        SUPER_1     = 9,
        SUPER_2     = 10
    };

    public enum MATCH_THRESHOLD
    {
        LINE_MATCH  = 3,
        GROUP_MATCH = 4
    };

    public enum SFX
    {
        POP,
        CREATE,
        CANT,
        DONT_MOVE,
        TADA,
        STAR,
        READY_GO,
        CLICK
    }

    public enum BGM
    {
        GAME,
        LOBBY
    }
}

