using UnityEngine;

namespace FPSFramework.Springs
{
    public class SphericalSpring : SpringComponent
    {
        public float dynamicStrength;
        public float staticStrength;
        public float maxDistance = -1f;

        protected override (Vector3, Quaternion) GetPositionRotation(Vector3 position, Quaternion rotation, float deltaTime)
        {
            float dConstant = Mathf.Pow(dynamicStrength, deltaTime);

            Vector3 temp = this.position - position;
            float mag = temp.magnitude;
            if(mag != 0)
            {
                float newMag = mag * dConstant;
                newMag = Mathf.Max(0f, newMag - staticStrength);

                temp = temp * newMag / mag;
            }

            //Vector3 temp = MathFunctions.ELerp(this.position, position, dynamicStrength, deltaTime);
            //temp = Vector3.MoveTowards(temp, position, Mathf.Clamp01(staticStrength * deltaTime));

            return (position + temp, rotation);
        }

        protected override (Vector3 position, Quaternion rotation) PropegateReset(Vector3 position, Quaternion rotation)
        {
            return (position, rotation);
        }
    }
}