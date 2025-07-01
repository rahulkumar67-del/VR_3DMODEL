using UnityEngine;

public class Intialization : MonoBehaviour
{
    [SerializeField] private GameObject ViewManager;
    [SerializeField] private GameObject StatusCanvas;
    private void Awake()
    {
        ViewManager.SetActive(false);
        StatusCanvas.SetActive(false);
    }
    public void OnNExt()
    {
        StatusCanvas.SetActive(true);
        ViewManager.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
