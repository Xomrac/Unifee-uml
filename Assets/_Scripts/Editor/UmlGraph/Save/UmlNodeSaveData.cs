using System;
using _Scripts.Editor.UmlGraph.Elements;
using UnityEngine;

namespace _Scripts.Editor.UmlGraph.Save
{

	[Serializable]
	public class UmlNodeSaveData
	{
		
		[field: SerializeField]public string ClassName { get; set; }
		[field: SerializeField,OnlyReadable] public string Guid { get; set; }
		[field: SerializeField,TextArea]public string ClassElements { get; set; }
		[field: SerializeField]public string ClassNote { get; set; }
		[field: SerializeField]public string ClassGroup { get; set; }
		[field: SerializeField]public ClassType ClassType { get; set; }
		[field: SerializeField,OnlyReadable]public Vector2 Position { get; set; }

		public UmlNodeSaveData(string guid, string className, string classElements, string classNote, string classGroup, ClassType classType, Vector2 position)
		{
			Guid = guid;
			ClassName = className;
			ClassElements = classElements;
			ClassNote = classNote;
			ClassGroup = classGroup;
			ClassType = classType;
			Position = position;
		}
	}

}