using Unity.XR.CoreUtils;
using UnityEngine;

public class ScanningButton : MonoBehaviour
{
    private bool TriggerIsOn;
    [SerializeField] private GameObject MeshOptionCanvas;
    private void Awake()
    {
        MeshOptionCanvas.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trigger") && !TriggerIsOn)
        {
            MeshOptionCanvas.SetActive(true);
            TriggerIsOn = true;

        }
        if (other.CompareTag("Trigger") && TriggerIsOn)
        {
            MeshOptionCanvas.SetActive(false);
            TriggerIsOn = false;
        }
    }
}
