using UnityEngine;
using System.Collections;

public class SC_brawler : MonoBehaviour {

	public GameObject _GO_button_brawler;
	public SC_animation _animation;

	[HideInInspector]
	public int _i_index { get; private set; }
	public int _i_index_in_team { get; private set; }
	public GridPosition _position { get; private set; }

	public Transform _T_brawler { get; private set; }

	public bool _b_team { get; private set; }
	public bool _b_have_the_ball = false;
	public bool _b_is_KO = false;
	public int _i_KO_round_remaining = 0;

	[HideInInspector]
	public Action[] _actions;

	[SerializeField]
	public Material _Mat_team_true;
	[SerializeField]
	public Material _Mat_team_false;
	[SerializeField]
	public Material _Mat_team_true_ball;
	[SerializeField]
	public Material _Mat_team_false_ball;
	[SerializeField]
	public Material _Mat_highligth_ball;
	[SerializeField]
	public Material _Mat_highligth;


	/// SUMMARY : Initialize the brawler.
	/// PARAMETERS : Index in the brawlers array. Index of the brawler in his team brawlers array. His team.
	/// RETURN : Void.
	public void Init(int i_index, int i_index_in_team, bool b_team)
	{
		_T_brawler = transform;
		_i_index = i_index;
		_i_index_in_team = i_index_in_team;
		_b_team = b_team;
		renderer.material = b_team ? _Mat_team_true : _Mat_team_false;
		_T_brawler.Rotate(new Vector3(0,0, b_team ? 90 : 270));
		//sprite
		_GO_button_brawler.SetActive(false);
		InitActions(3);
	}

	public void ResetMat(){
		if (_b_have_the_ball) {			
			renderer.material = _b_team ? _Mat_team_true_ball : _Mat_team_false_ball;
		} else {
			renderer.material = _b_team ? _Mat_team_true : _Mat_team_false;
		}
		if (_b_is_KO)
			renderer.material.color = new Color(0.4f, 0.4f, 0.4f, 1f);
	}

	public void HighLightBrawler(){		
		if (_b_have_the_ball) {			
			renderer.material = _Mat_highligth_ball;
		} else {
			renderer.material = _Mat_highligth;
		}
	}

	/// SUMMARY : Init Actions array.
	/// PARAMETERS : Number of actions.
	/// RETURN : Void.
	private void InitActions(int i_nb_actions)
	{
		_actions = new Action[i_nb_actions];
		for(int i = 0; i < i_nb_actions; i++)
		{
			_actions[i] = new Action();
			_actions[i].SetNone();
		}
	}

	/// SUMMARY : Set the position of the brawlers.
	/// PARAMETERS : The position in the terrain grid.
	/// RETURN : Void.
	public void SetPosition(GridPosition position)
	{
		_position = position;
		_T_brawler.position = position.GetWorldPosition() + Vector3.back;
	}

	/// SUMMARY : 
	/// PARAMETERS : None.
	/// RETURN : Void.
	public void OpenMenuActions()
	{		
		SC_game_manager_client._instance.OpenMenuActionsSlots(this);
	}
}
