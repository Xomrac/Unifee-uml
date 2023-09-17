using System;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Editor.UmlGraph.Save
{

	[Serializable]
	[CreateAssetMenu(fileName = "UmlGraph_", menuName = "Xomrac's Labs/UML/New Graph")]
	public class UmlGraphSaveData : ScriptableObject
	{
		[field: SerializeField]
		public List<UmlGroupSaveData> Groups { get; set; } = new();
		[field: SerializeField]
		public List<UmlNodeSaveData> Nodes { get; set; } = new();
		[field: SerializeField]
		public List<UmlConnectionSaveData> Connections { get; set; } = new();

		private string fileName;
		public string FileName => fileName;

		private void OnValidate()
		{
			fileName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
		}

		public void SaveDatas(IEnumerable<UmlGroupSaveData> groups, IEnumerable<UmlNodeSaveData> nodes, IEnumerable<UmlConnectionSaveData> connections)
		{
			Groups = new List<UmlGroupSaveData>(groups);
			Nodes = new List<UmlNodeSaveData>(nodes);
			Connections = new List<UmlConnectionSaveData>(connections);
			AssetDatabase.Refresh();
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}
	}

}