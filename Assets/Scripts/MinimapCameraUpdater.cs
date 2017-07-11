using System.Collections;
using System.Collections.Generic;
using Tango;
using UnityEngine;

// TODO: This class must implement ITangoPose to receive the pose data updates.
public class MinimapCameraUpdater : MonoBehaviour
{
	/// <summary>
	/// The tango application.
	/// </summary>
	private TangoApplication m_tangoApplication;
	
	/// <summary>
	/// The precompute matrix from unity world to start of service.
	/// </summary>
	private Matrix4x4 m_unityWorld2StartOfService;
	
	// Use this for initialization
	void Start () {
		m_tangoApplication = FindObjectOfType<TangoApplication>();
		
		if (m_tangoApplication == null)
		{
			Debug.Log("No Tango Manager found in scene.");
			Application.Quit();
		}

		// TODO: Register this object with the TangoApplication

		m_unityWorld2StartOfService.SetColumn(0, new Vector4(1.0f, 0.0f, 0.0f, 0.0f));
		m_unityWorld2StartOfService.SetColumn(1, new Vector4(0.0f, 0.0f, 1.0f, 0.0f));
		m_unityWorld2StartOfService.SetColumn(2, new Vector4(0.0f, 1.0f, 0.0f, 0.0f));
		m_unityWorld2StartOfService.SetColumn(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	/// <summary>
	/// OnTangoPoseAvailable event from Tango.
	/// </summary>
	/// <param name="poseData">Returned pose data from TangoService.</param>
	public void OnTangoPoseAvailable(Tango.TangoPoseData poseData)
	{	
		// TODO: Update this camera's X and Z position to match the device.
		// This way it will always be directly above the user.
		
		// 1. Check that you can use this poseData
		// You must make sure the poseData status is valid and that the frame pair
		// is from start-of-service to device.
		// https://developers.google.com/tango/overview/frames-of-reference

		// 2. Map the device position to Unity world coordinates.
		// https://developers.google.com/tango/overview/coordinate-systems
		
		// 3. Update the position of this camera
	}
	
	/// <summary>
	/// Get unity world position of the device from the pose data.
	/// </summary>
	/// <returns>The unity world position or the device.</returns>
	/// <param name="poseData">Pose data from Start of Service to Device.</param>
	private Vector3 _UnityWorldPosition(TangoPoseData poseData) {
		Matrix4x4 startOfService2Device = poseData.ToMatrix4x4();
		Matrix4x4 unityWorld2Device = m_unityWorld2StartOfService * startOfService2Device;
		return unityWorld2Device.GetColumn(3);
	}
}