using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DebugLabels : MonoBehaviour {
	public int maxLabelCount = 100;
	public Color labelColor = Color.white;
	public int fontSize = 24;
	public Camera camera;

	int label0;
	Canvas canvas;
	Text[] labels;

	Vector3?[] worldPositions;
	string[] labelTexts;

	// Use this for initialization
	void Start () {
		var canvasGo = new GameObject ();
		canvas = canvasGo.AddComponent<Canvas> ();
		//canvas.runInEditMode = true;
		//canvas.GetComponent<RectTransform>().size
	}

	void EnsureBuffer<T>(T[] buf) {
		if (buf == null) {
			buf = new T[maxLabelCount];
		}
		else if (buf.Length < maxLabelCount) {
			System.Array.Resize (ref buf, maxLabelCount);
		}
	}
	
	// Update is called once per frame
	void Update () {
		EnsureBuffer (labels);
		EnsureBuffer (worldPositions);
		EnsureBuffer (labelTexts);

		// create all missing labels
		for (; label0 < maxLabelCount; ++label0) {
			var i = label0;
			var label = labels [i];
			if (label == null) {
				var go = new GameObject ();
				labels[i] = label = go.GetComponent<Text>();
				label.horizontalOverflow = HorizontalWrapMode.Overflow;
				label.verticalOverflow = VerticalWrapMode.Overflow;
				label.alignment = TextAnchor.MiddleCenter;
				label.resizeTextForBestFit = true;
				label.runInEditMode = true;
				label.color = labelColor;
				label.fontSize = fontSize;
			}
		}

		for (var i = 0; i < maxLabelCount; ++i) {
			var worldPosRef = worldPositions [i];
			if (!worldPosRef.HasValue)
				continue;
			
			var pos = camera.WorldToScreenPoint (worldPosRef.Value);
			var txt = labelTexts [i];
			// TODO: place label at position and display text
		}
	}
}
