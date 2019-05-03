using UnityEngine;
using System.Collections;
using UnityUtilities.ExecutionOrder.ExecutionOrderControl;

namespace FPSFramework.Movement.CoordinateSpaces
{
    [ExecutionOrder(ExecutionOrderValue.PostLogic)]
    public class Platform : MonoBehaviour
    {
        public Matrix4x4 positionThisFrame { get; private set; }
        Matrix4x4 positionLastFrame, positionLastFrameInverse, diff;

        //Vector3 positionLastFrame;
        //Quaternion rotationLastFrame;

        //Quaternion rotationLastFrameInverse;

        //Vector3 positionThisFrame;
        //Quaternion rotationThisFrame;


        private void Start()
        {
            positionThisFrame = transform.localToWorldMatrix;
            positionLastFrame = positionThisFrame;
            positionLastFrameInverse = positionLastFrame.inverse;

            foreach (var collider in gameObject.GetComponentsInChildren<Collider>())
            {
                PlatformCollider.Create(collider.gameObject, this);
            }
        }

        private void Update()
        {
            positionLastFrame = positionThisFrame;
            positionLastFrameInverse = positionLastFrame.inverse;
            positionThisFrame = transform.localToWorldMatrix;

            diff = positionThisFrame * positionLastFrameInverse;
        }

        public (Matrix4x4 positionThisFrame, Matrix4x4 diff) GetDelta()
        {
            return (positionThisFrame, diff);
        }
    }
}
