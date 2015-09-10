using UnityEngine;
using System.Collections;

public class Muerte : MonoBehaviour {

	ControlJuego control;

	void OnTriggerEnter (Collider other) {
		control = GameObject.FindGameObjectWithTag("GameController").GetComponent<ControlJuego>();

		if (other.tag == "bola") {
			control.Fin();
		}
	}
}
