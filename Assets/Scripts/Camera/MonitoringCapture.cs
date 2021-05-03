using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class MonitoringCapture : MonoBehaviour
{
    class ServerResponse
    {
        public string name, type, size, timestamp;
    }

    public enum ImageFormat { JPG, PNG }

    public float interval;

    [Space]
    public int resWidth = 1080;
    public int resHeight = 720;

    [Space]
    public ImageFormat imageFormat;

    [Space]
    public string uploadURL = "http://www.my-server.com/myform";
    public float pingInterval = 3f;
    public static bool serverON = false;

    [Space]
    public EventMessage eventMessage;

    private bool takeHiResShot = false;
    Camera camera;

    public float minimumInterval = 1.0f;
    private float currentIntervalCount;
    private bool screenShotEnable;

    private void Start()
    {
        screenShotEnable = true;
        camera = GetComponent<Camera>();
        //StartCoroutine(UploadImage("D:\\Workspace\\Unity\\LCS - bridge simulation\\Assets\\Screenshots\\screen_810x540_2021-03-20_20-33-54.jpg"));

        currentIntervalCount = minimumInterval;
    }

    private void Update()
    {
        if (!screenShotEnable)
        {
            currentIntervalCount -= Time.deltaTime;
            if(currentIntervalCount <= minimumInterval)
            {
                currentIntervalCount = minimumInterval;
                screenShotEnable = true;
            }
        }
    }

    void LateUpdate()
    {
        takeHiResShot |= Input.GetKeyDown("k");
        if (takeHiResShot)
        {
            //UploadImage(filename, "Acclerometer 1", 1, true);
            takeHiResShot = false;
        }
    }

    public void TakeScreenShot(string sensorName, int level, bool sendToServer = false)
    {
        if (!screenShotEnable) return;

        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);

        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);

        byte[] bytes;
        if (imageFormat == ImageFormat.PNG) bytes = screenShot.EncodeToPNG();
        else bytes = screenShot.EncodeToJPG();

        string filename = ScreenShotName(resWidth, resHeight, imageFormat);
        //System.IO.File.WriteAllBytes(filename, bytes);

        if (sendToServer)
        {
            StartCoroutine(UploadImage(filename, bytes, sensorName, level));
        }
    }

    public static string ScreenShotName(int width, int height, ImageFormat imageFormat)
    {
        if(imageFormat == ImageFormat.PNG)
            return string.Format("screen_{0}x{1}_{2}.png",
                                 width, height,
                                 System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        else
            return string.Format("screen_{0}x{1}_{2}.jpg",
                                 width, height,
                                 System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }

    private IEnumerator UploadImage(string filename, byte[] imageBytes, string sensorName = "Acclerometer 1", int level = 1)
    {
        string contentType = "image/" + filename.Split('.')[1];

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("fileUpload", imageBytes, filename, contentType));
        formData.Add(new MultipartFormDataSection("sensorName", sensorName));
        formData.Add(new MultipartFormDataSection("level", level.ToString()));

        UnityWebRequest www = UnityWebRequest.Post(uploadURL, formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            eventMessage.SetMessage(string.Format("Cannot send image : {0}", www.error));
        }
        else
        {
            // Body
            string json = www.downloadHandler.text;
            ServerResponse response = JsonUtility.FromJson<ServerResponse>(json);

            string info = "";
            switch(level){
                case 1:
                    {
                        info = "Danger";
                        break;
                    }
                case 2:
                    {
                        info = "Warning";
                        break;
                    }
            }

            eventMessage.SetMessage(string.Format("{0} Caution. Sending image {1}", info, response.name));
        }
    }
}
