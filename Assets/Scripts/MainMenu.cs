using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public int vidas = 3;
	public int tiempo = 60; //en segundos
	public int bloquesACrear = 15;
	public int puntuacionInicial = 0;

	AudioSource audioSource;

	void Start () {
		audioSource = GetComponent<AudioSource>();
	}

	public void Jugar () {
		ControlJuego.vidasRestantes = vidas;
		ControlJuego.tiempo = tiempo;
		ControlJuego.bloquesACrear = bloquesACrear;
		ControlJuego.puntuacionTotal = puntuacionInicial;

		Application.LoadLevel(1);
	}

	public void MostrarPanel (Animator anim) {
		anim.SetBool("visible", true);
	}

	public void OcultarPanel (Animator anim) {
		anim.SetBool("visible", false);
	}

	public void InterruptorMusica () {
		audioSource.mute = !audioSource.mute;
	}

	public void Salir () {
		Application.Quit();
	}
}
