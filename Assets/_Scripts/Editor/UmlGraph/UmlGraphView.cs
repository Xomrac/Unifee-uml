using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Editor.UmlGraph;
using _Scripts.Editor.UmlGraph.Elements;
using _Scripts.Editor.UmlGraph.Save;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class UmlGraphView : GraphView
{

	private UmlGraphSaveData graphData;
	public UmlGraphSaveData GraphData => graphData;

	public UmlNode selectedNode;

	private VisualElement background;
	private List<UmlGroup> groups;
	public List<UmlNode> graphNodes;
	public List<UmlConnection> connections;
	public bool connectionDeleteOn = false;
	public ConnectionType currentConnectionType;

	public void SaveGraph()
	{
		foreach (UmlGroup group in groups)
		{
			IEnumerable<GraphElement> elements = group.containedElements;
			List<GraphElement> enumerable = elements.ToList();
			if (!enumerable.Any())
			{
				continue;
			}
			Type nodeType = typeof(UmlNode);
			foreach (UmlNode node in enumerable.Where(graphElement => graphElement.GetType() == nodeType).Cast<UmlNode>())
			{
				node.group = group;
			}
		}
		var groupDatas = new List<UmlGroupSaveData>();
		if (groups.Any())
		{
			groupDatas.AddRange(groups.Select(group => group.GetSaveData()).ToList());
		}
		var nodesDatas = new List<UmlNodeSaveData>();
		if (graphNodes.Any())
		{
			nodesDatas.AddRange(graphNodes.Select(node => node.GetSaveData()).ToList());
		}
		var connectionsDatas = new List<UmlConnectionSaveData>();
		if (connections.Any())
		{
			connectionsDatas.AddRange(connections.Select(connection => connection.GetSaveData()).ToList());
		}
		graphData.SaveDatas(groupDatas, nodesDatas, connectionsDatas);
	}

	public UmlGraphView(UmlGraphSaveData data)
	{
		graphData = data;
		Debug.Log(data);
		groups = new List<UmlGroup>();
		graphNodes = new List<UmlNode>();
		connections = new List<UmlConnection>();
		SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
		this.AddManipulator(new ContentDragger());
		this.AddManipulator(new SelectionDragger());
		this.AddManipulator(new RectangleSelector());
		this.AddManipulator(CreateGroupContextualMenu());
		AddBackground();
		AddStyles();
		OnElementsDeleted();
		OnGraphViewChanged();
		SetupElements();
	}

	private IManipulator CreateGroupContextualMenu()
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
			menuEvent => menuEvent.menu.AppendAction("Add group", actionEvent => AddElement(CreateGroup("Package", actionEvent.eventInfo.localMousePosition))));

		return contextualMenuManipulator;
	}

	private UmlGroup CreateGroup(string title, Vector2 eventInfoLocalMousePosition)
	{
		var group = new UmlGroup(title, eventInfoLocalMousePosition);
		groups.Add(group);
		foreach (GraphElement element in selection)
		{
			if (element is not UmlNode)
			{
				continue;
			}
			UmlNode node = element as UmlNode;
			node.group = group;
			group.AddElement(node);
		}
		return group;
	}

	public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
	{
		base.BuildContextualMenu(evt);

		if (evt.target is UmlGraphView)
		{
			evt.menu.AppendAction("Create New Node", actionEvent => NodeCreationWindow.ShowCreationWindow(this, actionEvent.eventInfo.localMousePosition));
		}
	}

	public void SetData(UmlGraphSaveData data)
	{
		Debug.Log(data);
		Debug.Log(UmlGraph.graphData);
		graphData = data;
		SetupElements();
	}

	private void SetupElements()
	{
		if (graphData.Groups.Count > 0)
		{
			foreach (UmlGroupSaveData groupData in graphData.Groups)
			{
				var group = new UmlGroup(groupData.Name, groupData.Position);
				group.SetGuid(groupData.Guid);
				groups.Add(group);
				AddElement(group);
			}
		}
		if (graphData.Nodes.Count > 0)
		{
			foreach (UmlNodeSaveData nodeData in graphData.Nodes)
			{
				var newNode = new UmlNode();
				newNode.Init(this, nodeData.Position, nodeData.ClassName, nodeData.ClassType, nodeData.ClassNote, nodeData.ClassElements);
				newNode.SetGuid(nodeData.Guid);
				graphNodes.Add(newNode);
				AddElement(newNode);
				if (string.IsNullOrEmpty(nodeData.ClassGroup))
				{
					continue;
				}
				foreach (UmlGroup group in groups.Where(group => group.ID == nodeData.ClassGroup))
				{
					group.AddElement(newNode);
				}
			}
		}

		if (graphData.Connections.Count > 0)
		{
			foreach (UmlConnectionSaveData connectionData in graphData.Connections)
			{
				UmlNode firstNode = graphNodes.Find(node => node.Guid == connectionData.Node1Guid);
				UmlNode secondNode = graphNodes.Find(node => node.Guid == connectionData.Node2Guid);
				if (firstNode == null || secondNode == null)
				{
					continue;
				}
				var connection = new UmlConnection(firstNode, secondNode,connectionData.ConnectionType);
				var lineElement = new ConnectionLine
				{
					name = $"{firstNode.title} to {secondNode.title}",
					style =
					{
						backgroundColor = new StyleColor(new Color(1, 1, 1, 0))
					},
					pickingMode = PickingMode.Position
				};
				contentViewContainer.Insert(0, lineElement);
				lineElement.StretchToParentSize();
				lineElement.generateVisualContent = OnGenerateVisualContent;
				lineElement.MarkDirtyRepaint();
				connection.connectionLine = lineElement;
				connections.Add(connection);
				selectedNode = null;
				return;

				void OnGenerateVisualContent(MeshGenerationContext cxt)
				{
					Painter2D painter = cxt.painter2D;
					painter.strokeColor = connection.connectionsColor[connectionData.ConnectionType];
					painter.lineJoin = LineJoin.Round;
					painter.lineCap = LineCap.Round;
					painter.lineWidth = 5.0f;

					painter.BeginPath();
					painter.MoveTo(connection.firstNode.GetPosition().center);
					painter.LineTo(connection.finalNode.GetPosition().center);

					painter.Stroke();
				}
			}
		}
	}
	
	public void DeleteConnection(UmlNode clickedNode)
	{
		for (int i = connections.Count - 1; i >= 0; i--)
		{
			UmlConnection currentConnection = connections[i];
			if (currentConnection.Nodes.Contains(selectedNode) && currentConnection.Nodes.Contains(clickedNode))
			{
				connections.Remove(currentConnection);
				contentViewContainer.Remove(currentConnection.connectionLine);
			}
		}
		selectedNode = null;
		SaveGraph();
	}

	public void DeleteConnections()
	{
		for (int i = connections.Count - 1; i >= 0; i--)
		{
			UmlConnection currentConnection = connections[i];
			if (currentConnection.Nodes.Contains(selectedNode))
			{
				connections.Remove(currentConnection);
				contentViewContainer.Remove(currentConnection.connectionLine);
			}
		}
		selectedNode = null;
		SaveGraph();
	}
	
	// public void DeleteConnection()

	public void CreateConnection(UmlConnection umlConnection)
	{
		var lineElement = new ConnectionLine
		{
			name = $"{umlConnection.firstNode.title} to {umlConnection.finalNode.title}",
			style =
			{
				backgroundColor = new StyleColor(new Color(1, 1, 1, 0))
			},
			pickingMode = PickingMode.Position
		};
		contentViewContainer.Insert(0, lineElement);
		lineElement.StretchToParentSize();
		lineElement.generateVisualContent = OnGenerateVisualContent;
		lineElement.MarkDirtyRepaint();
		umlConnection.connectionLine = lineElement;
		connections.Add(umlConnection);
		selectedNode = null;
		SaveGraph();
		return;

		void OnGenerateVisualContent(MeshGenerationContext cxt)
		{
			Painter2D painter = cxt.painter2D;
			painter.strokeColor = umlConnection.connectionsColor[umlConnection.connectionType];
			painter.lineJoin = LineJoin.Round;
			painter.lineCap = LineCap.Round;
			painter.lineWidth = 5.0f;

			painter.BeginPath();
			painter.MoveTo(umlConnection.firstNode.GetPosition().center);
			painter.LineTo(umlConnection.finalNode.GetPosition().center);

			painter.Stroke();
		}
	}

	private void AddStyles()
	{
		StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Uml Graph/UmlGraphViewStyle.uss");
		styleSheets.Add(styleSheet);
	}

	private void AddBackground()
	{
		background = new GridBackground();
		background.StretchToParentSize();
		Insert(0, background);
	}

	private void OnGraphViewChanged()
	{
		graphViewChanged = changes =>
		{
			if (changes.movedElements != null)
			{
				Type nodeType = typeof(UmlNode);
				foreach (GraphElement graphElement in changes.movedElements)
				{
					if (graphElement.GetType() != nodeType)
					{
						continue;
					}
					var node = (UmlNode)graphElement;
					foreach (UmlConnection connection in connections.Where(connection => connection.Nodes.Contains(node)))
					{
						connection.Repaint();
					}
				}
			}


			SaveGraph();
			return changes;
		};
	}

	private void OnElementsDeleted()
	{
		deleteSelection = (_, _) =>
		{
			Type edgeType = typeof(Edge);
			Type groupType = typeof(UmlGroup);

			var nodesToDelete = new List<UmlNode>();
			var connectionsToDelete = new List<UmlConnection>();
			var groupsToDelete = new List<UmlGroup>();

			foreach (ISelectable selectable in selection)
			{
				var selectedElement = (GraphElement)selectable;
				switch (selectedElement)
				{
					case UmlNode node:
						nodesToDelete.Add(node);
						continue;
					case UmlGroup group:
						groupsToDelete.Add(group);
						continue;
				}
			}
			Debug.Log($"NODES TO DELETE: {nodesToDelete.Count}");
			Debug.Log($"GROUPS TO DELETE: {groupsToDelete.Count}");

			

			foreach (UmlGroup groupToDelete in groupsToDelete)
			{
				var groupNodes = new List<UmlNode>();

				foreach (GraphElement groupElement in groupToDelete.containedElements)
				{
					if (!(groupElement is UmlNode))
					{
						continue;
					}

					var groupNode = (UmlNode)groupElement;

					groupNodes.Add(groupNode);
				}

				groupToDelete.RemoveElements(groupNodes);

				groups.Remove(groupToDelete);

				RemoveElement(groupToDelete);
			}

			foreach (UmlNode nodeToDelete in nodesToDelete)
			{
				foreach (UmlConnection connection in connections)
				{
					if (connection.Nodes.Contains(nodeToDelete))
					{
						connectionsToDelete.Add(connection);
					}
				}
				graphNodes.Remove(nodeToDelete);
				RemoveElement(nodeToDelete);
			}

			foreach (UmlConnection connection in connectionsToDelete)
			{
				connections.Remove(connection);
				RemoveElement(connection.connectionLine);
			}

			SaveGraph();
		};
	}

}