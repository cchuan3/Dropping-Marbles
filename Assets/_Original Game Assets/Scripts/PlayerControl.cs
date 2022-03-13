using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameManager;

public class PlayerControl : MonoBehaviour
{
    // Forward speed
    [SerializeField, Range(0f, 100f)]
    float FSpeed = 10f;
    [SerializeField, Range(0f, 100f)]
    float FAcceleration = 15f;
    
    // L/R speed
    [SerializeField, Range(0f, 100f)]
    float LRSpeed = 10f;
    [SerializeField, Range(0f, 100f)]
    float LRAcceleration = 10f;

    // Allowed L/R area
    float allowedLRArea = 4.3f;

    // Physical body
    Rigidbody rb;
    // Game Manager
    public GameManager gm;

    // Player stats
    int strength = 1;
    int speed = 1;
    int control = 1;

    private void Start() {
        strength = 1;
        speed = 1;
        control = 1;
    }

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

	void Update() {
        Movement();

        // Temp
        if (Input.GetKeyDown(KeyCode.G)) {
            LoseSpeed(8f);
        }
        else if (Input.GetKeyDown(KeyCode.F)) {
            LoseSpeed(15f);
        }
        else if (Input.GetKeyDown(KeyCode.V)) {
            LoseSpeed(30f);
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            GameObject[] del = GameObject.FindGameObjectsWithTag("Level1");
            foreach(GameObject obj in del) {
                Destroy(obj);
            }
        }
    }

    // L/R movement with input and automatic forward movement
    void Movement() {
        Vector3 desiredVelocity = new Vector3(Input.GetAxis("Horizontal") * LRSpeed, 0, FSpeed);
        Vector3 velocity = rb.velocity;
        float LRSpeedChange = LRAcceleration * Time.deltaTime;
        float FSpeedChange = FAcceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, LRSpeedChange);
        if ((velocity.x > 0 && transform.localPosition.x >= allowedLRArea) || (velocity.x < 0 && transform.localPosition.x <= -allowedLRArea)) {
            velocity.x = 0;
        }
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, FSpeedChange);
        rb.velocity = velocity;
    }

    public void CalculateStats() {
        FSpeed = speed * 3 + 7;
        FAcceleration = speed * .2f + 1.3f;
        LRSpeed = control * 2 + 8;
        LRAcceleration = control + 9;
    }

    public int[] GetStats() {
        int[] stats = {strength, speed, control};
        return stats;
    }

    public void ChangeStrength(int increment) {
        strength += increment;
    }
    
    public void ChangeSpeed(int increment) {
        speed += increment;
    }
    
    public void ChangeControl(int increment) {
        control += increment;
    }

    // Lose speed on crash
    void LoseSpeed(float loss) {
        loss = loss - loss * (strength - 1) / 20;
        if (loss < 1) {
            loss = 1;
        }
        float slower = rb.velocity.z - loss;
        Vector3 currVelocity = rb.velocity;
        currVelocity.z = slower;
        rb.velocity = currVelocity;
    }

    // Slow down (limit to 0)
    public void Slow(float loss) {
        loss = loss - loss * (strength - 1) / 20;
        if (loss < 1) {
            loss = 1;
        }
        Vector3 currVelocity = rb.velocity;
        currVelocity.x = Mathf.MoveTowards(currVelocity.x, 0, loss);
        currVelocity.z = Mathf.MoveTowards(currVelocity.z, 0, loss);
        rb.velocity = currVelocity;
    }

    // Bounce back (no limit)
    public void Bounce(float loss) {
        float slower = rb.velocity.z - loss;
        Vector3 currVelocity = rb.velocity;
        currVelocity.z = slower;
        rb.velocity = currVelocity;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "EndLevel") {
            gm.EndLevel();
        }
        else if (other.tag == "LoadLevel") {
            gm.NextLevel();
        }
        else if (other.tag == "Chaser") {
            gm.EndGame();
        }
    }

    // Return how far the player is
    public float GetProgress() {
        return transform.position.z;
    }
}