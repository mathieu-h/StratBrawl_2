using UnityEngine;
using System.Collections;

public class SC_replay_manager : MonoBehaviour {

	static public Replay _replay;

	[SerializeField]
	SC_board_game _board_game;


	void Start ()
	{
		_board_game.Init(_replay._game_settings);
		_board_game.SetGameFromSnap(_replay._start_game_snap);
		StartCoroutine(PlayRecord(_replay._record));
	}

	private IEnumerator PlayRecord(SimulationResult[][] _record)
	{
		yield return new WaitForSeconds(2f);
		for (int i = 0; i < _record.GetLength(0); ++i)
		{
			yield return StartCoroutine(_board_game.Animate(_record[i]));
			yield return new WaitForSeconds(1f);
		}
		yield return new WaitForSeconds(2f);
		Application.LoadLevel("Menu");
	}
}
