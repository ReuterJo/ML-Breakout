using UnityEngine;
using UnityEngine.UI;

public class CoverImageHandler : MonoBehaviour
{
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        if (image != null)
        {
            image.GetComponent<Button>().onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        image.enabled = false;
    }
}