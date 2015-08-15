using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RnMUI_EscapeListener : MonoBehaviour {

	private const int GameMenuWindowID = 5;

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			// Check if there are any open windows
			List<UIWindow> windows = UIWindow.GetWindows();

			bool hadOpenWindow = false;

			// Loop through the windows and close all the opened ones
			foreach (UIWindow w in windows)
			{
				if (w.IsOpen)
				{
					hadOpenWindow = true;
					w.Hide();
				}
			}

			// If we had no open window open up the game menu
			if (!hadOpenWindow)
			{
				UIWindow gameMenu = UIWindow.GetWindow(GameMenuWindowID);

				if (gameMenu != null)
				{
					if (gameMenu.IsOpen)
						gameMenu.Hide();
					else
						gameMenu.Show();
				}
			}
		}
	}
}
