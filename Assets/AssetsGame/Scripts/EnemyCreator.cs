using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyCreator : MonoBehaviour
{
	public float TimeForNextSpawn = 4f;

	public bool WaveActive;

	/// <summary>
	/// lista com todos os inimigos encontrados na pasta.
	/// </summary>
	List<CharacterEntry> listWithAllEnemy = new List<CharacterEntry>();

	public int WaveActual = 0;

	public int WavePoints = 100;
	int _wavePointsRemaining;

	WaitForSeconds _waitTimeForNextSpawn;


	void Start() {
		_waitTimeForNextSpawn = new WaitForSeconds(TimeForNextSpawn);

		// Pega todos os inimigos possiveis da pasta
		string dir = EditorManager.GetCreatedCharactersDir();
		foreach (var enemy in Directory.GetFiles(dir)) {
			var arquivoCarregado = File.OpenRead(enemy);
			string fileContents;
			using (StreamReader reader = new StreamReader(arquivoCarregado)) {
				fileContents = reader.ReadToEnd();
			}
			listWithAllEnemy.Add(JsonUtility.FromJson<CharacterEntry>(fileContents));

		}

		// Comeca a spawnar inimigos
		StartCoroutine(SpawnEnemies());
	}

	IEnumerator SpawnEnemies() {
		do {
			yield return _waitTimeForNextSpawn;

			int enemyIndexToSpawn = UnityEngine.Random.Range(0, listWithAllEnemy.Count);
			GameObject createdEnemy = Instantiate(GameManager._.EnemyPrefab, GameManager._.SpawnLocationsObj.GetASpawnPoint().position, Quaternion.identity);

			// passa os atributos ao inimigo
			EnemyAI enemyAi = createdEnemy.GetComponent<EnemyAI>();
			enemyAi.SetValues(listWithAllEnemy[enemyIndexToSpawn]);

			RemovePoints(enemyAi.GetAttributes().Pontos);

		} while (WaveActive);

	}

	void RemovePoints(int value) {

		_wavePointsRemaining -= value;

		if (_wavePointsRemaining < 0) {
			// acabou

		}

	}


}