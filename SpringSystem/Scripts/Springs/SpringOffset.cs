using UnityEngine;

public class SpringOffset : SpringComponent
{
    public Vector3 offset;
    public Quaternion rotationOffset = Quaternion.identity;

    protected override (Vector3, Quaternion) GetPositionRotation(Vector3 position, Quaternion rotation, float deltaTime)
    {
        return (position + rotation * offset, rotation * rotationOffset);
    }

    protected override (Vector3 position, Quaternion rotation) PropegateReset(Vector3 position, Quaternion rotation)
    {
        return (offset + rotation * offset, rotationOffset * rotation);
    }
}
