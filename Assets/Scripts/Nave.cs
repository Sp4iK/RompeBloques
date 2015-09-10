using UnityEngine;
using System.Collections;

public class Nave : MonoBehaviour {

	public int velocidadNave = 10;
	public int velocidadBola = 5;
	public GameObject bolaPrefab, explosionPrefab;
	GameObject bola;
	Transform naveTransform;
	bool bolaLanzada = false, lanzarBola = false;

	void Start () {
		bolaLanzada = false;
		naveTransform = GetComponent<Transform> ();
		Vector3 desfase = new Vector3 (0f, 0f, 0.35f);
		bola = (GameObject) Instantiate(bolaPrefab, naveTransform.position + desfase, Quaternion.identity);
		bola.transform.SetParent(naveTransform);
		bola.GetComponent<Bola>().velocidad = velocidadBola;
	}

	void Update () {
		float horizontal = Input.GetAxis("Horizontal") + Input.GetAxis("MouseX");

		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
			horizontal = Input.GetTouch(0).deltaPosition.x / 4;
		}

		naveTransform.Translate(horizontal * velocidadNave * Time.deltaTime, 0f, 0f);
	}

	void FixedUpdate () {
		if (/*(Input.GetButtonDown("Fire1")*/lanzarBola && bolaLanzada == false) {
			lanzarBola = false;
			bolaLanzada = true;
			bola.transform.parent = null;
			bola.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(0f, 1f), 0f, 1f) * velocidadBola;
		}
	}

	void LateUpdate () {
		naveTransform.position = new Vector3(Mathf.Clamp(naveTransform.position.x, -2.6f, 2.6f), naveTransform.position.y, naveTransform.position.z);
	}

	// Evento que responde tanto en PC como en Android
	void OnMouseUp () {
		lanzarBola = true;
	}
	
	void OnDestroy () {
		Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
		StartCoroutine(EsperarYDestruir());
	}

	private IEnumerator EsperarYDestruir () {
		yield return new WaitForSeconds(2f);
		Destroy(explosionPrefab.gameObject);
	}
}
