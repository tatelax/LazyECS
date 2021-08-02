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
				GUIStyle worldFoldoutStyle = new GUIStyle(EditorStyles.foldout);

				if (world.Value.GetType().BaseType.Name == "NetworkWorld")
				{
					Color networkWorldFoldoutColor = new Color(0, 204, 255);
					worldFoldoutStyle.normal.textColor = networkWorldFoldoutColor;
					worldFoldoutStyle.onNormal.textColor = networkWorldFoldoutColor;
					worldFoldoutStyle.hover.textColor = networkWorldFoldoutColor;
					worldFoldoutStyle.onHover.textColor = networkWorldFoldoutColor;
					worldFoldoutStyle.focused.textColor = networkWorldFoldoutColor;
					worldFoldoutStyle.onFocused.textColor = networkWorldFoldoutColor;
					worldFoldoutStyle.active.textColor = networkWorldFoldoutColor;
					worldFoldoutStyle.onActive.textColor = networkWorldFoldoutColor;
				}
				
				string worldFoldoutName = world.Value.GetType().Name
				                          + "(ID: "
				                          + world.Key
				                          + ", Groups: "
				                          + world.Value.Groups.Count
				                          + ", Entities: "
				                          + world.Value.Entities.Count
				                          + ", Features: "
				                          + world.Value.Features.Length
				                          + ")";

				worldsFoldoutsState[currWorldFoldout] = EditorGUILayout.Foldout(worldsFoldoutsState[currWorldFoldout], worldFoldoutName, worldFoldoutStyle);

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
								GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);

								if (component.Key.GetInterface("INetworkComponent") != null)
								{
									Color networkComponentColor = new Color(0, 204, 255);
									foldoutStyle.normal.textColor = networkComponentColor;
									foldoutStyle.onNormal.textColor = networkComponentColor;
									foldoutStyle.hover.textColor = networkComponentColor;
									foldoutStyle.onHover.textColor = networkComponentColor;
									foldoutStyle.focused.textColor = networkComponentColor;
									foldoutStyle.onFocused.textColor = networkComponentColor;
									foldoutStyle.active.textColor = networkComponentColor;
									foldoutStyle.onActive.textColor = networkComponentColor;
								}

								string componentValue;

								if (component.Value.Get() == null)
									componentValue = "NULL";
								else
									componentValue = component.Value.Get().GetType().Name;

								string componentName;

								if (component.Key == null)
									componentName = "NULL";
								else
									componentName = component.Key.Name;	
								
								
								EditorGUILayout.Foldout(true, componentName + " (" + componentValue + ")", foldoutStyle);
								EditorGUI.indentLevel++;

								if (component.Value.Get() == null) continue;

								switch (component.Value.Get().GetType().Name)
								{
									case "UInt16":
									case "UInt32":
										EditorGUILayout.SelectableLabel(((uint) component.Value.Get()).ToString(), GUILayout.Height(15));
										break;
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
										EditorGUILayout.Toggle((bool) component.Value.Get());
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
									case "String[]":
										EditorGUILayout.SelectableLabel(CreateStringArrayDisplayLabel((string[])component.Value.Get()), GUILayout.Height(15));
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
	
	private string CreateStringArrayDisplayLabel(string[] data)
	{
		string labelValue = "";
										
		for (var i = 0; i < data.Length; i++)
		{
			labelValue += i + ": " + data[i];

			if (i != data.Length - 1)
				labelValue += ", ";
		}

		return labelValue;
	}
}