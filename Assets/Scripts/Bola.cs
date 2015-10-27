using UnityEngine;
using System.Collections;

public class Bola : MonoBehaviour {

	public bool velocidadControlada = true;
	[HideInInspector] public float velocidad;

	void OnCollisionEnter (Collision col) {
		// Obtenemos la velocidad actual de la pelota
		Rigidbody rg = GetComponent<Rigidbody>();
		Vector3 velocidadBola = rg.velocity;

		// Modificamos el vector en el rebote para que no se quede rebotando en horizontal o vertical
		if (col.relativeVelocity.z > -0.1f && col.relativeVelocity.z < 0.1f && col.relativeVelocity.x != 0f) {
			// Si esta rebotando en horizontal, lo alteramos
			velocidadBola += new Vector3(0f, 0f, Random.Range(-velocidad, velocidad));
		} else if (col.relativeVelocity.x > -0.1f && col.relativeVelocity.x < 0.1f && col.relativeVelocity.z != 0f) {
			// Si esta rebotando en vertical, lo alteramos
			velocidadBola += new Vector3(Random.Range(-velocidad, velocidad), 0f, 0f);
		}

		// CONTROL DE VELOCIDAD
		if (velocidadControlada) {
			if (velocidadBola.z > 0 && velocidadBola.z != velocidad) {
				// Si va demasiado lento o rapido hacia arriba en z, la normalizamos
				velocidadBola.z = velocidad;
			} else if (velocidadBola.z < 0 && velocidadBola.z != -velocidad) {
				// Si va demasiado lento o rapido hacia abajo en z, la normalizamos
				velocidadBola.z = -velocidad;
			} 

			if (velocidadBola.x > 0 && velocidadBola.x != velocidad) {
				// Si va demasiado lento o rapido hacia la derecha en x, la normalizamos
				velocidadBola.x = velocidad;
			} else if (velocidadBola.x < 0 && velocidadBola.x != -velocidad) {
				// Si va demasiado lento o rapido hacia la izquierda en x, la normalizamos
				velocidadBola.x = -velocidad;
			}
		}

		// Aplicamos la velocidad corregida a la pelota
		rg.velocity = velocidadBola;
	}
}
