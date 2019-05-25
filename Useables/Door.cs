using FPSFramework.HealthSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class Door : MonoBehaviour, CEventListener
{
    public bool toggle = false;
    public Vector3 targetRotation;
    public float speed = 180f;

    Quaternion baseRot, targetRot;

    void Awake()
    {
        foreach (Collider collider in gameObject.GetComponentsInChildren<Collider>())
        {
            var ebox = collider.gameObject.AddComponent<Eventbox>();
            ebox.eventHandler += AcceptEvent;
        }
        baseRot = transform.localRotation;
        targetRot = Quaternion.Euler(targetRotation) * baseRot;
    }

    void Update()
    {
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, toggle ? targetRot : baseRot, Time.deltaTime * 180f);
    }

    public void AcceptEvent(CEvent e)
    {
        if (e is UseEvent)
        {
            toggle = !toggle;
        }
    }
}