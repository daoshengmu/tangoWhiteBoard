using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualObjectCreator : MonoBehaviour {

	/// <summary>
	/// The object number to create.
	/// </summary>
	private int objectNumber = 40;

	/// <summary>
	/// The unique random object
	/// </summary>
	private System.Random rand = new System.Random();

	// Use this for initialization
	void Start () {		
		for(int i = 0; i < objectNumber; i ++){			
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(
				_RandomFloat(-10.0f, 10.0f),
				_RandomFloat(3.0f, 4.0f),
				_RandomFloat(-10.0f, 10.0f));
			Material material = new Material(Shader.Find("Diffuse"));
			material.color = new Color(
				_RandomFloat(0.1f, 0.9f),
				_RandomFloat(0.1f, 0.9f),
				_RandomFloat(0.1f, 0.9f));
			cube.GetComponent<Renderer>().material = material;
			cube.GetComponent<Renderer>().receiveShadows = false;
			cube.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			cube.GetComponent<Collider>().enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	// Random float within a certain range
	float _RandomFloat (float low, float high){
		return (float) rand.NextDouble() * (high - low) + low;
	}
}

