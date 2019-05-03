using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using static FPSFramework.Movement.MovementController;
using static FPSFramework.Movement.ViewController;
using UnityUtilities;

namespace FPSFramework.Movement
{
    public class InputController : MonoBehaviour
    {
        EventPlayerNumberChannel playerNumber = EventPlayerNumberChannel.player1;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            Vector2 input = Vector2.zero;
            if (Input.GetKey(KeyCode.W))
            {
                input.y += 1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                input.y -= 1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                input.x -= 1f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                input.x += 1f;
            }

            if (input.sqrMagnitude > 1f)
            {
                input = input.normalized;
            }

            //Sets the target input for the movement controller that receive this event.
            CEventSystem.Broadcast(EventChannel.input, playerNumber, new SetTargetInputEvent(input));

            //Adds an inpulse event to the movement controller that receives this event.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CEventSystem.Broadcast(EventChannel.input, playerNumber, new InpulseEvent(Vector3.up * 10f));
            }

            Vector2 viewInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            CEventSystem.Broadcast(EventChannel.input, playerNumber, new LookInputEvent(viewInput));

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                CEventSystem.Broadcast(EventChannel.input, playerNumber, new CrouchToggleEvent());
            }
        }
    }
}