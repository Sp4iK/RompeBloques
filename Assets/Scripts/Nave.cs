using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Nave : MonoBehaviour {

	public int velocidadNave = 10;
	public float velocidadBola = 5f;
	public GameObject bolaPrefab, explosionPrefab, teleportPrefab;
	public AudioClip clipTeleportar, clipExplosion;
	GameObject bola;
	Transform naveTransform;
	Renderer naveRenderer;
	AudioSource audioSource;
	bool bolaLanzada = false, lanzarBola = false;

	void Start () {
		bolaLanzada = false;
		naveTransform = GetComponent<Transform>();
		naveRenderer = GetComponent<Renderer>();
		audioSource = GetComponent<AudioSource>();
		Vector3 desfase = new Vector3 (0f, 0f, 0.35f);
		bola = (GameObject) Instantiate(bolaPrefab, naveTransform.position + desfase, Quaternion.identity);
		bola.transform.SetParent(naveTransform);
		velocidadBola += (int)ParametrosGlobales.instance.dificultadGlobal;
		velocidadBola += (int)ParametrosGlobales.instance.nivel * 0.5f;
		bola.GetComponent<Bola>().velocidad = velocidadBola;
	}

	void Update () {
		// Al pulsar la barra espaciadora del teclado/boton izq del raton se lanza la bola
		lanzarBola = Input.GetButton("Fire1");

		// Se recogen los movimientos horizontales del raton o las flechas del teclado
		float horizontal = Input.GetAxis("Horizontal") + Input.GetAxis("MouseX");

		#if UNITY_ANDROID || UNITY_IOS
		// Al tocar y levantar el dedo de la pantalla tactil se lanza la bola
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && !bolaLanzada) lanzarBola = true;

		// Se recogen los deslizamientos horizontales de un dedo sobre la pantalla
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
			horizontal = Input.GetTouch(0).deltaPosition.x / 4;
		}
		#endif

		// Se mueve la nave horizontalmente con respecto al input
		naveTransform.Translate(horizontal * velocidadNave * Time.deltaTime, 0f, 0f);
	}

	void FixedUpdate () {
		// Si la bola no ha sido lanzada aun...
		if (lanzarBola && !bolaLanzada) {
			lanzarBola = false;
			bolaLanzada = true;
			// se desemparenta de la nave
			bola.transform.parent = null;
			// y se lanza en una direccion aleatoria
			bola.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-1f, 1f), 0f, 1f) * velocidadBola;
		}
	}

	void LateUpdate () {
		// Se comprueba que la nave se mantiene entre los limites del escenario
		naveTransform.position = new Vector3(Mathf.Clamp(naveTransform.position.x, -2.6f, 2.6f), naveTransform.position.y, naveTransform.position.z);
	}

	public void DestruirNave (TipoDestruccion tipo) {
		// Ocultamos la nave para que no interfiera con el efecto visual
		naveRenderer.enabled = false;

		// Segun el caso, reproducimos el correspondiente sonido y efecto visual y destruimos el objeto nave
		switch (tipo) {
			case TipoDestruccion.Teleportar:
				audioSource.PlayOneShot(clipTeleportar);
				Instantiate(teleportPrefab, this.transform.position, Quaternion.identity);
				Destroy(this.gameObject, clipTeleportar.length);
				break;
			case TipoDestruccion.Destruir:
				audioSource.PlayOneShot(clipExplosion);
				Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
				Destroy(this.gameObject, clipExplosion.length);
				break;
		}
	}

	public enum TipoDestruccion { Teleportar, Destruir }
}
