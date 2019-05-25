using FPSFramework.Movement;
using FPSFramework.Movement.CoordinateSpaces;
using FPSFramework.Springs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;
using UnityUtilities.ExecutionOrder.ExecutionOrderControl;
using UnityUtilities.Networking;

namespace FPSFramework.Movement
{
    [ExecutionOrder(ExecutionOrderValue.PostPhysics)]
    public class NetworkMovementController : NetBehaviour
    {
        public MovementController movementController;
        public PlatformController platformController;

        public Vector3 oldPosition;
        public Vector2 targetInput;

        SphericalSpring spring;

        bool disable = false;

        protected override void Init()
        {
            if(isOwner)
            {
                movementController = gameObject.GetComponent<MovementController>();
                platformController = gameObject.GetComponent<PlatformController>();
            }
            else
            {
                spring = SpringComponent.Create<SphericalSpring>("spring", null);
                spring.transform.parent = transform;
                spring.transform.localPosition = Vector3.zero;
                spring.dynamicStrength = 0.00000001f;


                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                DestroyImmediate(sphere.GetComponent<Collider>());
                sphere.transform.parent = spring.transform;
                sphere.transform.localPosition = Vector3.zero;

                movementController = gameObject.AddComponent<MovementController>();
                movementController.OnCoordinateSpaceDelta += spring.PropegateDelta;

                gameObject.AddComponent<SphereCollider>();

                platformController = gameObject.AddComponent<PlatformController>();
            }
            try
            {
                targetInput = movementController.targetInput;
            }
            catch(Exception e)
            {
                Debug.LogError("Movement Controller Error: " + owner + " : " + isOwner);
                Debug.LogError(e);
            }

            oldPosition = transform.position;
        }

        bool synced = false;

        protected override void RunByOwner()
        {
            //error check
            if(movementController == null)
            {
                return;
            }

            int platformID = -1;
            NetBehaviour platformBehaviour = platformController?.platform?.GetComponent<NetBehaviour>();
            if(platformBehaviour != null)
            {
                platformID = platformBehaviour.id;
            }

            if(Vector3.Distance(movementController.localPosition, oldPosition) > 0.01f || Vector2.Distance(movementController.targetInput, targetInput) > 0.01f)
            {
                BroadcastToSelfUdp(new NetworkMovementControllerSync(platformID, movementController.localPosition, movementController.localVelocity, movementController.targetInput, NetTime.time));
                oldPosition = movementController.localPosition;
                targetInput = movementController.targetInput;
                synced = false;
            }
            else
            {
                if(!synced)
                {
                    synced = true;
                    BroadcastToSelfUdp(new NetworkMovementControllerSync(platformID, movementController.localPosition, Vector3.zero, movementController.targetInput, NetTime.time));
                }
            }
        }

        protected override void RunByClient()
        {
            spring.Propegate(transform.position, transform.rotation, Time.deltaTime);
        }

        public override void AcceptEvent(CEvent e)
        {
            if(e is NetworkMovementControllerSync sync)
            {
                if(!isOwner)
                {
                    if(sync.platformID >= 0)
                    {
                        platformController.platform = GUIDPool.GetObject<Platform>(sync.platformID);
                    }
                    else
                    {
                        platformController.platform = null;
                    }

                    float dt = NetTime.time - sync.time;

                    Vector3 pos = sync.pos, v = sync.localVelocity;

                    if(dt > 1f)
                    {
                        //Debug.Log("Strange Delta: " + guid.ToString() + dt);
                        movementController.localPosition = pos;
                    }
                    else
                    {
                        movementController.localPosition = pos + v * dt;
                    }

                    movementController.localVelocity = v;
                    movementController.targetInput = sync.input;
                }
            }
        }

        public override NetBehaviourSpawn GetSpawn()
        {
            return new MovementSpawn(oldPosition, id);
        }

        [Serializable]
        public class MovementSpawn : NetBehaviourSpawn
        {
            public readonly Vector3Wrapper pos;

            public MovementSpawn(Vector3Wrapper pos, int id) : base(id)
            {
                this.pos = pos;
            }

            public override void Spawn()
            {
                GameObject player = new GameObject("Net Player");
                player.transform.position = pos;
                Create<NetworkMovementController>(player, networkClientNumber, new NetGUID(guid));
            }
        }

        [Serializable]
        public class NetworkMovementControllerSync : NetworkedCEvent
        {
            public readonly int platformID;
            public readonly Vector3Wrapper pos, localVelocity;
            public readonly Vector2Wrapper input;
            public readonly float time;

            public NetworkMovementControllerSync(int platformID, Vector3Wrapper pos, Vector3Wrapper localVelocity, Vector2Wrapper input, float time)
            {
                this.platformID = platformID;
                this.pos = pos;
                this.localVelocity = localVelocity;
                this.input = input;
                this.time = time;
            }
            public override string ToString()
            {
                return string.Format("Sync(id {4}, pos {0}, velocity {1}, input {2}, time {3})", pos, localVelocity, input, time, platformID);
            }
        }
    }
}