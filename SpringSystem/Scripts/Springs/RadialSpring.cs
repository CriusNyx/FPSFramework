using UnityEngine;

namespace FPSFramework.Springs
{
    public class RadialSpring : SpringComponent
    {
        public float dynamicStrength;
        public float staticStrength;

        protected override (Vector3, Quaternion) GetPositionRotation(Vector3 position, Quaternion rotation, float deltaTime)
        {
            Quaternion temp = Quaternion.Slerp(this.rotation, rotation, dynamicStrength * deltaTime);
            temp = Quaternion.RotateTowards(temp, rotation, staticStrength * deltaTime);

            return (position, temp);
        }

        protected override (Vector3 position, Quaternion rotation) PropegateReset(Vector3 position, Quaternion rotation)
        {
            return (position, rotation);
        }
    }
}