using MatchMatch.Common;
using MatchMatch.Common.Const;

namespace MatchMatch.Game
{
    //===============================================================================================================================================
    //  목표를 달성하기 위한 블록 (2회 부셔야 함)
    //===============================================================================================================================================
    class GoalBlock : IBlockFunction
    {
        #region PUBLIC_METHOD
        //===========================================================================================================================================
        // 매칭이 가능한 블록인지 판단하는 메서드
        //===========================================================================================================================================
        /// <summary>
        /// 이 블럭이 매칭 연산이 가능한 블럭인지 아닌지 반환.
        /// </summary>
        /// <param name="CurrentBlock">결과를 저장할 블럭</param>
        public void SetMatchableBlock(Block CurrentBlock)
        {
            CurrentBlock.IsMatchable = false;
        }

        //===========================================================================================================================================
        // 주변에서 블럭이 터질 때 실행되는 메서드
        //===========================================================================================================================================
        /// <summary>
        /// 주변에서 블럭이 터질 때 실행되는 메서드.
        /// </summary>
        /// <param name="CurrentBlock">이 메서드를 실행하는 블럭</param>
        /// <returns>이 블럭또한 같이 터지는지 아닌지 반환</returns>
        public bool AdditionalEffect(Block CurrentBlock)
        {
            if (CurrentBlock.Type == BLOCK_TYPE.OFF )
            {
                CurrentBlock.Type = BLOCK_TYPE.ON;
                CurrentBlock.SetBlockIcon(CurrentBlock.Type);
                return false;
            }
            else if ( CurrentBlock.Type == BLOCK_TYPE.ON )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //===========================================================================================================================================
        // 블럭을 클릭했을 때 실행되는 메서드
        //===========================================================================================================================================
        /// <summary>
        /// 블럭을 클릭했을 때 실행되는 메서드.
        /// </summary>
        /// <param name="CurrentBlock">이 메서드를 실행하는 블럭</param>
        /// <returns>이 블럭이 터지는지 아닌지 반환</returns>
        public bool OnBlockClicked(Block CurrentBlock)
        {
            return false;
        }

        //===========================================================================================================================================
        // 터졌을 때 얻을 수 있는 점수를 리턴하는 메서드
        //===========================================================================================================================================
        /// <summary>
        /// 터졌을 때 얻을 수 있는 점수를 리턴하는 메서드.<br />
        /// <see cref="BlockScore"/>에서 정한 값을 사용합니다.<br />
        /// </summary>
        /// <param name="CurrentBlock">이 메서드를 실행하는 블럭</param>
        /// <returns>이 블럭이 터졌을 때 얻을 수 있는 점수</returns>
        public int ReturnScore(Block CurrentBlock)
        {
            if(CurrentBlock.Type == BLOCK_TYPE.ON)
            {
                MatchMatch.UI.HeaderUIHandler.Instance.SubGoalValue();
                return BlockScoreValue.ON;
            }
            else
            {
                return BlockScoreValue.ZERO;
            }
        }
        #endregion
    }
}
