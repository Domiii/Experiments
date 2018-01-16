using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Draggable sprites can be dragged by mouse.
/// </summary>
public class DraggableSprite : MonoBehaviour {
	public SpriteRenderer sprite;
	public bool canDrag = true;

	private bool isDragging = false;
	private Vector3 dragOffset;
	private bool restrictX;
	private bool restrictY;
	private float fakeX;
	private float fakeY;
	private float myWidth;
	private float myHeight;
	private Rigidbody2D body;

	void Start()
	{
		body = GetComponent<Rigidbody2D> ();
		if (sprite == null) {
			sprite = GetComponent<SpriteRenderer> ();
		}
		if (sprite == null) {
			Debug.LogError ("sprite not assigned in DraggableSprite", this);
			return;
		}
		//SnapToGrid ();
	}


	void OnMouseDown() 
	{
		if (canDrag) {
			isDragging = true;
			var mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			mousePos.z = 0;

			dragOffset = (transform.position - mousePos);

			// for grids: make sure that the cell of the sprite that the mouse touched is always in the same cell as the mouse
			//dragOffset = (transform.position - sprite.bounds.min) - BuildingGrid.Instance.SnapToGridFloor (mousePos - sprite.bounds.min + Vector3.one * 0.01f);

			body.gravityScale = 0;
		}
	}

	void OnMouseUp() 
	{
		if (isDragging) {
			isDragging = false;
			body.gravityScale = 1;
		}
	}

//	void SnapToGrid() {
//		Grid.SnapToGrid (sprite);
//	}


	void Update () 
	{
		if (isDragging) {
			var mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			mousePos.z = 0;
			//transform.position = mousePos + dragOffset;
			body.MovePosition(mousePos);
			//SnapToGrid ();
		}
	}
}