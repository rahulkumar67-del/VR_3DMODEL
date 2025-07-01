using System.IO;
using Dummiesman;
using UnityEngine;
using UnityEngine.Networking;

public class MeshLoader : MonoBehaviour
{
    private string fileName = ReconstructionClient.savedMeshPath;  // or use `ReconstructionClient.savedMeshPath`
    private Material fallbackMaterial;     // Optional default material

    void Start()
    {
        string meshPath = Path.Combine(Application.persistentDataPath, "DownloadedMeshes", fileName);

        if (File.Exists(meshPath))
        {
            GameObject loadedObject = new OBJLoader().Load(meshPath);
            loadedObject.transform.position = Vector3.zero;

            if (fallbackMaterial != null)
            {
                foreach (var renderer in loadedObject.GetComponentsInChildren<MeshRenderer>())
                {
                    renderer.material = fallbackMaterial;
                }
            }
        }
        else
        {
            Debug.LogError("❌ Mesh not found at: " + meshPath);
        }
    }
}

