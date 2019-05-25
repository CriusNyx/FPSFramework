using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtilities;
using UnityUtilities.Networking;

namespace FPSFramework.Movement
{
    public class Spinner : NetBehaviour
    {
        public Vector3 theta;

        protected override void RunByOwner()
        {
            Run();
        }

        protected override void RunByClient()
        {
            Run();
        }

        private void Run()
        {
            isOwner = NetworkServer.isServer;

            transform.rotation = Quaternion.Euler(theta * Time.deltaTime) * transform.rotation;

            if(UnityEngine.Random.value < 0.01f)
            {
                NetGUID.SendEventToServerUdp(guid.id, new SpinnerSync(NetTime.time, transform.rotation));
            }
        }

        [Serializable]
        public class SpinnerSync : NetworkedCEvent
        {
            public float time;
            public QuaternionWrapper rot;

            public SpinnerSync(float time, QuaternionWrapper rot)
            {
                this.time = time;
                this.rot = rot;
            }
        }

        public override void AcceptEvent(CEvent e)
        {
            if(!isOwner)
            {
                if(e is SpinnerSync sync)
                {
                    float dt = NetTime.time - sync.time;

                    transform.rotation = Quaternion.Euler(theta * dt) * sync.rot;
                }
            }
        }

        public override NetBehaviourSpawn GetSpawn()
        {
            //throw new NotImplementedException();
            return null;
        }

        //public void Update()
        //{
        //    //transform.Rotate(theta * Time.deltaTime);
        //    transform.rotation = Quaternion.Euler(theta * Time.deltaTime) * transform.rotation;
        //}
    }
}
