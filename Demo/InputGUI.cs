using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FPSFramework.Demo
{
    public class InputGUI : MonoBehaviour
    {
        private void OnGUI()
        {
            GUILayout.Label(
                @"


    W, A, S, D: Move Around
    Mouse: Look Around
    E: Reset Orientation (for debugging)
    R: Debug Break (for editor)
    Esc: Reset Scene
    Alt+F4: Exit
    Up Arrow, Down Arror: Increase/Decrease platform speed"
                );
        }
    }
}