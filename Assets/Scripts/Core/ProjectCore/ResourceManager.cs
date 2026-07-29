using Cysharp.Threading.Tasks;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class ResourceManager : BaseManager
    {
        public override async UniTask Task_Init()
        {
            await base.Task_Init();
        }

        public override void Init()
        {
            base.Init();
        }
        
        public TextAsset LoadText(string path) => Resources.Load<TextAsset>(path);
    }
}