using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    [SerializeField] private GameObject sword;
    public float rotationSpeed = 10f;
    public List<Projectile> notes = new List<Projectile>();
    int inputIndex = 0;
    float degreesMarginOfError = 22.5f;
    [SerializeField] GameObject notePrefab;

    float spawnDistance = 10f;
    public static Vector2 noteDespawn;
    public float bufferStart = 1;
    public float bufferEnd;

    private void Start()
    {
        Instance = this;
        noteDespawn = transform.position;
        StartCoroutine(TestSpawn());
    }
    void SpawnNote(float assignedAngle)
    {
        Vector3 notePosition = new Vector3(transform.position.x + spawnDistance * Mathf.Cos((assignedAngle + 90) * Mathf.Deg2Rad), transform.position.y + spawnDistance * Mathf.Sin((assignedAngle + 90) * Mathf.Deg2Rad), 0);
        var note = Instantiate(notePrefab, transform);
        notes.Add(note.GetComponent<Projectile>());
        note.GetComponent<Projectile>().assignedAngle = assignedAngle;
        note.transform.position = notePosition;
    }
    IEnumerator TestSpawn()
    {
        int numDirections = 16;
        for (int i = 0; i < numDirections; i++)
        {
            SpawnNote(360 / numDirections * i);
            yield return new WaitForSeconds(1f);
        }
    }
    bool slashing = false;
    float previousAngle;
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        float currentAngle = Mathf.LerpAngle(sword.transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
        sword.transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);

        if (inputIndex >= notes.Count) return;
        Projectile note = notes[inputIndex];
        if (note == null)
        {
            print($"Missed {inputIndex} note");
            inputIndex++;
            return;
        }
        float noteDistance = Vector2.Distance(note.transform.position, transform.position);
        bool noteInRange = noteDistance < bufferEnd;
        bool noteInBuffer = noteInRange && noteDistance > bufferStart;

        if (Input.GetMouseButtonDown(0))
        {
            float noteDirection = note.assignedAngle;
            bool pointingToNote = Mathf.Abs(noteDirection - currentAngle) < degreesMarginOfError || Mathf.Abs(noteDirection - 360 - currentAngle) < degreesMarginOfError;

            if (pointingToNote && noteInBuffer)
            {
                print($"Hit on {inputIndex} note");
                Destroy(note.gameObject);
                inputIndex++;
            }
            slashing = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            slashing = false;
        }
        if (slashing && noteInRange)
        {
            if ((note.assignedAngle > previousAngle && note.assignedAngle < currentAngle) || (note.assignedAngle < previousAngle && note.assignedAngle > currentAngle))
            {
                print($"Hit on {inputIndex} note");
                Destroy(note.gameObject);
                inputIndex++;
            }
        }
        if (noteDistance < bufferStart)
        {
            print($"Missed {inputIndex} note");
            inputIndex++;
        }
        previousAngle = currentAngle;
    }
}
