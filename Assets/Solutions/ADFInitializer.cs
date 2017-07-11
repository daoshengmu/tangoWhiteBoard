using System.Collections;
using System.Collections.Generic;
using Tango;
using UnityEngine;

namespace Solution{

	// This class must implement ITangoLifecycle in order to request
	// permissions and load an ADF.
	public class ADFInitializer : MonoBehaviour, ITangoPose, ITangoLifecycle
	{
		/// <summary>
		/// The UUID of local ADF to load
		/// </summary>
		public string m_adfUUID;
		
		/// <summary>
		/// Flag if device is localized in ADF
		/// </summary>
		private bool m_localized = false;

		private TangoApplication m_tangoApplication;

		// Use this for initialization
		void Start () {
			m_tangoApplication = FindObjectOfType<TangoApplication>();

			if (m_tangoApplication == null)
			{
				Debug.Log("No Tango Manager found in scene.");
				Application.Quit();
			}

			m_tangoApplication.Register(this);

			// Request permissions from TangoApplication
			m_tangoApplication.RequestPermissions();
		}

		// Update is called once per frame
		void Update () {
		}

		/// <summary>
		/// OnTangoPoseAvailable event from Tango.
		/// 
		/// </summary>
		/// <param name="poseData">Returned pose data from TangoService.</param>
		public void OnTangoPoseAvailable(Tango.TangoPoseData poseData)
		{
			// Inform the user when the device is localized

			// 1. Wait for a valid pose from area description to start of service.
			if (poseData.framePair.baseFrame == TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_AREA_DESCRIPTION &&
			    poseData.framePair.targetFrame == TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_START_OF_SERVICE)
			{
				if(poseData.status_code == TangoEnums.TangoPoseStatusType.TANGO_POSE_VALID){

					// 2. Inform the user with AndroidHelper.ShowAndroidToastMessage
					if(!m_localized) {
						AndroidHelper.ShowAndroidToastMessage("Device localized");
						m_localized = true;
					}
				}
			}
		}

		/// <summary>
		/// Internal callback when a permissions event happens.
		/// </summary>
		/// <param name="permissionsGranted">If set to <c>true</c> permissions granted.</param>
		public void OnTangoPermissions(bool permissionsGranted)
		{
			// Start up TangoApplication with the specified ADF once
			// permissions have been granted.
			if (permissionsGranted)
			{
				// 1. Get list of AreaDescriptions available on device
				AreaDescription[] list = AreaDescription.GetList();

				// 2. Find the ADF with UUID matching m_adfUUID
				foreach(AreaDescription ad in list){
					if(ad.m_uuid.Equals(m_adfUUID)){
						// 3. Startup TangoApplication
						m_tangoApplication.Startup(ad);
						return;
					}
				}
				AndroidHelper.ShowAndroidToastMessage("ADF does not exist");
			}
		}

		/// <summary>
		/// This is called when successfully connected to the Tango service.
		/// </summary>
		public void OnTangoServiceConnected()
		{
		}

		/// <summary>
		/// This is called when disconnected from the Tango service.
		/// </summary>
		public void OnTangoServiceDisconnected()
		{
		}
	}
}
