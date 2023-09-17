using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using _Scripts.Editor.UmlGraph;
using _Scripts.Editor.UmlGraph.Save;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UmlGraph : EditorWindow
{
    private UmlGraphView graphView;
    public static UmlGraphSaveData graphData;
    public static void OpenWindow()
    {
        var window = GetWindow<UmlGraph>();
        Debug.Log(graphData);
        window.titleContent = new GUIContent(graphData.FileName);
    }

    private void OnEnable()
    {
        AddGraph();
        AddStyles();
    }
    
    private void AddStyles()
    {
        var styleSheet = (StyleSheet)EditorGUIUtility.Load("Uml Graph/UmlGraphVariables.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
    }

    private void AddGraph()
    {
        Debug.Log(graphData);
        graphView = new UmlGraphView(graphData)
        {
            name = graphData.FileName
        };
        // graphView.SetData(graphData);
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }
}
