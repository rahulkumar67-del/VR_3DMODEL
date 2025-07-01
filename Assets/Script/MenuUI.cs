using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown Meshtype;






    private void Start()
    {
        
       
        
    }
    



    private void getDropeDownValue()
    {
        int index = Meshtype.value;
        string selectedoption = Meshtype.options[index].text;
        Debug.Log(selectedoption);
    }
    public void LoadScene(string Loadscene)
    {
        SceneManager.LoadScene(Loadscene);
        
    }

}

