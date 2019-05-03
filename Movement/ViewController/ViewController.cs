using FPSFramework.Springs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;
using UnityUtilities.ExecutionOrder.ExecutionOrderControl;

namespace FPSFramework.Movement
{
    [ExecutionOrder(ExecutionOrderValue.Camera)]
    [RequireComponent(typeof(MovementController))]
    public class ViewController : MonoBehaviour, CEventListener
    {
        MovementController movementController;

        public Quaternion xRotation { get; private set; } = Quaternion.identity;
        //Quaternion yRotation = Quaternion.identity;
        float yRotation;

        Vector2 input = Vector2.zero;

        //new Camera camera;
        SpringComponent root;

        float sensitivity = 90f;

        public EventPlayerNumberChannel player = EventPlayerNumberChannel.player1;

        void Start()
        {
            movementController = GetComponent<MovementController>();
            movementController.OnCoordinateSpaceDelta += OnCoordinateSpaceDelta;
            movementController.OnNewCoordinateSpace += OnNewCoordinateSpace;

            var springObject = new GameObject("Axial");
            springObject.transform.parent = transform;
            springObject.transform.localPosition = Vector3.zero;
            springObject.transform.localRotation = Quaternion.identity;

            var spring = springObject.AddComponent<AxialSpring>();

            spring.axis = Vector3.up;
            spring.dynamicStrength = 0.00000001f;

            var gyroObject = new GameObject("Gyro");
            gyroObject.transform.parent = springObject.transform;
            gyroObject.transform.localPosition = Vector3.zero;
            gyroObject.transform.localRotation = Quaternion.identity;

            var gyro = gyroObject.AddComponent<GyroSpring>();
            gyro.staticStrength = 10f;

            var camera = GetComponentInChildren<Camera>();
            camera.transform.parent = gyroObject.transform;
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localRotation = Quaternion.identity;

            root = SpringComponent.AutoLink(springObject);

            CEventSystem.AddEventListener(EventChannel.input, EventPlayerNumberChannel.player1, this);
        }

        void Update()
        {
            //input.x = Input.GetAxisRaw("Mouse X");
            //input.y = Input.GetAxisRaw("Mouse Y");

            Vector2 localInput = input * Time.deltaTime * sensitivity;
            input = Vector2.zero;
            xRotation = xRotation * Quaternion.Euler(0f, localInput.x, 0f);
            //yRotation = Quaternion.Euler(-localInput.y, 0f, 0f) * yRotation;
            yRotation += localInput.y;
            yRotation = Mathf.Clamp(yRotation, -80f, 80f);

            Vector3 forward = xRotation * Vector3.forward;
            Vector3 up = movementController.coordinateSpaceToWorldSpace.MultiplyVector(Vector3.up);

            xRotation = Quaternion.LookRotation(forward, up);

            movementController.inputRotation = xRotation;

            //transform.rotation = xRotation * yRotation;

            root.Propegate(transform.position, xRotation * Quaternion.Euler(-yRotation, 0f, 0f));
        }

        void OnDestroy()
        {
            CEventSystem.RemoveEventListener(EventChannel.input, EventPlayerNumberChannel.player1, this);
        }

        void OnCoordinateSpaceDelta(Matrix4x4 delta)
        {
            xRotation = delta.rotation * xRotation;
            root.PropegateDelta(delta);
        }

        void OnNewCoordinateSpace(Matrix4x4 space)
        {
            Vector3 localUp = space.MultiplyVector(Vector3.up);

            Quaternion rot = xRotation * Quaternion.Euler(-yRotation, 0f, 0f);

            Vector3 vForward = rot * Vector3.forward;
            Vector3 vUp = rot * Vector3.up;

            Vector3 newForward = vForward - Vector3.Dot(vForward, localUp) * localUp;
            if(newForward.sqrMagnitude < 0.01f)
            {
                newForward = vUp - Vector3.Dot(vUp, localUp) * localUp;
            }

            newForward = newForward.normalized;

            xRotation = Quaternion.LookRotation(newForward, localUp);

            float angle = Vector3.Angle(newForward, vForward);
            angle = Mathf.Sign(Vector3.Dot(vForward, localUp)) * angle;

            yRotation = angle;
        }

        public void AcceptEvent(CEvent e)
        {
            if(e is LookInputEvent le)
            {
                input = le.input;
            }
        }

        public class LookInputEvent : CEvent
        {
            public readonly Vector2 input;

            public LookInputEvent(Vector2 input)
            {
                this.input = input;
            }
        }
    }
}