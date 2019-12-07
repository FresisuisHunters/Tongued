using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

    public void OpenLink()
    {
#if WEBGL
        openWindow(Field.text);
#else
        Application.OpenURL(url);
#endif
    }


    [DllImport("__Internal")]
    private static extern void openWindow(string url);
}
