using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtilities;
using UnityUtilities.Networking;

namespace Assets.FPSFramework.Demo.NetworkDemo
{
    public class TestGUIDSpawner : MonoBehaviour, CEventListener
    {
        void Awake()
        {
            CEventSystem.AddEventListener(ServerEventChannel.channel, ServerEventChannel.subchannel, this);
        }

        public void AcceptEvent(CEvent e)
        {
            //if (e is SpawnRemoveEvent se)
            //{
            //    NetBehaviour net = GUIDPool.GetBehaviour(se.guid);
            //    if (net == null)
            //    {
            //        net = NetBehaviour.Create<TestGUIDObjectRemote>(new GameObject("remote"), , new NetGUID(se.guid));
            //        net.transform.position = se.position;
            //    }
            //}
        }

        [System.Serializable]
        public class SpawnRemoveEvent : NetworkedCEvent
        {
            public readonly int guid;
            public readonly Vector3Wrapper position;

            public SpawnRemoveEvent(int guid, Vector3 position)
            {
                this.guid = guid;
                this.position = position;
            }
        }
    }
}
