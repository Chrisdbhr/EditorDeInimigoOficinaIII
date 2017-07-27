using System.Collections;
using UnityEngine;

public class RandomActAnim : MonoBehaviour
{
	private Animator anim;
	public int ActionIndleAnimations;
	public float WaitTime = 3f;
	[Space]
	public int SelectedAnim;

	private void OnEnable() {
		StartCoroutine("Contar");
	}

	void Start() {
		anim = GetComponent<Animator>();
	}

	IEnumerator Contar() {
		do {
			yield return new WaitForSeconds(WaitTime);
			SelectedAnim = Random.Range(0, ActionIndleAnimations);

			anim.SetInteger("Stance", SelectedAnim);
			anim.SetTrigger("Act");

		} while (true);
	}
}
