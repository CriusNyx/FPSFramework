using FPSFramework.Movement.CoordinateSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtilities;

namespace FPSFramework.Demo.Ship
{
    public class Ship : MonoBehaviour, CEventListener
    {
        public enum ShipChannel { channel, subchannel }

        Vector3 velocity;

        Vector3? target;
        Vector3? rotTarget;

        const float MAX_SPEED = 80f;
        const float ACCELERATION = 10f;
        const float ROT_SPEED = 15f;
        const float STOP_DISTANCE = 60f;

        public void Start()
        {
            CEventSystem.AddEventListener(ShipChannel.channel, ShipChannel.subchannel, this);

            var plat = gameObject.AddComponent<Platform>();
        }

        void OnDestroy()
        {
            CEventSystem.RemoveEventListener(ShipChannel.channel, ShipChannel.subchannel, this);
        }

        public void Update()
        {
            if (target != null)
            {
                Vector3 targetV = target.Value - transform.position;
                if (targetV.magnitude > MAX_SPEED) targetV = targetV.normalized * MAX_SPEED;
                velocity = Vector3.MoveTowards(velocity, targetV, Time.deltaTime * ACCELERATION);

                transform.position += velocity * Time.deltaTime;
                if (Vector3.Distance(transform.position, target.Value) < STOP_DISTANCE)
                {
                    target = null;
                }
            }
            else
            {
                velocity = Vector3.MoveTowards(velocity, Vector3.zero, ACCELERATION * 15f * Time.deltaTime);
                transform.position += velocity * Time.deltaTime;
            }
            if (rotTarget != null)
            {
                Quaternion rot = Quaternion.LookRotation(transform.position - rotTarget.Value);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, ROT_SPEED * Time.deltaTime);
                if (target == null && Quaternion.Angle(transform.rotation, rot) < 0.1f)
                {
                    rotTarget = null;
                }
            }
        }

        public void AcceptEvent(CEvent e)
        {
            if (e is TargetEvent te)
            {
                target = te.target;
                rotTarget = te.target;
            }
        }

        public class TargetEvent : CEvent
        {
            public readonly Vector3 target;

            public TargetEvent(Vector3 target)
            {
                this.target = target;
            }
        }
    }
}