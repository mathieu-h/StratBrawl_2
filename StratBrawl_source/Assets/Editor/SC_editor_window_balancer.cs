using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml;

public class SC_editor_window_balancer : EditorWindow {

	private string _s_xml_name = "xml_balance";

	private int _i_pass_length;
	private int _i_ko_nb_actions;


	[MenuItem("StratBrawl/Balancer")]
	static void OpenWindow ()
	{
		SC_editor_window_balancer window = (SC_editor_window_balancer)EditorWindow.GetWindow (typeof (SC_editor_window_balancer));
		window.Init ();
	}

	private void Init()
	{
		TextAsset text_xml = (TextAsset)Resources.Load(_s_xml_name, typeof(TextAsset));
		XmlDocument _xml_behaviour_settings = new XmlDocument();
		_xml_behaviour_settings.LoadXml(text_xml.text);
		
		XmlNode node_root = _xml_behaviour_settings.FirstChild;
		
		_i_pass_length = int.Parse(node_root.SelectSingleNode("//pass_length").InnerText);
		_i_ko_nb_actions = int.Parse(node_root.SelectSingleNode("//ko_nb_actions").InnerText);
	}
	
	void OnGUI ()
	{
		GUILayout.Label ("Rotate Settings", EditorStyles.boldLabel);
		_i_pass_length = EditorGUILayout.IntSlider ("Pass length", _i_pass_length, 1, 100);
		_i_ko_nb_actions = EditorGUILayout.IntSlider ("KO duration (nb actions)", _i_ko_nb_actions, 1, 100);
		if (GUILayout.Button("Save"))
			SaveBalance();
	}

	private void SaveBalance()
	{
		TextAsset text_xml = (TextAsset)Resources.Load(_s_xml_name, typeof(TextAsset));
		XmlDocument _xml_behaviour_settings = new XmlDocument();
		_xml_behaviour_settings.LoadXml(text_xml.text);
		
		XmlNode node_root = _xml_behaviour_settings.FirstChild;
		node_root.SelectSingleNode("//pass_length").InnerText = _i_pass_length.ToString();
		node_root.SelectSingleNode("//ko_nb_actions").InnerText = _i_ko_nb_actions.ToString();

		string s_path = AssetDatabase.GetAssetPath(text_xml);
		_xml_behaviour_settings.Save(s_path);
	}

	private void CreateXml()
	{

	}
}
