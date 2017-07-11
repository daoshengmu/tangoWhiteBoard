using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushColor : MonoBehaviour {
	public Color brushColor;

	void OnSetColor(Color color)
	{
		//Material mt = new Material(GetComponent<Renderer>().sharedMaterial);
		//mt.color = color;
		//GetComponent<Renderer>().material = mt;
		this.gameObject.GetComponent<ChangePixelColor>().drawColor = color;
	}

	void OnGetColor(ColorPicker picker)
	{
		Color color = this.gameObject.GetComponent<ChangePixelColor>().drawColor;
//		picker.NotifyColor(GetComponent<Renderer>().material.color);
		picker.NotifyColor(color);
	}
}
