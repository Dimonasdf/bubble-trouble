using UnityEngine;
using UnityEngine.InputSystem;

public class Scraper : MonoBehaviour
{
    [SerializeField] private Collider blade;

    [SerializeField] private float inactiveHeight;
    [SerializeField] private float inactiveAngleX;

    [SerializeField] private float readyAngleX;

    [SerializeField] private float activeHeight;
    [SerializeField] private float activeAngleX;

    [SerializeField] private float linearLerpPower = 1f;
    [SerializeField] private float angularLerpPowerY = 1f;
    [SerializeField] private float pressedAngularLerpPowerYDebuff = 1f;

    [SerializeField] private float angularLerpPowerX = 1f;
    [SerializeField] private float angularLerpPower = 1f;

    private Rigidbody _rigidBody;
    private Camera _mainCamera;

    private Vector3 _worldMousePosition;

    private bool _isPressed;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        _isPressed = Mouse.current.leftButton.isPressed;
        blade.enabled = _isPressed;
    }

    private void FixedUpdate()
    {
        var delta = Time.fixedDeltaTime;
        var isValidTarget = Physics.Raycast(transform.position, Vector3.down, 1f, Consts.Screen.MaskFromLayer());
        var shouldActivate = _isPressed && isValidTarget;

        var targetPosition = _worldMousePosition.WithY(shouldActivate ? activeHeight : inactiveHeight);
        var newPosition = Vector3.Lerp(transform.position, targetPosition, delta * linearLerpPower);

        var fromRotation = Quaternion.LookRotation(transform.forward, Vector3.up).eulerAngles.y;
        var travelDirection = targetPosition - transform.position;
        var toRotation = fromRotation;

        if (travelDirection.sqrMagnitude > 0.000001f)
            toRotation = Quaternion.LookRotation(travelDirection, Vector3.up).eulerAngles.y;

        var lerpPowerY = angularLerpPowerY * (_isPressed ? pressedAngularLerpPowerYDebuff : 1f);
        var angleYLerp = Mathf.LerpAngle(fromRotation, toRotation, delta * lerpPowerY);

        var angleX = isValidTarget ? (_isPressed ? activeAngleX : readyAngleX) : inactiveAngleX;
        var angleXLerp = Mathf.LerpAngle(transform.rotation.eulerAngles.x, angleX, delta * angularLerpPowerX);

        var targetRotation = Quaternion.Euler(angleXLerp, angleYLerp, 0f);
        var newRotation = Quaternion.Slerp(transform.rotation, targetRotation, delta * angularLerpPower);

        newRotation = targetRotation;

        transform.SetPositionAndRotation(newPosition, newRotation);
        _rigidBody.position = newPosition;
        _rigidBody.rotation = newRotation;
    }

    private void OnGUI()
    {
        var currentEvent = Event.current;
        var mousePos = new Vector2
        {
            // Get the mouse position from Event.
            // Note that the y position from Event is inverted.
            x = currentEvent.mousePosition.x,
            y = _mainCamera.pixelHeight - currentEvent.mousePosition.y
        };

        var cameraFarPlaneDistance = _mainCamera.farClipPlane;
        _worldMousePosition = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cameraFarPlaneDistance));

        var mainCameraPosition = _mainCamera.transform.position;
        if (Physics.Raycast(mainCameraPosition, _worldMousePosition - mainCameraPosition, out var raycastHitInfo, cameraFarPlaneDistance, Consts.MouseCastMask))
            _worldMousePosition = raycastHitInfo.point;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_mainCamera.transform.position, _worldMousePosition);
            Gizmos.DrawSphere(_worldMousePosition, 0.01f);
        }
    }
}