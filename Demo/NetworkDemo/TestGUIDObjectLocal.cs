using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtilities;
using UnityUtilities.Networking;
using static Assets.FPSFramework.Demo.NetworkDemo.TestGUIDSpawner;

namespace Assets.FPSFramework.Demo.NetworkDemo
{
    public class TestGUIDObjectLocal : NetBehaviour
    {
        public int num;

        Vector3 posLastFrame;

        protected override void Init()
        {
            CEventSystem.Broadcast(ServerEventChannel.channel, ServerEventChannel.subchannel, new SpawnRemoveEvent(GUID, transform.position));
            this.num = guid.id;
            posLastFrame = transform.position;
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position, posLastFrame) > 0.1f)
            {
                NetworkClient.SendObjectToServerTCP(NetworkControlCode.runOnClient, new GUIDBroadcast(GUID, new GUIDPositionEvent(transform.position)));
                posLastFrame = transform.position;
            }
        }

        public override void AcceptEvent(CEvent e)
        {

        }

        public override NetBehaviourSpawn GetSpawn()
        {
            //throw new NotImplementedException();
            return null;
        }

        [Serializable]
        public class GUIDPositionEvent : CEvent
        {
            public readonly Vector3Wrapper v;

            public GUIDPositionEvent(Vector3Wrapper v)
            {
                this.v = v;
            }
        }
    }
}
