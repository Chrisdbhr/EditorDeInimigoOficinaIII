using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	/// <summary>
	/// Atributes and all parameters of the enemy.
	/// </summary>
	[SerializeField]
	CharacterEntry atb;

	CharacterLimbs _limbs;
	Rigidbody2D _rb;
	Animator _anim;

	float vida;

	// Enemy current actions
	enum AI_Movement { flee, stand, followPlayer };
	AI_Movement myMovement;

	bool _attack;
	bool _isAlive = true;


	void Start() {
		_limbs = GetComponent<CharacterLimbs>();
		_rb = GetComponent<Rigidbody2D>();
		_anim = GetComponentInChildren<Animator>();

		StartCoroutine(Brain());
		StartCoroutine(Attacks());

		vida = atb.Vida;
	}

	public void SetValues(CharacterEntry value) {
		atb = value;

		if (_limbs == null) _limbs = GetComponent<CharacterLimbs>();

		_limbs.Attributes = value;

	}

	public CharacterEntry GetAttributes() {
		return atb;
	}

	// How the enemy think
	IEnumerator Brain() {
		do {
			float waitTime = atb.MinimumTimeOnAction;

			// decide if will shoot/punch
			if (atb.AtkPriority > atb.FleePriority) {
				_attack = false;
			}
			else {
				_attack = true;
			}

			// random action
			int action = UnityEngine.Random.Range(0, 3);
			switch (action) {
				case 0:
					myMovement = AI_Movement.flee;
					if (atb.Classe == 1)
						waitTime += UnityEngine.Random.Range(0, atb.MaxTimeOnFlee);
					break;
				case 1:
					myMovement = AI_Movement.stand;
					if (atb.Classe == 1)
						waitTime += UnityEngine.Random.Range(0, atb.MaxTimeOnStand);
					break;
				default:
					myMovement = AI_Movement.followPlayer;
					_attack = true;
					if (atb.Classe == 0)
						waitTime += UnityEngine.Random.Range(0, atb.MaxTimeFollowingPlayer);
					break;
			}

			yield return new WaitForSeconds(waitTime);
		} while (_isAlive);
	}

	// Enemy attacks
	IEnumerator Attacks() {
		do {
			if (_attack) {
				if (atb.Classe == 0) { // melee

					var createdObj = Instantiate(GameManager._.PrefabMeleeAtk, transform.position, transform.rotation);

					createdObj.GetComponent<EnemyMeeleAtk>().myDmg = atb.Ataque;
				}
				else { // ranged
					var createdObj = Instantiate(GameManager._.PrefabBulletAtk, transform.position, transform.rotation);
					createdObj.transform.LookAt(GameManager._.PlayerObj.transform);

					createdObj.GetComponent<EnemyRangedAtk>().myDmg = atb.Ataque;
					createdObj.GetComponent<EnemyRangedAtk>().myVelocity = atb.Velocidade;

				}
			}

			yield return new WaitForSeconds(atb.HitDelay / atb.Velocidade * UnityEngine.Random.Range(0.5f, 2));
		} while (_isAlive);
	}

	void Update() {
		if (vida < 0) {
			Destroy(gameObject);
		}

		// movement
		switch (myMovement) {
			case AI_Movement.flee:
				FollowPlayer(false);

				break;
			case AI_Movement.stand:
				// paarado
				break;
			case AI_Movement.followPlayer:
				FollowPlayer(true);
				break;

		}
		_anim.SetFloat("Speed", _rb.velocity.magnitude / 10);

	}

	public void TakeDamage(float value) {
		vida -= (int)value;
	}

	void FollowPlayer(bool followIt) {
		Vector2 direction = transform.position - GameManager._.PlayerObj.transform.position;
		direction.Normalize();
		if (!followIt) {
			direction *= -1;
		}
		_rb.velocity = direction * atb.Velocidade * 2 * Time.deltaTime;
	}

}