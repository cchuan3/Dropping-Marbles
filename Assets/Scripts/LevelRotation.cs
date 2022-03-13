using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRotation : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)]
    float angularSpeed = 30f;


    Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

	private void Update() {
        Rotate();
    }

    private void Rotate() {
        Vector3 desiredRotation = new Vector3(0, 0, -Input.GetAxis("Horizontal"));
        Quaternion deltaRotation = Quaternion.Euler(desiredRotation * angularSpeed * Time.deltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}