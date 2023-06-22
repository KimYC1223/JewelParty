using System.Collections;
using UnityEngine;

namespace HexaBlast.UI {
    //==========================================================================================================
    //  UI ������Ʈ�� ũ�⸦ Screen�� ũ�⿡ �°� ����
    //==========================================================================================================
    public class UISizeHandler : MonoBehaviour 
    {
        public RectTransform Tiles;                     // ������ RectTransform
        public RectTransform ScoreLayout;               // ������ RectTransform
        public RectTransform MainLayout;                // ������ RectTransform
        public Vector2 ScreenSize = Vector2.zero;       // ��ũ�� ������

        public static UISizeHandler Instance;

        public void Awake()
        {
            Instance = this;
        }

        public void Update() 
        {
#if UNITY_EDITOR
            // Editor�� ���, ī�޶��� pixelWidth�� ������
            ScreenSize.x = Camera.main.pixelWidth;
            ScreenSize.y = Camera.main.pixelHeight;
#else
            // ����̽��� ���, Screen�� width�� ������
            ScreenSize.x  = Screen.width;
            ScreenSize.y = Screen.height;
#endif
            //==================================================================================================
            //  Score Layout ũ�� ����
            //==================================================================================================
            // ���� Tile Root�� ���� ������
            float scoreLayoutHeight = ScreenSize.x * 0.35f;

            // ��ũ�� �������� 35%, 400px �� �� ���� ������ ����
            float newScoreLayoutheight = scoreLayoutHeight < 250f ? scoreLayoutHeight : 250f;
            ScoreLayout.sizeDelta = new Vector2(0, newScoreLayoutheight);

            //==================================================================================================
            //  Main Layout ũ�� ����
            //==================================================================================================
            // ������ ���� scoreLayoutHeight ��ŭ ����
            MainLayout.offsetMin = new Vector2(0, 200f);
            MainLayout.offsetMax = new Vector2(0, -newScoreLayoutheight -25f);

            //==================================================================================================
            //  Tiles ũ�� ����
            //==================================================================================================
            // ���� Tile Root�� ����/���� ������, mainLayout ���� ������
            float gameFieldWidth   = Tiles.sizeDelta.x;
            float gameFieldHeight  = Tiles.sizeDelta.y;
            float mainLayoutHeight = ScreenSize.y - 225f -newScoreLayoutheight;

            // ����/���� �������� �� ������ ����
            float gameFieldSize   = ( ScreenSize.x < mainLayoutHeight ) ? gameFieldWidth : gameFieldHeight;
            float layoutSize      = ( ScreenSize.x < mainLayoutHeight ) ? ScreenSize.x : mainLayoutHeight;

            // Layout �������� 90%��ŭ �����ϵ��� ���� (Scale ����)
            float ScaleFactor = ( layoutSize * 0.9f ) / gameFieldSize;
            Tiles.localScale = new Vector3(ScaleFactor, ScaleFactor, ScaleFactor);
        }
    }
}
