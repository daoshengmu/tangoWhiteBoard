using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Tango;
using UnityEngine;


namespace Solution{
	public class CylinderIOController : MonoBehaviour, ITangoPose
	{	
		/// <summary>
		/// Path to the data xml file.
		/// </summary>
		private const string FILE_PATH = "/sdcard/cylinders.xml";

		private TangoApplication m_tangoApplication;
		
		/// <summary>
		/// The precompute matrix from unity world to adf
		/// </summary>
		private Matrix4x4 m_unityWorld2ADF;

		/// <summary>
		/// Flag if the cylinders have been loaded
		/// </summary>
		private bool m_loaded = false;

		// Use this for initialization
		void Start () {
			m_tangoApplication = FindObjectOfType<TangoApplication>();

			if (m_tangoApplication == null)
			{
				Debug.Log("No Tango Manager found in scene.");
				Application.Quit();
			}

			m_tangoApplication.Register(this);

			m_unityWorld2ADF.SetColumn(0, new Vector4(1.0f, 0.0f, 0.0f, 0.0f));
			m_unityWorld2ADF.SetColumn(1, new Vector4(0.0f, 0.0f, 1.0f, 0.0f));
			m_unityWorld2ADF.SetColumn(2, new Vector4(0.0f, 1.0f, 0.0f, 0.0f));
			m_unityWorld2ADF.SetColumn(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		/// <summary>
		/// Unity OnGUI function.
		/// </summary>
		public void OnGUI()
		{
			if(!m_loaded){
				return;
			}
			if (GUI.Button(new Rect(10, 10, 200, 100), "<size=30>Save</size>"))
			{
				_SaveCylinders();
			}
			if (GUI.Button(new Rect(260, 10, 200, 100), "<size=30>Clear</size>"))
			{
				_ClearCylinders();
			}
		}
		
		/// <summary>
		/// OnTangoPoseAvailable event from Tango.
		/// 
		/// </summary>
		/// <param name="poseData">Returned pose data from TangoService.</param>
		public void OnTangoPoseAvailable(Tango.TangoPoseData poseData)
		{	
			// TODO: call _loadCylinders only when the device is localized
			// Hint: refer to OnTangoPoseAvalible in ADFInitializer. 
			// Cylinder should be loaded only once.
			if (poseData.framePair.baseFrame == TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_AREA_DESCRIPTION &&
			    poseData.framePair.targetFrame == TangoEnums.TangoCoordinateFrameType.TANGO_COORDINATE_FRAME_START_OF_SERVICE)
			{
				if(poseData.status_code == TangoEnums.TangoPoseStatusType.TANGO_POSE_VALID){
					if(!m_loaded){
						m_loaded = true;
						_LoadCylinders();
					}
				}
			}
		}

		// Clean all cylinders from screen
		private void _ClearCylinders(){
			List<CylinderData> xmlDataList = new List<CylinderData>();
			GameObject[] cylinders = GameObject.FindGameObjectsWithTag("Cylinder");
			
			foreach (GameObject cylinder in cylinders)
			{
				Destroy(cylinder);
			}
		}

		// Save cylinders to file
		private void _SaveCylinders(){		
			// Compose a XML data list.
			List<CylinderData> xmlDataList = new List<CylinderData>();
			GameObject[] cylinders = GameObject.FindGameObjectsWithTag("Cylinder");

			// Create xml data
			foreach (GameObject cylinder in cylinders)
			{
				CylinderData data = new CylinderData();
				Matrix4x4 unityWorld2Cylinder = Matrix4x4.TRS(cylinder.transform.position, cylinder.transform.rotation, new Vector3(1.0f, 1.0f, 1.0f));

				// TODO: Obtain the transformation of a cylinder w.r.t. the ADF, from m_unityWorld2ADF and unityWorld2Cylinder
				// And then store this transformation in data. 
				// Refer to _UnityWorldPosition of MinimapCameraUpdater.cs
				Matrix4x4 adf2Cylinder =  m_unityWorld2ADF.inverse * unityWorld2Cylinder;
				data.m_transformation = _Matrix4x4ToArray(adf2Cylinder);

				xmlDataList.Add(data);
			}
			
			var serializer = new XmlSerializer(typeof(List<CylinderData>));
			using (var stream = new FileStream(FILE_PATH, FileMode.Create))
			{
				serializer.Serialize(stream, xmlDataList);
			}
			
			AndroidHelper.ShowAndroidToastMessage("Cylinder saved");
		}

		// Load cylinders from file
		private void _LoadCylinders(){		
			XmlSerializer serializer = new XmlSerializer(typeof(List<CylinderData>));
			FileStream stream = new FileStream(FILE_PATH, FileMode.Open);
			List<CylinderData> xmlDataList = serializer.Deserialize(stream) as List<CylinderData>;
			
			if (xmlDataList == null)
			{
				return;
			}

			foreach (CylinderData data in xmlDataList)
			{
				Matrix4x4 adfTcylinder = _ArrayToMatrix4x4(data.m_transformation);
				Matrix4x4 uwTcylinder = m_unityWorld2ADF * adfTcylinder;

				// Create cylinder
				CylinderFactory.CreateCylinder(uwTcylinder.GetColumn(3), uwTcylinder.GetColumn(1));
			}
			
			AndroidHelper.ShowAndroidToastMessage("Cylinders loaded");
		}

		// Convert array to Matrix4x4
		private Matrix4x4 _ArrayToMatrix4x4(float[] elements){
			Matrix4x4 matrix = new Matrix4x4();
			matrix.m00 = elements[0];
			matrix.m01 = elements[1];
			matrix.m02 = elements[2];
			matrix.m03 = elements[3];
			matrix.m10 = elements[4];
			matrix.m11 = elements[5];
			matrix.m12 = elements[6];
			matrix.m13 = elements[7];		
			matrix.m20 = elements[8];
			matrix.m21 = elements[9];
			matrix.m22 = elements[10];
			matrix.m23 = elements[11];
			matrix.m30 = elements[12];
			matrix.m31 = elements[13];
			matrix.m32 = elements[14];
			matrix.m33 = elements[15];
			return matrix;
		}

		// Convert Matrix4x4 to array
		private float[] _Matrix4x4ToArray(Matrix4x4 matrix){
			float[] elements = new float[16];
			elements[0] = matrix.m00;
			elements[1] = matrix.m01;
			elements[2] = matrix.m02;
			elements[3] = matrix.m03;
			elements[4] = matrix.m10;
			elements[5] = matrix.m11;
			elements[6] = matrix.m12;
			elements[7] = matrix.m13;
			elements[8] = matrix.m20;
			elements[9] = matrix.m21;
			elements[10] = matrix.m22;
			elements[11] = matrix.m23;
			elements[12] = matrix.m30;
			elements[13] = matrix.m31;
			elements[14] = matrix.m32;
			elements[15] = matrix.m33;
			return elements;
		}

		/// <summary>
	    /// Used for serializing/deserializing cylinder to xml.
	    /// </summary>
	    [System.Serializable]
	    public class CylinderData
	    {
	        /// <summary>
	        /// Position of the cylinder, with respect to the origin of ADF
	        /// </summary>
	        [XmlArray("transformation")]
			public float[] m_transformation;
	    }
	}
}
