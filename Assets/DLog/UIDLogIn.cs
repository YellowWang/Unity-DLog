using UnityEngine;
using System.Collections;

public class UIDLogIn : MonoBehaviour {

	UIDLogBase logbase;
	public bool inGame = false;

	void Awake()
	{
		logbase = new UIDLogBase (false);
	}

	void OnGUI ()
	{		
		if (inGame)
			logbase.OnGUI();
	}
}
