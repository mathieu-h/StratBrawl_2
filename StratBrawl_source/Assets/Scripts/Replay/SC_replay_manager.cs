using UnityEngine;
using System.Collections;

public class SC_replay_manager : MonoBehaviour {

	static public Replay _replay;

	[SerializeField]
	private SC_board_game _board_game;

	[SerializeField]
	private SC_manager_ui _manager_ui;

	private int _i_current_turn = 0;

	private int _i_score_team_true = 0;
	private int _i_score_team_false = 0;

	private bool _b_is_playing = false;
	private bool _b_is_asking_for_pause = false;


	void Start ()
	{
		_board_game.Init(_replay._game_settings);
		_board_game.SetGameFromSnap(_replay._start_game_snap);
		_manager_ui.InitRound(1, _replay._game_settings._i_nb_turn_max);
	}

	public void Play()
	{
		_b_is_asking_for_pause = false;
		if (!_b_is_playing)
			StartCoroutine(PlayRecord(_replay._record));
	}

	public void Pause()
	{
		_b_is_asking_for_pause = true;
	}

	public void PlayOneTurn()
	{
		if (!_b_is_playing)
			StartCoroutine(PlayOneTurn_Coroutine(_replay._record));
	}

	public void ReturnToStart()
	{
		StopAllCoroutines();
		_board_game.StopAnimation();
		_board_game.SetGameFromSnap(_replay._start_game_snap);
		_manager_ui.InitRound(1, _replay._game_settings._i_nb_turn_max);
		_b_is_playing = false;
		_i_current_turn = 0;
		_i_score_team_true = 0;
		_i_score_team_false = 0;
		_manager_ui.SetScore(false, 0);
		_manager_ui.SetScore(true, 0);
	}

	public void Quit()
	{
		Application.LoadLevel("Menu");
	}

	private IEnumerator PlayRecord(SimulationResult[][] _record)
	{
		_b_is_playing = true;

		if(_i_current_turn >= _record.Length)
			_b_is_asking_for_pause = true;

		while (!_b_is_asking_for_pause)
		{
			yield return StartCoroutine(_board_game.Animate(_record[_i_current_turn]));
			yield return new WaitForSeconds(1f);
			_i_current_turn++;
			if(_i_current_turn >= _record.Length)
				break;
			_manager_ui.UpdateRound(_i_current_turn + 1);
		}

		_b_is_playing = false;
	}

	private IEnumerator PlayOneTurn_Coroutine(SimulationResult[][] _record)
	{
		_b_is_playing = true;
		
		if(!(_i_current_turn >= _record.Length))
		{
			yield return StartCoroutine(_board_game.Animate(_record[_i_current_turn]));
			_i_current_turn++;
			if(!(_i_current_turn >= _record.Length))
				_manager_ui.UpdateRound(_i_current_turn + 1);
		}

		_b_is_playing = false;
	}

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
}
