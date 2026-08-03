using System.Collections;
using TMPro;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class ResultUI : UIBase
    {
        public override UIType UIType { get { return UIType.ResultUI; } }
        [SerializeField] private TextMeshProUGUI mainResultTxt;
        private string mainText = "오늘 하루도 무사히 지나갔다...";
        [SerializeField] private TextMeshProUGUI dayTxt;
        [SerializeField] private TextMeshProUGUI kidsNumTxt;
        [SerializeField] private TextMeshProUGUI careFeeTxt;
        [SerializeField] private TextMeshProUGUI miniGameBouseTxt;
        [SerializeField] private TextMeshProUGUI satisfactionTxt;
        [SerializeField] private TextMeshProUGUI minusForMasterTxt;
        [SerializeField] private TextMeshProUGUI penaltyTxt;
        [SerializeField] private TextMeshProUGUI totalEarnTxt;

        private float speed = 100f;

        IEnumerator _typeEffect(TextMeshProUGUI TMPROtext, string text)
        {
            yield return new WaitForSeconds(2f);
            for (int i = 0; i <= text.Length - 1; i++)
            {
                TMPROtext.text = text.Substring(0, i);
                yield return new WaitForSeconds(0.15f);
            }
        }

        IEnumerator _numCountEffect(TextMeshProUGUI TMPROtext, float fTargetNum)
        {
            float fCount = 0;
            while (fCount <= fTargetNum)
            {
                fCount += Time.deltaTime * speed;
                TMPROtext.text = fCount.ToString("F0");
                yield return null;
            }
            TMPROtext.text = fTargetNum.ToString("F0");
        }

    }
}
