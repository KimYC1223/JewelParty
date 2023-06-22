using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HexaBlast.UI
{
    public class HeaderUIHandler : MonoBehaviour
    {
        public TMP_Text GoalValueText;
        public Animator GoalUIAnimator;
        public TMP_Text MoveValueText;
        public TMP_Text ScoreValueText;
        public Slider ScoreValueSlider;
        public int[] StarThreshold;

        public int Move = 0;
        public int Score = 0;
        public int Goal = 0;

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

        public void SetGoalValue(int value)
        {
            Goal = value;
            GoalValueText.text = value.ToString();
            if(Goal == 0 )
            {
                GoalUIAnimator.SetTrigger("Done");
            }
        }

        public void SubGoalValue()
        {
            Goal -= 1;
            SetGoalValue(Goal);
        }

        public void SetMoveValue(int value)
        {
            Move = value;
            MoveValueText.text = value.ToString();
        }

        public void SubMoveValue()
        {
            Move -= 1;
            SetMoveValue(Move);
        }
        public void SetScoreValue(int value)
        {
            Score = value;
            ScoreValueSlider.value = Score;
            ScoreValueText.text = Score.ToString();
        }
        public void AddScoreValue(int value)
        {
            Score += value;
            SetScoreValue(Score);
        }

        public void SetStarThreshold(int[] thresholds)
        {
            StarThreshold = thresholds;
            SetScoreMaxValue(StarThreshold[StarThreshold.Length - 1]);
        }



        private void SetScoreMaxValue(int value)
        {
            ScoreValueSlider.minValue = 0;
            ScoreValueSlider.maxValue = value;
            SetScoreValue(0);
        }
    }
}

