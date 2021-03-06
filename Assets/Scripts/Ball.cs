﻿using UnityEngine;

public class Ball : MonoBehaviour {

	private float angle;
	public float force, yForce;

	private Terrain terrain;

	private void Start() {
		terrain = FindObjectOfType<Terrain>();
		Vector3 dir = transform.position;

		int rez = terrain.terrainData.heightmapResolution;

		angle = Mathf.Atan2(-dir.z, -dir.x);
	}

	private void FixedUpdate() {
		float x = Mathf.Cos(angle) * force;
		float z = Mathf.Sin(angle) * force;

		Rigidbody rb = GetComponent<Rigidbody>();

		Vector3 dir = new Vector3(x, 0, z).normalized;
		float mag = Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2));
		rb.velocity = (dir * mag) + new Vector3(0, rb.velocity.y);

		float maxX = terrain.terrainData.size.x/2;
		float maxY = terrain.terrainData.size.z/2;

		transform.LookAt(rb.velocity);

		if (transform.position.x > maxX || transform.position.x < -maxX || transform.position.z > maxY || transform.position.z < -maxY) {
			if (Player.hasBounceback) {
				angle += Mathf.PI;
				Player.hasBounceback = false;
				transform.Translate(Mathf.Cos(angle), 0, Mathf.Sign(angle));
			} else {
				Player.mode = Player.Mode.MOVING_NET;
				Player.lives--;

				Animation ani = Camera.main.GetComponent<Animation>();

				//foreach (Powerup p in Player.instance.powerups) {
				//	p.gameObject.SetActive(true);
				//}

				if (Player.lives == 0) {
					Player.mode = Player.Mode.ENDGAME;

					ani.clip = ani.GetClip("CameraSad");
					ani.Play();

					Player.audio.PlayOneShot(Player.instance.gameover);

					FindObjectOfType<Floor>().s = 0;
					Vector3 scale = FindObjectOfType<Net>().transform.lossyScale;
					scale.x = Player.instance.netSize;
					scale.y = Player.instance.netSize;
					scale.z = Player.instance.netSize;
					FindObjectOfType<Net>().transform.localScale = scale;

					foreach (Powerup p in FindObjectsOfType<Powerup>()) {
						Destroy(p.gameObject);
					}
				} else {
					ani.clip = ani.GetClip("CameraShake");
					ani.Play();

					Player.audio.PlayOneShot(Player.instance.miss);
				}

				Destroy(gameObject);
			}
		}
	}

	public void OnCollisionEnter(Collision collision) {
		float x = Mathf.Cos(angle) * force;
		float z = Mathf.Sin(angle) * force;
		float y = collision.contacts[0].normal.y * yForce;

		Rigidbody rb = GetComponent<Rigidbody>();

		rb.AddForce(new Vector3(x, y, z));

		Player.audio.PlayOneShot(Player.instance.bounce);
	}
}