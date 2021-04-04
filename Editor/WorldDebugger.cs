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
        WorldDebugger window = (WorldDebugger) GetWindow(typeof(WorldDebugger), false, "World Debugger");
        window.Show();
    }

    [SerializeField]
    public SimulationController simulationController;
    private int currTab;
    private bool[] worldsFoldoutsState;
    private Vector2 scrollPos;

    public void Awake()
    {
        worldsFoldoutsState = new bool[simulationController.Worlds.Count];
    }

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
            simulationController = FindObjectOfType<SimulationController>();

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

        // TODO: remove settings tab or something
        // under world add 2 more foldouts for entities and groups
        
        string[] tabs = {"Worlds", "Settings"};
        currTab = GUILayout.Toolbar(currTab, tabs);

        if (currTab == 0)
        {

            int worldFoldout = 0;
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            foreach (KeyValuePair<Type,IWorld> world in simulationController.Worlds)
            {
                string worldFoldoutName = world.Value.GetType().Name
                                          + " (Groups: "
                                          + world.Value.Groups.Count
                                          + ", Entities: "
                                          + world.Value.Entities.Count
                                          + ")";

                worldsFoldoutsState[worldFoldout] = EditorGUILayout.Foldout(worldsFoldoutsState[worldFoldout], worldFoldoutName, EditorStyles.foldout);

                if (worldsFoldoutsState[worldFoldout])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Foldout(true, "Groups (" + world.Value.Groups.Count + ")");
                    EditorGUI.indentLevel++;
                    int groupCount = 0;
                    foreach (Group group in world.Value.Groups)
                    {
                        string label = "Group " + groupCount + " entities: " + group.Entities.Count + ", filters (" + group.Filters.Count + ") : ";
                        
                        int compCount = 0;
                        foreach (Type filter in group.Filters)
                        {
                            label += filter.Name;

                            if (compCount < group.Filters.Count - 1)
                                label += ", ";
                            
                            compCount++;
                        }
                        
                        EditorGUILayout.LabelField(label);
                        groupCount++;
                    }                    
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Foldout(true, "Entities (" + world.Value.Entities.Count + ")");
                    EditorGUI.indentLevel++;
                    foreach (KeyValuePair<int,Entity> entity in world.Value.Entities)
                    {
                        string label = "Entity " + entity.Key + " (";
                        
                        int compCount = 0;
                        foreach (KeyValuePair<Type,IComponent> component in entity.Value.Components)
                        {
                            label += component.Key.Name;

                            if (compCount < entity.Value.Components.Count - 1)
                                label += ", ";
                            
                            compCount++;
                        }
                        
                        label += ")";

                        EditorGUILayout.LabelField(label);
                    }
                    EditorGUI.indentLevel = 0;
                }
            
                worldFoldout++;
            }
            
            EditorGUILayout.EndScrollView();
        }
    }

    private void Update()
    {
        if (!EditorApplication.isPlaying) return;
        Repaint();
    }
}