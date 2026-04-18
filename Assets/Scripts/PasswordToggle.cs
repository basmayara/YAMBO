using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordToggle : MonoBehaviour
{

    public Sprite openEye;
    public Sprite closedEye;

    public void TogglePassword(TMP_InputField targetField)
    {
        if (targetField.contentType == TMP_InputField.ContentType.Password)
        {
            targetField.contentType = TMP_InputField.ContentType.Standard;
            UpdateIcon(targetField, openEye);
        }
        else
        {
            targetField.contentType = TMP_InputField.ContentType.Password;
            UpdateIcon(targetField, closedEye);
        }

        targetField.ForceLabelUpdate();
    }

    private void UpdateIcon(TMP_InputField field, Sprite newSprite)
    {
        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite = newSprite;
    }
}