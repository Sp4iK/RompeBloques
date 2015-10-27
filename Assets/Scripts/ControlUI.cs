using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlUI : MonoBehaviour {

	// Instancia global
	public static ControlUI instance;

	// El componente Animator del panel de fin de juego
	public Animator animatorFinJuego;
	// Referencia al Animator del panel de pausa
	public Animator animatorPausa;
	// Referencia a los Animator del mensaje de 'timeout'
	public Animator animatorTimeout;

	// Referencias a los marcadores
	public Text txtPuntuacion, txtVidas, txtTiempo;
	// Referencias a los componentes de los paneles de mensaje
	public Text txtTitulo, txtPuntuacionFinal;
	public InputField inputNombre;
	public GameObject panelNombre, botonContinuar, botonSalir;

	void Awake () {
		instance = this;
	}

	// Metodo para mostrar la ventana de fin de juego
	public void MostrarFinJuego (string titulo, int puntuacion) {
		// Ocultamos los marcadores
		txtVidas.CrossFadeAlpha(0f, 1f, true);
		txtTiempo.CrossFadeAlpha(0f, 1f, true);
		txtPuntuacion.CrossFadeAlpha(0f, 1f, true);

		txtTitulo.text = titulo;

		// Mostramos el mensaje y boton adecuados segun el estado del juego
		if (ControlJuego.estado == ControlJuego.ESTADO.FinNivel) {
			txtPuntuacionFinal.text = "PUNTUACION: \n" + puntuacion;
			botonContinuar.SetActive(true);
			botonSalir.SetActive(false);
		} else if (ControlJuego.estado == ControlJuego.ESTADO.FinJuego) {
			txtPuntuacionFinal.text = "PUNTUACION FINAL: " + puntuacion;
			panelNombre.SetActive(true);
			botonContinuar.SetActive(false);
			botonSalir.SetActive(true);
		}

		// Si hay un nombre de jugador ya guardado en la partida, se muestra
		if (ParametrosGlobales.instance.nombreJugador != null) inputNombre.text = ParametrosGlobales.instance.nombreJugador;

		// Mostramos la ventana
		animatorFinJuego.SetInteger("estado", (int)Transicion.Mostrar);
	}

	// Metodo para ocultar la ventana de fin de juego
	public void OcultarFinJuego () {
		animatorFinJuego.SetInteger("estado", (int)Transicion.Ocultar);

		ParametrosGlobales.instance.nombreJugador = inputNombre.text.ToUpper();
	}

	// Metodo para mostrar el panel de pausa
	public void MostrarPausa () {
		animatorPausa.SetInteger("estado", (int)Transicion.Mostrar);
		animatorPausa.speed = 5f;
	}

	// Metodo para ocultar el panel de pausa
	public void OcultarPausa () {
		animatorPausa.SetInteger("estado", (int)Transicion.Ocultar);
	}

	// Metodo para mostrar el mensaje de timeout
	public void MostrarTimeout () {
		animatorTimeout.Play("timeout_slide");
	}
}

// Definimos los estados para las transiciones de la animacion con tipos enumerados
public enum Transicion {
	Ocultar,	// Estado por ocultar
	Mostrar		// Estado de mostrar
}
