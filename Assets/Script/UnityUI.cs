using System.IO;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using TMPro;
using UnityEngine;

public class UnityUI : MonoBehaviour
{
    //public TMP_InputField pythonPathField;
    //public TMP_InputField imageDirField;
    //public TMP_InputField workspaceField;
    //public TMP_InputField colmapPathField;
    //public TMP_InputField scriptPathField;

    [SerializeField] private GameObject SaveImagePanel;

    public string folderPath = "Assets/CapturedImages";

    [HideInInspector] public int imageCount;

    [SerializeField] private TMPro.TextMeshProUGUI ImageNOText;


    void Start()
    {
        ImageNOText.text = 0f.ToString();
        Debug.Log("Number of image files: " + imageCount);
    }
    private void Update()
    {
        int imageCount = CountImages(folderPath);
        ImageNOText.text = "ImageNo:" + imageCount.ToString();
    }
    int CountImages(string path)
    {
        if (!Directory.Exists(path))
        {
            Debug.LogError("Folder not found: " + path);
            return 0;
        }

        string[] extensions = { "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.tiff" };
        int count = 0;


        foreach (string ext in extensions)
        {
            string[] files = Directory.GetFiles(path, ext, SearchOption.TopDirectoryOnly);
            count += files.Length;
        }

        return count;
    }
    public void ImageDelter()
    {

    }
    private void ImageFinal()
    {

    }

    public void SubmitButton()
    {

    }

}
