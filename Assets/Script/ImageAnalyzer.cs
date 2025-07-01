using UnityEngine;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using System.Collections.Generic;
using PassthroughCameraSamples;
using TMPro;

public class ImageAnalyzer : MonoBehaviour
{
    public OpenAIConfiguration Configuration;
    
    public TextMeshProUGUI resultText;
 

    public WebCamTextureManager webcamManager; // Make sure this is assigned in the Inspector

    private Texture2D picture;

    async void Start()
    {
        Submitimage();
      

        // Capture and analyze an image at the start (optional)
        if (webcamManager != null && webcamManager.WebCamTexture != null)
        {
            TakePicture();
          
        }
    }

    void Update()
    {
        if (webcamManager.WebCamTexture != null)
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                TakePicture(); 
                Submitimage();
         
            }
        }
    }

    public async void Submitimage()
    {
        var api = new OpenAIClient(Configuration);  
        var messages =  new List<Message>();
        Message systemMessage = new Message(Role.System,"you are a helpful assistant");
        List<Content> imageContents = new List<Content>();
        string textContent = "what is in this image";
        Texture2D imageContent = picture;
        imageContents.Add(textContent);
        imageContents.Add(imageContent);
        Message imageMessage = new Message(Role.User, imageContents);
        messages.Add(systemMessage);
        messages.Add(imageMessage);
        var chatRequest = new ChatRequest(messages, model: Model.GPT4o);
        //var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
        //Debug.Log(result.FirstChoice);
        //resultText.text = result.FirstChoice;
    }
    public void TakePicture()
    {


        int width = webcamManager.WebCamTexture.width;
        int height = webcamManager.WebCamTexture.height;

        if (picture == null)
        {
            picture = new Texture2D(width, height, TextureFormat.RGB24, false);
        }

        Color32[] pixels = webcamManager.WebCamTexture.GetPixels32();
        picture.SetPixels32(pixels);
        picture.Apply();

      

    }

}
