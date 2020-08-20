using UnityEngine;
using UnityEngine.UI;

namespace Mirror
{
    public class ConnectionStatus : MonoBehaviour
    {

        public Slider slider;
        [SerializeField] int TimeToCheckForPing = 40;

        GUIStyle style;

        void Start()
        {
            InvokeRepeating("ApplyConnection", 3, TimeToCheckForPing);
        }


        void ApplyConnection()
        {
            int connectin = (int)(NetworkTime.rtt * 1000);
            if (connectin <= 120)
            {
                slider.value = 4;
            }
            else if (connectin >= 150)
            {
                slider.value = 3;
            }
            else if (connectin >= 350)
            {
                slider.value = 2;
            }
            else if (connectin > 450)
            {
                slider.value = 1;
            }
        }
    }
}
