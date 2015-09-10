using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bloque : MonoBehaviour {

	public enum tipoBloque {Simple, Medio, Macizo};
	public tipoBloque tipo;
	public int valorBloque;
	public Material materialRoto, materialMuyRoto;
	ControlJuego control;
	Renderer renderizador;
	int golpeado = 0;

	// Definimos el compponente de audio
	AudioSource audioSource;

	void Start () {
		// Obtenemos el componente de audio
		audioSource = GetComponent<AudioSource>();

		valorBloque = ((int)tipo + 1) * 100;
	}

	void OnTriggerEnter (Collider other) {
		control = GameObject.FindGameObjectWithTag("GameController").GetComponent<ControlJuego>();
		renderizador = GetComponent<Renderer>();

		if (other.tag == "bola") {
			audioSource.Play();

			if (tipo == tipoBloque.Simple || golpeado >= 2 || (tipo == tipoBloque.Medio && golpeado == 1)) {
				//Debug.Log("dentro del if | tipoBloque: " + tipo + " & golpeado: " + golpeado);
				StartCoroutine(DestruirBloque());
				control.RestaBloque();
				control.SumaPuntos(valorBloque);
			} else {
				if ((tipo == tipoBloque.Macizo && golpeado >= 1) || tipo == tipoBloque.Medio) {
					//Debug.Log ("dentro del else - if | tipoBloque: " + tipo + " & golpeado: " + golpeado);
					//renderizador.materials[1] = materialMuyRoto;
					Material[] materiales = renderizador.materials;
					materiales[1] = materialMuyRoto;
					renderizador.materials = materiales;
				} else {
					//Debug.Log ("dentro del else - else | tipoBloque: " + tipo + " & golpeado: " + golpeado);
					//renderizador.materials[1] = materialRoto;
					Material[] materiales = renderizador.materials;
					materiales[1] = materialRoto;
					renderizador.materials = materiales;
				}
				golpeado++;
			}
		}
	}

	IEnumerator DestruirBloque() {
		yield return new WaitForSeconds(audioSource.clip.length);
		Destroy(this.gameObject);
	}
}
