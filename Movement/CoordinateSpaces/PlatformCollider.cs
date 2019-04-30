using UnityEngine;
using System.Collections;

namespace GameEngine.Movement
{
    [ExecutionOrder(ExecutionOrderValue.PostLogic)]
    public class PlatformCollider : MonoBehaviour
    {
        public Platform platform { get; private set; }

        public static PlatformCollider Create(GameObject gameObject, Platform platform)
        {
            return gameObject.AddComponent<PlatformCollider>().Init(platform);
        }

        private PlatformCollider Init(Platform platform)
        {
            this.platform = platform;
            return this;
        }
    }
}