using UnityEngine;
using System.Collections;

public class SC_menu_tmp : MonoBehaviour {

	private int i_Index = 0;
	
	private string s_ServerIP;
	private string s_MyServerIP = "";

	private float f_BoutonPositionX;
	private float f_BoutonSizeX;
	private float f_BoutonSizeY;


	void Start ()
	{
		Network.isMessageQueueRunning = false;
		s_MyServerIP = Network.player.ipAddress;
		s_ServerIP = s_MyServerIP;

		f_BoutonPositionX = Screen.width * 0.5f - Screen.height * 0.4f;
		f_BoutonSizeX = Screen.height * 0.8f;
		f_BoutonSizeY = Screen.height * 0.2f;
	}
	
	void OnGUI ()
	{
		switch (i_Index)
		{
		case 0:
			if (GUI.Button(new Rect(f_BoutonPositionX, Screen.height * 0.4f, f_BoutonSizeX, f_BoutonSizeY), "Create Game"))
			{
				Network.InitializeServer(1, 25002, false);
				i_Index = 1;
			}
			if (GUI.Button(new Rect(f_BoutonPositionX, Screen.height * 0.65f, f_BoutonSizeX, f_BoutonSizeY), "Join Game"))
			{
				i_Index = 2;
			}
			break;
		case 1:
			GUI.Label(new Rect(f_BoutonPositionX, Screen.height * 0.15f, f_BoutonSizeX, f_BoutonSizeY), s_MyServerIP);
			GUI.Label(new Rect(f_BoutonPositionX, Screen.height * 0.4f, f_BoutonSizeX, f_BoutonSizeY), "Waiting for a player");
			if (GUI.Button(new Rect(f_BoutonPositionX, Screen.height * 0.65f, f_BoutonSizeX, f_BoutonSizeY), "Back to menu"))
			{
				Network.Disconnect();
				i_Index = 0;
			}
			break;
		case 2:
			s_ServerIP = GUI.TextField(new Rect(f_BoutonPositionX, Screen.height * 0.15f, f_BoutonSizeX, f_BoutonSizeY), s_ServerIP);
			if (GUI.Button(new Rect(f_BoutonPositionX, Screen.height * 0.4f, f_BoutonSizeX, f_BoutonSizeY), "Connect"))
			{
				Network.Connect(s_ServerIP, 25002);
				i_Index = 3;
			}
			if (GUI.Button(new Rect(f_BoutonPositionX, Screen.height * 0.65f, f_BoutonSizeX, f_BoutonSizeY), "Back to menu"))
			{
				i_Index = 0;
			}
			break;
		case 3:
			GUI.Label(new Rect(f_BoutonPositionX, Screen.height * 0.4f, f_BoutonSizeX, f_BoutonSizeY), "Connecting to server");
			if (GUI.Button(new Rect(f_BoutonPositionX, Screen.height * 0.65f, f_BoutonSizeX, f_BoutonSizeY), "Back to menu"))
			{
				Network.Disconnect();
				i_Index = 0;
			}
			break;
		}
	}

	void OnPlayerConnected(NetworkPlayer _connectedPlayer)
	{
		Application.LoadLevel(1);
	}
	
	void OnConnectedToServer()	
	{
		Application.LoadLevel(1);
	}
}
