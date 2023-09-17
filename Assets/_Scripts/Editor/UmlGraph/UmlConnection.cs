using System.Collections.Generic;
using _Scripts.Editor.UmlGraph.Save;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public enum ConnectionType
{
	Association,
	Inheritance,
	Implementation,
	Dependency,
	Aggregation,
	Composition
}

public class UmlConnection
{
	public UmlNode firstNode;
	public UmlNode finalNode;
	public ConnectionLine connectionLine;
	public ConnectionType connectionType;
	public Dictionary<ConnectionType, Color> connectionsColor = new()
	{
		{ ConnectionType.Association, Color.white },
		{ ConnectionType.Inheritance, Color.red },
		{ ConnectionType.Implementation, new Color(1, 94 / 255f, 5 / 255f, 1) },
		{ ConnectionType.Dependency, Color.yellow },
		{ ConnectionType.Aggregation, Color.green },
		{ ConnectionType.Composition, Color.blue }
	};

	public List<UmlNode> Nodes => new() { firstNode, finalNode };

	public UmlConnectionSaveData GetSaveData()
	{
		return new UmlConnectionSaveData(firstNode.Guid,finalNode.Guid,connectionType);
	}

	public UmlConnection(UmlNode node1, UmlNode node2, ConnectionType connectionType)
	{
		firstNode = node1;
		finalNode = node2;
		this.connectionType = connectionType;
	}

	public void Repaint()
	{
		connectionLine.generateVisualContent = OnGenerateVisualContent;
		connectionLine.MarkDirtyRepaint();
		return;

		void OnGenerateVisualContent(MeshGenerationContext cxt)
		{
			Painter2D painter = cxt.painter2D;
			painter.strokeColor = connectionsColor[connectionType];
			painter.lineJoin = LineJoin.Round;
			painter.lineCap = LineCap.Round;
			painter.lineWidth = 5.0f;

			painter.BeginPath();
			painter.MoveTo(firstNode.GetPosition().center);
			painter.LineTo(finalNode.GetPosition().center);
			
			painter.Stroke();
		}
	}
}