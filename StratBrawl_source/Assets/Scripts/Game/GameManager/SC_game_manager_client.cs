using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public partial class SC_game_manager_client : MonoBehaviour {

	public SC_board_game _board_game;

	private int _i_score_team_true = 0;
	private int _i_score_team_false = 0;

	[SerializeField]
	private Camera _camera;
	[SerializeField]
	private Transform _T_camera;
	
	private NetworkView _network_view;
	[SerializeField]
	private SC_manager_ui _manager_ui;
	[SerializeField]
	private SC_ball _ball;
	
	public static SC_game_manager_client _instance;
	private bool _b_player_team;

	private GameSettings _game_settings;

	private byte[] _data_replay;
	private bool _b_relpay_is_already_saved = false;

	
	/// SUMMARY : Initialize th game manager.
	/// PARAMETERS : None.
	/// RETURN : Void.
	void Start()
	{
		_instance = this;
		_b_player_team = Network.isServer;
		_network_view = networkView;
	}


	[RPC]
	private void InitGame(byte[] _data_game_snap, byte[] _data_game_settings)
	{
		BinaryFormatter _BF = new BinaryFormatter();
		MemoryStream _MS = new MemoryStream();
		_MS.Write(_data_game_snap,0,_data_game_snap.Length); 
		_MS.Seek(0, SeekOrigin.Begin); 
		GameSnap _game_snap = (GameSnap)_BF.Deserialize(_MS);

		_BF = new BinaryFormatter();
		_MS = new MemoryStream();
		_MS.Write(_data_game_settings,0,_data_game_settings.Length); 
		_MS.Seek(0, SeekOrigin.Begin); 
		_game_settings = (GameSettings)_BF.Deserialize(_MS);

		_i_score_team_true = _game_snap._i_score_team_true;
		_i_score_team_false = _game_snap._i_score_team_false;

		_board_game.Init(_game_settings);
		_board_game.SetGameFromSnap(_game_snap);

		_manager_ui.SetScore(true, _i_score_team_true);
		_manager_ui.SetScore(false, _i_score_team_false);
		_manager_ui.EndTimer();

		Network.isMessageQueueRunning = true;
		if (Network.isClient)
			_network_view.RPC("ClientIsReadyToStart", RPCMode.Server);
	}

	
	/// SUMMARY : Start the planification phase on client side.
	/// PARAMETERS : None.
	/// RETURN : Void.
	[RPC]
	private void StartPlanification_Client()
	{
		_board_game.ResetActionsOfAllBrawlers();
		InitPlanification ();
	}

	
	/// SUMMARY : End the planification phase on client side.
	/// PARAMETERS : None.
	/// RETURN : Void.
	[RPC]
	private void EndPlanification_Client()
	{
		_board_game.SetActiveButtonsBrawlers(false, _b_player_team);
		RemoveAllActionDisplay();
		_manager_ui.EndTimer();
		if (Network.isClient)
		{
			byte[] _actions = GenerateActionsDataToSend();
			_network_view.RPC("SendActions", RPCMode.Server, _actions);
		}
	}

	
	/// SUMMARY : Generate, serialize and return data of the brawler's actions.
	/// PARAMETERS : None.
	/// RETURN : Return serialize data.
	private byte[] GenerateActionsDataToSend()
	{
		Action[,] _actions = new Action[_board_game._brawlers_team_false.Length, 3];
		for (int i = 0; i < _board_game._brawlers_team_false.Length; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				_actions[i, j] = _board_game._brawlers_team_false[i]._actions[j];
			}
		}
		
		BinaryFormatter _BF = new BinaryFormatter();
		MemoryStream _MS = new MemoryStream();
		_BF.Serialize(_MS, _actions);
		byte[] _data_actions = _MS.ToArray();
		_MS.Close();
		
		return _data_actions;
	}

	
	/// SUMMARY : Send the serialized data of the simulation result and deserialize it on client side.
	/// PARAMETERS : Serialed data of the simulation result.
	/// RETURN : Void.
	[RPC]
	private void SendResultOfSimulation(byte[] _data_simulation_result)
	{
		BinaryFormatter _BF = new BinaryFormatter();
		MemoryStream _MS = new MemoryStream();
		_MS.Write(_data_simulation_result,0,_data_simulation_result.Length); 
		_MS.Seek(0, SeekOrigin.Begin); 
		SimulationResult[] _simulation_result = (SimulationResult[])_BF.Deserialize(_MS);
		
		StartCoroutine(ResultAnimation(_simulation_result));
	}

	
	/// SUMMARY : Launch the result animation of the simulation and send Ready to the server when animation is done.
	/// PARAMETERS : Result of the simulation.
	/// RETURN : Void.
	private IEnumerator ResultAnimation(SimulationResult[] _simulation_result)
	{
		yield return StartCoroutine(_board_game.Animate(_simulation_result));
		
		if (Network.isClient)
		{
			_network_view.RPC("ClientIsReadyAnimation", RPCMode.Server);
		}
		else if (Network.isServer)
		{
			SC_game_manager_server._instance.ServerIsReadyAnimation();
		}
	}


	/// SUMMARY : Increment score of the team who score goal and update UI.
	/// PARAMETERS : The team who scores.
	/// RETURN : Void.
	private void IncrementScore(bool b_team)
	{
		if (b_team)
		{
			_i_score_team_true++;
			_manager_ui.SetScore(true, _i_score_team_true);
		}
		else
		{
			_i_score_team_false++;
			_manager_ui.SetScore(false, _i_score_team_false);
		}
	}


	/// SUMMARY :
	/// PARAMETERS : 
	/// RETURN : Void.
	[RPC]
	private void EndGame(byte[] data_replay)
	{
		_data_replay = data_replay;

		GameResult game_result;
		if (_i_score_team_true > _i_score_team_false)
		{
			if (_b_player_team)
				game_result = GameResult.Win;
			else
				game_result = GameResult.Lose;
		}
		else if (_i_score_team_true < _i_score_team_false)
		{
			if (_b_player_team)
				game_result = GameResult.Lose;
			else
				game_result = GameResult.Win;
		}
		else
		{
			game_result = GameResult.Draw;
		}
		_manager_ui.SetEnd(game_result);
	}


	public void SaveReplay()
	{
		if (!_b_relpay_is_already_saved)
		{
			_b_relpay_is_already_saved = true;
			SC_replay_files_manager.SaveReplay(_data_replay);
		}
	}


	public void GoToMenu()
	{
		Application.LoadLevel("Menu");
	}
}
