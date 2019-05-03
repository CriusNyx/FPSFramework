using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace FPSFramework.Springs
{
    public class EditorSpringSimulator : MonoBehaviour
    {
#if UNITY_EDITOR
        float time = -1f;

        public void OnDrawGizmos()
        {
            //compute delta time
            float deltaTime = 0f;
            if (time == -1f)
            {
                time = (float)EditorApplication.timeSinceStartup;
            }
            deltaTime = (float)EditorApplication.timeSinceStartup - time;
            time = (float)EditorApplication.timeSinceStartup;

            if (Application.isPlaying)
            {
                deltaTime = Time.deltaTime;
            }
            else
            {
                SpringComponent.AutoUpdate(gameObject);
            }

            foreach (Transform child in transform)
            {
                var comp = child.GetComponent<SpringComponent>();
                if (comp != null)
                {
                    comp.Propegate(transform.position, transform.rotation, deltaTime);
                }
            }

            Draw(transform);
        }

        private static void Draw(Transform transform)
        {
            var comp = transform.GetComponent<SpringComponent>();
            if (comp != null)
            {
                Gizmos.DrawSphere(comp.position, 0.1f);
            }
            foreach (Transform child in transform)
            {
                Draw(child);
            }
        }
#endif
    }
}