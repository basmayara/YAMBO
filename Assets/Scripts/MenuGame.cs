using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuManager : MonoBehaviour
{
    public void GoToConnexion()
    {
        SceneManager.LoadScene("SampleScene"); 
    }

    public void GoToInscription()
    {
        SceneManager.LoadScene("Inscription"); 
    }
}