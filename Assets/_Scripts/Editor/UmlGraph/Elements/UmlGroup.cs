using System;
using _Scripts.Editor.UmlGraph.Save;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace _Scripts.Editor.UmlGraph.Elements
{

	public class UmlGroup : Group
	{
			public string ID { get; set; }

			private Color defaultBorderColor;
			private float defaultBorderWidth;

			public UmlGroupSaveData GetSaveData()
			{
				return new UmlGroupSaveData(ID,title,GetPosition().position);
			}

			public void SetGuid(string newGuid)
			{

				ID = newGuid;

			}
			public UmlGroup(string groupTitle, Vector2 position)
			{
				ID = Guid.NewGuid().ToString();
				title = groupTitle;

				SetPosition(new Rect(position, Vector2.zero));
				defaultBorderColor = contentContainer.style.borderBottomColor.value;
				defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
			}

			public void SetErrorStyle(Color color)
			{
				contentContainer.style.borderBottomColor = color;
				contentContainer.style.borderBottomWidth = 2f;
			}

			public void ResetStyle()
			{
				contentContainer.style.borderBottomColor = defaultBorderColor;
				contentContainer.style.borderBottomWidth = defaultBorderWidth;
			}
		
	}

}