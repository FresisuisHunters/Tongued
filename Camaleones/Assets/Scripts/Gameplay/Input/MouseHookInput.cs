using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Componente que maneja la lógica de input del gancho. 
/// Recibe eventos de InputEventReceiver y da órdenes a HookThrower.
/// </summary>
[RequireComponent(typeof(HookThrower))]
public class MouseHookInput : MonoBehaviour
{
    private HookThrower hookThrower;
    private bool isHoldingPointerDown;

    private void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isHoldingPointerDown = true;

            if (!hookThrower.HookIsOut)
            {
                hookThrower.ThrowHook(eventData.pressEventCamera.ScreenToWorldPoint(eventData.position));
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            hookThrower.DisableHook();
        }
    }

    private void OnPointerUp()
    {
        isHoldingPointerDown = false;
    }

    private void Update()
    {
        if (hookThrower.HookIsOut && isHoldingPointerDown)
        {
            hookThrower.Retract(Time.deltaTime, 1);
        }
    }


    private void Awake()
    {
        hookThrower = GetComponent<HookThrower>();
    }

    private void Start()
    {
        Photon.Pun.PhotonView photonView = GetComponent<Photon.Pun.PhotonView>();
        if (photonView && !photonView.IsMine)
        {
            Destroy(this);
            return;
        }

        if ((ControlScheme) PlayerPrefs.GetInt(SettingsMenuScreen.CONTROL_SCHEME_PREF_KEY, 0) != ControlScheme.Mouse)
        {
            Destroy(this);
            return;
        }

        //Busca el InputEventReceiver en la escena y se suscribe a los eventos que nos importan
        InputEventReceiver inputEventReceiver = FindObjectOfType<InputEventReceiver>();
        if (!inputEventReceiver)
        {
            Debug.LogWarning("HookInput necesita que exista un InputEventReceiver en la escena para funcionar.");
            return;
        }

        inputEventReceiver.AddListener(EventTriggerType.PointerDown, (data) => { if (enabled) OnPointerDown(data as PointerEventData); });
        inputEventReceiver.AddListener(EventTriggerType.PointerUp, (data) => { if (enabled) OnPointerUp(); });
    }
}
