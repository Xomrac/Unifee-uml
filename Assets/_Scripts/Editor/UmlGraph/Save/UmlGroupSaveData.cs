using System;
using UnityEngine;

namespace _Scripts.Editor.UmlGraph.Save
{

	[Serializable]
	public class UmlGroupSaveData
	{
		[field: SerializeField] public string Name { get; set; }
		[field: SerializeField] public string Guid { get; set; }
		[field: SerializeField] public Vector2 Position { get; set; }

		public UmlGroupSaveData(string guid, string name, Vector2 position)
		{
			Guid = guid;
			Name = name;
			Position = position;
		}
	}

}