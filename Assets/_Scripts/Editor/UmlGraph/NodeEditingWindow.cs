using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Scripts.Editor.UmlGraph
{

	public class NodeEditingWindow : EditorWindow
{

	[SerializeField] private VisualTreeAsset visualAsset = default;
    

	private EnumField classTypeField;
	private TextField classNameField;
	private TextField classNoteField;
	private TextField classElementField;
	private Button createButton;
	private Button closeButton;
	private  static UmlNode editedNode;
	
    
	public static void ShowEditWindow(UmlNode node)
	{
		editedNode = node;
		var window = GetWindow<NodeEditingWindow>();
		
		window.titleContent = new GUIContent($"Edit {node.ClassName} Node");
	}

	private void GetElements(VisualElement root)
	{
		var node = editedNode;
		classTypeField = root.Query<EnumField>("class-type");
		classTypeField.value = node.ClassType;

		classNameField = root.Query<TextField>("class-name");
		classNameField.value = node.ClassName;

		classNoteField = root.Query<TextField>("class-note");
		classNoteField.value = node.ClassNote;


		classElementField = root.Query<TextField>("class-elements");
		classElementField.value = node.ClassElements;

		closeButton = root.Query<Button>("close");
		createButton = root.Query<Button>("create");
	}

	public void CreateGUI()
	{
		VisualElement root = rootVisualElement;
		VisualElement uxml = visualAsset.Instantiate();
		root.Add(uxml);
		GetElements(root);
        Debug.Log("edit Mode");
        if (createButton != null)
        {
            createButton.clickable.clicked += () => { EditNode(); };
        }
		if (closeButton != null)
		{
			closeButton.clickable.clicked += Close;
		}
		Repaint();
	}
    
	private void EditNode()
	{
		Debug.Log("Edited");
		editedNode.Edit(classNameField.value, (ClassType)classTypeField.value, classNoteField.value, classElementField.value);
		Close();
	}
}

}