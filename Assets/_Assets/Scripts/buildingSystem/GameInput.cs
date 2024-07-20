using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private PlayerInputActions inputActions;

    public static event Action<Vector2> movementPerformed;
    public static event Action<float> pinchPerformed;
    public static event Action pinchCanceled;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if(!EnhancedTouchSupport.enabled)
        {
            EnhancedTouchSupport.Enable();
        }

        inputActions = new PlayerInputActions();
        inputActions.Building.Enable();
        inputActions.Building.Movement.performed += OnMovementPerformed;
        inputActions.Building.Pinch.performed += OnPinchPerformed;
        inputActions.Building.Pinch.canceled += PinchCancelled;
    }

    private void PinchCancelled(InputAction.CallbackContext context)
    {
        pinchCanceled?.Invoke();   
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            inputActions.Building.Movement.performed -= OnMovementPerformed;
            inputActions.Building.Pinch.performed -= OnPinchPerformed;
        }
    }

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = context.ReadValue<Vector2>();
        movementPerformed?.Invoke(screenPosition);
    }


    private void OnPinchPerformed(InputAction.CallbackContext context)
    {
        if(Touch.activeTouches.Count < 2) { return; }

        Touch primary = Touch.activeTouches[0];
        Touch secondary= Touch.activeTouches[1];

        if (primary.phase == TouchPhase.Moved || secondary.phase == TouchPhase.Moved)
        {
            if (primary.history.Count < 1 || secondary.history.Count<1)
            { return; }

            float currentDistance = Vector2.Distance(primary.screenPosition, secondary.screenPosition);
            float previousDistance = Vector2.Distance(primary.history[0].screenPosition, secondary.history[0].screenPosition);

            float pinchDistance = currentDistance - previousDistance;
            pinchPerformed?.Invoke(pinchDistance);
        }
    }
}
