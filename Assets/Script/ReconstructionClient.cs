using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ReconstructionClient : MonoBehaviour
{
    [Header("API URLs")]
    public string startJobUrl = "https://in-kid-firmly.ngrok-free.app/start_job/";
    public string statusUrl = "https://in-kid-firmly.ngrok-free.app/job_status/";
    public string downloadUrl = "https://in-kid-firmly.ngrok-free.app/download/";


    [Header("UI References")]
    public TMP_Text statusText;
    public Toggle isVideoToggle;
    public Toggle removeBgToggle;
    public TMP_Dropdown scaleDropdown;   // "small" or "large"
    public TMP_Dropdown gpuDropdown;     // "true" or "false"

    [Header("Capture Settings")]
    public string capturedFolderName = "CapturedImages";
    public string meshSaveName = "mesh.obj";

    public static string savedMeshPath;

    public void StartReconstruction()
    {
        StartCoroutine(UploadAndStartJob());
    }
    IEnumerator UploadAndStartJob()
    {
        string inputPath = Path.Combine(Application.persistentDataPath, "CapturedImages");
        if (!Directory.Exists(inputPath))
        {
            statusText.text = "❌ Capture folder not found.";
            yield break;
        }

        string[] files = Directory.GetFiles(inputPath);
        if (files.Length == 0)
        {
            statusText.text = "❌ No captured images found.";
            yield break;
        }

        statusText.text = "📤 Uploading images...";

        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("is_video", isVideoToggle.isOn.ToString().ToLower()));
        form.Add(new MultipartFormDataSection("remove_bg", removeBgToggle.isOn.ToString().ToLower()));
        form.Add(new MultipartFormDataSection("object_scale", scaleDropdown.options[scaleDropdown.value].text));
        form.Add(new MultipartFormDataSection("use_gpu", gpuDropdown.options[gpuDropdown.value].text.ToLower()));

        foreach (string path in files)
        {
            byte[] fileData = File.ReadAllBytes(path);
            string mime = path.EndsWith(".mp4") ? "video/mp4" : "image/png";
            form.Add(new MultipartFormFileSection("files", fileData, Path.GetFileName(path), mime));
        }

        UnityWebRequest request = UnityWebRequest.Post(startJobUrl, form);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            statusText.text = "❌ Upload failed: " + request.error;
            yield break;
        }

        string json = request.downloadHandler.text;
        JobResponse response = JsonUtility.FromJson<JobResponse>(json);

        statusText.text = $"⏳ Job started (ID: {response.job_id})";
        StartCoroutine(CheckJobStatus(response.job_id));
    }

    IEnumerator CheckJobStatus(string jobId)
    {
        string pollUrl = statusUrl + jobId;

        while (true)
        {
            UnityWebRequest statusReq = UnityWebRequest.Get(pollUrl);
            yield return statusReq.SendWebRequest();

            if (statusReq.result != UnityWebRequest.Result.Success)
            {
                statusText.text = "❌ Status error: " + statusReq.error;
                yield break;
            }

            string json = statusReq.downloadHandler.text;
            JobResponse response = JsonUtility.FromJson<JobResponse>(json);
            string jobStatus = response.status;

            statusText.text = $"🔄 Status: {jobStatus}";

            if (jobStatus == "complete")
            {
                StartCoroutine(DownloadMesh(jobId));
                yield break;
            }
            else if (jobStatus.StartsWith("error"))
            {
                statusText.text = "❌ Reconstruction failed: " + jobStatus;
                yield break;
            }

            yield return new WaitForSeconds(5); // poll every 5 seconds
        }
    }

    IEnumerator DownloadMesh(string jobId)
    {
        string meshUrl = downloadUrl + jobId + "/mesh.obj";
        UnityWebRequest meshReq = UnityWebRequest.Get(meshUrl);
        yield return meshReq.SendWebRequest();

        if (meshReq.result != UnityWebRequest.Result.Success)
        {
            statusText.text = "❌ Mesh download failed.";
            yield break;
        }

        string meshFolder = Path.Combine(Application.persistentDataPath, "DownloadedMeshes");
        Directory.CreateDirectory(meshFolder);
        string savePath = Path.Combine(meshFolder, meshSaveName);
        File.WriteAllBytes(savePath, meshReq.downloadHandler.data);

        statusText.text = "✅ Mesh downloaded. Loading...";

        savedMeshPath = savePath;
        yield return new WaitForSeconds(1);
       
    }



    public void ViewMesh(string MeshLoaderScene)
    {
        SceneManager.LoadScene(MeshLoaderScene);
    }
    [System.Serializable]
    public class JobResponse
    {
        public string job_id;
        public string status;
    }
}
