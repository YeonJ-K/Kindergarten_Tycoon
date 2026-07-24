using UnityEngine;
using Cysharp.Threading.Tasks;

namespace YEONJI.Kindergarten
{
    public class BaseManager : MonoBehaviour
    {
        protected bool isReady = false;

        public virtual async UniTask Task_Init()
        {
            await UniTask.Yield();
            isReady = true;
        }

        public virtual void Init()
        {
            // 로깅
        }

    }
}