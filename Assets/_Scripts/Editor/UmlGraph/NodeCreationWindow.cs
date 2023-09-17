using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class NodeCreationWindow : EditorWindow
{

	[SerializeField] private VisualTreeAsset visualAsset = default;

	private static UmlGraphView graphView;

	private EnumField classTypeField;
	private TextField classNameField;
	private TextField classNoteField;
	private TextField classElementField;
	private Button createButton;
	private Button closeButton;

	private static Vector2 clickPosition;

	public static void ShowCreationWindow(UmlGraphView graph, Vector2 clickPos = default)
	{
		var window = GetWindow<NodeCreationWindow>();
		window.titleContent = new GUIContent("New Node");
		clickPosition = clickPos;
		graphView = graph;
	}

	private void GetElements(VisualElement root)
	{
		classTypeField = root.Query<EnumField>("class-type");
		classTypeField.value = ClassType.Monobehaviour;

		classNameField = root.Query<TextField>("class-name");
		classNameField.value = "";

		classNoteField = root.Query<TextField>("class-note");
		classNoteField.value = "";


		classElementField = root.Query<TextField>("class-elements");
		classElementField.value = "";

		closeButton = root.Query<Button>("close");
		createButton = root.Query<Button>("create");
	}

	public void CreateGUI()
	{
		VisualElement root = rootVisualElement;
		VisualElement uxml = visualAsset.Instantiate();
		root.Add(uxml);
		GetElements(root);

		Debug.Log("create Mode");
		if (createButton != null)
		{
			createButton.clickable.clicked += () => { CreateNode(clickPosition); };
		}


		if (closeButton != null)
		{
			closeButton.clickable.clicked += Close;
		}
	}

	private void CreateNode(Vector2 mousePos)
	{
		var newNode = new UmlNode();
		graphView.graphNodes.Add(newNode);
		graphView.AddElement(newNode);
		newNode.Init(graphView, mousePos, classNameField.value, (ClassType)classTypeField.value, classNoteField.value, classElementField.value);
		Close();
	}

}