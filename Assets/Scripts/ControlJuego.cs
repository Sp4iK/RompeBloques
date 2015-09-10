using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlJuego : MonoBehaviour {

	//'static' es usado para preservar su valor durante la ejecucion del programa, independientemente de las escenas
	//[HideInInspector] por encima de las variables sirve para ocultarlas en el inspector

	public GameObject navePrefab;
	public GameObject posicionInicialNave;
	public GameObject bloquePrefab;
	GameObject nave;
	GameObject[] bloquesCreados;

	public Material bloqueSimple, bloqueMedio, bloqueMacizo;

	public static int vidasRestantes, tiempo, bloquesACrear, puntuacionTotal;
	public bool aleatorio = true;
	int bloques;
	int puntuacion, vidas;
	float tiempoRestante;

	// La referencia al texto en pantalla
	public Text txtPuntuacion, txtVidas, txtTiempo;

	// Referencia a la ventana emergente para poder mostrarla
	public FinJuego finJuego;

	// Referencia al animator del panel de pausa
	public Animator animatorPausa;

	[HideInInspector]
	public enum ESTADO {Jugando, Pausa, Finalizado};
	ESTADO estado;

	void Start () {
		InicializarEscena();
		InicializarNave();
		estado = ESTADO.Jugando;
	}

	void Update () {
		tiempoRestante -= Time.deltaTime;
		if (estado != ESTADO.Finalizado) txtTiempo.text = Mathf.RoundToInt(tiempoRestante).ToString();

		if (Input.GetKeyDown(KeyCode.Escape)) {
			estado = ESTADO.Pausa;
			// Llamada a funcion para mostrar menu de pausa
			Pausa((int)estado);
		}

		if (Input.GetKeyDown(KeyCode.Space) && estado == ESTADO.Finalizado) {
			ReiniciarJuego();
		}

		if (bloques <= 0 && estado != ESTADO.Finalizado) {
			Fin();
		}

		if (tiempoRestante <= 0f && estado != ESTADO.Finalizado) {
			Debug.Log("¡SE ACABO EL TIEMPO!");
			Fin();
		}
	}
	
	void InicializarNave () {
		nave = (GameObject) Instantiate(navePrefab, posicionInicialNave.transform.position, Quaternion.identity);
	}
	
	void InicializarEscena () {
		tiempoRestante = tiempo;
		txtTiempo.text = tiempoRestante.ToString();
		puntuacion = 0;
		vidas = vidasRestantes;
		txtPuntuacion.text = "PUNTUACION:\n" + puntuacion;
		txtVidas.text = "VIDAS: " + vidas;

		if (aleatorio) {
			bloquesCreados = new GameObject[bloquesACrear];

			for (int i=0; i<bloquesACrear; i++) {
				float posX = Random.Range(-2.6f, 2.6f);
				float posZ = Random.Range(0f, 4f);
				Vector3 posicionBloque = new Vector3(posX, 0.25f, posZ);
				bloquesCreados[i] = (GameObject) Instantiate(bloquePrefab, posicionBloque, Quaternion.Euler(0f, 0f, 90f));
				int bloqueTipo = Random.Range(0,3);
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

				if (i > 0) bloquesCreados[i].transform.position = CompruebaSuperposicion(posicionBloque, i);
			}
		}

		bloques = GameObject.FindGameObjectsWithTag("bloque").Length;
		Debug.Log("Bloques: " + bloques);
	}

	public void RestaBloque () {
		bloques--;
	}

	public void SumaPuntos (int puntos) {
		puntuacion += puntos;
		txtPuntuacion.text = "PUNTUACION:\n" + puntuacion;
	}

	public void Pausa (int estado) {
		switch (estado) {
		case (int)ESTADO.Pausa:
			animatorPausa.SetInteger("estado", estado);
			animatorPausa.speed = 5f;
			Time.timeScale = 0.001f;
			break;
		case (int)ESTADO.Jugando:
			animatorPausa.SetInteger("estado", estado);
			Time.timeScale = 1f;
			break;
		case -1:
			Application.LoadLevel(0);
			break;
		}
	}

	public void Fin () {
		Destroy(nave.gameObject);
		Destroy(GameObject.FindGameObjectWithTag("bola").gameObject);

		if (vidas <= 0) {
			estado = ESTADO.Finalizado;
			Debug.Log("HAS PERDIDO, ¡PAQUETE!" + "\n" + "PUNTUACION: " + puntuacion);
			txtVidas.CrossFadeAlpha(0f, 1f, true);
			txtTiempo.CrossFadeAlpha(0f,1f, true);
			txtPuntuacion.CrossFadeAlpha(0f, 1f, true);
			finJuego.Mostrar("HAS PERDIDO, ¡PAQUETE!", puntuacion);
		} else if (bloques <= 0) {
			estado = ESTADO.Finalizado;
			puntuacionTotal += puntuacion * (int) tiempoRestante * vidas / 10;
			Debug.Log("¡HAS GANADO!" + "\n" + "PUNTUACION NIVEL: " + (puntuacion * (int) tiempoRestante * vidas / 10) + "| PUNTUACION TOTAL: " + puntuacionTotal);
			txtVidas.CrossFadeAlpha(0f, 1f, true);
			txtTiempo.CrossFadeAlpha(0f,1f, true);
			txtPuntuacion.CrossFadeAlpha(0f, 1f, true);
			finJuego.Mostrar("¡HAS GANADO!", puntuacionTotal);
		} else {
			vidas--;
			txtVidas.text = "VIDAS: " + vidas;
			InicializarNave();
			tiempoRestante = tiempo;
			Debug.Log("VUELVE A INTENTARLO" + "\n" + "VIDAS: " + vidas);
		}
	}

	public void ReiniciarJuego () {
		// Lanzar la animacion de ocultar la ventana
		finJuego.Ocultar();
		Debug.Log("JUEGO REINICIADO");
		// Lanzamos la corrutina
		StartCoroutine(EsperarYCargar(2f, Application.loadedLevel));
	}

	public void Salir () {
		finJuego.Ocultar();
		StartCoroutine(EsperarYCargar(2f, 0));
	}

	private IEnumerator EsperarYCargar (float segundos, int nivel) {
		// Forzar al programa a esperar
		yield return new WaitForSeconds(segundos);
		Application.LoadLevel(nivel);
	}

	Vector3 CompruebaSuperposicion (Vector3 pos, int cont) {
		for (int i=0; i<cont; i++) {
			float distancia = Vector3.Distance(pos, bloquesCreados[i].transform.position);

			if (distancia < 1) {
				pos.x = Random.Range(-2.6f, 2.6f);
				pos.z = Random.Range(0f, 4f);

				i = 0;
			}
		}

		return pos;
	}
}
