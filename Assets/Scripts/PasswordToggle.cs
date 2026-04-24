using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordToggle : MonoBehaviour
{
    public TMP_InputField passwordField;
    public Image eyeIcon;

    public Sprite openEye;
    public Sprite closedEye;

    private bool isHidden = true;

    void Start()
    {
        passwordField.contentType = TMP_InputField.ContentType.Password;
        eyeIcon.sprite = closedEye;
        passwordField.ForceLabelUpdate();
        isHidden = true;
    }

    public void TogglePassword()
    {
        isHidden = !isHidden;

        if (isHidden)
        {
            passwordField.contentType = TMP_InputField.ContentType.Password;
            eyeIcon.sprite = closedEye;
        }
        else
        {
            passwordField.contentType = TMP_InputField.ContentType.Standard;
            eyeIcon.sprite = openEye;
        }

        passwordField.ForceLabelUpdate();
    }
}