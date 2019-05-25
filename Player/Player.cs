using FPSFramework.Actors;
using FPSFramework.Demo;
using FPSFramework.Demo.Ship;
using FPSFramework.Movement;
using FPSFramework.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtilities;

namespace FPSFramework.Player
{
    class Player : Actor
    {
        public EventPlayerNumberChannel playerNumber = EventPlayerNumberChannel.player1;

        PlayerCharacter playerCharacter;
        ShipGuideWeapon shipGuide;
        BasicWeapon basic;
        TestWeapon testWeapon;
        new Camera camera;

        private void Start()
        {
            InitializeActor();
            playerCharacter = gameObject.AddComponent<PlayerCharacter>();
            playerCharacter.playerNumber = playerNumber;
            //var weapon = gameObject.AddComponent<TestWeapon>();
            shipGuide = new ShipGuideWeapon();
            testWeapon = gameObject.AddComponent<TestWeapon>();
            camera = transform.GetComponentInChildren<Camera>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.F))
            {
                Debug.DrawRay(camera.transform.position, camera.transform.forward, Color.white, 1f);
                shipGuide.Fire(new WeaponFireEvent(new Ray(camera.transform.position, camera.transform.forward)));
            }

            if(Input.GetKey(KeyCode.Escape))
            {
                playerCharacter.transform.position = Vector3.back * 10f + Vector3.up * 2f;
                playerCharacter.GetComponentInChildren<PlatformController>().platform = null;
                playerCharacter.GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }
}