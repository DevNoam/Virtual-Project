using UnityEngine;
using UnityEngine.UI;

namespace Mirror
{
    public class ConnectionStatus : MonoBehaviour
    {

        public Slider slider;
        [SerializeField] int TimeToCheckForPing = 20;

        GUIStyle style;

        void Start()
        {
            InvokeRepeating("ApplyConnection", 0, TimeToCheckForPing);
        }


        void ApplyConnection()
        {
            int connectin = (int)(NetworkTime.rtt * 1000);
            if (connectin <= 100)
            {
                slider.value = 4;
            }
            else if (connectin >= 150)
            {
                slider.value = 3;
            }
            else if (connectin >= 250)
            {
                slider.value = 2;
            }
            else if (connectin > 350)
            {
                slider.value = 1;
            }
        }
    }
}
