using UnityEngine;
using System.Collections;

public class SC_replay_manager : MonoBehaviour {

	static public Replay _replay;

	[SerializeField]
	SC_board_game _board_game;

	[SerializeField]
	SC_manager_ui _manager_ui;

	private int _i_score_team_true = 0;
	private int _i_score_team_false = 0;


	void Start ()
	{
		_board_game.Init(_replay._game_settings);
		_board_game.SetGameFromSnap(_replay._start_game_snap);

		_manager_ui.InitRound(1, _replay._game_settings._i_nb_turn_max);

		StartCoroutine(PlayRecord(_replay._record));
	}

	private IEnumerator PlayRecord(SimulationResult[][] _record)
	{
		yield return new WaitForSeconds(2f);
		for (int i = 0; i < _record.GetLength(0); ++i)
		{
			yield return StartCoroutine(_board_game.Animate(_record[i]));
			yield return new WaitForSeconds(1f);
			_manager_ui.UpdateRound(i + 1);
		}
		yield return new WaitForSeconds(2f);
		Application.LoadLevel("Menu");
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
