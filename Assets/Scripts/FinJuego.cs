using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FinJuego : MonoBehaviour {

	// El componente Animator de la ventana emergente
	public Animator animator;

	public Text txtTitulo, txtPuntuacion;

	// Metodo para mostrar la ventana
	public void Mostrar(string titulo, int puntuacion) {
		txtTitulo.text = titulo;
		txtPuntuacion.text = "PUNTUACION FINAL:\n" + puntuacion;
		animator.SetInteger("estado", (int) TransicionPerder.Mostrar);
	}

	// Metodo para ocultar la ventana
	public void Ocultar() {
		animator.SetInteger("estado", (int) TransicionPerder.Ocultar);
	}
}

// Definimos los estados para las transiciones de la animacion con tipos enumerados
public enum TransicionPerder {
	Ocultar,	// Estado por ocultar
	Mostrar	// Estado de mostrar
}
