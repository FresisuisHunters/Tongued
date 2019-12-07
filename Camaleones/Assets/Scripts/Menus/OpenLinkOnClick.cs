using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OpenLinkOnClick : MonoBehaviour
{
    public string url;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OpenLink);
    }

    private void OpenLink()
    {
        Application.OpenURL(url);
    }
}
