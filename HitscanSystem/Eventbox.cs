using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

namespace FPSFramework.HealthSystem {
    public class Eventbox : MonoBehaviour, CEventListener
    {
        public Action<CEvent> eventHandler;

        void Awake()
        {
            Scanbox scanbox = gameObject.AddOrGetComponent<Scanbox>();
            scanbox.eventbox = this;
        }

        public void AcceptEvent(CEvent e)
        {
            eventHandler?.Invoke(e);
        }
    }
}