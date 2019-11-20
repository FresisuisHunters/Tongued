using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Componente que maneja la lógica de input del gancho. 
/// Recibe eventos de InputEventReceiver y da órdenes a HookThrower.
/// </summary>
[RequireComponent(typeof(HookThrower))]
public class HookInput : MonoBehaviour
{
    private HookThrower hookThrower;
    private bool isHoldingPointerDown;


    private void OnPointerDown(PointerEventData eventData)
    {
        if (!this || !enabled) return;

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
            hookThrower.LetGo();
        }
    }

    private void OnPointerUp()
    {
        if (!this || !enabled) return;

        isHoldingPointerDown = false;
    }

    private void Update()
    {
        if (hookThrower.HookIsOut && isHoldingPointerDown)
        {
            hookThrower.Retract(Time.deltaTime);
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

        //Busca el InputEventReceiver en la escena y se suscribe a los eventos que nos importan
        InputEventReceiver inputEventReceiver = FindObjectOfType<InputEventReceiver>();
        if (!inputEventReceiver)
        {
            Debug.LogWarning("HookInput necesita que exista un InputEventReceiver en la escena para funcionar.");
            return;
        }

        inputEventReceiver.AddListener(EventTriggerType.PointerDown, (data) => OnPointerDown(data as PointerEventData));
        inputEventReceiver.AddListener(EventTriggerType.PointerUp, (data) => OnPointerUp());
    }
}
