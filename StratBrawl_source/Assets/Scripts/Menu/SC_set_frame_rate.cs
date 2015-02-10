using UnityEngine;
using System.Collections;

public class SC_set_frame_rate : MonoBehaviour {

	[SerializeField]
	private int i_frame_rate = 60;

	void Start ()
	{
		Application.targetFrameRate = i_frame_rate;
		Destroy(this);
	}
}
