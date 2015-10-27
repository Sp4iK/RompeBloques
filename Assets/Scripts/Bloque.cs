using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bloque : MonoBehaviour {

	public enum tipoBloque {Simple, Medio, Macizo};
	public tipoBloque tipo;
	public int valorBloque;
	public Material materialRoto, materialMuyRoto;
	ControlJuego controlJuego;
	Renderer renderizador;
	int golpeado = 0;

	// Definimos el compponente de audio
	AudioSource audioSource;

	void Start () {
		controlJuego = ControlJuego.instance;

		// Obtenemos el componente de audio y renderer
		audioSource = GetComponent<AudioSource>();
		renderizador = GetComponent<Renderer>();

		valorBloque = ((int)tipo + 1) * 10;
	}

	void OnCollisionEnter (Collision col) {
		if (col.gameObject.tag == "bola") {
			audioSource.Play();
			golpeado++;

			if (golpeado == (int)tipo + 1) {
				StartCoroutine(DestruirBloque());
				controlJuego.RestaBloque();
				controlJuego.SumaPuntos(valorBloque);
			} else {
				if ((tipo == tipoBloque.Macizo && golpeado >= 2) || tipo == tipoBloque.Medio) {
					//renderizador.materials[1] = materialMuyRoto;
					Material[] materiales = renderizador.materials;
					materiales[1] = materialMuyRoto;
					renderizador.materials = materiales;
				} else {
					//renderizador.materials[1] = materialRoto;
					Material[] materiales = renderizador.materials;
					materiales[1] = materialRoto;
					renderizador.materials = materiales;
				}
			}
		}
	}

	IEnumerator DestruirBloque() {
		yield return new WaitForSeconds(audioSource.clip.length);
		Destroy(this.gameObject);
	}
}
