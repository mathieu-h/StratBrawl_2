using UnityEngine;
using System.Collections;


public enum GameState {None, Planification, AnimationResult, End}
public enum ActionType {None, Move, Tackle, Pass, Defense}
public enum Direction {Up, Down, Right, Left}
public enum BallStatus {Null, OnBrawler, OnGround}
public enum GameResult {Win, Lose, Draw}
