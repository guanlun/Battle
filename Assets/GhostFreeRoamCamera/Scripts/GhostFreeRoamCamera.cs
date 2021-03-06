﻿using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GhostFreeRoamCamera : MonoBehaviour {
    public float initialSpeed = 5f;
    public float increaseSpeed = 0.25f;

    public bool allowMovement = true;
    public bool allowRotation = true;

    public KeyCode forwardButton = KeyCode.W;
    public KeyCode backwardButton = KeyCode.S;
    public KeyCode rightButton = KeyCode.D;
    public KeyCode leftButton = KeyCode.A;
    public KeyCode upwardButton = KeyCode.Space;

    public float cursorSensitivity = 0.025f;
    public bool cursorToggleAllowed = true;
    public KeyCode cursorToggleButton = KeyCode.Escape;

    private float currentSpeed = 0f;
    private bool moving = false;
    private bool togglePressed = false;

    private void OnEnable() {
        if (cursorToggleAllowed) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void SpawnSoldier(Vector3 position, string team) {
        StateManager.instantiateSoldier(position, StateManager.currWeaponType, team);
    }

    void Start() {
    }

    private void Update() {
        bool leftButtonUp = Input.GetMouseButtonUp(0);
        bool rightButtonUp = Input.GetMouseButtonUp(1);

        if (leftButtonUp || rightButtonUp) {
            Camera camera = gameObject.GetComponent<Camera>();
            RaycastHit hit;

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {
                Transform hitObj = hit.transform;
                if (hitObj.name == "Terrain") {
                    string team = leftButtonUp ? "red" : "blue";

                    SpawnSoldier(hit.point, team);
                }
            }
        }

        if (allowMovement) {
            bool lastMoving = moving;
            Vector3 deltaPosition = Vector3.zero;

            if (moving)
                currentSpeed += increaseSpeed * Time.deltaTime;

            moving = false;

            CheckMove(forwardButton, ref deltaPosition, transform.forward);
            CheckMove(backwardButton, ref deltaPosition, -transform.forward);
            CheckMove(rightButton, ref deltaPosition, transform.right);
            CheckMove(leftButton, ref deltaPosition, -transform.right);
            CheckMove(upwardButton, ref deltaPosition, new Vector3(0, 1, 0));

            if (moving) {
                if (moving != lastMoving)
                    currentSpeed = initialSpeed;

                transform.position += deltaPosition * currentSpeed * Time.deltaTime;
            } else currentSpeed = 0f;
        }

        if (allowRotation) {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x += -Input.GetAxis("Mouse Y") * 359f * cursorSensitivity;
            eulerAngles.y += Input.GetAxis("Mouse X") * 359f * cursorSensitivity;
            transform.eulerAngles = eulerAngles;
        }

        if (cursorToggleAllowed) {
            if (Input.GetKey(cursorToggleButton)) {
                if (!togglePressed) {
                    togglePressed = true;
                    if (Cursor.lockState == CursorLockMode.Locked) {
                        Cursor.lockState = CursorLockMode.None;
                    } else {
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                    Cursor.visible = !Cursor.visible;
                }
            } else togglePressed = false;
        } else {
            togglePressed = false;
            Cursor.visible = false;
        }
    }

    private void CheckMove(KeyCode keyCode, ref Vector3 deltaPosition, Vector3 directionVector) {
        if (Input.GetKey(keyCode)) {
            moving = true;
            deltaPosition += directionVector;
        }
    }
}
