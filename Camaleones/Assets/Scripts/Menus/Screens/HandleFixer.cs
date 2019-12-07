using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Este script se usa para fijar el tamaño del handle de la scrollbar de los créditos a la fuerza y también para ajustar el tamaño de la lengua
/// </summary>
public class HandleFixer : MonoBehaviour
{
    private Scrollbar scrollbar;
    Image image;

    private void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
        scrollbar.size = 0.25f;
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        scrollbar.size = 0.25f;
        image.fillAmount = Mathf.Lerp(0.18f, 1, 1-scrollbar.value);
    }
}
