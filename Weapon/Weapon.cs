using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IWeapon : CEventListener
{
    void Fire(WeaponFireEvent fireEvent);
}

public class WeaponFireEvent : CEvent
{
    public Ray ray;

    public WeaponFireEvent(Ray ray)
    {
        this.ray = ray;
    }
}