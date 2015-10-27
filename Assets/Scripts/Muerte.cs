using UnityEngine;
using System.Collections;

public class Muerte : MonoBehaviour {

	void OnTriggerEnter (Collider other) {
		if (other.tag == "bola") {
			ControlJuego.instance.Fin();
		}
	}
}
