using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

namespace FPSFramework.HealthSystem
{
    public class Scanbox : MonoBehaviour
    {
        public Hurtbox hurtbox { get; set; }
        public Eventbox eventbox { get; set; }

        public bool hasHurtbox
        {
            get
            {
                return hurtbox != null;
            }
        }
        public bool hasEventbox
        {
            get
            {
                return eventbox != null;
            }
        }

        public void AcceptScan(HitscanType type, DeltaHealth dHealth, CEvent cEvent)
        {
            if (hasHurtbox && (type & HitscanType.damage) == HitscanType.damage)
            {
                hurtbox.ApplyDeltaHealth(dHealth);
            }
            if (hasEventbox && (type & HitscanType.eventScan) == HitscanType.eventScan)
            {
                eventbox.AcceptEvent(cEvent);
            }
        }
    }
}