using FPSFramework.Movement.CoordinateSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtilities;
using UnityUtilities.Networking;

namespace FPSFramework.Demo.Ship
{
    public class Ship : NetBehaviour
    {
        public enum ShipChannel { shipChannel, shipSubchannel }

        Vector3 velocity;

        Vector3? target;
        Vector3? rotTarget;

        Platform platform;

        const float 
            MAX_SPEED = 80f,
            ACCELERATION = 10f,
            ROT_SPEED = 15f,
            STOP_DISTANCE = 60f;

        protected override void _Start()
        {
            CEventSystem.AddEventListener(ShipChannel.shipChannel, ShipChannel.shipSubchannel, this);

            platform = gameObject.AddComponent<Platform>();
        }

        protected override void _OnDestroy()
        {
            CEventSystem.RemoveEventListener(ShipChannel.shipChannel, ShipChannel.shipSubchannel, this);
        }

        protected override void RunByOwner()
        {
            Run(Time.deltaTime);
        }

        protected override void RunByClient()
        {
            Run(Time.deltaTime);
        }

        private void Run(float deltaTime)
        {
            isOwner = NetworkServer.isServer;
            if(isOwner)
            {
                owner = NetworkClient.ClientNumber;
            }
            else
            {
                owner = -1;
            }

            if (target != null)
            {
                Vector3 targetV = target.Value - transform.position;
                if (targetV.magnitude > MAX_SPEED) targetV = targetV.normalized * MAX_SPEED;
                velocity = Vector3.MoveTowards(velocity, targetV, deltaTime * ACCELERATION);

                transform.position += velocity * deltaTime;
                if (Vector3.Distance(transform.position, target.Value) < STOP_DISTANCE)
                {
                    target = null;
                }
            }
            else
            {
                velocity = Vector3.MoveTowards(velocity, Vector3.zero, ACCELERATION * 15f * deltaTime);
                transform.position += velocity * deltaTime;
            }
            if (rotTarget != null)
            {
                Quaternion rot = Quaternion.LookRotation(rotTarget.Value - transform.position);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, ROT_SPEED * deltaTime);
                if (target == null && Quaternion.Angle(transform.rotation, rot) < 0.1f)
                {
                    rotTarget = null;
                }
            }
        }

        public override void AcceptEvent(CEvent e)
        {
            if (e is TargetEvent te)
            {
                target = te.target;
                rotTarget = te.target;
            }
            if(e is Sync sync)
            {
                if(sync.networkClientNumber != NetworkClient.ClientNumber)
                {
                    float dt = NetTime.time - sync.time;

                    transform.position = sync.pos;
                    transform.rotation = sync.rot;
                    velocity = sync.velocity;

                    Run(dt);
                }
            }
        }

        public override NetBehaviourSpawn GetSpawn()
        {
            return null;
        }

        [Serializable]
        public class TargetEvent : NetworkedCEvent
        {
            public readonly Vector3Wrapper target;

            public TargetEvent(Vector3Wrapper target)
            {
                this.target = target;
            }

            public override string ToString()
            {
                return ("TargetEvent(" + target.ToString() + ")");
            }
        }

        [Serializable]
        public class Sync : NetworkedCEvent
        {
            public readonly float time;
            public readonly Vector3Wrapper pos, velocity;
            public readonly QuaternionWrapper rot;

            public Sync(float time, Vector3Wrapper pos, Vector3Wrapper velocity, QuaternionWrapper rot)
            {
                this.time = time;
                this.pos = pos;
                this.velocity = velocity;
                this.rot = rot;
            }
        }
    }
}