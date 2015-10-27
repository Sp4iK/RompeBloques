using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MainMenu : MonoBehaviour {

	// Parametros para configurar el juego
	public int vidas = 3;
	public int tiempo = 60; //en segundos
	public int bloquesACrear = 15;
	public int puntuacionInicial = 0;
	public ParametrosGlobales.DIFICULTAD dificultad;

	// Referencias a los nombres y puntuaciones del panel de puntuaciones
	public Text txtNombres, txtPuntuaciones, txtAyuda;

	// Referencia e interruptor de la musica del menu
	AudioSource audioSource;
	//static bool musicaOff;

	// Objeto para capturar las puntuaciones guardadas
	List<Scores> puntuaciones;

	void Start () {
		puntuaciones = new List<Scores>();
		audioSource = GetComponent<AudioSource>();
		ParametrosGlobales.instance.musicaOff = Convert.ToBoolean(PlayerPrefs.GetInt("musicaOff", 0));
		audioSource.mute = ParametrosGlobales.instance.musicaOff;
		if (!audioSource.mute) audioSource.Play();

		#if UNITY_ANDROID || UNITY_IOS
		txtAyuda.text += "Mueve la nave horizontalmente desplazando tu dedo sobre la pantalla y dispara la bola al levantar el dedo.";
		#elif UNITY_STANDALONE
		txt.Ayuda.Text += "Mueve la nave horizontalmente con las flechas del teclado o con tu ratón y dispara la bola con la barra espaciadora o el botón izquierdo del ratón.";
		#endif
	}

	public void Jugar () {
		ParametrosGlobales.instance.dificultadGlobal = dificultad;
		ParametrosGlobales.instance.vidasRestantes = vidas;
		ParametrosGlobales.instance.tiempo = tiempo - ((int)dificultad * 10);
		ParametrosGlobales.instance.bloquesACrear = bloquesACrear + ((int)dificultad * 5);
		ParametrosGlobales.instance.puntuacionTotal = puntuacionInicial;
		ParametrosGlobales.instance.nivel = 1;

		Application.LoadLevel(1);
	}

	public void CargarPuntuaciones () {
		// Recuperamos las puntuaciones guardadas
		puntuaciones = HighScoreManager.instance.GetHighScores();

		// Si existen puntuaciones guardadas
		if (puntuaciones.Count > 0) {
			// limpiamos los campos de texto
			txtNombres.text = "";
			txtPuntuaciones.text = "";

			// y cargamos las puntuaciones
			foreach (Scores puntuacion in puntuaciones) {
				txtNombres.text += puntuacion.name + "\n";
				txtPuntuaciones.text += puntuacion.score.ToString() + "\n";
				print("PUNTUACION: " + puntuacion.name + " - " + puntuacion.score);
			}
		}
	}

	public void MostrarPanel (Animator anim) {
		anim.SetBool("visible", true);
	}

	public void OcultarPanel (Animator anim) {
		anim.SetBool("visible", false);
	}

	public void InterruptorMusica () {
		ParametrosGlobales.instance.musicaOff = !ParametrosGlobales.instance.musicaOff;
		audioSource.mute = ParametrosGlobales.instance.musicaOff;
		if (!audioSource.mute) audioSource.Play();
		PlayerPrefs.SetInt("musicaOff", Convert.ToInt32(ParametrosGlobales.instance.musicaOff));
		//AudioListener.volume = 0;
	}

	public void BorrarPuntuaciones () {
		HighScoreManager.instance.ClearLeaderBoard();
		txtNombres.text = "";
		txtPuntuaciones.text = "";
	}

	public void Salir () {
		PlayerPrefs.Save();
		Application.Quit();
	}
}
