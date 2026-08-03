using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class CanvasRoot : MonoBehaviour
    {
        // ----- Param 
        public static CanvasRoot instance = null;

        [Header("UI Canvas Parents")] 
        public Transform trScreenParent;
        public Transform trHudParent;
        public Transform trHighLightParent;
        public Transform trPopupParent;

        [Header("UI Loading")]
        public GameObject objLoading;
        public TextMeshProUGUI txtGauge;
        private string loadingText;

        //  ----- Init
        private void Awake()
        {
            instance = this;
            loadingText = "Loading {0:0}%";
        }
        
        //  ----- Set
        public void SetLoadingIsOn(bool isOn) => objLoading.SetActive(isOn);

        public void SetLoadingGaugeText(float gauge)
        {
            txtGauge.text = string.Format(loadingText, gauge);
        }


    }
}