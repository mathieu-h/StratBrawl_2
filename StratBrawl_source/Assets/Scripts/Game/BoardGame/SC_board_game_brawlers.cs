using UnityEngine;
using System.Collections;

public partial class SC_board_game : MonoBehaviour {

	[SerializeField]
	private Transform _T_root_brawlers;
	[SerializeField]
	private GameObject _GO_prefab_brawler;
	
	private SC_brawler[] _brawlers;
	[HideInInspector]
	public SC_brawler[] _brawlers_team_true;
	[HideInInspector]
	public SC_brawler[] _brawlers_team_false;
	
	
	/// SUMMARY : Generate brawlers of both teams and store them in arrays.
	/// PARAMETERS : Number of Brawlers per teams.
	/// RETURN : Void.
	public void GenerateBrawlers(int i_nb_brawlers_per_team)
	{
		_brawlers = new SC_brawler[i_nb_brawlers_per_team * 2];
		_brawlers_team_true = new SC_brawler[i_nb_brawlers_per_team];
		_brawlers_team_false = new SC_brawler[i_nb_brawlers_per_team];
		
		for (int i = 0; i < i_nb_brawlers_per_team * 2; i++)
		{
			GameObject GO_tmp = (GameObject) Instantiate(_GO_prefab_brawler);
			GO_tmp.transform.parent = _T_root_brawlers;
			_brawlers[i] = GO_tmp.GetComponent<SC_brawler>();
			bool b_team;
			int i_index_in_team;
			if (i < i_nb_brawlers_per_team)
			{
				b_team = true;
				i_index_in_team = i;
				_brawlers_team_true[i] = _brawlers[i];
			}
			else
			{
				b_team = false;
				i_index_in_team = i - i_nb_brawlers_per_team;
				_brawlers_team_false[i_index_in_team] = _brawlers[i];
			}
			_brawlers[i].Init(i, i_index_in_team, b_team);
		}
	}


	/// SUMMARY : Set positions of both teams brawlers for the engage.
	/// PARAMETERS : Arrays of positions of the team with the ball and the team without the ball. And wich team have the ball. 
	/// RETURN : Void.
	private void SetBrawlersEngagePositions(GridPosition[] _positions_brawlers_team_with_ball, GridPosition[] _positions_brawlers_team_without_ball, bool b_team_with_ball)
	{
		for (int i = 0; i < _brawlers.Length; i++)
		{
			int i_index_in_team = _brawlers[i]._i_index_in_team;
			GridPosition _position = _brawlers[i]._b_team == b_team_with_ball ? _positions_brawlers_team_with_ball[i_index_in_team] : _positions_brawlers_team_without_ball[i_index_in_team];
			if (!_brawlers[i]._b_team)
				_position = GetSymmetricPosition(_position);
			_brawlers[i].SetPosition(_position);
			_brawlers[i]._b_is_KO = false;
			_brawlers[i].renderer.material.color = new Color(1f, 1f, 1f, 1f);
		}
	}


	/// SUMMARY : Set arrays of action to Action.None for all brawlers.
	/// PARAMETERS : None.
	/// RETURN : Void.
	public void ResetActionsOfAllBrawlers()
	{
		for (int i = 0; i < _brawlers.Length; i++)
		{
			// TODO _game_settings._i_nb_actions_per_turn;
			for (int j = 0; j < 3; j++)
			{
				_brawlers[i]._actions[j].SetNone();
			}
		}
	}


	/// SUMMARY : Return the brawler captain (inex 0 in team array) of the team.
	/// PARAMETERS : Team of the captain we want.
	/// RETURN : Return the brawler captain (inex 0 in team array) of the team.
	private SC_brawler GetTeamCaptain(bool b_team)
	{
		return b_team ? _brawlers_team_true[0] : _brawlers_team_false[0];
	}


	/// SUMMARY : Active or unactive buttons to select brawlers
	/// PARAMETERS : Boolean to choose "Active" or "Unactive", and the team of the brawlers which buttons need to be set.
	/// RETURN : Void.
	public void SetActiveButtonsBrawlers(bool b_active, bool b_team)
	{
		for (int i = 0; i < _brawlers.Length; i++)
		{
			if (_brawlers[i]._b_team == b_team)
				_brawlers[i]._GO_button_brawler.SetActive(b_active);
		}
	}
}
