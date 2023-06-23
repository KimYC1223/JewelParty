namespace MatchMatch.Common.Const
{
    //===============================================================================================================================================
    //  레벨에서 사용하는 목표
    //===============================================================================================================================================
    public static class GoalType
    {
        public static readonly string BLOCK_COUNT            = "BLOCK_COUNT";               // 블럭 지우기
        public static readonly string GOAL_BLOCK_COUNT       = "GOAL_BLOCK_COUNT";          // Goal 블럭 개수 지우기
        public static readonly string COMBO_COUNT            = "COMBO_COUNT";               // 콤보 쌓기 (누적)
        public static readonly string COMBO_COUNT_ONCE       = "COMBO_COUNT_ONCE";          // 콤보 쌓기 (한번에)
        public static readonly string TIME_ATTACK            = "TIME_ATTACK";               // 타임 어택
        public static readonly string COMBO_BLOCK_COUNT      = "COMBO_BLOCK_COUNT";         // 콤보 블럭 개수 (누적)
        public static readonly string COMBO_BLOCK_COUNT_ONCE = "COMBO_BLOCK_COUNT";         // 콤보 블럭 개수 (한번에)
        public static readonly string SPECIFIC_BLOCK_COUNT   = "SPECIFIC_BLOCK_COUNT";      // 특정 블럭 제거
    }
}