using UnityEngine;
using System.Collections;

public partial class SC_board_game : MonoBehaviour {

	private int _i_gameField_width ;
	private int _i_gameField_height;
	
	[SerializeField]
	private Transform _T_root_cells_gameField;
	[SerializeField]
	private GameObject _GO_prefab_cell_gameField;

	[SerializeField]
	private Material _Mat_gamefield_border;
	[SerializeField]
	private Material _Mat_gamefield_center;
	[SerializeField]
	private Material _Mat_gamefield_score_zone_border;
	[SerializeField]
	private Material _Mat_gamefield_score_zone_center;

	public SC_cell[,] _cells_gameField;
	
	
	/// SUMMARY : Generate the gameField.
	/// PARAMETERS : Size of the gameField.
	/// RETURN : Void.
	public void GenerateGameField(int i_width, int i_height)
	{
		_i_gameField_width = i_width;
		_i_gameField_height = i_height;
		_cells_gameField = new SC_cell[i_width, i_height];
		
		for(int i = 0; i < i_width; i++)
		{
			for(int j = 0; j < i_height; j++)
			{
				GameObject GO_tmp = (GameObject) Instantiate(_GO_prefab_cell_gameField, new Vector3(i, j, 0f), Quaternion.identity);
				GO_tmp.transform.parent = _T_root_cells_gameField;
				_cells_gameField[i,j] = GO_tmp.GetComponent<SC_cell>();
				_cells_gameField[i,j].Init(new GridPosition(i, j));
				_cells_gameField[i,j]._T_graphic.renderer.sharedMaterial = _Mat_gamefield_center;
			}
		}

		for(int i = 1; i < i_width - 1; i++)
		{
			_cells_gameField[i,0]._T_graphic.renderer.sharedMaterial = _Mat_gamefield_border;
			_cells_gameField[i,0]._T_graphic.eulerAngles = Vector3.forward * 180;
			_cells_gameField[i,i_height - 1]._T_graphic.renderer.sharedMaterial = _Mat_gamefield_border;
		}

		for(int i = 1; i < i_height - 1; i++)
		{
			_cells_gameField[0,i]._T_graphic.renderer.sharedMaterial = _Mat_gamefield_score_zone_center;
			_cells_gameField[i_width - 1,i]._T_graphic.renderer.sharedMaterial = _Mat_gamefield_score_zone_center;
		}

		_cells_gameField[0, 0]._T_graphic.renderer.sharedMaterial = _Mat_gamefield_score_zone_border;
		_cells_gameField[0, 0]._T_graphic.eulerAngles = Vector3.forward * 180;
		_cells_gameField[i_width - 1, 0]._T_graphic.renderer.sharedMaterial = _Mat_gamefield_score_zone_border;
		_cells_gameField[i_width - 1, 0]._T_graphic.eulerAngles = Vector3.forward * 180;
		_cells_gameField[0, i_height - 1]._T_graphic.renderer.sharedMaterial = _Mat_gamefield_score_zone_border;
		_cells_gameField[i_width - 1, i_height - 1]._T_graphic.renderer.sharedMaterial = _Mat_gamefield_score_zone_border;



		_T_camera.position = new Vector3((i_width - 1) * 0.5f, (i_height - 1) * 0.5f, -10);

		float f_ortho_size = Mathf.Min((float)i_height * 0.5f, (float)i_width / (float)Screen.width * (float)Screen.height * 0.5f);
		f_ortho_size += f_ortho_size * 0.3f;
		_camera.orthographicSize = f_ortho_size;
	}


	/// SUMMARY : 
	/// PARAMETERS : 
	/// RETURN : 
	private GridPosition GetSymmetricPosition(GridPosition grid_position)
	{
		return new GridPosition(_i_gameField_width - grid_position._i_x -1 , grid_position._i_y);
	}
}
