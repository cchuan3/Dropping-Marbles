using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraIntroControl : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;
    private float timer = 0;

    void Start() {
        offset = transform.position - target.position;
        offset.z = -offset.z;
    }

	private void Update() {
        timer += Time.deltaTime;

        if (timer < 4) {
            if (timer > 2) {
                RotateCamera();
            }
        }
        else {
            Destroy(gameObject);
        }
    }

    private void RotateCamera() {
        transform.position = target.position;
        transform.Rotate(new Vector3(0, 1, 0), 180 / 2 * Time.deltaTime, Space.World);
        transform.Rotate(new Vector3(1, 0, 0), -25);
        transform.Translate(offset);
        transform.Rotate(new Vector3(1, 0, 0), 25);
    }
}