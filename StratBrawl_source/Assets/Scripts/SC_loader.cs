using UnityEngine;
using System.Collections;

public class SC_loader : MonoBehaviour {

	void Awake()
	{
		SC_game_balance.Init();
		Application.LoadLevel("Menu");
	}
}
