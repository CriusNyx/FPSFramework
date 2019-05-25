using FPSFramework.Springs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtilities;
using UnityUtilities.Networking;
using static Assets.FPSFramework.Demo.NetworkDemo.TestGUIDObjectLocal;

namespace Assets.FPSFramework.Demo.NetworkDemo
{
    public class TestGUIDObjectRemote : NetBehaviour
    {
        SpringComponent root;

        private void Start()
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = transform.position;
            RadialSpring spring = SpringComponent.Create<RadialSpring>("Radial");
            spring.transform.position = transform.position;
            spring.transform.parent = transform;
            sphere.transform.parent = spring.transform;

            root = spring;
        }

        private void Update()
        {
            root.Propegate(transform.position, transform.rotation, Time.deltaTime);
        }

        public override void AcceptEvent(CEvent e)
        {
            if(e is GUIDPositionEvent pe)
            {
                transform.position = pe.v;
            }
        }

        public override NetBehaviourSpawn GetSpawn()
        {
            //throw new NotImplementedException();
            return null;
        }
    }
}
