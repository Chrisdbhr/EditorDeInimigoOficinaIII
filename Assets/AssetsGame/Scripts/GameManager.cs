using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{

	public FolderDatabase folders;
	public GameObject EnemyPrefab;
	public SpawnLocations SpawnLocationsObj;
	public PlayerMovement PlayerObj;
	[Space]
	// enemy atks
	public GameObject PrefabBulletAtk;
	public GameObject PrefabMeleeAtk;

	[Space]
	public GameObject PrefabPlayerShot;
	[Space]

	[SerializeField]
	Image hpBar;


	#region Singleton Part

	// s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
	private static GameManager s_Instance = null;

	// This defines a static instance property that attempts to find the manager object in the scene and
	// returns it to the caller.
	public static GameManager _
	{
		get {
			if (s_Instance == null) {
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance = FindObjectOfType(typeof(GameManager)) as GameManager;
			}

			// If it is still null, create a new instance
			if (s_Instance == null) {
				GameObject obj = new GameObject("AManager");
				s_Instance = obj.AddComponent(typeof(GameManager)) as GameManager;
				Debug.Log("Could not locate an GameManager object. // AManager was Generated Automaticly.");
			}

			return s_Instance;
		}
	}

	// Ensure that the instance is destroyed when the game is stopped in the editor.
	void OnApplicationQuit() {
		s_Instance = null;
	}

	#endregion

	void OnEnable() {
		folders = EditorManager.GetAllSpritesInFolder();

	}

	void Start() {
		if (SpawnLocationsObj == null) {
			SpawnLocationsObj = FindObjectOfType<SpawnLocations>();
		}
	}

	void Update() {
		if (PlayerObj.PlayerHP > 0) {
			hpBar.fillAmount = PlayerObj.PlayerHP / PlayerObj.PlayerMaxHP;
		}
	}
}
