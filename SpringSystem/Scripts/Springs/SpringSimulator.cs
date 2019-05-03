using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FPSFramework.Springs
{
    public class SpringSimulator : MonoBehaviour
    {
        SpringComponent root;

        private void Start()
        {
            root = SpringComponent.AutoLink(gameObject);
            root.Reset(transform.position, transform.rotation);
        }

        private void Update()
        {
            root.Propegate(transform.position, transform.rotation, Time.deltaTime);
        }
    }
}