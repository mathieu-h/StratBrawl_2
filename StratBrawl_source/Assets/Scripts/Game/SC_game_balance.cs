using UnityEngine;
using System.Collections;
using System.Xml;

public class SC_game_balance
{
	static private int i_pass_length = 5;
	static public int _i_pass_length { get{ return i_pass_length; } }

	static private int i_ko_nb_actions = 3;
	static public int _i_ko_nb_actions { get{ return i_ko_nb_actions; } }


	static public void Init()
	{
		TextAsset text_xml = (TextAsset)Resources.Load("xml_balance", typeof(TextAsset));
		XmlDocument _xml_behaviour_settings = new XmlDocument();
		_xml_behaviour_settings.LoadXml(text_xml.text);
		
		XmlNode node_root = _xml_behaviour_settings.FirstChild;
		
		i_pass_length = int.Parse(node_root.SelectSingleNode("//pass_length").InnerText);
		i_ko_nb_actions = int.Parse(node_root.SelectSingleNode("//ko_nb_actions").InnerText);
	}
}
