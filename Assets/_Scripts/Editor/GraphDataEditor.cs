using System.Collections;
using System.Collections.Generic;
using _Scripts.Editor.UmlGraph;
using _Scripts.Editor.UmlGraph.Save;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UmlGraphSaveData))]
public class GraphDataEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if (GUILayout.Button("Open Graph"))
		{
			UmlGraph.graphData = target as UmlGraphSaveData;
			UmlGraph.OpenWindow();
		}
	}
}