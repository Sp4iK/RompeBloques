using UnityEngine;
using System.Collections;

public class FijarFondo : MonoBehaviour {

	public Texture[] fondos;

	Renderer _renderer;
	
	void Start () {
		_renderer = GetComponent<Renderer>();

		int fondo = ParametrosGlobales.instance.nivel < fondos.Length ? ParametrosGlobales.instance.nivel - 1 : 0;
		_renderer.material.mainTexture = fondos[fondo];
	}
}
