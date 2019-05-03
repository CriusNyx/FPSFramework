using FPSFramework.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtilities;
using UnityUtilities.DataStructures;

namespace FPSFramework.Demo.Ship
{
    public class ShipGuideWeapon : IWeapon
    {
        public void AcceptEvent(CEvent e)
        {
            if (e is WeaponFireEvent fe)
            {
                Fire(fe);
            }
        }

        Thunk<int> layerMask = new Thunk<int>(() => LayerMask.GetMask("Default"));

        public void Fire(WeaponFireEvent fireEvent)
        {
            RaycastHit hit;
            if (Physics.Raycast(fireEvent.ray, out hit, Mathf.Infinity, layerMask))
            {
                CEventSystem.Broadcast(Ship.ShipChannel.channel, Ship.ShipChannel.subchannel, new Ship.TargetEvent(hit.point));
            }
        }
    }
}