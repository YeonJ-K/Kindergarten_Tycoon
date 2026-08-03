using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YEONJI.Kindergarten
{
    public class GameStatusUI : UIBase, IPointerDownHandler, IPointerUpHandler
    {
        public override UIType UIType { get { return UIType.GameStatusUI; } }
        [SerializeField] private Image roundTimeBar; 
        [SerializeField] TextMeshProUGUI satisfactionTxt;
        [SerializeField] private GameObject satisfactionInfo;
        [SerializeField] TextMeshProUGUI stressTxt;
        
        public void Init(int satisfaction)
        {
            roundTimeBar.fillAmount = 0;
            satisfactionTxt.text = satisfaction.ToString();
            satisfactionInfo.SetActive(false);
            stressTxt.text = "0";
        }

        public void SetRoundTime(float roundTimer, float roundTime)
        {
            roundTimeBar.fillAmount = roundTimer / roundTime;
        }

        public void SettingStress(int stress)
        {
            stressTxt.text = stress.ToString();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            satisfactionInfo.SetActive(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            satisfactionInfo.SetActive(false);
        }
        
    }
}
