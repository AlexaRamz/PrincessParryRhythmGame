using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    [SerializeField] private GameObject sword;
    public float rotationSpeed = 10f;
    [SerializeField] GameObject notePrefab;

    public float spawnDistance = 10f;
    public static Vector2 noteDespawn;
    public float bufferStart = 1;
    public float bufferEnd;

    public float noteTime;
    [HideInInspector] public float noteSpeed;
    public float currentAngle;
    [SerializeField] Animator anim;

    private void Start()
    {
        Instance = this;
        noteDespawn = transform.position;
        noteSpeed = (spawnDistance - (bufferStart + (bufferEnd - bufferStart) / 2)) / noteTime;
        //StartCoroutine(TestSpawn());
    }
    private void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        currentAngle = Mathf.LerpAngle(sword.transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
        sword.transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
        Debug.Log(currentAngle);
        anim.SetFloat("Direction", currentAngle / 360f);
    }
}
