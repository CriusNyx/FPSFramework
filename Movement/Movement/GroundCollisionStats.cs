namespace FPSFramework.Movement
{
    /// <summary>
    /// Container to hold ground collision stats.
    /// </summary>
    public struct GroundCollisionStats
    {
        public bool crouch;
        public int layerMask;
        public float hoverDistance;
        public float crouchHoverDistance;
        public float groundedRaycastBonus;

        public GroundCollisionStats(bool crouch, int layerMask, float hoverDistance, float crouchHoverDistance, float groundedRaycastBonus)
        {
            this.crouch = crouch;
            this.layerMask = layerMask;
            this.hoverDistance = hoverDistance;
            this.crouchHoverDistance = crouchHoverDistance;
            this.groundedRaycastBonus = groundedRaycastBonus;
        }

        public float GetHoverDistance()
        {
            return crouch ? crouchHoverDistance : hoverDistance;
        }
    }
}