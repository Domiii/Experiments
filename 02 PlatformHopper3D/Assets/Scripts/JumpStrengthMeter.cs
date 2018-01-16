using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class JumpStrengthMeter : MonoBehaviour {
	public Color minColor = Color.red;
	public Color maxColor = Color.green;
	public Player player;
	Slider slider;

	void Awake() {
		slider = GetComponent<Slider> ();
	}

	void Update () {
		slider.value = player.jumpStrength;
		//slider.colors.normalColor
	}
}
