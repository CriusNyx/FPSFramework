using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FPSFramework.Movement
{
    public class Spinner : MonoBehaviour
    {
        public Vector3 theta;

        public void Update()
        {
            //transform.Rotate(theta * Time.deltaTime);
            transform.rotation = Quaternion.Euler(theta * Time.deltaTime) * transform.rotation;
        }
    }
}
