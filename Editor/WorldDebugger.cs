using System;
using System.Collections.Generic;
using LazyECS;
using LazyECS.Component;
using LazyECS.Entity;
using UnityEditor;
using UnityEngine;

[Serializable]
class WorldDebugger : EditorWindow
{
    [MenuItem("Tools/World Debugger")]
    public static void Init()
    {
        WorldDebugger window = (WorldDebugger) GetWindow(typeof(WorldDebugger));
        window.Show();
    }

    [SerializeField]
    public SimulationController simulationController;
    private int currTab;
    private bool[] foldoutsState;

    private void OnGUI()
    {
        if (!EditorApplication.isPlaying)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Enter PlayMode", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            return;
        }
        
        if (simulationController == null)
        {
            simulationController = GameObject.FindObjectOfType<SimulationController>();

            if (simulationController == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Initialize Simulation Controller", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                
                return;
            }
        }

        string[] tabs = {"Worlds", "Settings"};
        currTab = GUILayout.Toolbar(currTab, tabs);

        if (currTab == 0)
        {
            if(foldoutsState.Length == 0)
                foldoutsState = new bool[simulationController.Worlds.Count];

            int foldout = 0;
            foreach (KeyValuePair<Type,IWorld> world in simulationController.Worlds)
            {
                foldoutsState[foldout] = EditorGUILayout.Foldout(foldoutsState[foldout], world.Value.GetType().Name, EditorStyles.foldout);

                if (foldoutsState[foldout])
                {
                    foreach (KeyValuePair<int,Entity> entity in world.Value.Entities)
                    {
                        string label = "Entity " + entity.Key + " (";

                        foreach (KeyValuePair<Type,IComponent> component in entity.Value.Components)
                        {
                            label += component.Key.Name + ", ";
                        }
                        
                        label += ")";
                        GUILayout.Label(label);
                    }
                }
            
                foldout++;
            }
        }
    }

    private void Update()
    {
        if (!EditorApplication.isPlaying) return;
        Repaint();
    }
}