using UnityEngine;
using UnityEngine.Audio; // Importamos el nombre de espacio para anejar audio
using System.Collections;

public class AudioMixer : MonoBehaviour {

	public AudioSource source;
	public AudioMixerGroup master, eco;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Keypad1)) {
			source.outputAudioMixerGroup = eco;
		} else if (Input.GetKeyDown(KeyCode.Keypad2)) {
			source.outputAudioMixerGroup = master;
		}
	}
}
