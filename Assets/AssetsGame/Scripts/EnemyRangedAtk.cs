using UnityEngine;

public class EnemyRangedAtk : MonoBehaviour
{

	public float myDmg = 4f;
	public float myVelocity = 10f;

	void Start() {
		Destroy(gameObject, 2f);

	}

	private void OnCollisionEnter2D(Collision2D collision) {
		collision.gameObject.SendMessage("TakeDamage", myDmg, SendMessageOptions.DontRequireReceiver);
		if (collision.gameObject.GetComponent<Rigidbody2D>()) {
			collision.gameObject.GetComponent<Rigidbody2D>().AddForce((transform.position - collision.transform.position).normalized * (myVelocity / 10), ForceMode2D.Impulse);
		}
		//if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy") {
		Destroy(gameObject);
		//}

	}


	void Update() {
		transform.position += transform.forward * (myVelocity / 10) * Time.deltaTime;
	}

}
