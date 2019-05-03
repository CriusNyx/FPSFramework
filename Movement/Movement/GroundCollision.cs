using UnityEngine;
using UnityUtilities;

namespace FPSFramework.Movement
{
    /// <summary>
    /// Stores the output of a ground collision detection
    /// </summary>
    public struct GroundCollision
    {
        public readonly bool collision;
        public readonly Vector3 contactPoint;
        public readonly Vector3 normal;
        public readonly Collider collider;

        /// <summary>
        /// Hides constructor, so that ground collisions can only be generated from GetCollision method
        /// </summary>
        /// <param name="collision"></param>
        /// <param name="contactPoint"></param>
        /// <param name="normal"></param>
        /// <param name="modifiedVelocity"></param>
        public GroundCollision(bool collision, Vector3 contactPoint, Vector3 normal, Collider collider)
        {
            this.collision = collision;
            this.contactPoint = contactPoint;
            this.normal = normal;
            this.collider = collider;
        }

        /// <summary>
        /// Get a ground collision for a character
        /// Returns a collision with point, normal, and a modified velocity for velocity calculation
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="isGrounded"></param>
        /// <param name="velocity"></param>
        /// <param name="colliders"></param>
        /// <returns></returns>
        public static GroundCollision GetCollision(GroundCollisionStats stats, Vector3 position, Vector3 direction, bool isGrounded, Collider[] colliders = null)
        {
            //calculate raycast distance
            float distance = stats.GetHoverDistance();
            if (isGrounded)
            {
                distance += stats.groundedRaycastBonus;
            }

            //normal physics raycast
            if (colliders == null)
            {
                //get collision info
                RaycastHit hit;
                bool collision = Physics.Raycast(position, direction, out hit, distance, stats.layerMask);

                //generate output
                if (collision)
                {
                    return new GroundCollision(collision, hit.point, hit.normal, hit.collider);
                }
                else
                {
                    return new GroundCollision(collision, Vector3.zero, Vector3.zero, null);
                }
            }
            //collider cast
            else
            {
                //test collision for each collider
                foreach (var collider in colliders)
                {
                    RaycastHit hit;
                    bool collision = collider.Raycast(new Ray(position, direction), out hit, distance);
                    if (collision)
                    {
                        return new GroundCollision(collision, hit.point, hit.normal, hit.collider);
                    }
                }
                //return the collision
                return new GroundCollision(false, Vector3.zero, Vector3.zero, null);
            }
        }

        public Vector3 ModifyVelocity(Vector3 velocity, Vector3 up)
        {
            if (collision)
            {
                return ModifyVelocity(velocity, normal, up);
            }
            else
            {
                return velocity;
            }
        }

        /// <summary>
        /// Calculate the modified velocity from the velocity
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="normal"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        private static Vector3 ModifyVelocity(Vector3 velocity, Vector3 normal, Vector3 up)
        {
            float mag = velocity.magnitude;

            float denom = Vector3.Dot(up, normal);
            if (denom == 0)
            {
                return Vector3.zero;
            }
            return (velocity + up * (Vector3.Dot(-velocity, normal) / denom)).normalized * mag;
        }

        /// <summary>
        /// Syntactic sugar for if statements
        /// </summary>
        /// <param name="collision"></param>
        public static implicit operator bool(GroundCollision collision)
        {
            return collision.collision;
        }

        /// <summary>
        /// Draw the ground collision
        /// </summary>
        /// <param name="pointColor"></param>
        /// <param name="normalColor"></param>
        /// <param name="vColor"></param>
        /// <param name="size"></param>
        /// <param name="normalDistance"></param>
        /// <param name="time"></param>
        public void Draw(Color pointColor, Color normalColor, Color vColor, float size, float normalDistance, float time = -1f)
        {
            float size2 = size * 2f;
            if (collision)
            {
                contactPoint.DrawCross(size, pointColor, time);
                Debug.DrawRay(contactPoint, normal * normalDistance, normalColor, time);
            }
        }
    }
}