using System.Collections;
using System.Collections.Generic;
using Tango;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solution{

	// This class must implement ITangoPointCloud to recieve point cloud updates
	public class TouchController : MonoBehaviour, ITangoPointCloud
	{
		/// <summary>
		/// The point cloud object in the scene.
		/// https://developers.google.com/tango/apis/unity/reference/class/tango-point-cloud
		/// </summary>
		public TangoPointCloud m_pointCloud;

		private TangoApplication m_tangoApplication;

		private Vector2 m_touchPosition;

		/// <summary>
		/// Flag if user touches screen and waits for a plane to be created
		/// </summary>
		private bool m_touched = false;

		// Use this for initialization
		void Start () {
			m_tangoApplication = FindObjectOfType<TangoApplication>();

			if (m_tangoApplication == null)
			{
				Debug.Log("No Tango Manager found in scene.");
				Application.Quit();
			}

			m_tangoApplication.Register(this);
		}
		
		// Update is called once per frame
		void Update () {
			if (EventSystem.current.IsPointerOverGameObject(0) || GUIUtility.hotControl != 0)
			{
				return;
			}
			if (Input.touchCount == 1)
			{
				Touch t = Input.GetTouch(0);
				Vector2 guiPosition = new Vector2(t.position.x, Screen.height - t.position.y);
				Camera cam = Camera.main;
				RaycastHit hitInfo;

				if (t.phase != TouchPhase.Began)
				{
					return;
				}

				// Don't put a cylinder on another cylinder
				if (Physics.Raycast(cam.ScreenPointToRay(t.position), out hitInfo))
				{
					return;
				} 
				else
				{
					// Store the touch position
					m_touchPosition = t.position;
					m_touched = true;
				}
			}
		}
		
		/// <summary>
		/// Callback that gets called when depth is available from the Tango Service.
		/// </summary>
		/// <param name="pointCloud">Depth information from Tango.</param>
		public void OnTangoPointCloudAvailable(TangoPointCloudData pointCloud)
		{
			// If the user touched the screen, call _FindPlaneAndCreateCylinder.
			if(m_touched) {
				m_touched = false;
				_FindPlaneAndCreateCylinder(m_touchPosition);
			}
		}

		/// <summary>
		/// Find the plane from touch position and create a new cylinder
		/// </summary>
		/// <param name="touchPosition">Touch position to find a plane at.</param>
		private void _FindPlaneAndCreateCylinder(Vector2 touchPosition)
		{
			// From point cloud data, find the plane where the user touched, and place a new cylinder
			Camera camera = Camera.main;
			Vector3 planeCenter;
			Plane plane;

			// 1. Use TangoPointCloud.FindPlane to get a plane where the user touched.
			// If a plane cannot be found, inform the user about the failure. You can use
			// AndroidHelper.ShowAndroidToastMessage to do so.
			// https://developers.google.com/tango/apis/unity/reference/class/tango-point-cloud
			if (m_pointCloud.FindPlane(camera, touchPosition, out planeCenter, out plane))
			{
				// 2. Use CylinderFactory to create a new cylinder at the plane center.
				CylinderFactory.CreateCylinder(planeCenter, plane.normal);
			}
			else {
				AndroidHelper.ShowAndroidToastMessage("Plane cannot be found here.");
			}
		}
	}
}