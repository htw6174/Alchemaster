﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// User Interface Handler
/// 
/// 
/// This Singleton class keeps track of all gameObjects with a Menu.cs script attached to them.
/// gameObjects must have a Canvas and a CanvasGroup component attached to them. 
/// Canvas gameObjects must be a child of the UI master gameObject
/// 
/// Authored by Lucas Rumney of Jet Drift Studios 
/// August 2015
/// 
/// 
/// </summary>

public class UIHandler : MonoBehaviour {

	public static UIHandler UI;											//Static reference to this class

	public Menu[] menus = new Menu[10];									//array to hold menus, increase if you have more menus
	public bool pause = false;											//whether or not the game is pause (time.timescale = 0.0f)


	/// <summary>
	/// Awake Singleton Pattern
	/// 
	/// </summary>

	void Awake() 														//Ensures only 1 instance of UI exists at any given time
	{
		if(UI == null)
		{
			DontDestroyOnLoad(gameObject);								//makes gameObject persist between level loads
			UI = this;													//sets this UI as the only UI
		}
		else
		{
			Destroy(gameObject);										//if its not the only UI, destroy it
		}
	}

	// Use this for initialization
	void Start () 
	{
		Time.timeScale = 1.0f;											//When project opens, make sure Timescale is normal
		menus = this.gameObject.GetComponentsInChildren<Menu>();		//menus is initialized with all Menu gameobjects that are children
		if (Application.loadedLevel == 0)								//if first scene, display main menu, close all others
		{
			openMenu("main");
			closeMenu("options");
			closeMenu("pause");											//not completely necessary, but convenient.
			closeMenu("ingameoptions");
		}
	}

	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyUp(KeyCode.Escape) && Application.loadedLevel != 0)		//toggles pause and timescale
		{
			togglePause();
			pauseMenu(pause);
		}
	}


	/// <summary>
	/// Opens the menu "Menu"
	/// </summary>
	public void openMenu(string Menu) //Displays all of the Canvas' associated with the nextMenu tag
	{
		foreach (Menu M in menus)
		{
			if (M.name == Menu)
			{
				M.gameObject.GetComponent<Canvas>().enabled = true;
				M.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
				M.gameObject.GetComponent<CanvasGroup>().interactable = true;
			}
		}
	}


	/// <summary>
	/// Closes the menu "Menu"
	/// </summary>
	public void closeMenu(string Menu) 
	{
		foreach (Menu M in menus)
		{
			if (M.name == Menu)
			{
				M.gameObject.GetComponent<Canvas>().enabled = false;
				M.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
				M.gameObject.GetComponent<CanvasGroup>().interactable = false;
			}
		}
	}


	/// <summary>
	/// Button Action
	/// Changes Scene using the scene name String
	/// </summary>
	public void changeSceneString(string scene)
	{
		Application.LoadLevel(scene);
	}

	/// <summary>
	/// Button Action
	/// Changes Scene using the scene integer found in File>Build Settings. Will not function without Build Settings Scene Order set!
	/// </summary>
	public void changeSceneIndex(int scene)
	{
		Application.LoadLevel(scene);
	}


	/// <summary>
	/// Button Action
	/// Toggles the pause boolean value. 
	/// Note: this function will only affect the pause boolean which changes the behavior of pauseMenu() next time it's called
	/// </summary>
	public void togglePause()
	{
		pause = !pause;
	}

	/// <summary>
	/// Button Action
	/// Displays/Hides pause menu and starts/stops time based on pause boolean
	/// </summary>	
	public void pauseMenu(bool pause)
	{
		if (pause)
		{
			Time.timeScale = 0.0f;
			openMenu("pause");
		}
		if (!pause)
		{
			Time.timeScale = 1.0f;
			closeMenu("pause");
		}
	}

	public void closeGame()
	{
		Application.Quit();
	}
}
