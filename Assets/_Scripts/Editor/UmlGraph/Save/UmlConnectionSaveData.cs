using System;
using UnityEngine;

namespace _Scripts.Editor.UmlGraph.Save
{

	[Serializable]
	
	public class UmlConnectionSaveData
	{
		[field: SerializeField] public string Node1Guid { get; set; }
		[field: SerializeField] public string Node2Guid { get; set; }

		public UmlConnectionSaveData(string node1Guid, string node2Guid)
		{
			Node1Guid = node1Guid;
			Node2Guid = node2Guid;
		}
		
	}

}