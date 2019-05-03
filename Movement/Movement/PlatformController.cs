using UnityEngine;
using System.Collections;
using FPSFramework.Movement.CoordinateSpaces;
using UnityUtilities.ExecutionOrder.ExecutionOrderControl;

namespace FPSFramework.Movement
{
    [ExecutionOrder(ExecutionOrderValue.PrePhysics)]
    [RequireComponent(typeof(MovementController))]
    public class PlatformController : MonoBehaviour
    {
        MovementController movementController;
        Platform platformLastFrame;
        Platform platform;

        private void Start()
        {
            movementController = GetComponent<MovementController>();
            movementController.OnGroundCollision += OnGroundCollisionHandler;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                platform = null;
            }

            if(platformLastFrame != platform)
            {
                if(platform == null)
                {
                    movementController.ApplySpace(Matrix4x4.identity);
                }
                else
                {
                    movementController.ApplySpace(platform.positionThisFrame);
                }
            }
            else
            {
                if (platform != null)
                {
                    (Matrix4x4 positionThisFrame, Matrix4x4 diff) = platform.GetDelta();
                    movementController.ApplyDeltaSpace(positionThisFrame, diff);
                }
            }

            platformLastFrame = platform;
        }

        private void OnGroundCollisionHandler(GroundCollision collision)
        {
            if (collision)
            {
                PlatformCollider coll = collision.collider.GetComponent<PlatformCollider>();
                platform = coll?.platform;
            }
        }
    }
}