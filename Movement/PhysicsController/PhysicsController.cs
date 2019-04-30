using UnityEngine;
using System.Collections;

[AutoInit]
[ExecutionOrder(ExecutionOrderValue.PhysicsStep)]
public class PhysicsController : MonoBehaviour
{
    void Start()
    {
        Physics.autoSimulation = false;
    }

    void Update()
    {
        Physics.Simulate(Mathf.Max(0f, Time.deltaTime));
    }
}