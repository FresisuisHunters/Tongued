using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#pragma warning disable 649
[RequireComponent(typeof(Rigidbody2D))]
public class HookThrower : MonoBehaviour
{
    [SerializeField] private Hook hookPrefab;
    [SerializeField] private float retractDistancePerSecond = 10f;
    public Rigidbody2D Rigidbody { get; private set; }

    private Hook hook;
    private bool isHoldingPointerDown;


    public void ThrowHook(Vector2 targetPoint)
    {
        hook.Throw(this, targetPoint);
    }

    public void LetGo()
    {
        hook.Disable();
    }


    private void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isHoldingPointerDown = true;
            if (!hook.isActiveAndEnabled)
            {
                ThrowHook(eventData.pressEventCamera.ScreenToWorldPoint(eventData.position));
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            LetGo();
        }
    }

    private void StopHoldingPointerDown()
    {
        isHoldingPointerDown = false;
        print("Stopped holding");
    }

    private void Update()
    {
        if (hook.isActiveAndEnabled && isHoldingPointerDown)
        {
            hook.Length -= retractDistancePerSecond * Time.deltaTime;
        }
    }

    private void Awake()
    {
        hook = Instantiate(hookPrefab);
        hook.Disable();

        Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        InputEventReceiver inputEventReceiver = FindObjectOfType<InputEventReceiver>();
        if (!inputEventReceiver) return;

        inputEventReceiver.AddListener(EventTriggerType.PointerDown, (data) => OnPointerDown(data as PointerEventData));
        inputEventReceiver.AddListener(EventTriggerType.PointerUp, (data) => StopHoldingPointerDown());
        //inputEventReceiver.AddListener(EventTriggerType.PointerExit, (data) => StopHoldingPointerDown());

    }
}
