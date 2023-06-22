using UnityEngine;

namespace HexaBlast.Game
{
    public class FPSController : MonoBehaviour
    {
        public void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}