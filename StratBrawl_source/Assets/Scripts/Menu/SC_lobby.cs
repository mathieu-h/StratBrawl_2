﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// SUMMARY : This script runs when the current panel is lobby.
public class SC_lobby : MonoBehaviour {

	[SerializeField]
	private Text _TE_ServerIP;

	void Start()
	{
		_TE_ServerIP.text = Network.player.ipAddress;
	}

	/// SUMMARY : (Server side) A new player has just connected. It means there are 2 players in the game, we can load the game.
	/// PARAMETERS : The player.
	/// RETURN : Void.
	void OnPlayerConnected(NetworkPlayer player){
		Application.LoadLevel ("Game");
	}

	/// SUMMARY : (Client side) A new player has just connected. It means there are 2 players in the game, we can load the game.
	/// PARAMETERS : None.
	/// RETURN : Void.
	void OnConnectedToServer()	
	{
		Application.LoadLevel("Game");
	}

}
