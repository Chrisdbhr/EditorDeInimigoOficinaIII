using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
	public int MovementSpeed;

	public float PlayerHP;

	public float PlayerMaxHP;

	Rigidbody2D _rb;
	CircleCollider2D _col;
	Vector3 _inputDir;
	Animator _anim;
	Camera cam;

	void Start() {
		PlayerHP = PlayerMaxHP;

		_rb = GetComponent<Rigidbody2D>();
		_col = GetComponent<CircleCollider2D>();
		_anim = GetComponentInChildren<Animator>();
		cam = Camera.main;

	}

	// Update is called once per frame
	void Update() {

		if (PlayerHP < 0) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		_anim.SetFloat("Speed", _rb.velocity.magnitude / 10);


		// controls

		if (Input.GetMouseButtonDown(0)) {
			Instantiate(GameManager._.PrefabPlayerShot, transform.position, transform.rotation).transform.LookAt(cam.ScreenToWorldPoint(Input.mousePosition));


		}




	}

	public void TakeDamage(float value) {
		PlayerHP -= value;
	}

	void FixedUpdate() {
		_inputDir = new Vector3(Input.GetAxis("Horizontal") * MovementSpeed, Input.GetAxis("Vertical") * MovementSpeed);
		_rb.velocity = _inputDir;
	}
}
