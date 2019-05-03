using FPSFramework.Actors;
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

        ShipGuideWeapon shipGuide;
        new Camera camera;

        private void Start()
        {
            InitializeActor();
            var playerCharacter = gameObject.AddComponent<PlayerCharacter>();
            playerCharacter.playerNumber = playerNumber;
            //var weapon = gameObject.AddComponent<TestWeapon>();
            shipGuide = new ShipGuideWeapon();
            camera = transform.GetComponentInChildren<Camera>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.F))
            {
                shipGuide.Fire(new WeaponFireEvent(new Ray(camera.transform.position, camera.transform.forward)));
            }
        }
    }
}