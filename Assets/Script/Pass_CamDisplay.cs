using UnityEngine;
using PassthroughCameraSamples;
using System.IO;
using System.Collections;

public class Pass_CamDisplay : MonoBehaviour
{
    public Renderer quadRenderer;
    public WebCamTextureManager webcamManager;
    public string textureName = "_MainTex";
    public float quadDistance = 2.0f;
    public float captureInterval = 0.5f; // seconds between captures

    private Texture2D picture;
    private bool isCapturing = false;
    private Coroutine captureCoroutine;
    private int imageCounter = 0;

    void Update()
    {
        if (webcamManager.WebCamTexture != null)
        {
            // Start capturing when button is pressed
            if (OVRInput.GetDown(OVRInput.Button.One) && !isCapturing)
            {
                isCapturing = true;
                captureCoroutine = StartCoroutine(CaptureImagesContinuously());
                quadRenderer.gameObject.SetActive(true); // Show quad on first press
            }

            // Stop capturing when button is released
            if (OVRInput.GetUp(OVRInput.Button.One) && isCapturing)
            {
                isCapturing = false;
                if (captureCoroutine != null)
                    StopCoroutine(captureCoroutine);
            }
        }
    }

    IEnumerator CaptureImagesContinuously()
    {
        while (isCapturing)
        {
            TakePicture();
            yield return new WaitForSeconds(captureInterval);
        }
    }

    public void TakePicture()
    {
        int width = webcamManager.WebCamTexture.width;
        int height = webcamManager.WebCamTexture.height;

        if (picture == null || picture.width != width || picture.height != height)
        {
            picture = new Texture2D(width, height, TextureFormat.RGB24, false);
        }

        Color32[] pixels = webcamManager.WebCamTexture.GetPixels32();
        picture.SetPixels32(pixels);
        picture.Apply();

        quadRenderer.material.SetTexture(textureName, picture);

        string captureFolder = Path.Combine(Application.persistentDataPath, "CapturedImages");
        Directory.CreateDirectory(captureFolder);
        string fileName = $"CapturedImage_{imageCounter:D4}.png";
        string filePath = Path.Combine(captureFolder, fileName);
        File.WriteAllBytes(filePath, picture.EncodeToPNG());
        imageCounter++;

        Debug.Log("Image saved: " + filePath);
    }


    public void PlaceQuad()
    {
        Transform quadTransform = quadRenderer.transform;

        Pose cameraPose = PassthroughCameraUtils.GetCameraPoseInWorld(PassthroughCameraEye.Left);
        Vector2Int resolution = PassthroughCameraUtils.GetCameraIntrinsics(PassthroughCameraEye.Left).Resolution;

        quadTransform.position = cameraPose.position + cameraPose.forward * quadDistance;
        quadTransform.rotation = cameraPose.rotation;

        Ray leftSide = PassthroughCameraUtils.ScreenPointToRayInCamera(PassthroughCameraEye.Left, new Vector2Int(0, resolution.y / 2));
        Ray rightSide = PassthroughCameraUtils.ScreenPointToRayInCamera(PassthroughCameraEye.Left, new Vector2Int(resolution.x, resolution.y / 2));

        float horizontalFov = Vector3.Angle(leftSide.direction, rightSide.direction);
        float quadWidth = 2 * quadDistance * Mathf.Tan(horizontalFov * Mathf.Deg2Rad / 2);

        quadTransform.localScale = new Vector3(quadWidth, quadWidth * ((float)resolution.y / resolution.x), 1);
    }
}
