using System;
using System.Collections.Generic;
using System.Globalization;
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
		WorldDebugger window = (WorldDebugger) GetWindow(typeof(WorldDebugger), false);
		window.Show();
	}

	private void Update()
	{
		if (!EditorApplication.isPlaying) return;
		Repaint();
	}

	private void OnEnable()
	{
		string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
		string fileName = "lazyecsworlddebugger.png";

		string fullPath = "";

		for (int i = 0; i < allAssetPaths.Length; ++i)
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

	[SerializeField] public SimulationController simulationController;
	private int currTab;
	private bool[] worldsFoldoutsState;
	private bool[] groupsFoldoutsState;
	private bool[] entitiesFoldoutsState;

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

		if (worldsFoldoutsState == null || worldsFoldoutsState.Length != simulationController.Worlds.Count)
		{
			worldsFoldoutsState = new bool[simulationController.Worlds.Count];
			groupsFoldoutsState = new bool[simulationController.Worlds.Count];
			entitiesFoldoutsState = new bool[simulationController.Worlds.Count];
		}

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
			if (GUILayout.Button("Collapse", GUILayout.Width(75)))
			{
				for (var i = 0; i < worldsFoldoutsState.Length; i++)
				{
					worldsFoldoutsState[i] = false;
				}

				for (var i = 0; i < groupsFoldoutsState.Length; i++)
				{
					groupsFoldoutsState[i] = false;
				}

				for (var i = 0; i < entitiesFoldoutsState.Length; i++)
				{
					entitiesFoldoutsState[i] = false;
				}
			}
			
			int currWorldFoldout = 0;

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			foreach (KeyValuePair<int, IWorld> world in simulationController.Worlds)
			{
				string worldFoldoutName = world.Value.GetType().Name
				                          + " (Groups: "
				                          + world.Value.Groups.Count
				                          + ", Entities: "
				                          + world.Value.Entities.Count
				                          + ", Features: "
				                          + world.Value.Features.Length
				                          + ")";

				worldsFoldoutsState[currWorldFoldout] = EditorGUILayout.Foldout(worldsFoldoutsState[currWorldFoldout], worldFoldoutName, EditorStyles.foldout);

				if (worldsFoldoutsState[currWorldFoldout])
				{
					EditorGUI.indentLevel++;
					groupsFoldoutsState[currWorldFoldout] = EditorGUILayout.Foldout(groupsFoldoutsState[currWorldFoldout], "Groups (" + world.Value.Groups.Count + ")");
					EditorGUI.indentLevel++;

					if (groupsFoldoutsState[currWorldFoldout])
					{
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
					}

					EditorGUI.indentLevel--;
					entitiesFoldoutsState[currWorldFoldout] = EditorGUILayout.Foldout(entitiesFoldoutsState[currWorldFoldout], "Entities (" + world.Value.Entities.Count + ")");
					EditorGUI.indentLevel++;

					if (entitiesFoldoutsState[currWorldFoldout])
					{
						foreach (KeyValuePair<int, Entity> entity in world.Value.Entities)
						{
							string label = "Entity " + entity.Key + " (";


							int compCount = 0;
							foreach (KeyValuePair<Type, IComponent> component in entity.Value.Components)
							{
								label += component.Key.Name;

								if (compCount < entity.Value.Components.Count - 1)
									label += ", ";

								compCount++;
							}

							label += ")";

							EditorGUILayout.Foldout(true, label);

							EditorGUI.indentLevel++;

							foreach (KeyValuePair<Type, IComponent> component in entity.Value.Components)
							{
								EditorGUILayout.Foldout(true, component.Key.Name);
								EditorGUI.indentLevel++;

								if (component.Value.Get() == null) continue;

								switch (component.Value.Get().GetType().Name)
								{
									case "Int16":
									case "Int32":
										EditorGUILayout.SelectableLabel(((int) component.Value.Get()).ToString(), GUILayout.Height(15));
										break;
									case "Single":
									case "Double":
									case "Long":
										EditorGUILayout.SelectableLabel(((float) component.Value.Get()).ToString(CultureInfo.InvariantCulture), GUILayout.Height(15));
										break;
									case "Boolean":
										EditorGUILayout.Toggle(((bool) component.Value.Get()));
										break;
									case "String":
										EditorGUILayout.SelectableLabel((string) component.Value.Get(), GUILayout.Height(15));
										break;
									case "Vector3":
										Vector3 vector3 = (Vector3) component.Value.Get();
										EditorGUILayout.SelectableLabel($"x: {vector3.x}, y: {vector3.y}, z: {vector3.z}", GUILayout.Height(15));
										break;
									case "Vector2":
										Vector2 vector2 = (Vector2) component.Value.Get();
										EditorGUILayout.SelectableLabel($"x: {vector2.x}, y: {vector2.y}", GUILayout.Height(15));
										break;
									case "GameObject":
										EditorGUILayout.ObjectField((GameObject) component.Value.Get(), typeof(GameObject), true);
										break;
									default:
										EditorGUILayout.SelectableLabel(component.Value.Get().GetType().Name + " (Custom editor needed to display value)", GUILayout.Height(15));
										break;
								}

								EditorGUI.indentLevel--;
							}

							EditorGUI.indentLevel--;
						}
					}
				}

				EditorGUI.indentLevel = 0;
				currWorldFoldout++;
			}

			EditorGUILayout.EndScrollView();
		}
	}
}