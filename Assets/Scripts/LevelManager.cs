using System.Collections;
using System.Collections.Generic;
using static System.Enum;
using static System.Array;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Determines level difficulty
    public int level;
    public float timer;

    // Obstacle prefabs
    public GameObject slowPrefab;
    public GameObject bouncePrefab;
	
    private void Start() {
        SetupLevel();
    }

    // Level management
    public void SetupLevel() {
        SetupObstacles((level - 1) * 210 + 10);
    }

    // Obstacle generation management
    private void SetupObstacles(float startDistance) {
        float trackLen = startDistance + 200;
        float distance = startDistance + 10;
        while(distance < trackLen - 10) {
            distance += ChooseGeneratedObstacle(trackLen, distance);
        }
    }

    // Obstacle types
    enum ObstacleType
    {
        Single,
        Wall,
        Zigzag
    }

    // Random obstacle generation
    private float ChooseGeneratedObstacle(float trackLen, float distance) {
        var vals = GetValues(typeof(ObstacleType));
        ObstacleType genObstacle = (ObstacleType)vals.GetValue((int)Random.Range(0, vals.Length));

        int deltaDistance = 10;
        switch (genObstacle) {
            case ObstacleType.Single:
                GenerateSingleObstacle(distance);
                break;
            case ObstacleType.Wall:
                GenerateWallObstacle(distance);
                break;
            case ObstacleType.Zigzag:
                if (distance < trackLen - 30) {
                    GenerateZigzagObstacle(distance);
                    deltaDistance = 30;
                }
                else {
                    GenerateWallObstacle(distance);
                }
                break;
            default: 
                return 0;
        }
        return deltaDistance + Random.Range(0,5);
    }

    private void GenerateSlowObstacle(Vector3 pos, float slowAmount, float scale) {
        GameObject obstacle = Instantiate(slowPrefab, pos, Quaternion.identity, gameObject.transform);
        obstacle.GetComponent<SlowObstacle>().slowAmount = slowAmount;
        obstacle.GetComponent<SlowObstacle>().scale = scale;
    }

    // Single obstacle
    private void GenerateSingleObstacle(float zPos) {
        float scale = Random.Range(2f, Mathf.Min(level + 2, 6f));
        float xOffset = scale / 2f * 1.5f;
        float xPos = Random.Range(-5 + xOffset, 5 - xOffset);
        GenerateSlowObstacle(new Vector3(xPos, scale, zPos), 5f + timer * 0.2f, scale);
    }

    // "Wall" obstacles
    private void GenerateWallObstacle(float zPos) {
        int numMissing = Random.Range(1, 3);
        int[] missing = new int[numMissing];
        for (int i = 0; i < numMissing; i++) {
            missing[i] = Random.Range(0,5);
        }
        float xPos = -4;
        for (int i = 0; i < 5; i++) {
            if (!missing.Contains(i)) {
                GenerateSlowObstacle(new Vector3(xPos, 1, zPos), 5f + timer * 0.05f, 1);
            }
            xPos += 2;
        }
    }

    // Zig-zag obstacles
    private void GenerateZigzagObstacle(float zPos) {
        bool rightSide = Random.value > 0.5f;
        float xPos = -4;
        if (!rightSide) {
            xPos = -xPos;
        }
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 3; j++) {
                GenerateSlowObstacle(new Vector3(xPos, 1, zPos + j * 10), 5f + timer * 0.03f, 1);
                xPos = -xPos;
            }
            xPos = -xPos;
            xPos = Mathf.MoveTowards(xPos, 0, 2);
        }
        for (int i = 0; i < 3; i++) {
            GenerateSlowObstacle(new Vector3(0, 1, zPos + i * 10), 5f + timer * 0.03f, 1);
            xPos = -xPos;
        }
    }
}