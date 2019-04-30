using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameEngine.Movement
{
    [RequireComponent(typeof(MovementController))]
    public class PlayerCharacter : MonoBehaviour
    {
        public EventPlayerNumberChannel playerNumber = EventPlayerNumberChannel.player1;

        MovementController movementController;

        private void Awake()
        {
            movementController = GetComponent<MovementController>();
            CEventSystem.AddEventListener(EventChannel.input, playerNumber, movementController);
        }

        private void OnDestroy()
        {
            CEventSystem.RemoveEventListener(EventChannel.input, playerNumber, movementController);
        }
    }
}