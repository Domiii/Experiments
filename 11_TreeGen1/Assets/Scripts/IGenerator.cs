using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGenerator {
	GameObject gameObject {
		get;
	}
}

public interface IMeshGenerator : IGenerator {
	MeshFilter GenerateMesh(int seed);
}


public interface ISpriteGenerator : IGenerator {
	SpriteRenderer GenerateSprite(int seed);
}