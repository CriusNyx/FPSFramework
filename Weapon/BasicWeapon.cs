using FPSFramework.HealthSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtilities;

namespace FPSFramework.Weapons
{
    public abstract class BasicWeapon : IWeapon
    {
        public void AcceptEvent(CEvent e)
        {
            if (e is WeaponFireEvent we)
            {
                Fire(we);
            }
        }

        protected DeltaHealth dHealth;
        protected int layerMask;
        protected float distance;
        protected bool penetrate;
        protected Action<Scanbox, RaycastHit> OnHit;

        public void Fire(WeaponFireEvent fireEvent)
        {
            Hitscan.Cast(fireEvent.ray, HitscanType.damage, dHealth, null, distance, layerMask, penetrate, OnHit: OnHit);
        }
    }
}