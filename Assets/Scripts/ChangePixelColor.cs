using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangePixelColor : MonoBehaviour {
	public Material surfMat;
	public Color drawColor = Color.red;
	private Texture2D _texture;
	private bool bDebugMode = true;

	// Use this for initialization
	void Start () {
		this.gameObject.GetComponent<Renderer>().enabled = false;
		_texture = new Texture2D(128, 128);

		// Fill the texture with white pixels!
		for (int y  = 0; y < _texture.height; ++y) {
			for (int x = 0; x < _texture.width; ++x) {
				var color = Color.clear;
				_texture.SetPixel(x, y, color);
			}
		}
		// Apply all SetPixel calls
		_texture.Apply();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI() {

		if (bDebugMode) {
			Event evt = Event.current;

			if (evt.isMouse && Input.GetMouseButton (0)) {
				// Send a ray to collide with the plane
				//Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
					return;

				PaintPixel(hit);
			}
		}else {
			if (EventSystem.current.IsPointerOverGameObject(0) || GUIUtility.hotControl != 0) {
				return;
			}
			if (Input.touchCount == 1) {
				Touch t = Input.GetTouch(0);
				//Vector2 guiPosition = new Vector2(t.position.x, Screen.height - t.position.y);

				if (t.phase != TouchPhase.Began) {
					return;
				}

				RaycastHit hit;
				// We didn't touch the surface that we created
				if (!Physics.Raycast(Camera.main.ScreenPointToRay(t.position), out hit)) {
					return;
				}

				PaintPixel(hit);
			}
		}
	}

	void PaintPixel(RaycastHit hit)
	{
		this.gameObject.GetComponent<Renderer>().enabled = true;
		//if (collider.Raycast(ray, out hit, Mathf.Infinity))
		{
			// Find the u,v coordinate of the Texture
			//Vector2 uv;
//			uv.x = (hit.point.x - hit.collider.bounds.min.x) / hit.collider.bounds.size.x;
//			uv.y = (hit.point.y - hit.collider.bounds.min.z) / hit.collider.bounds.size.z;
			// Paint it red
			var renderer = hit.transform.gameObject.GetComponent<Renderer> ();

			if (renderer) {
				// Texture2D tex = (Texture2D)renderer.material.mainTexture;
				//_texture.SetPixel ((int)(uv.x * _texture.width), (int)(uv.y * _texture.height), Color.red);
				_texture.SetPixel((int)(hit.textureCoord.x * _texture.width),
								  (int)(hit.textureCoord.y * _texture.height),
								   drawColor); 
				_texture.Apply ();
				renderer.material.mainTexture = _texture;
			}
		}
	}
}
