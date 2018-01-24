using UnityEditor;
using UnityEngine;
using System.Collections;



[CustomEditor(typeof(WoodProceduralTexture))]
[CanEditMultipleObjects]
public class WoodProceduralTextureEditor : BasicProceduralTextureEditor {	
}

[CustomEditor(typeof(MarbleProceduralTexture))]
[CanEditMultipleObjects]
public class MarbleProceduralTextureEditor : BasicProceduralTextureEditor {	
}

[CustomEditor(typeof(BasicProcedualTexture))]
[CanEditMultipleObjects]
public class BasicProceduralTextureEditor : Editor {
	
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if(GUILayout.Button("Randomize + Gen Texture")) {
			BasicProcedualTexture tileMap = (BasicProcedualTexture)target;
			tileMap.seed = Random.Range (1, 10000);
			tileMap.BuildTexture();
		}

		if(GUILayout.Button("Gen Texture")) {
			BasicProcedualTexture tileMap = (BasicProcedualTexture)target;
			tileMap.BuildTexture();
		}

		if(GUILayout.Button("Build all!")) {
			BasicProcedualTexture tileMap = (BasicProcedualTexture)target;
			tileMap.BuildMesh();
		}
	}
}
