﻿using UnityEngine;

public class Net : MonoBehaviour {

	public static bool[] offset = new bool[3];

	public void Start() {
		transform.localScale = new Vector3(Player.instance.netSize, Player.instance.netSize, Player.instance.netSize);
	}

	public void OnTriggerEnter(Collider other) {
		Destroy(other.gameObject);

		Player.mode = Player.Mode.MOVING_NET;
		Player.score++;

		Player.audio.PlayOneShot(Player.instance.success);

		FindObjectOfType<Floor>().s += FindObjectOfType<Floor>().speedIncrease;
		Vector3 scale = transform.lossyScale;
		scale.x -= Player.instance.netSizeChange;
		scale.y -= Player.instance.netSizeChange;
		scale.z -= Player.instance.netSizeChange;

		if (scale.x < Player.instance.netMinSize) {
			scale = new Vector3(Player.instance.netMinSize, Player.instance.netMinSize, Player.instance.netMinSize);
		}

		transform.localScale = scale;

		if (FindObjectsOfType<Powerup>().Length < 3) {
			int i = Random.Range(0, Player.instance.powerups.Length);
			Powerup p = Instantiate(Player.instance.powerups[i], new Vector3(0, -100, 0), Quaternion.Euler(0, 0, 90));

			for (int j = 0; j < 3; j++) {
				if (!offset[j]) {
					p.offset = j*120;
					offset[j] = true;
					break;
				}
			}
		}

		if (Player.score > Player.highScore) {
			Player.highScore = Player.score;

			PlayerPrefs.SetInt("highscore", Player.highScore);
			PlayerPrefs.Save();
		}
	}

	private void FixedUpdate() {
		Ball ball = FindObjectOfType<Ball>();

		if (ball != null) {
			transform.GetChild(0).LookAt(ball.transform);
		}
	}
}