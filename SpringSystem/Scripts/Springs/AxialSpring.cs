using UnityEngine;

namespace FPSFramework.Springs
{
    public class AxialSpring : SpringComponent
    {
        public Vector3 axis;
        public float dynamicStrength;
        public float staticStrength;
        public float maxDistance = -1f;

        protected override (Vector3, Quaternion) GetPositionRotation(Vector3 position, Quaternion rotation, float deltaTime)
        {
            float dConstant = Mathf.Pow(dynamicStrength, deltaTime);

            Vector3 offset = this.position - position;
            float value = Vector3.Dot(rotation * axis, offset);
            value *= dConstant;
            value = Mathf.MoveTowards(value, 0f, staticStrength * deltaTime);

            if(maxDistance > 0f && Mathf.Abs(value) > maxDistance)
            {
                value = Mathf.Sign(value) * maxDistance;
            }

            return (position + rotation * axis * value, rotation);
        }

        protected override (Vector3 position, Quaternion rotation) PropegateReset(Vector3 position, Quaternion rotation)
        {
            return (position, rotation);
        }
    }
}