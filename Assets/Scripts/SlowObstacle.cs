using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlayerControl;

public class SlowObstacle : MonoBehaviour
{
    // How much the player is slowed by
    public float slowAmount;
    
    // Size of object
    public float scale;

    // Has the obstacle been hit already
    private bool hit = false;

    // Size to destroy object when shrinking
    private float destroySize;

    void Start() {
        destroySize = transform.localScale.x / 3;
        transform.localScale *= scale;
    }

    void Update() {
        if (hit) {
            Shrink();
        }
    }

	void OnTriggerEnter(Collider other) {
        if (!hit && other.CompareTag("Player")) {
            other.GetComponent<PlayerControl>().Slow(slowAmount);
            hit = true;
        }
    }

    void Shrink () {
        Vector3 currScale = transform.localScale;
        if (currScale.x < 0.8) {
            Destroy(gameObject);
        }
        currScale.x = Mathf.MoveTowards(currScale.x, 0, scale * Time.deltaTime);
        currScale.y = Mathf.MoveTowards(currScale.y, 0, scale * Time.deltaTime);
        currScale.z = Mathf.MoveTowards(currScale.z, 0, scale * Time.deltaTime);

        transform.localScale = currScale;
    }
}