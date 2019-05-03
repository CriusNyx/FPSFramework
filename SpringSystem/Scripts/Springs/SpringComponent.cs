using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework.Springs
{
    public abstract class SpringComponent : MonoBehaviour
    {
        public SpringComponent parent { get; private set; }
        public readonly List<SpringComponent> children = new List<SpringComponent>();

        /// <summary>
        /// Position is world space
        /// This position is used over transform position to preserve world space in transformation heigharchy
        /// </summary>
        public Vector3 position { get; private set; }

        /// <summary>
        /// Rotation in world space
        /// This rotation is used over transform position to preserve world space in transformation heigharchys
        /// </summary>
        public Quaternion rotation { get; private set; }

        public static T Create<T>(string name = "", SpringComponent parent = null) where T : SpringComponent
        {
            return new GameObject().AddComponent<T>()._Init<T>(name, parent);
        }

        private T _Init<T>(string name = "", SpringComponent parent = null) where T : SpringComponent
        {
            this.name = name;
            if (parent != null)
            {
                this.parent = parent;
                parent.children.Add(this);
            }

            transform.parent = parent?.transform;

            return (T)this;
        }

        public void Start()
        {
            this.position = transform.position;
            this.rotation = transform.rotation;
        }

#if UNITY_EDITOR
        public static void AutoUpdate(GameObject gameObject)
        {
            AutoUpdateScan(null, gameObject);
        }

        private static void AutoUpdateScan(SpringComponent parent, GameObject gameObject)
        {
            var comp = gameObject.GetComponent<SpringComponent>();
            if (comp != null)
            {
                comp.parent = parent;

                if (parent != null)
                {
                    parent.children.Add(comp);
                }

                comp.children.Clear();
            }
            else
            {
                comp = parent;
            }

            foreach (Transform child in gameObject.transform)
            {
                AutoUpdateScan(comp, child.gameObject);
            }
        }
#endif

        /// <summary>
        /// Modify the position and rotation of the spring component
        /// 
        /// Accepts the current position and rotation from the parent
        /// Returns the position and rotation of this spring
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        protected abstract (Vector3, Quaternion) GetPositionRotation(Vector3 position, Quaternion rotation, float deltaTime);

        public void Propegate(Vector3 position, Quaternion rotation, float deltaTime = -1f)
        {
            if (deltaTime < 0f)
            {
                deltaTime = Time.deltaTime;
            }
            (position, rotation) = GetPositionRotation(position, rotation, deltaTime);

            this.position = position;
            this.rotation = rotation;

            transform.position = this.position;
            transform.rotation = this.rotation;

            foreach (var child in children)
            {
                child.Propegate(position, rotation, deltaTime);
            }
        }

        public void Reset(Vector3 position, Quaternion rotation)
        {
            (position, rotation) = PropegateReset(position, rotation);

            foreach (var child in children)
            {
                child.Reset(position, rotation);
            }
        }

        public void PropegateDelta(Matrix4x4 delta)
        {
            PropegateDelta(delta, delta.rotation);
        }

        private void PropegateDelta(Matrix4x4 delta, Quaternion rotation)
        {
            position = delta.MultiplyPoint(position);
            this.rotation = rotation * this.rotation;

            foreach (var child in children)
            {
                child.PropegateDelta(delta, rotation);
            }
        }

        protected abstract (Vector3 position, Quaternion rotation) PropegateReset(Vector3 position, Quaternion rotation);

        public static SpringComponent AutoLink(GameObject gameObject)
        {
            SpringComponent root = gameObject.GetComponent<SpringComponent>();

            if (root != null)
            {
                foreach (Transform child in gameObject.transform)
                {
                    SpringComponent c = AutoLink(child.gameObject);
                    if (c != null)
                    {
                        c.parent = root;
                        root.children.Add(c);
                    }
                }
            }

            return root;
        }
    }
}