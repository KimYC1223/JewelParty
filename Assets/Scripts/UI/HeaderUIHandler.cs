using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MatchMatch.UI
{
    //===============================================================================================================================================
    //  헤더 UI를 컨트롤하는 핸들러
    //===============================================================================================================================================
    public class HeaderUIHandler : MonoBehaviour
    {
        #region UI_COMPONENT_FIELD
        public TMP_Text GoalValueText;
        public Animator GoalUIAnimator;
        public TMP_Text MoveValueText;
        public TMP_Text ScoreValueText;
        public Slider ScoreValueSlider;
        #endregion

        #region FIELD
        public int[] StarThreshold;
        public int Move = 0;
        public int Score = 0;
        public int Goal = 0;
        #endregion

        #region INSTANCE
        private static HeaderUIHandler _Instance;
        public static HeaderUIHandler Instance
        {
            get
            {
                if ( _Instance == null )
                {
                    _Instance = GameObject.Find("Header Layout").GetComponent<HeaderUIHandler>();
                }
                return _Instance;
            }

            private set { }
        }
        #endregion

        #region PUBLIC_METHOD
        //===========================================================================================================================================
        //
        //  Goal Value를 설정하는 메서드.
        //
        //===========================================================================================================================================
        /// <summary>
        /// Goal Value를 설정하는 메서드.
        /// </summary>
        /// <param name="value">이 값으로 설정</param>
        public void SetGoalValue(int value)
        {
            // Goal을 설정
            Goal = value;
            GoalValueText.text = value.ToString();

            //=======================================================================================================================================
            //  Goal == 0 이라면, 완료 애니메이션 재생
            //=======================================================================================================================================
            if ( Goal == 0 )
            {
                GoalUIAnimator.SetTrigger("Done");
            }
        }

        //===========================================================================================================================================
        //
        //  Goal Value를 하나 제거하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// Goal Value를 하나 제거하는 메서드.
        /// </summary>
        public void SubGoalValue()
        {
            Goal -= 1;
            SetGoalValue(Goal);
        }

        //===========================================================================================================================================
        //
        //  Move Value를 설정하는 메서드.
        //
        //===========================================================================================================================================
        /// <summary>
        /// Move Value를 설정하는 메서드.
        /// </summary>
        /// <param name="value">이 값으로 설정</param>
        public void SetMoveValue(int value)
        {
            Move = value;
            MoveValueText.text = value.ToString();
        }

        //===========================================================================================================================================
        //
        //  Move Value를 하나 제거하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// Move Value를 하나 제거하는 메서드.
        /// </summary>
        public void SubMoveValue()
        {
            Move -= 1;
            SetMoveValue(Move);
        }

        //===========================================================================================================================================
        //
        //  Score Value를 설정하는 메서드.
        //
        //===========================================================================================================================================
        /// <summary>
        /// Score Value를 설정하는 메서드.
        /// </summary>
        /// <param name="value">이 값으로 설정</param>
        public void SetScoreValue(int value)
        {
            Score = value;
            ScoreValueSlider.value = Score;
            ScoreValueText.text = Score.ToString();
        }

        //===========================================================================================================================================
        //
        //  Score Value에 값을 추가하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// Score Value에 값을 추가하는 메서드.
        /// </summary>
        public void AddScoreValue(int value)
        {
            Score += value;
            SetScoreValue(Score);
        }

        //===========================================================================================================================================
        //
        //  별의 개수를 결정하는 기준값을 설정하는 메서드
        //
        //===========================================================================================================================================
        /// <summary>
        /// 별의 개수를 결정하는 기준값을 설정하는 메서드.
        /// </summary>
        /// <param name="thresholds">이 배열로 기준값 설정</param>
        public void SetStarThreshold(int[] thresholds)
        {
            StarThreshold = thresholds;
            SetScoreMaxValue(StarThreshold[StarThreshold.Length - 1]);
        }
        #endregion

        #region PRIVATE_METHOD
        //===========================================================================================================================================
        //
        //  Score Slider의 최대 / 최소를 설정하는 메서드
        //
        //===========================================================================================================================================
        private void SetScoreMaxValue(int value)
        {
            // 최소는 0, 최대는 전달받은 값으로 설정.
            ScoreValueSlider.minValue = 0;
            ScoreValueSlider.maxValue = value;

            // 설정 후, Slider는 0으로 설정
            SetScoreValue(0);
        }
        #endregion
    }
}

