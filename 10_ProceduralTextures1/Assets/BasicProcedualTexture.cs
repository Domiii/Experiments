using UnityEngine;
using System.Collections;


/// <summary>
/// Based on work by quillCreates and Lode
/// @see https://www.youtube.com/watch?v=owBt9SNKXCI
/// @see http://lodev.org/cgtutor/randomnoise.html
/// </summary>
//[ExecuteInEditMode]
[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshCollider))]
public class BasicProcedualTexture : MonoBehaviour
{
	public int seed;
	public int width = 255;
	public int height = 255;

	public PerlinNoise noise;

	protected virtual void Reset ()
	{
		seed = Random.Range (1, 1000000);
		noise = new PerlinNoise ();
	}
	
	// Use this for initialization
	protected virtual void Start ()
	{
		Reset();
		BuildMesh ();
	}

	protected virtual void StartGen() {
	}

	public virtual Color GenColor (float x, float y, int xi, int yi) {
		var c = noise.SampleNoise (x, y, seed, seed * 2);
		return new Color (c, c, c);
	}

	public void BuildTexture ()
	{
		int texWidth = width;
		int texHeight = height;
		Texture2D texture = new Texture2D (texWidth, texHeight);

		var wInv = 1.0f / width;
		var hInv = 1.0f / height;

		StartGen ();
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				var color = GenColor (x * wInv, y * hInv, x, y);
				texture.SetPixel (x, y, color);
			}
		}
		
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply ();
		
		MeshRenderer mesh_renderer = GetComponent<MeshRenderer> ();
		mesh_renderer.sharedMaterials [0].mainTexture = texture;
		
		Debug.Log ("Done Texture!");
	}

	public void BuildMesh ()
	{
		int numTiles = width * height;
		int numTris = numTiles * 2;
		
		int vsize_x = width + 1;
		int vsize_z = height + 1;
		int numVerts = vsize_x * vsize_z;
		
		// Generate the mesh data
		Vector3[] vertices = new Vector3[ numVerts ];
		Vector3[] normals = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];
		
		int[] triangles = new int[ numTris * 3 ];

		int x, z;
		for (z = 0; z < vsize_z; z++) {
			for (x = 0; x < vsize_x; x++) {
				vertices [z * vsize_x + x] = new Vector3 (x, 0, -z);
				normals [z * vsize_x + x] = Vector3.up;
				uv [z * vsize_x + x] = new Vector2 ((float)x / width, 1f - (float)z / height);
			}
		}
		Debug.Log ("Done Verts!");
		
		for (z = 0; z < height; z++) {
			for (x = 0; x < width; x++) {
				int squareIndex = z * width + x;
				int triOffset = squareIndex * 6;
				triangles [triOffset + 0] = z * vsize_x + x + 0;
				triangles [triOffset + 2] = z * vsize_x + x + vsize_x + 0;
				triangles [triOffset + 1] = z * vsize_x + x + vsize_x + 1;
				
				triangles [triOffset + 3] = z * vsize_x + x + 0;
				triangles [triOffset + 5] = z * vsize_x + x + vsize_x + 1;
				triangles [triOffset + 4] = z * vsize_x + x + 1;
			}
		}
		
		Debug.Log ("Done Triangles!");
		
		// Create a new Mesh and populate with the data
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
		
		// Assign our mesh to our filter/renderer/collider
		MeshFilter mesh_filter = GetComponent<MeshFilter> ();
		MeshCollider mesh_collider = GetComponent<MeshCollider> ();
		
		mesh_filter.mesh = mesh;
		mesh_collider.sharedMesh = mesh;
		Debug.Log ("Done Mesh!");
		
		BuildTexture ();
	}
	
	
}
