using UnityEngine;
using System.Collections;

public class SC_set_screen_orientation : MonoBehaviour {

	[SerializeField]
	private ScreenOrientation _screen_orientation = ScreenOrientation.Landscape;

	void Start ()
	{
		Screen.orientation = _screen_orientation;
		Destroy(this);
	}
}
