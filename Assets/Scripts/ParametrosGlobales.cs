using UnityEngine;
using System.Collections;

public class ParametrosGlobales : MonoBehaviour {

	#region SINGLETON
	private static ParametrosGlobales _instance;

	public static ParametrosGlobales instance {
		get {
			if (_instance == null) {
				_instance = new GameObject("ParametrosGlobales").AddComponent<ParametrosGlobales>();                
			}
			return _instance;
		}
	}
	
	void Awake () {
		if (_instance == null) {
			_instance = this;            
		} else if (_instance != this)        
			Destroy(gameObject);    
		
		DontDestroyOnLoad(gameObject);
	}
	#endregion

	// Nivel de dificultad del juego
	public enum DIFICULTAD {Facil, Media, Dificil};

	// Parametros globales para el juego
	public int nivel, vidasRestantes, tiempo, bloquesACrear, puntuacionTotal;
	public DIFICULTAD dificultadGlobal;
	public string nombreJugador;
	public bool musicaOff = true;
}
