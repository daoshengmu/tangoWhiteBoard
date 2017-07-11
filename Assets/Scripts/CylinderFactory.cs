using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderFactory {	
	/// <summary>
	/// The material used for cylinder.
	/// </summary>
	private static Material m_material;

	/// <summary>
	/// The UP vector.
	/// </summary>
	private static Vector3 m_upVector = new Vector3(0.0f, 1.0f, 0.0f); 

	static CylinderFactory(){		
		m_material = new Material(Shader.Find("Diffuse"));
		m_material.color = new Color(0.3f, 0.2f, 0.6f);
	}

	/// <summary>
	/// Creates a cylinder gameobject.
	/// </summary>
	/// <param name="position">Position.</param>
	/// <param name="direction">Direction.</param>
	public static void CreateCylinder(Vector3 position, Vector3 direction){
		GameObject container = new GameObject();
		container.tag = "Cylinder";

		GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		cylinder.transform.position = new Vector3(0.0f, 0.5f, 0.0f);
		cylinder.transform.parent = container.transform;
		cylinder.GetComponent<Renderer>().material = m_material;
		cylinder.GetComponent<Renderer>().receiveShadows = false;
		cylinder.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		cylinder.GetComponent<Collider>().enabled = false;
		
		container.transform.localScale = new Vector3(0.2f, 0.1f, 0.2f);
		container.transform.position = position;
		container.transform.rotation = Quaternion.FromToRotation(m_upVector, direction);
	}
}
