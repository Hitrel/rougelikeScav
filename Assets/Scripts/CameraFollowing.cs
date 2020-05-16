using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public Transform target;
    public float smoothing = 5f;
    Camera camera = null;

    Vector3 offset;

    float initCameraSize = 0f;
    float delta = 50f;

    private void Start() {
        offset = transform.position - target.position;
        camera = GetComponent<Camera>();
        initCameraSize = camera.orthographicSize;
        delta += camera.orthographicSize;
    }

    private void FixedUpdate() {
        Vector3 targetCamPos = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        if (Input.GetKey(KeyCode.Tab)) {
            ZoomOut();
        } else {
            ZoomIn();
        }
    }

    private void ZoomOut() {
        camera.orthographicSize = delta;
    }

    private void ZoomIn() {
        camera.orthographicSize = initCameraSize;
    }
}
