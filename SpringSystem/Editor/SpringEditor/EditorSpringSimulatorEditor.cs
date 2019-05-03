using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FPSFramework.Springs;

namespace FPSFramework.Springs
{
    [CustomEditor(typeof(EditorSpringSimulator))]
    public class EditorSpringSimulatorEditor : Editor
    {
        int count;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            //((EditorSpringSimulator)target).Run();

            GUILayout.Label(count++.ToString());

            EditorSpringSimulator sim = (EditorSpringSimulator)target;
            sim.transform.position = sim.transform.position;

            EditorUtility.SetDirty(sim);

            Repaint();
        }
    }
}