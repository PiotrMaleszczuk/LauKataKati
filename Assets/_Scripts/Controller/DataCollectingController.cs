using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DataCollectingController : MonoBehaviour {

	private string fileName;
	private string startTime;
	private static DataCollectingController instance;
	public static DataCollectingController Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		if (DataCollectingController.instance == null)
		{
			DataCollectingController.instance = this;
			Init ();
			DontDestroyOnLoad(this.gameObject);
		}
		startTime = System.DateTime.UtcNow.ToString("HH:mm dd MMMM, yyyy");
	}

	void Init()
	{
		fileName = Application.persistentDataPath+'/'+"dataCollecting.txt";
		if (!File.Exists (fileName)) {

			string deviceInfo = "Model: " + SystemInfo.deviceModel.ToString() +
			" | Type: " + SystemInfo.deviceType + 
			" | OS Version: " + SystemInfo.operatingSystem + 
			" | System Memory: " + SystemInfo.systemMemorySize + 
			" | Graphic Device: " + SystemInfo.graphicsDeviceName + 
			" | (" + SystemInfo.graphicsDeviceVersion + ")" + 
			" | Graphic Memory: " + SystemInfo.graphicsMemorySize + 
			" | Graphic Fill Rate: " + SystemInfo.graphicsPixelFillrate + 
			" | Graphic Max TexSize: " + SystemInfo.maxTextureSize + 
			" | Graphic Shader Levl: " + SystemInfo.graphicsShaderLevel + 
			" | Support Compute Shader: " + SystemInfo.supportsComputeShaders + 
			" | Processor Count: " + SystemInfo.processorCount + 
			" | Processor Type: " + SystemInfo.processorType + 
			" | Support 3D Texture: " + SystemInfo.supports3DTextures + 
			" | Support Shadow: " + SystemInfo.supportsShadows;
			SaveData(deviceInfo);
		}
		StartCoroutine (SaveGpsInfo());
	}

	void OnApplicationQuit()
	{
		SaveData ("ApplicationQuit - StartTime: " + startTime + ", EndTime: " + System.DateTime.UtcNow.ToString ("HH:mm dd MMMM, yyyy"));
	}

	public void SaveData(String s)
	{
		using (StreamWriter sw = new StreamWriter (fileName, true)) {
			Debug.Log ("Zapisuje...");
			sw.WriteLine (s+'\n');
			sw.Close ();
		}


	}
	public void PrintData()
	{
		using (StreamReader sr = new StreamReader (fileName)) {
			Debug.Log ("Odczytuje...");
			Debug.Log (sr.ReadToEnd());
			sr.Close ();
		}
	}


	private IEnumerator SaveGpsInfo()
	{
		Debug.Log ("begin");
		// First, check if user has location service enabled
		if (!Input.location.isEnabledByUser)
			yield break;
		Debug.Log ("enabled by user");
		// Start service before querying location
		Input.location.Start();
		Debug.Log ("started");

		// Wait until service initializes
		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
			yield return new WaitForSeconds(1);
			maxWait--;
			Debug.Log ("Initializing: "+maxWait);

		}

		// Service didn't initialize in 20 seconds
		if (maxWait < 1)
		{
			print("Timed out");
			yield break;
		}

		// Connection has failed
		if (Input.location.status == LocationServiceStatus.Failed)
		{
			print("Unable to determine device location");
			yield break;
		}
		else
		{
			// Access granted and location value could be retrieved
			SaveData("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
		}

		// Stop service if there is no need to query location updates continuously
		Input.location.Stop();
	}
		
}
