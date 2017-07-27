using UnityEngine;

public class EnemyMeeleAtk : MonoBehaviour
{

	public float myDmg = 10f;

	void Start() {
		Destroy(gameObject, 0.3f);

	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag != "Wall") {
			collision.gameObject.SendMessage("TakeDamage", myDmg, SendMessageOptions.DontRequireReceiver);
			collision.rigidbody.AddForce((transform.position - collision.transform.position).normalized * myDmg, ForceMode2D.Impulse);
			Destroy(gameObject, 0.1f);
		}
	}


}
