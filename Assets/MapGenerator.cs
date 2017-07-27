using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public SpriteRenderer GeneratedMap;
	public GameObject TilePrefab;

	public List<MapInfos> AllMaps = new List<MapInfos>();

	float tileSize = 1.44f;

	void Start() {

		// pegar todos os txt dos mapas
		string dir = EditorManager.GetMapsDir();
		foreach (var mapTxt in Directory.GetFiles(dir, "*.txt")) {
			var arquivoCarregado = File.OpenRead(mapTxt);
			string fileContents;
			using (StreamReader reader = new StreamReader(arquivoCarregado)) {
				fileContents = reader.ReadToEnd();
			}
			string mapName = mapTxt.Replace(dir, "").Replace(".txt", "").Replace("\\", "").Replace("/", "");
			AllMaps.Add(new MapInfos(mapName, mapTxt, fileContents,
									EditorManager.LoadMapSpriteByName(mapName)));
		}

		// mapa escolhido
		int mapChoose = 0;
		GeneratedMap.sprite = AllMaps[mapChoose].MapSprite;

		string[] lines = AllMaps[mapChoose].MapData.Split('\n');
		for (int i = 0; i < lines.Length; i++) {
			lines[i] = lines[i].Replace(" ", "");
		}

		int tilesX = 0;
		int tilesY = 0;
		for (int i = 0; i < lines[0].Length; i++) {
			tilesX = i;
		}


		if (lines[lines.Length - 1].Length > 2) {
			tilesY = lines.Length;
		}
		else {
			tilesY = lines.Length - 1;
		}

		Debug.Log("Loaded Map  x " + tilesX + "  y " + tilesY);

		// Spawn colliders
		for (int i = 0; i < lines.Length; i++) { // linha
			for (int j = 0; j < lines[i].Length; j++) { // letra da linha
				if (lines[i][j] == '1') {
					Vector3 offset = new Vector3(j * tileSize, (i * tileSize) * -1, 0);
					Instantiate(TilePrefab, offset, Quaternion.identity, GeneratedMap.transform);
				}
			}
		}

		// Centralizar o mapa
		Vector3 mapOffset = new Vector3(((tilesX / 2) * tileSize) * -1, (tilesY / 2) * tileSize, 0);
		GeneratedMap.transform.position += mapOffset;

	}


}

[System.Serializable]
public class MapInfos
{
	public string MapName = "";
	public string MapTxtPath;
	public string MapData;
	public Sprite MapSprite;

	public MapInfos(string mapName, string mapTxtPath, string mapData, Sprite mapSprite) {
		MapName = mapName;
		MapTxtPath = mapTxtPath;
		MapData = mapData;
		MapSprite = mapSprite;
	}
}