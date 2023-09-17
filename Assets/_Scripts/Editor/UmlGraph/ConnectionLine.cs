using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ConnectionLine : GraphElement
{
	public ConnectionLine()
	{
		capabilities |= Capabilities.Selectable | Capabilities.Deletable;
	}
}