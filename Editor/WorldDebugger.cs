using System;
using System.Collections.Generic;
using System.Globalization;
using LazyECS;
using LazyECS.Component;
using LazyECS.Entity;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
class WorldDebugger : EditorWindow
{
    [MenuItem("Tools/World Debugger")]
    public static void Init()
    {
        WorldDebugger window = (WorldDebugger) GetWindow(typeof(WorldDebugger), false);
        window.Show();
    }

    private void OnEnable()
    {
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        string fileName = "lazyecsworlddebugger.png";

        string fullPath = "";
        
        for(int i = 0; i < allAssetPaths.Length; ++i)
        {
            if (allAssetPaths[i].EndsWith(fileName))
            {
                fullPath = allAssetPaths[i];
            }
        }

        Texture icon = AssetDatabase.LoadAssetAtPath<Texture>(fullPath);
        
        titleContent = new GUIContent("World Debugger",
            icon,
            "Debug a LazyECS world.");
    }

    [SerializeField]
    public SimulationController simulationController;
    private int currTab;
    private bool[] worldsFoldoutsState;
    private Vector2 scrollPos;

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

        if (worldsFoldoutsState.Length == 0)
            worldsFoldoutsState = new bool[simulationController.Worlds.Count];
        
        if (worldsFoldoutsState.Length == 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("No worlds found", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
                
            return;
        }
        
        string[] tabs = {"Worlds", "Settings"};
        currTab = GUILayout.Toolbar(currTab, tabs);

        if (currTab == 0)
        {

            int worldFoldout = 0;
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            foreach (KeyValuePair<int,IWorld> world in simulationController.Worlds)
            {
                string worldFoldoutName = world.Value.GetType().Name
                                          + " (Groups: "
                                          + world.Value.Groups.Count
                                          + ", Entities: "
                                          + world.Value.Entities.Count
                                          + ", Features: "
                                          + world.Value.Features.Length
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

                        EditorGUI.indentLevel++;
                        
                        foreach (KeyValuePair<Type, IComponent> component in entity.Value.Components)
                        {
                            EditorGUILayout.Foldout(true, component.Key.Name);
                            EditorGUI.indentLevel++;
                            
                            Debug.Log(component.Value.Get().GetType().ToString());
                            
                            switch (component.Value.Get().GetType().Name)
                            {
                                case "Int16":
                                case "Int32":
                                    EditorGUILayout.SelectableLabel(((int)component.Value.Get()).ToString());
                                    break;
                                case "Single":
                                case "Double":
                                case "Long":
                                    EditorGUILayout.SelectableLabel(((float)component.Value.Get()).ToString(CultureInfo.InvariantCulture));
                                    break;
                                case "Boolean":
                                    EditorGUILayout.Toggle(((bool)component.Value.Get()));
                                    break;
                                case "String":
                                    EditorGUILayout.SelectableLabel(((string)component.Value.Get()));
                                    break;
                                case "Vector3":
                                    Vector3 vector3 = (Vector3) component.Value.Get();
                                    EditorGUILayout.SelectableLabel($"x: {vector3.x}, y: {vector3.y}, z: {vector3.z}");
                                    break;
                                case "Vector2":
                                    Vector2 vector2 = (Vector2) component.Value.Get();
                                    EditorGUILayout.SelectableLabel($"x: {vector2.x}, y: {vector2.y}");
                                    break;
                                case "GameObject":
                                    EditorGUILayout.ObjectField((GameObject)component.Value.Get(), typeof(GameObject), true);
                                    break;
                                default:
                                    EditorGUILayout.SelectableLabel(component.Value.Get().GetType().Name + " (Custom editor needed to display value)");
                                    break;
                            }

                            EditorGUI.indentLevel--;
                        }

                        EditorGUI.indentLevel--;
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