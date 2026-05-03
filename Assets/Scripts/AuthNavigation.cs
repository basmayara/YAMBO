using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthNavigation : MonoBehaviour
{
    public void GoToSignup()
    {
        SceneManager.LoadScene("inscription");
    }

    public void GoToLogin()
    {
        SceneManager.LoadScene("SampleScene");
    }

}