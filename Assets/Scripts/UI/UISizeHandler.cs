using System.Collections;
using UnityEngine;

namespace MatchMatch.UI {
    //===============================================================================================================================================
    //  UI 오브젝트의 크기를 Screen의 크기에 맞게 설정
    //===============================================================================================================================================
    public class UISizeHandler : MonoBehaviour 
    {
        #region FIELD
        public RectTransform Tiles;                     // 변경할 RectTransform
        public RectTransform ScoreLayout;               // 변경할 RectTransform
        public RectTransform MainLayout;                // 변경할 RectTransform
        public Vector2 ScreenSize = Vector2.zero;       // 스크린 사이즈
        public static UISizeHandler Instance;           // 인스턴스
        #endregion

        #region PRIVATE_METHOD
        //===========================================================================================================================================
        //
        //  Update 프레임마다 실행되는 메서드
        //
        //===========================================================================================================================================
        private void Update() 
        {
#if UNITY_EDITOR
            // Editor의 경우, 카메라의 pixelWidth를 가져옴
            ScreenSize.x = Camera.main.pixelWidth;
            ScreenSize.y = Camera.main.pixelHeight;
#else
            // 디바이스일 경우, Screen의 width를 가져옴
            ScreenSize.x  = Screen.width;
            ScreenSize.y = Screen.height;
#endif
            //=======================================================================================================================================
            //  Score Layout 크기 조절
            //=======================================================================================================================================
            // 현재 Tile Root의 가로 사이즈
            float scoreLayoutHeight = ScreenSize.x * 0.35f;

            // 스크린 사이즈의 35%, 400px 중 더 작은 값으로 설정
            float newScoreLayoutheight = scoreLayoutHeight < 250f ? scoreLayoutHeight : 250f;
            ScoreLayout.sizeDelta = new Vector2(0, newScoreLayoutheight);

            //=======================================================================================================================================
            //  Main Layout 크기 조절
            //=======================================================================================================================================
            // 위에서 구한 scoreLayoutHeight 만큼 띄우기
            MainLayout.offsetMin = new Vector2(0, 200f);
            MainLayout.offsetMax = new Vector2(0, -newScoreLayoutheight -25f);

            //=======================================================================================================================================
            //  Tiles 크기 조절
            //=======================================================================================================================================
            // 현재 Tile Root의 가로/세로 사이즈, mainLayout 세로 사이즈
            float gameFieldWidth   = Tiles.sizeDelta.x;
            float gameFieldHeight  = Tiles.sizeDelta.y;
            float mainLayoutHeight = ScreenSize.y - 225f -newScoreLayoutheight;

            // 가로/세로 사이즈중 더 작은걸 고르기
            float gameFieldSize   = ( ScreenSize.x < mainLayoutHeight ) ? gameFieldWidth : gameFieldHeight;
            float layoutSize      = ( ScreenSize.x < mainLayoutHeight ) ? ScreenSize.x : mainLayoutHeight;

            // Layout 사이즈의 90%만큼 차지하도록 변경 (Scale 조정)
            float ScaleFactor = ( layoutSize * 0.9f ) / gameFieldSize;
            Tiles.localScale = new Vector3(ScaleFactor, ScaleFactor, ScaleFactor);
        }

        //===========================================================================================================================================
        //
        //  Awake 라이프사이클에 실행되는 메서드
        //
        //===========================================================================================================================================
        public void Awake()
        {
            Instance = this;                // 인스턴스 변수 설정
        }
        #endregion
    }
}
