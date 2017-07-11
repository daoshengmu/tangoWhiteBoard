using System.Collections;
using System.Collections.Generic;
using Tango;
using UnityEngine;
	
// This class must implement ITangoLifecycle in order to request
// permissions and load an ADF.
public class ADFLifeCycle : MonoBehaviour, ITangoPose, ITangoLifecycle
{
	/// <summary>
	/// The UUID of local ADF to load
	/// </summary>
	public string m_adfUUID;
	
	/// <summary>
	/// The overlay image of the relocalization process.
	/// </summary>
	public GameObject m_relocalizationOverlay;
	
	/// <summary>
	/// Flag if tango application is started
	/// </summary>
	private bool m_initialize = false;
	
	/// <summary>
	/// Flag if cylinders are loaded
	/// </summary>
	private bool m_loaded = false;
	
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
		
		m_relocalizationOverlay.SetActive(true);
		
		// Request permissions from TangoApplication
		m_tangoApplication.RequestPermissions();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	/// <summary>
	/// Application onPause / onResume callback.
	/// </summary>
	/// <param name="pauseStatus"><c>true</c> if the application about to pause, otherwise <c>false</c>.</param>
	public void OnApplicationPause(bool pauseStatus)
	{
		if(!m_initialize) return;
		
		// TODO: Handle status of Tango Application when the app is paused and resumed

		
		// TODO: Activate overlay image for both pause and resume

	}
	
	/// <summary>
	/// OnTangoPoseAvailable event from Tango.
	/// 
	/// </summary>
	/// <param name="poseData">Returned pose data from TangoService.</param>
	public void OnTangoPoseAvailable(Tango.TangoPoseData poseData)
	{
		// TODO: Set Activity of overlay image, based on frame pair and validity of pose data
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
			_StartTango();
			m_initialize = true;
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
	
	private void _StartTango(){
		
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
