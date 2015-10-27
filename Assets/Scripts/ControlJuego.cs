using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ControlJuego : MonoBehaviour {
	
	// Instancia global
	public static ControlJuego instance;

	// Objetos necesarios para el juego
	public GameObject navePrefab;
	public GameObject posicionInicialNave;
	public GameObject bloquePrefab;
	GameObject nave;
	GameObject[] bloquesCreados;
	AudioSource audioSource;

	// Materiales para los bloques
	public Material bloqueSimple, bloqueMedio, bloqueMacizo;

	// Clips para la musica
	public AudioClip[] audioClips;
	
	// Parametros locales
	int bloques, puntuacion, vidas;
	float tiempoRestante;

	// Referencia al controlador del UI
	ControlUI controlUI;

	// Referencia a los parametros globales
	ParametrosGlobales parametrosGlobales;
	
	// Estado de ejecucion del juego
	public enum ESTADO {Jugando, Pausa, FinNivel, FinJuego};
	[HideInInspector] public static ESTADO estado;

	void Awake () {
		instance = this;
	}

	void Start () {
		controlUI = ControlUI.instance;
		parametrosGlobales = ParametrosGlobales.instance;
		audioSource = GetComponent<AudioSource>();
		audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
		audioSource.mute = ParametrosGlobales.instance.musicaOff;
		if (!audioSource.mute) audioSource.Play();
		InicializarEscena();
		InicializarNave();
		estado = ESTADO.Jugando;
	}

	void Update () {
		tiempoRestante -= Time.deltaTime;
		// Actualizamos el marcador del tiempo, solo si estamos jugando y no en pausa
		if (estado == ESTADO.Jugando) controlUI.txtTiempo.text = Mathf.RoundToInt(tiempoRestante).ToString();

		// Si se pulsa 'Esc' (PC) o 'Atras' (movil)...
		if (Input.GetKeyDown(KeyCode.Escape)) {
			estado = ESTADO.Pausa;
			// Llamada a funcion para mostrar menu de pausa
			Pausa((int)estado);
		}

		// Si has terminado con todos los bloques...
		if (bloques <= 0 && estado == ESTADO.Jugando) {
			Fin();
		}

		// Si se ha acabado el tiempo...
		if (tiempoRestante <= 0f && estado == ESTADO.Jugando) {
			Debug.Log("¡SE ACABO EL TIEMPO!");
			Fin();
		}
	}
	
	void InicializarNave () {
		// Instanciacion de la nave en la escena
		nave = (GameObject) Instantiate(navePrefab, posicionInicialNave.transform.position, Quaternion.identity);
		estado = ESTADO.Jugando;
	}
	
	void InicializarEscena () {
		// Inicializacion de los parametros
		puntuacion = 0;
		vidas = parametrosGlobales.vidasRestantes;
		tiempoRestante = parametrosGlobales.tiempo;
		controlUI.txtTiempo.text = tiempoRestante.ToString();
		controlUI.txtPuntuacion.text = "PUNTUACION:\n" + puntuacion;
		controlUI.txtVidas.text = "VIDAS: " + vidas;
		float espacioInferior = ((parametrosGlobales.nivel - 1) * -0.5f);

		// Creacion y posicionamiento de los bloques
		bloquesCreados = new GameObject[parametrosGlobales.bloquesACrear];

		for (int i = 0; i < parametrosGlobales.bloquesACrear; i++) {
			float posX = Random.Range(-2.6f, 2.6f);
			float posZ = Random.Range(espacioInferior, 4f);
			Vector3 posicionBloque = new Vector3(posX, 0.25f, posZ);
			bloquesCreados[i] = (GameObject) Instantiate(bloquePrefab, posicionBloque, Quaternion.Euler(0f, 0f, 90f));
			int bloqueTipo = Random.Range(0, 3);
			bloquesCreados[i].GetComponent<Bloque>().tipo = (Bloque.tipoBloque) bloqueTipo;

			switch (bloqueTipo) {
				case 0:
					bloquesCreados[i].GetComponent<Renderer>().material = bloqueSimple;
					break;
				case 1:
					bloquesCreados[i].GetComponent<Renderer>().material = bloqueMedio;
					break;
				case 2:
					bloquesCreados[i].GetComponent<Renderer>().material = bloqueMacizo;
					break;
			}

			// Comprueba que los bloques no se superpongan
			if (i > 0) bloquesCreados[i].transform.position = CompruebaSuperposicion(posicionBloque, i);
		}

		bloques = bloquesCreados.Length;
		Debug.Log("Dificultad " + parametrosGlobales.dificultadGlobal.ToString() + " | Bloques: " + bloques);

		estado = ESTADO.Jugando;
	}

	public void RestaBloque () {
		bloques--;
	}

	public void SumaPuntos (int puntos) {
		puntuacion += puntos;
		controlUI.txtPuntuacion.text = "PUNTUACION:\n" + puntuacion;
	}

	public void Pausa (int estadoPausa) {
		switch (estadoPausa) {
			case (int)ESTADO.Pausa:
				controlUI.MostrarPausa();
				audioSource.Pause();
				Time.timeScale = 0.001f;
				break;
			case (int)ESTADO.Jugando:
				controlUI.OcultarPausa();
				audioSource.UnPause();
				Time.timeScale = 1f;
				estado = ESTADO.Jugando;
				break;
			case -1:
				Time.timeScale = 1f;
				Application.LoadLevel(0);
				break;
		}
	}

	public void Fin () {
		// Destruimos la bola
		Destroy(GameObject.FindGameObjectWithTag("bola").gameObject);

		if (bloques <= 0) {
			nave.GetComponent<Nave>().DestruirNave(Nave.TipoDestruccion.Teleportar);
			estado = ESTADO.FinNivel;
			int vidasNivel = (vidas > 0) ? vidas : 1;
			puntuacion = (puntuacion * (int)tiempoRestante * vidasNivel) / 10;
			parametrosGlobales.puntuacionTotal += puntuacion;
			Debug.Log("¡HAS GANADO!" + "\n" + "PUNTUACION NIVEL: " + puntuacion + " | PUNTUACION TOTAL: " + parametrosGlobales.puntuacionTotal);
			controlUI.MostrarFinJuego("¡HAS GANADO!", puntuacion);
		} else if (vidas <= 0) {
			nave.GetComponent<Nave>().DestruirNave(Nave.TipoDestruccion.Destruir);
			estado = ESTADO.FinJuego;
			parametrosGlobales.puntuacionTotal += (puntuacion * (int)tiempoRestante) / 10;
			Debug.Log("HAS PERDIDO, ¡PAQUETE!" + "\n" + "PUNTUACION NIVEL: " + ((puntuacion * (int)tiempoRestante) / 10) + " | PUNTUACION TOTAL: " + parametrosGlobales.puntuacionTotal);
			controlUI.MostrarFinJuego("HAS PERDIDO, ¡PAQUETE!", parametrosGlobales.puntuacionTotal);
		} else {
			nave.GetComponent<Nave>().DestruirNave(Nave.TipoDestruccion.Destruir);
			estado = ESTADO.Pausa;
			if (tiempoRestante <= 0f) controlUI.MostrarTimeout();
			Invoke("QuitarVida", 2f);
		}
	}

	private void QuitarVida () {
		vidas--;
		controlUI.txtVidas.text = "VIDAS: " + vidas;
		InicializarNave();
		tiempoRestante = parametrosGlobales.tiempo;
		Debug.Log("VUELVE A INTENTARLO" + "\n" + "VIDAS: " + vidas);
	}

	public void ContinuarJuego () {
		// Lanzar la animacion de ocultar la ventana
		controlUI.OcultarFinJuego();
		// Ajustamos los parametros para el siguiente nivel
		parametrosGlobales.vidasRestantes = vidas;
		parametrosGlobales.tiempo -= 5 * ((int)parametrosGlobales.dificultadGlobal + 1);
		parametrosGlobales.bloquesACrear += 5 * ((int)parametrosGlobales.dificultadGlobal + 1);
		parametrosGlobales.nivel++;
		Debug.Log("SIGUIENTE NIVEL");
		StartCoroutine(EsperarYCargar(2f, Application.loadedLevel));
	}

	public void Salir () {
		controlUI.OcultarFinJuego();
		// Si la puntuacion ha sido la mas alta, se guarda
		if (parametrosGlobales.puntuacionTotal > HighScoreManager.instance.GetHighestScore().score) GuardarPuntuacion();
		StartCoroutine(EsperarYCargar(2f, 0));
	}

	private void GuardarPuntuacion () {
		if (parametrosGlobales.nombreJugador.Length == 0) parametrosGlobales.nombreJugador = "AAA";
		HighScoreManager.instance.SaveHighScore(parametrosGlobales.nombreJugador, parametrosGlobales.puntuacionTotal);
	}

	private IEnumerator EsperarYCargar (float segundos, int nivel) {
		// Forzar al programa a esperar
		yield return new WaitForSeconds(segundos);
		Application.LoadLevel(nivel);
	}

	Vector3 CompruebaSuperposicion (Vector3 pos, int cont) {
		int contador = 0; // Contador de intentos, para que no se bloquee.

		for (int i = 0; i < cont; i++) {
			float distancia = Vector3.Distance(pos, bloquesCreados[i].transform.position);

			if (distancia < 1 && contador < 10) {
				contador++;

				pos.x = Random.Range(-2.6f, 2.6f);
				pos.z = Random.Range(0f, 4f);

				i = 0;
			} else {
				contador = 0;
				break;
			}
		}

		return pos;
	}
}
