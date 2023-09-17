using System.Collections.Generic;
using _Scripts.Editor.UmlGraph.Save;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class UmlConnection
{
	public UmlNode firstNode;
	public UmlNode finalNode;
	public ConnectionLine connectionLine;

	public List<UmlNode> Nodes => new() { firstNode, finalNode };

	public UmlConnectionSaveData GetSaveData()
	{
		return new UmlConnectionSaveData(firstNode.Guid,finalNode.Guid);
	}

	public UmlConnection(UmlNode node1, UmlNode node2)
	{
		firstNode = node1;
		finalNode = node2;
	}

	public void Repaint()
	{
		connectionLine.generateVisualContent = OnGenerateVisualContent;
		connectionLine.MarkDirtyRepaint();
		return;

		void OnGenerateVisualContent(MeshGenerationContext cxt)
		{
			Painter2D painter = cxt.painter2D;
			painter.strokeColor = Color.white;
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