using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocations : MonoBehaviour
{

	public List<Transform> PossibleSpawnPoints = new List<Transform>();

	void Start() {
		// Adiciona todos os filhos a lista
		foreach (Transform children in GetComponentInChildren<Transform>()) {
			PossibleSpawnPoints.Add(children);

		}
	}

	public Transform GetASpawnPoint() {
		int index = UnityEngine.Random.Range(0, PossibleSpawnPoints.Count);
		return PossibleSpawnPoints[index];
	}


}
