using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class SC_game_manager_server {

	[SerializeField]
	private SO_game_settings _game_settings;

	private int _i_score_team_true = 0;
	private int _i_score_team_false = 0;
	private int _i_turn = 0;

	private BallData _ball_data;
	private BrawlersData _brawlers_data;
	private TerrainData _terrain_data;

	private GameSnap _game_snap_start;
	private List<SimulationResult[]> _record;

	private class TerrainData
	{
		public int _i_terrain_width;
		public int _i_terrain_height;
		public int[,] _i_position_brawlers_current;
		public int[,] _i_position_brawlers_prevision;
		
		public TerrainData(int i_terrain_width, int i_terrain_height)
		{
			_i_terrain_width = i_terrain_width;
			_i_terrain_height = i_terrain_height;
			_i_position_brawlers_current = new int[i_terrain_width, i_terrain_height];
			_i_position_brawlers_prevision = new int[i_terrain_width, i_terrain_height];
			for (int i = 0; i < i_terrain_width; i++)
			{
				for (int j = 0; j < i_terrain_height; j++)
				{
					_i_position_brawlers_current[i, j] = -1;
					_i_position_brawlers_prevision[i, j] = -1;
				}
			}
		}
		
		public void SetBrawlersPositionCurrentInTerrain(BrawlersData brawlers_data)
		{
			_i_position_brawlers_current = new int[_i_terrain_width, _i_terrain_height];
			for (int i = 0; i < _i_terrain_width; i++)
			{
				for (int j = 0; j < _i_terrain_height; j++)
				{
					_i_position_brawlers_current[i, j] = -1;
					_i_position_brawlers_prevision[i, j] = -1;
				}
			}
			for (int i = 0; i < brawlers_data._i_nb_brawlers; i++)
			{
				_i_position_brawlers_current[brawlers_data._brawlers[i]._position_current._i_x, brawlers_data._brawlers[i]._position_current._i_y] = i;
			}
		}
		
		public void SetBrawlerPositionPrevisionInTerrain(BrawlerData brawler_data)
		{
			_i_position_brawlers_prevision[brawler_data._position_prevision._i_x, brawler_data._position_prevision._i_y] = brawler_data._i_index;
		}
		
		public void CancelPrevisionAtPosition(GridPosition position)
		{
			_i_position_brawlers_prevision[position._i_x, position._i_y] = -1;
		}
		
		public bool IsInsideTheTerrain(GridPosition position)
		{
			if (position._i_x < 0
			    || position._i_x >= _i_terrain_width
			    || position._i_y < 0
			    || position._i_y >= _i_terrain_height)
				return false;
			else
				return true;
		}

		public GridPosition GetSymmetricPosition(GridPosition grid_position)
		{
			return new GridPosition(_i_terrain_width - grid_position._i_x -1 , grid_position._i_y);
		}
		
		public int GetPrevisionBrawlerIndexAtPosition(GridPosition position)
		{
			if (IsInsideTheTerrain(position))
				return _i_position_brawlers_prevision[position._i_x, position._i_y];
			else
				return -1;
		}

		public int GetCurrentBrawlerIndexAtPosition(GridPosition position)
		{
			if (IsInsideTheTerrain(position))
				return _i_position_brawlers_current[position._i_x, position._i_y];
			else
				return -1;
		}

		public int GetFirstBrawlerIndexOnTrajectory(GridPosition position_start, GridPosition position_target)
		{
			GridPosition position_current = position_start;
			float f_distance_max = Vector2.Distance(position_start.ToVector2(), position_target.ToVector2());
			float f_distance_current = 0f;
			Vector2 V2_current_position = position_start.ToVector2();
			Vector2 V2_directory = (position_target - position_start).ToVector2();
			V2_directory.Normalize();

			float f_next_border_x;
			if (V2_directory.x > 0)
				f_next_border_x = position_current._i_x + 0.5f;
			else
				f_next_border_x = position_current._i_x - 0.5f;

			float f_next_border_y;
			if (V2_directory.y > 0)
				f_next_border_y = position_current._i_y + 0.5f;
			else
				f_next_border_y = position_current._i_y - 0.5f;

			while (position_current != position_target && f_distance_current < f_distance_max)
			{
				float f_factor_next_cell_x = (f_next_border_x - V2_current_position.x) / V2_directory.x;
				float f_factor_next_cell_y = (f_next_border_y - V2_current_position.y) / V2_directory.y;

				if (V2_directory.x == 0)
					f_factor_next_cell_x = 1000;
				if (V2_directory.y == 0)
					f_factor_next_cell_y = 1000;

				if (f_factor_next_cell_x <= f_factor_next_cell_y)
				{
					V2_current_position = new Vector2(f_next_border_x, V2_current_position.y + V2_directory.y * f_factor_next_cell_x);
					if (V2_directory.x > 0)
					{
						position_current._i_x ++;
						f_next_border_x = position_current._i_x + 0.5f;
					}
					else
					{
						position_current._i_x --;
						f_next_border_x = position_current._i_x - 0.5f;
					}
				}

				if (f_factor_next_cell_y <= f_factor_next_cell_x)
				{
					V2_current_position = new Vector2( V2_current_position.x + V2_directory.x * f_factor_next_cell_y, f_next_border_y);
					if (V2_directory.y > 0)
					{
						position_current._i_y ++;
						f_next_border_y = position_current._i_y + 0.5f;
					}
					else
					{
						position_current._i_y --;
						f_next_border_y = position_current._i_y - 0.5f;
					}
				}

				f_distance_current = Vector2.Distance(position_start.ToVector2(), V2_current_position);

				int i_brawler = GetCurrentBrawlerIndexAtPosition(position_current);
				if (i_brawler != -1)
					return i_brawler;

				i_brawler = GetPrevisionBrawlerIndexAtPosition(position_current);
				if (i_brawler != -1)
					return i_brawler;
			}

			return -1;
		}
		
		public void EndOfIteration()
		{
			for (int i = 0; i < _i_terrain_width; i++)
			{
				for (int j = 0; j < _i_terrain_height; j++)
				{
					_i_position_brawlers_current[i, j] = _i_position_brawlers_prevision[i, j];
					_i_position_brawlers_prevision[i, j] = -1;
				}
			}
		}
	}
	
	
	private class BrawlersData
	{
		public int _i_nb_brawlers;
		public int _i_nb_brawler_per_team;
		public BrawlerData[] _brawlers;
		
		public BrawlersData(SO_game_settings game_settings)
		{
			_i_nb_brawlers = game_settings._settings._i_nb_brawlers_per_team * 2;
			_i_nb_brawler_per_team = game_settings._settings._i_nb_brawlers_per_team;
			_brawlers = new BrawlerData[_i_nb_brawlers];
			for (int i = 0; i < _i_nb_brawlers; i++)
			{
				_brawlers[i] = new BrawlerData();
				_brawlers[i]._i_index = i;
				_brawlers[i]._b_team = i < game_settings._settings._i_nb_brawlers_per_team;
				_brawlers[i]._position_current = _brawlers[i]._b_team ? game_settings._settings._positions_brawlers_attack_formation[i] : game_settings._settings._positions_brawlers_defense_formation[i - game_settings._settings._i_nb_brawlers_per_team];
			}
		}

		public void SetActions(Action[,] actions_team_true, Action[,] actions_team_false)
		{
			for (int i = 0; i < _i_nb_brawler_per_team; i++)
			{
				for (int j = 0; j < 3; ++j)
				{
					_brawlers[i]._actions[j] = actions_team_true[i, j];
				}
			}

			for (int i = 0; i < _i_nb_brawler_per_team; i++)
			{
				for (int j = 0; j < 3; ++j)
				{
					_brawlers[i + _i_nb_brawler_per_team]._actions[j] = actions_team_false[i, j];
				}
			}
		}
		
		public void EndOfIteration()
		{
			for (int i = 0; i < _i_nb_brawlers; i++)
			{
				_brawlers[i].EndOfIteration();
			}
		}

		public BrawlerSimulationResult[] ToResult(int i_iteration)
		{
			BrawlerSimulationResult[] brawlers_simulation_result = new BrawlerSimulationResult[_i_nb_brawlers];
			for (int i = 0; i < _i_nb_brawlers; i++)
			{
				if (_brawlers[i]._actions[i_iteration]._action_type == ActionType.Tackle)
					brawlers_simulation_result[i] = new BrawlerSimulationResult(_brawlers[i]._actions[i_iteration]._action_type, _brawlers[i]._position_current + GridPosition.DirectionToGridPosition(_brawlers[i]._actions[i_iteration]._direction_move), _brawlers[i]._b_is_KO_current);
				else
					brawlers_simulation_result[i] = new BrawlerSimulationResult(_brawlers[i]._actions[i_iteration]._action_type, _brawlers[i]._position_current, _brawlers[i]._b_is_KO_current);
			}
			return brawlers_simulation_result;
		}
	}
	
	
	private class BrawlerData
	{
		public int _i_index;
		public bool _b_team;
		public Action[] _actions = new Action[3];
		public GridPosition _position_current = new GridPosition(-1, -1);
		public GridPosition _position_prevision = new GridPosition(-1, -1);
		public bool _b_have_the_ball_current = false;
		public bool _b_have_the_ball_prevision = false;
		public bool _b_is_KO_current = false;
		public bool _b_is_KO_prevision = false;
		public int _i_KO_round_remaining = 0;
		
		public void EndOfIteration()
		{
			_position_current = _position_prevision;
			_position_prevision = new GridPosition(-1, -1);
			_b_have_the_ball_current = _b_have_the_ball_prevision;
			if (_b_is_KO_prevision)
			{
				_b_is_KO_current = _b_is_KO_prevision;
				_b_is_KO_prevision = false;
				_i_KO_round_remaining = 3;
			}
			else if (_i_KO_round_remaining > 0)
				_i_KO_round_remaining--;

			if (_i_KO_round_remaining == 0)
				_b_is_KO_current = false;
		}
	}
	
	
	private class BallData
	{
		public BallStatus _ball_status_current;
		public BallStatus _ball_status_prevision;
		public int _i_brawler_with_the_ball_current;
		public int _i_brawler_with_the_ball_prevision;
		public GridPosition _position_on_ground_current;
		public GridPosition _position_on_ground_prevision;

		
		public BallData()
		{
			_ball_status_current = BallStatus.Null;
			_ball_status_prevision = BallStatus.Null;
			_i_brawler_with_the_ball_current = -1;
			_i_brawler_with_the_ball_prevision = -1;
			_position_on_ground_current = new GridPosition(-1, -1);
			_position_on_ground_prevision = new GridPosition(-1, -1);
		}

		public void SetOnBrawlerPrevision(int i_brawler_index)
		{
			_ball_status_prevision = BallStatus.OnBrawler;
			_i_brawler_with_the_ball_prevision = i_brawler_index;
			_position_on_ground_prevision = new GridPosition(-1, -1);
		}

		public void SetOnGroundPrevision(GridPosition position)
		{
			_ball_status_prevision = BallStatus.OnGround;
			_i_brawler_with_the_ball_prevision = -1;
			_position_on_ground_prevision = position;
		}
		
		public void EndOfIteration()
		{
			_ball_status_current = _ball_status_prevision;
			_i_brawler_with_the_ball_current = _i_brawler_with_the_ball_prevision;
			_position_on_ground_current = _position_on_ground_prevision;
		}

		public BallSimulationResult ToResult()
		{
			return new BallSimulationResult(_ball_status_current, _i_brawler_with_the_ball_current, _position_on_ground_current);
		}
	}
	

	public void InitSimulation()
	{
		_brawlers_data = new BrawlersData(_game_settings);

		_terrain_data = new TerrainData(_game_settings._settings._i_game_field_width, _game_settings._settings._i_game_field_height);

		_ball_data = new BallData();

		SetBrawlersEngagePositions(true);

		_record = new List<SimulationResult[]>(_game_settings._settings._i_nb_turn_max);
		_game_snap_start = SimulationDataToGameSnap();
	}

	
	/// SUMMARY : Simulate brawlers action.
	/// PARAMETERS : 
	/// RETURN : Return the results.
	public SimulationResult[] StartSimulation(Action[,] actions_team_true, Action[,] actions_team_false)
	{
		int i_nb_iteration = 3;
		SimulationResult[] simulation_result = new SimulationResult[i_nb_iteration];

		_brawlers_data.SetActions(actions_team_true, actions_team_false);

		// Make n simulation, which n is the number of action per brawler.
		for (int i = 0; i < i_nb_iteration; i++)
		{
			// Loop in brawler to simulate move action.
			for (int j = 0; j < _brawlers_data._i_nb_brawlers; j++)
			{
				// Don't simulate action if the brawler is KO
				if (!_brawlers_data._brawlers[j]._b_is_KO_current)
				{
					switch (_brawlers_data._brawlers[j]._actions[i]._action_type)
					{
					case ActionType.Move:
						// If action is a move, try to set the new position as prevision position
						GridPosition direction = GridPosition.DirectionToGridPosition(_brawlers_data._brawlers[j]._actions[i]._direction_move);
						GridPosition position_to_test = _brawlers_data._brawlers[j]._position_current + direction;
						SetPrevisionPosition(_brawlers_data._brawlers[j], position_to_test);
						break;

					default:
						// If action is not a move, set current position as prevision position
						SetPrevisionPosition(_brawlers_data._brawlers[j], _brawlers_data._brawlers[j]._position_current);
						break;
					}
				}
				else
				{
					// If brawler is KO, set current position as prevision position
					SetPrevisionPosition(_brawlers_data._brawlers[j], _brawlers_data._brawlers[j]._position_current);
					_brawlers_data._brawlers[j]._actions[i].SetNone();
				}
			}

			DebugPositions(_terrain_data);

			// Loop in brawler to simulate pass action.
			for (int j = 0; j < _brawlers_data._i_nb_brawlers; j++)
			{
				if (_brawlers_data._brawlers[j]._b_have_the_ball_current)
				{
					if (!_brawlers_data._brawlers[j]._b_is_KO_current
					    && _brawlers_data._brawlers[j]._actions[i]._action_type == ActionType.Pass)
					{
						_brawlers_data._brawlers[j]._b_have_the_ball_prevision = false;

						int i_brawler_on_trajectory = _terrain_data.GetFirstBrawlerIndexOnTrajectory(_brawlers_data._brawlers[j]._position_current, _brawlers_data._brawlers[j]._actions[i]._position);
						if (i_brawler_on_trajectory != -1)
						{
							_brawlers_data._brawlers[i_brawler_on_trajectory]._b_have_the_ball_prevision = true;
							_ball_data.SetOnBrawlerPrevision(i_brawler_on_trajectory);
						}
						else
						{
							_ball_data.SetOnGroundPrevision(_brawlers_data._brawlers[j]._actions[i]._position);
						}
					}
					else
					{
						_brawlers_data._brawlers[j]._b_have_the_ball_prevision = true;
						_ball_data._i_brawler_with_the_ball_prevision = j;
					}
				}
			}

			// If the ball is grounded, verify if a brawler move on the ball
			if (_ball_data._ball_status_prevision == BallStatus.OnGround)
			{
				int i_brawler = _terrain_data.GetPrevisionBrawlerIndexAtPosition(_ball_data._position_on_ground_prevision);
				if (i_brawler >= 0 && i_brawler < _brawlers_data._i_nb_brawlers)
				{
					_ball_data.SetOnBrawlerPrevision(i_brawler);
					_brawlers_data._brawlers[i_brawler]._b_have_the_ball_prevision = true;
				}
			}

			// Loop in brawler to simulate tackle action.
			for (int j = 0; j < _brawlers_data._i_nb_brawlers; j++)
			{
				if (!_brawlers_data._brawlers[j]._b_is_KO_current
				    && _brawlers_data._brawlers[j]._actions[i]._action_type == ActionType.Tackle)
				{
					GridPosition direction = GridPosition.DirectionToGridPosition(_brawlers_data._brawlers[j]._actions[i]._direction_move);
					GridPosition position_to_tackle = _brawlers_data._brawlers[j]._position_current + direction;
					TackleBrawler(j, _terrain_data.GetPrevisionBrawlerIndexAtPosition(position_to_tackle));
					TackleBrawler(j, _terrain_data.GetCurrentBrawlerIndexAtPosition(position_to_tackle));
					Debug.Log (_terrain_data.GetPrevisionBrawlerIndexAtPosition(position_to_tackle));
					Debug.Log (_terrain_data.GetCurrentBrawlerIndexAtPosition(position_to_tackle));
				}
			}

			_terrain_data.EndOfIteration();
			_brawlers_data.EndOfIteration();
			_ball_data.EndOfIteration();

			// Create resulte of the iteration of the simulation.
			BrawlerSimulationResult[] brawlers_simulation_result = _brawlers_data.ToResult(i);
			BallSimulationResult _ball_simulation_result = _ball_data.ToResult();

			// Verify if someone scores goal
			bool _b_is_goal = false;
			bool _b_team_who_scores = false;
			if (_ball_data._ball_status_current == BallStatus.OnBrawler)
			{
				if (_brawlers_data._brawlers[_ball_data._i_brawler_with_the_ball_current]._b_team == true && _brawlers_data._brawlers[_ball_data._i_brawler_with_the_ball_current]._position_current._i_x == (_terrain_data._i_terrain_width - 1))
				{
					_b_is_goal = true;
					_b_team_who_scores = true;
					++_i_score_team_true;
				}
				else if (_brawlers_data._brawlers[_ball_data._i_brawler_with_the_ball_current]._b_team == false && _brawlers_data._brawlers[_ball_data._i_brawler_with_the_ball_current]._position_current._i_x == 0)
				{
					_b_is_goal = true;
					_b_team_who_scores = false;
					++_i_score_team_false;
				}
			}

			// Add iteration result to the simulation
			simulation_result[i] = new SimulationResult(_ball_simulation_result, brawlers_simulation_result, _b_is_goal, _b_team_who_scores);

			// If someone scores goal, skip next iteration.
			if (_b_is_goal)
			{
				SetBrawlersEngagePositions(!_b_team_who_scores);
				break;
			}
		}

		_record.Add(simulation_result);

		return simulation_result;
	}
	

	private void SetPrevisionPosition(BrawlerData brawler, GridPosition position)
	{
		if (!_terrain_data.IsInsideTheTerrain(position))
		{
			brawler._position_prevision = brawler._position_current;
			_terrain_data.SetBrawlerPositionPrevisionInTerrain(brawler);
			return;
		}

		int i_brawler_prevision_at_current_position = _terrain_data.GetPrevisionBrawlerIndexAtPosition(brawler._position_current);
		int i_brawler_current_at_prevision_position = _terrain_data.GetCurrentBrawlerIndexAtPosition(position);
		if (i_brawler_prevision_at_current_position != -1
		    && i_brawler_prevision_at_current_position == i_brawler_current_at_prevision_position)
		{
			_terrain_data.CancelPrevisionAtPosition(_brawlers_data._brawlers[i_brawler_prevision_at_current_position]._position_prevision);
			SetPrevisionPosition(_brawlers_data._brawlers[i_brawler_prevision_at_current_position], _brawlers_data._brawlers[i_brawler_prevision_at_current_position]._position_current);
			
			brawler._position_prevision = brawler._position_current;
			_terrain_data.SetBrawlerPositionPrevisionInTerrain(brawler);
			return;
		}

		int i_brawler_at_prevision_position = _terrain_data.GetPrevisionBrawlerIndexAtPosition(position);
		if (i_brawler_at_prevision_position != -1)
		{
			_terrain_data.CancelPrevisionAtPosition(_brawlers_data._brawlers[i_brawler_at_prevision_position]._position_prevision);
			SetPrevisionPosition(_brawlers_data._brawlers[i_brawler_at_prevision_position], _brawlers_data._brawlers[i_brawler_at_prevision_position]._position_current);

			brawler._position_prevision = brawler._position_current;
			_terrain_data.SetBrawlerPositionPrevisionInTerrain(brawler);
			return;
		}

		brawler._position_prevision = position;
		_terrain_data.SetBrawlerPositionPrevisionInTerrain(brawler);
	}

	private void TackleBrawler(int i_brawler_from, int i_brawler_to_tackle)
	{
		if (i_brawler_to_tackle != -1)
		{
			_brawlers_data._brawlers[i_brawler_to_tackle]._b_is_KO_prevision = true;
			if (_brawlers_data._brawlers[i_brawler_to_tackle]._b_have_the_ball_prevision)
			{
				_brawlers_data._brawlers[i_brawler_to_tackle]._b_have_the_ball_prevision = false;
				_brawlers_data._brawlers[i_brawler_from]._b_have_the_ball_prevision = true;
				_ball_data._i_brawler_with_the_ball_prevision = i_brawler_from;
			}
		}
	}

	private void SetBrawlersEngagePositions(bool b_team_with_ball)
	{
		for (int i = 0; i < _brawlers_data._brawlers.Length; i++)
		{
			int i_index_in_team = i % _brawlers_data._i_nb_brawler_per_team;
			GridPosition _position = _brawlers_data._brawlers[i]._b_team == b_team_with_ball ? _game_settings._settings._positions_brawlers_attack_formation[i_index_in_team] : _game_settings._settings._positions_brawlers_defense_formation[i_index_in_team];
			if (!_brawlers_data._brawlers[i]._b_team)
				_position = _terrain_data.GetSymmetricPosition(_position);
			_brawlers_data._brawlers[i]._position_current = _position;
			_brawlers_data._brawlers[i]._position_prevision = _position;
			_brawlers_data._brawlers[i]._b_is_KO_current = false;
			_brawlers_data._brawlers[i]._b_is_KO_prevision = false;
			_brawlers_data._brawlers[i]._b_have_the_ball_current = false;
			_brawlers_data._brawlers[i]._b_have_the_ball_prevision = false;
		}

		_ball_data._ball_status_current = BallStatus.OnBrawler;
		_ball_data._ball_status_prevision = BallStatus.OnBrawler;
		if (b_team_with_ball)
		{
			_brawlers_data._brawlers[0]._b_have_the_ball_current = true;
			_ball_data._i_brawler_with_the_ball_current = 0;
			_ball_data._i_brawler_with_the_ball_prevision = 0;
		}
		else
		{
			_brawlers_data._brawlers[_brawlers_data._i_nb_brawler_per_team]._b_have_the_ball_current = true;
			_ball_data._i_brawler_with_the_ball_current = _brawlers_data._i_nb_brawler_per_team;
			_ball_data._i_brawler_with_the_ball_prevision = _brawlers_data._i_nb_brawler_per_team;
		}

		_terrain_data.SetBrawlersPositionCurrentInTerrain(_brawlers_data);
	}

	private GameSnap SimulationDataToGameSnap()
	{
		GameSnap game_snap = new GameSnap();

		game_snap._i_score_team_true = _i_score_team_true;
		game_snap._i_score_team_false = _i_score_team_false;

		game_snap._brawlers = new BrawlerSnap[_brawlers_data._i_nb_brawlers];
		for (int i = 0; i < _brawlers_data._i_nb_brawlers; ++i)
		{
			game_snap._brawlers[i]._position = _brawlers_data._brawlers[i]._position_current;
			game_snap._brawlers[i]._b_is_KO = _brawlers_data._brawlers[i]._b_is_KO_current;
			game_snap._brawlers[i]._i_KO_round_remaining = _brawlers_data._brawlers[i]._i_KO_round_remaining;
		}

		game_snap._ball_status = _ball_data._ball_status_current;
		game_snap._i_brawler_with_the_ball = _ball_data._i_brawler_with_the_ball_current;
		game_snap._cell_with_the_ball = _ball_data._position_on_ground_current;

		return game_snap;
	}


	private void DebugPositions(TerrainData _terrain)
	{
		for (int i = 0; i < _terrain._i_terrain_height; ++i)
		{
			string s_positions = "";
			for (int j = 0; j < _terrain._i_terrain_width; ++j)
			{
				s_positions += " | " + _terrain._i_position_brawlers_current[j,i];
			}
			s_positions += "          ";
			for (int j = 0; j < _terrain._i_terrain_width; ++j)
			{
				s_positions += " | " + _terrain._i_position_brawlers_prevision[j,i];
			}
			Debug.Log(s_positions);
		}
		Debug.Log(" " );
		Debug.Log(" " );
	}
}
