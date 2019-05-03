using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FPSFramework.Springs
{
    public class SpringTest : MonoBehaviour
    {
        SpringOffset root;
        SphericalSpring arm;
        RadialSpring rotSpring;
        //IReadOnlyWrapper<Vector3> pos;
        //IReadOnlyWrapper<Quaternion> rot;

        GameObject cube;

        public float dymStrength = 0.999f;
        public float statStrength = 0.1f;

        private void Awake()
        {
            root = SpringComponent.Create<SpringOffset>("root");
            root.transform.parent = transform;
            arm = SpringComponent.Create<SphericalSpring>("arm", root);
            rotSpring = SpringComponent.Create<RadialSpring>("rot", arm);

            //pos = rotSpring.GetPositionRef();
            //rot = rotSpring.GetRotationRef();

            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }

        private void Update()
        {
            arm.dynamicStrength = dymStrength;
            arm.staticStrength = statStrength;
            rotSpring.dynamicStrength = dymStrength;
            arm.staticStrength = statStrength;

            root.Propegate(transform.position, transform.rotation);

            //cube.transform.position = pos.Get();
            //cube.transform.rotation = rot.Get();
        }
    }
}