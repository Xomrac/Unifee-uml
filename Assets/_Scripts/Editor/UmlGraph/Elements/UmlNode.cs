using System;
using System.Text.RegularExpressions;
using _Scripts.Editor.UmlGraph;
using _Scripts.Editor.UmlGraph.Elements;
using _Scripts.Editor.UmlGraph.Save;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public enum ClassType
{
	Monobehaviour,
	ScriptableObject,
	Normal
}

public class UmlNode : Node
{
	private static readonly Regex PUBLIC_PATTERN = new(@"\b(?:Public|public|PUBLIC)\b");
	private static readonly Regex PRIVATE_PATTERN = new(@"\b(?:Private|private|PRIVATE)\b");
	private static readonly Regex PROTECTED_PATTERN = new(@"\b(?:Protected|protected|PROTECTED)\b");
	private const string SEPARATOR_STRING = "---";

	private string guid;
	public string Guid => guid;

	private string className;
	public string ClassName => className;

	private string classElements;
	public string ClassElements => classElements;

	private string classNote;
	public string ClassNote => classNote;

	public UmlGroup group;

	private ClassType classType;
	public ClassType ClassType => classType;

	private VisualElement titleBackground;
	public VisualElement TitleBackground => titleBackground;

	private VisualElement classIcon;
	public VisualElement ClassIcon => classIcon;

	private Label noteLabel;
	public Label NoteLabel => noteLabel;

	private Label titleLabel;
	public Label TitleLabel => titleLabel;

	private VisualTreeAsset elementTemplate;
	private VisualTreeAsset separator;

	private UmlGraphView graphView;

	private ClickSelector clickSelector;

	public UmlNodeSaveData GetSaveData()
	{
		var groupId = group != null ? group.ID : "";
		return new UmlNodeSaveData(guid, className, classElements, classNote, groupId, classType, GetPosition().position);
	}
	public UmlNode() : base("Assets/Editor Default Resources/Uml Graph/UmlNodeView.uxml")
	{
		elementTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor Default Resources/Uml Graph/Element.uxml");
		separator = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor Default Resources/Uml Graph/Separator.uxml");
		focusable = true;
		pickingMode = PickingMode.Position;
		titleBackground = mainContainer.Query("title");
		noteLabel = mainContainer.Query<Label>("note");
		classIcon = mainContainer.Query("title-icon");
		titleLabel = mainContainer.Query<Label>("title-label");
	}
	public void SetGuid(string newGuid)
	{
		guid = newGuid;
	}
	

	public void Init(UmlGraphView graph, Vector2 position, string nodeName, ClassType type, string note = "", string elements = "")
	{
		
		SetPosition(new Rect(position, Vector2.zero));
		graphView = graph;
		
		RegisterCallback<MouseDownEvent>(_ =>
		{
			if ((graph as UmlGraphView).selectedNode==this|| (graph as UmlGraphView).selectedNode==null)
			{
				return;
			}
			var connection = new UmlConnection((graph as UmlGraphView).selectedNode, this);
			(graph as UmlGraphView).CreateConnection(connection);
		});
		Color MONOBEHAVIOUR_COLOR = new(253 / 255f, 208 / 255f, 23 / 255f, 1);
		Color SCRIPTABLEOBJECT_COLOR = new(152 / 255f, 103 / 255f, 197 / 255f, 1);
		Color NORMAL_COLOR = new(89 / 255f, 125 / 255f, 53 / 255f, 1);

		guid = System.Guid.NewGuid().ToString();
		className = nodeName;
		classType = type;
		classElements = elements;
		classNote = note;
		titleLabel.text = className;

		switch (classType)
		{
			case ClassType.Monobehaviour:
				titleBackground.style.backgroundColor = MONOBEHAVIOUR_COLOR;
				GUIContent monoIcon = EditorGUIUtility.IconContent("Prefab Icon");
				classIcon.style.backgroundImage = (Background)monoIcon.image;

				break;
			case ClassType.ScriptableObject:
				titleBackground.style.backgroundColor = SCRIPTABLEOBJECT_COLOR;

				GUIContent scriptableIcon = EditorGUIUtility.IconContent("ScriptableObject Icon");
				classIcon.style.backgroundImage = (Background)scriptableIcon.image;

				break;
			case ClassType.Normal:
				titleBackground.style.backgroundColor = NORMAL_COLOR;
				GUIContent csIcon = EditorGUIUtility.IconContent("cs Script Icon");
				classIcon.style.backgroundImage = (Background)csIcon.image;
				break;
		}

		noteLabel.text = note;
		AddElements();
		graphView.SaveGraph();

	}

	public void Edit(string nodeName, ClassType type, string note = "", string elements = "")
	{
		Color MONOBEHAVIOUR_COLOR = new(253 / 255f, 208 / 255f, 23 / 255f, 1);
		Color SCRIPTABLEOBJECT_COLOR = new(152 / 255f, 103 / 255f, 197 / 255f, 1);
		Color NORMAL_COLOR = new(89 / 255f, 125 / 255f, 53 / 255f, 1);

		className = nodeName;
		classType = type;
		classElements = elements;
		classNote = note;

		title = className;
		titleBackground.style.backgroundColor = new StyleColor(classType switch
		{
			ClassType.Monobehaviour => MONOBEHAVIOUR_COLOR,
			ClassType.ScriptableObject => SCRIPTABLEOBJECT_COLOR,
			ClassType.Normal => NORMAL_COLOR,
			_ => throw new ArgumentOutOfRangeException()
		});
		noteLabel.text = note;
		AddElements();
		graphView.SaveGraph();
	}

	private void AddElements()
	{
		if (string.IsNullOrEmpty(classElements)) return;
		VisualElement elementsParent = mainContainer.Query("elements");
		elementsParent.Clear();

		string[] elements = classElements.Split("\n");
		foreach (string element in elements)
		{
			if (element == SEPARATOR_STRING)
			{
				var separatorElement = separator.Instantiate();
				separatorElement.style.paddingTop = 2;
				separatorElement.style.paddingLeft = 4;
				separatorElement.style.paddingRight = 4;
				separatorElement.style.paddingBottom = 2;
				elementsParent.Add(separatorElement);
				continue;
			}
			TemplateContainer template = elementTemplate.Instantiate();
			elementsParent.Add(template);
			Label label = template.contentContainer.Query<Label>("element-text");
			VisualElement iconRect = template.contentContainer.Query<VisualElement>("element-icon");
			iconRect.generateVisualContent += DrawCircles;
			if (CheckPatterns(element))
			{
				
				string displayedString = element[(element.Split()[0].Length + 1)..];
				label.text = displayedString;
			}
			else
			{
				label.text = element;
			}

			bool CheckPatterns(string s)
			{
				return PUBLIC_PATTERN.IsMatch(s) || PRIVATE_PATTERN.IsMatch(s) || PROTECTED_PATTERN.IsMatch(s);
			}

			void DrawCircles(MeshGenerationContext ctx)
			{
				var rect = ctx.visualElement.contentRect;
				var radius = rect.height >= rect.width ? rect.width / 2 : rect.height / 2;
				Painter2D painter = ctx.painter2D;
				if (PUBLIC_PATTERN.IsMatch(element))
				{
					painter.fillColor = Color.green;
					Debug.Log("public");
				}
				else if (PRIVATE_PATTERN.IsMatch(element))
				{
					painter.fillColor = Color.red;
					Debug.Log("private");
				}
				else if (PROTECTED_PATTERN.IsMatch(element))
				{
					painter.fillColor = new Color(255 / 255f, 140 / 255f, 0, 1);
					Debug.Log("protected");
				}
				else
				{
					Debug.Log("Nothing");

					return;
				}
				painter.BeginPath();
				painter.Arc(ctx.visualElement.contentRect.center, radius, 0.0f, 360.0f);
				painter.Fill();
			}
		}
	}
	
	

	public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
	{
		base.BuildContextualMenu(evt);

		(graphView as UmlGraphView).selectedNode = this;
		Debug.Log((graphView as UmlGraphView).selectedNode);
		if (evt.target is UmlNode)
		{
			evt.menu.AppendAction("Deb node", _ => Debug.Log(this));
			evt.menu.AppendAction("Edit Node", _ =>
			{
				Debug.Log((graphView as UmlGraphView).selectedNode);
				NodeEditingWindow.ShowEditWindow((graphView as UmlGraphView).selectedNode);
			});
			evt.menu.AppendAction("Print Pos",_=> Debug.Log(GetPosition().position));
			evt.menu.AppendAction("Connections/Create Connection", _ =>
			{
				(graphView as UmlGraphView).selectedNode = this;
				Debug.Log((graphView as UmlGraphView).selectedNode.className);
			});
			evt.menu.AppendAction("Connections/Remove All Connections", _ => Debug.Log("Remove All Connections"));
		}
	}
}