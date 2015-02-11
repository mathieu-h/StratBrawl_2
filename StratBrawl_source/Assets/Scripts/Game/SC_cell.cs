using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SC_cell : MonoBehaviour {

	[SerializeField]
	public GameObject _GO_button_canvas;
	[SerializeField]
	public GameObject _GO_button;
	[SerializeField]
	public Image _IMG_action_display;
	[SerializeField]
	public Transform _T_graphic;

	public GridPosition _position;
	public Transform _T_cell { get; private set;}
	[HideInInspector]
	public SC_brawler _brawler_on_the_cell;
	[HideInInspector]
	public bool _b_is_ball_on_this_cell = false;



	/// SUMMARY : Initialize the cell position data of the terrain grid.
	/// PARAMETERS : Position of the cell.
	/// RETURN : Void.
	public void Init(GridPosition position)
	{
		_T_cell = transform;
		_position = position;
		_T_cell.position = position.GetWorldPosition();
	}

	public void RegisterSelectedCellForAction(){
		SC_game_manager_client._instance.RegisterSelectedCellPositionForAction (this);
	}

	public void UpdateActionSlotText(){
		SC_game_manager_client._instance.UpdateActionSlotText ();
	}
}
