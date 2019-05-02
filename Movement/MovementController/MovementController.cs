using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Movement
{
    /// <summary>
    /// A movement controller with the following features
    /// 
    /// Move smothely up and down slopes / stairs
    /// Can snap to different coordiante spaces
    /// </summary>
    [ExecutionOrder(ExecutionOrderValue.Physics)]
    [RequireComponent(typeof(Rigidbody))]
    public class MovementController : MonoBehaviour, CEventListener
    {
        /// <summary>
        /// Creates a physics matrial with no fiction for colliders
        /// </summary>
        public static Thunk<PhysicMaterial> physicsMaterial = new Thunk<PhysicMaterial>(
            () => new PhysicMaterial("No Friction") { frictionCombine = PhysicMaterialCombine.Minimum, staticFriction = 0f, dynamicFriction = 0f }
            );

        #region Space
        /// <summary>
        /// Maps coordinate space to world space
        /// </summary>
        public Matrix4x4 coordinateSpaceToWorldSpace { get; private set; } = Matrix4x4.identity;

        public Matrix4x4 worldSpaceToCoordinateSpace { get; private set; } = Matrix4x4.identity;

        public Quaternion inputRotation = Quaternion.identity;

        /// <summary>
        /// Called on coordinate space changes.
        /// Signatur Call(Matrix4x4 space)
        /// </summary>
        public Action<Matrix4x4> OnCoordinateSpaceDelta, OnNewCoordinateSpace, OnSetSpace;

        public Vector3 localUp { get; private set; } = Vector3.up;

        //private Vector3? localUpCache;

        /// <summary>
        /// Applys a change in the coordinate space.
        /// This will conserve the local velocity for the component, changing the global velocity
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="oldSpace"></param>
        /// <param name="newSpace"></param>
        public void ApplyDeltaSpace(Matrix4x4 positionThisFrame, Matrix4x4 diff)
        {
            //transform.position.DrawCross(0.1f, 0.1f);

            Vector4 pos = transform.position;
            pos.w = 1f;
            pos = diff * pos;
            transform.position = pos;

            //transform.position.DrawCross(0.1f, Color.red, 0.1f);

            ConserveLocalVelocity(diff);

            OnCoordinateSpaceDelta?.Invoke(diff);

            SetSpace(positionThisFrame);
        }

        /// <summary>
        /// Sets a new coordinate space.
        /// This will conserve the world velocity for the component, changing the local velocity
        /// </summary>
        /// <param name="space"></param>
        public void ApplySpace(Matrix4x4 space)
        {
            ConserveWorldVelocity(space);

            OnNewCoordinateSpace?.Invoke(space);

            SetSpace(space);
        }

        /// <summary>
        /// Set a new coordinate space
        /// </summary>
        /// <param name="space"></param>
        private void SetSpace(Matrix4x4 space)
        {
            SetSpace(space, space.inverse);
            OnSetSpace?.Invoke(space);
        }

        /// <summary>
        /// Set a new coordinate space
        /// </summary>
        /// <param name="space"></param>
        /// <param name="inverse"></param>
        private void SetSpace(Matrix4x4 space, Matrix4x4 inverse)
        {
            this.coordinateSpaceToWorldSpace = space;
            localUp = coordinateSpaceToWorldSpace.MultiplyVector(Vector3.up);
            this.worldSpaceToCoordinateSpace = inverse;
        }

        /// <summary>
        /// Conserve the characters local velocity after modifying coordinate space
        /// </summary>
        /// <param name="delta"></param>
        private void ConserveLocalVelocity(Matrix4x4 diff)
        {
            Vector3 v = velocity;

            v = diff.MultiplyVector(v);

            velocity = v;
        }

        /// <summary>
        /// Conserve the characters world velocity after modifying coordinate space
        /// </summary>
        /// <param name="newCoordinateSpaceToWorldSpace"></param>
        private void ConserveWorldVelocity(Matrix4x4 newCoordinateSpaceToWorldSpace)
        {
            //Matrix4x4 newSpaceInverse = newCoordinateSpaceToWorldSpace.inverse;
        }
        #endregion

        #region Physics State
        /// <summary>
        /// The controller will attempt (in normalized space, relitive to maxSpeed) to match this velocity
        /// This vector is in 2D coordinate space
        /// </summary>
        public Vector2 targetInput = Vector2.zero;

        /// <summary>
        /// The velocity, in world space, that this controller will attempt to match.
        /// This vector is in 2D coordinate space
        /// </summary>
        public Vector2 targetVelocity
        {
            get
            {
                return targetInput * stats.maxSpeed;
            }
        }

        public Vector3 targetVelocityInWorldSpace
        {
            get
            {
                Vector2 cache = targetVelocity;
                return coordinateSpaceToWorldSpace.MultiplyVector(new Vector3(cache.x, 0f, cache.y));
            }
        }

        /// <summary>
        /// The current velocity of this controller in coordinate space
        /// </summary>
        //public Vector3 localVelocity { get; private set; } = Vector3.zero;

        /// <summary>
        /// The current velocity of this controller in world space
        /// </summary>
        public Vector3 velocity
        {
            get
            {
                return rigidbody.velocity;
            }
            set
            {
                rigidbody.velocity = value;
            }
        }

        #endregion

        #region Physics Defaults
        public const float DEFAULT_MAX_SPEED = 10F, DEFAULT_GRAVITY = -30F, DEFAULT_ACCELERATION_RATIO = 4F, DEFAULT_AIR_ACCELERATION_RATIO = 1f;
        public const int GROUND_MAX_FREEZE_FRAMES = 4;
        public static readonly Stats DEFAULT_PLAYER_STATS = new Stats(DEFAULT_MAX_SPEED, DEFAULT_GRAVITY, DEFAULT_ACCELERATION_RATIO, DEFAULT_AIR_ACCELERATION_RATIO, GROUND_MAX_FREEZE_FRAMES);
        #endregion

        #region Physics Properties
        public struct Stats
        {
            public float maxSpeed, gravity, accelerationRatio, airAccelerationRatio;
            public int groundMaxFreezeFrames;

            public Stats(float maxSpeed, float gravity, float accelerationRatio, float airAccelerationRatio, int groundMaxFreezeFrames)
            {
                this.maxSpeed = maxSpeed;
                this.gravity = gravity;
                this.accelerationRatio = accelerationRatio;
                this.airAccelerationRatio = airAccelerationRatio;
                this.groundMaxFreezeFrames = groundMaxFreezeFrames;
            }
        }

        public Stats stats = DEFAULT_PLAYER_STATS;
        public float acceleration
        {
            get
            {
                return stats.maxSpeed * stats.accelerationRatio;
            }
            set
            {
                stats.accelerationRatio = value / stats.maxSpeed;
            }
        }
        #endregion

        #region Ground Snapping
        /// <summary>
        /// The stats used to cast for ground collisions
        /// </summary>
        public GroundCollisionStats groundCollisionStats = new GroundCollisionStats() { layerMask = -1, groundedRaycastBonus = 0.1f, hoverDistance = 2f, crouchHoverDistance = 1f, crouch = false };

        bool groundedLastFrame = false;

        int groundIgnoreFrames = 0;

        /// <summary>
        /// Get the ground collision
        /// </summary>
        /// <returns></returns>
        public GroundCollision GetGround()
        {
            if (groundIgnoreFrames > 0)
            {
                groundIgnoreFrames--;
                return new GroundCollision(false, Vector3.zero, Vector3.zero, null);
            }
            return GroundCollision.GetCollision(groundCollisionStats, transform.position, coordinateSpaceToWorldSpace.MultiplyVector(Vector3.down), true);
        }

        

        /// <summary>
        /// Switch the collider to air mode
        /// </summary>
        public void SwitchToAir()
        {
            groundIgnoreFrames = stats.groundMaxFreezeFrames;
        }

        /// <summary>
        /// Switch the collider to ground mode
        /// </summary>
        public void SwitchToGround() => FlattenV();

        /// <summary>
        /// Flatten the velocity along the up axis
        /// </summary>
        public void FlattenV()
        {
            //Vector3 localUp = coordinateSpaceToWorldSpace.MultiplyVector(Vector3.up);
            (Vector3 localVelocity, float yVelocity) = GetFlatVelocity(velocity, localUp, false);
            velocity = localVelocity;
        }

        /// <summary>
        /// Adds a simple inpulse to the velocity
        /// </summary>
        /// <param name="deltaV"></param>
        public void AddInpulse(Vector3 deltaV)
        {
            Vector3 v = velocity;
            v += deltaV;

            //Vector3 localUp = coordinateSpaceToWorldSpace.MultiplyVector(Vector3.up);
            if (Vector3.Dot(v, localUp) > 0f)
            {
                SwitchToAir();
            }

            velocity = v;
        }

        /// <summary>
        /// Flattens the velocity along the axis of the inpulse, and then applies the inpulse to it
        /// </summary>
        /// <param name="deltaV"></param>
        public void ProjectToInpulse(Vector3 deltaV)
        {
            Vector3 temp = deltaV.normalized;

            Vector3 flattenedVelocity = velocity - Vector3.Dot(temp, velocity) * temp;

            flattenedVelocity += deltaV;

            velocity = flattenedVelocity;

            //Vector3 localUp = coordinateSpaceToWorldSpace.MultiplyVector(Vector3.up);

            if (Vector3.Dot(deltaV, localUp) > 0f)
            {
                SwitchToAir();
            }
        }


        #endregion

        #region MonoBehaviour
        new Rigidbody rigidbody;

        /// <summary>
        /// OnGroundCollision(GroundCollision groundCollision);
        /// </summary>
        public Action<GroundCollision> OnGroundCollision = null;

        private void Awake()
        {
            InitRigidbody();
            if (!Physics.autoSyncTransforms)
            {
                Debug.LogError("For this to function as expected, ProjectSettings/Physics/AutoSyncTransforms must be enabled.");
            }
        }

        /// <summary>
        /// Initialize physics components for object
        /// </summary>
        private void InitRigidbody()
        {
            //get the rigidbody
            rigidbody = GetComponent<Rigidbody>();

            //disable gravity and rotation
            rigidbody.useGravity = false;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            //apply no friction material to collider
            foreach (var collider in gameObject.GetComponentsInChildren<Collider>())
            {
                collider.material = physicsMaterial;
            }
        }

        private void Update()
        {
            /*  Get ground, and preform either ground, or air update
             *  
             *  Ground update:
             *      set coordinate space velocity
             *      set world space velocity
             *      pass world space velocity to rigidbody
             *  
             *  Air Update:
             *      set coordinate space velocity
             *      set world space velocity
             */

            var groundCollision = GetGround();

            //groundCollision.Draw(Color.blue, Color.red, Color.white, 0.1f, 1f);

            OnGroundCollision?.Invoke(groundCollision);

            if (groundCollision)
            {
                GroundUpdate(groundCollision);
            }
            else
            {
                AirUpdate(groundCollision);
            }

            groundedLastFrame = groundCollision;
        }

        /// <summary>
        /// Apply ground update
        /// </summary>
        /// <param name="groundCollision"></param>
        private void GroundUpdate(GroundCollision groundCollision)
        {
            if (!groundedLastFrame)
            {
                SwitchToGround();
            }

            var collider = groundCollision.collider;
            if(collider.attachedRigidbody != null)
            {
                collider.attachedRigidbody.AddForceAtPosition(-localUp, groundCollision.contactPoint, ForceMode.Force);
            }

            //Apply acceleration, and get the vlocity
            (Vector3 localVelocity, float yVelocity) = ApplyAcceleration(true);

            //snap to ground
            transform.position = groundCollision.contactPoint + localUp * groundCollisionStats.GetHoverDistance();

            //Modify local velocity to follow the ground
            localVelocity = groundCollision.ModifyVelocity(localVelocity, localUp);

            velocity = localVelocity;
        }

        /// <summary>
        /// Apply air update
        /// </summary>
        /// <param name="groundCollision"></param>
        private void AirUpdate(GroundCollision groundCollision)
        {
            //Apply acceleration and get velocity
            (Vector3 localVelocity, float yVelocity) = ApplyAcceleration(false);

            //apply gravity
            yVelocity += stats.gravity * Time.deltaTime;

            //combine velocity and local velocity
            localVelocity += localUp * yVelocity;

            //set velocity
            velocity = localVelocity;
        }

        private (Vector3 localVelocity, float yVelocity) ApplyAcceleration(bool grounded)
        {
            //get up direction in coordinate space
            //Vector3 localUp = coordinateSpaceToWorldSpace.MultiplyVector(Vector3.up);

            //Get flat velocity and y Velocity
            (Vector3 localVelocity, float yVelocity) = GetFlatVelocity(velocity, localUp, grounded);

            //Apply acceleration to local velocity
            float accel = GetAcceleration(grounded);

            localVelocity = Vector3.MoveTowards(localVelocity, targetVelocityInWorldSpace, accel * Time.deltaTime);

            return (localVelocity, yVelocity);
        }

        /// <summary>
        /// Flatten the velocity, to apply acceleration
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="up"></param>
        /// <param name="grounded"></param>
        /// <returns></returns>
        private static (Vector3 flatVeloicty, float yVelocity) GetFlatVelocity(Vector3 velocity, Vector3 up, bool grounded)
        {
            //If grounded, correct velocity for slopes
            if (grounded)
            {
                float mag = velocity.magnitude;
                float yVelocity = Vector3.Dot(velocity, up);
                Vector3 flatVelocity = velocity - (up * yVelocity);

                flatVelocity = flatVelocity.normalized * mag;

                return (flatVelocity, yVelocity);
            }
            //if not grounded, just simple linear algebra to seperate components
            else
            {
                float yVelocity = Vector3.Dot(velocity, up);
                Vector3 flatVelocity = velocity - (up * yVelocity);

                return (flatVelocity, yVelocity);
            }
        }

        private float GetAcceleration(bool grounded)
        {
            return grounded ? acceleration : acceleration * stats.airAccelerationRatio;
        }

        public void AcceptEvent(CEvent e)
        {
            if (e is SetTargetInputEvent setE)
            {
                Quaternion rot = Quaternion.Euler(-90f, 0f, 0f);

                Vector3 temp = new Vector3(setE.input.x, 0f, setE.input.y);

                targetInput = rot * worldSpaceToCoordinateSpace.MultiplyVector(inputRotation * temp);
            }
            if (e is InpulseEvent inpulseE)
            {
                Vector3 inpulse = inpulseE.inpulse;
                if (!inpulseE.inWorldSpace)
                {
                    inpulse = coordinateSpaceToWorldSpace.MultiplyVector(inpulse);
                }

                if (inpulseE.isHard)
                {
                    ProjectToInpulse(inpulse);
                }
                else
                {
                    AddInpulse(inpulse);
                }
            }
            if(e is CrouchEvent)
            {
                Crouch();
            }
            if(e is UnCrouchEvent)
            {
                UnCrouch();
            }
            if(e is CrouchToggleEvent)
            {
                ToggleCrouch();
            }
        }

        public void Crouch()
        {
            groundCollisionStats.crouch = true;
        }

        public void UnCrouch()
        {
            float diff = groundCollisionStats.hoverDistance - groundCollisionStats.crouchHoverDistance;
            diff += 0.5f;
            //Vector3 localUp = coordinateSpaceToWorldSpace.MultiplyVector(Vector3.up);
            if(!Physics.Raycast(transform.position, localUp, diff, groundCollisionStats.layerMask))
            {
                groundCollisionStats.crouch = false;
            }
        }

        public void ToggleCrouch()
        {
            if (groundCollisionStats.crouch)
            {
                UnCrouch();
            }
            else
            {
                Crouch();
            }
        }
        #endregion

        #region Events
        public class SetTargetInputEvent : CEvent
        {
            public readonly Vector2 input;

            public SetTargetInputEvent(Vector2 input)
            {
                this.input = input;
            }
        }

        public class InpulseEvent : CEvent
        {
            public readonly bool isHard;
            public readonly bool inWorldSpace;
            public readonly Vector3 inpulse;

            public InpulseEvent(Vector3 inpulse, bool isHard = true, bool inWorldSpace = false)
            {
                this.inpulse = inpulse;
                this.inWorldSpace = inWorldSpace;
                this.isHard = isHard;
            }
        }

        public class CrouchEvent : CEvent
        {

        }
        public class UnCrouchEvent : CEvent
        {

        }
        public class CrouchToggleEvent : CEvent
        {

        }
        #endregion
    }
}