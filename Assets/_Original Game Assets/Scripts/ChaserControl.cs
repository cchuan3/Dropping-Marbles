using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserControl : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)]
    private float velocity = 5;

    Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        if (!GameManager.paused) {
            CalculateSpeed();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Enemy") {
            // Destroy obstacle
            Destroy(other.gameObject);
        }
    }

    private void CalculateSpeed() {
        velocity = (5 + GameManager.level) * (1 + GameManager.timer / 240);
        rb.velocity = new Vector3(0, 0, velocity);
    }

    // Move chaser to just before a level
    public void JumpToLevel(int level) {
        Vector3 newPos = new Vector3(0, 5, (level - 1) * 210 - 20);
        transform.position = newPos;
        CalculateSpeed();
    }

    // Return how far the chaser is
    public float GetProgress() {
        return transform.position.z;
    }
}