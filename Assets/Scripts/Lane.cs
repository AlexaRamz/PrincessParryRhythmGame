using Melanchall.DryWetMidi.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public GameObject notePrefab;
    public List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();
    public float incomingAngle;

    int inputIndex = 0;
    float degreesMarginOfError = 22.5f;
    float distanceMarginOfError = 0.2f;
    int spawnIndex = 0;

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteName == noteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.midiFile.GetTempoMap());
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }
    bool slashing = false;
    float previousAngle;
    private void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - Player.Instance.noteTime)
            {
                SpawnNote(incomingAngle);
                spawnIndex++;
            }
        }

        if (inputIndex >= notes.Count) return;
        Note note = notes[inputIndex];
        if (note == null)
        {
            print($"Missed {inputIndex} note");
            inputIndex++;
            return;
        }
        float noteDistance = Vector2.Distance(note.transform.position, Vector2.zero);
        bool noteInRange = noteDistance < (Player.Instance.bufferEnd + distanceMarginOfError);
        bool noteInBuffer = noteInRange && noteDistance > (Player.Instance.bufferStart - distanceMarginOfError);

        if (Input.GetMouseButtonDown(0))
        {
            float noteDirection = incomingAngle;
            bool pointingToNote = Mathf.Abs(noteDirection - Player.Instance.currentAngle) < degreesMarginOfError || Mathf.Abs(noteDirection - 360 - Player.Instance.currentAngle) < degreesMarginOfError;

            if (pointingToNote && noteInBuffer)
            {
                print($"Hit on {inputIndex} note");
                ScoreManager.Hit();
                note.Kill();
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
            if ((incomingAngle > previousAngle && incomingAngle < Player.Instance.currentAngle) || (incomingAngle < previousAngle && incomingAngle > Player.Instance.currentAngle))
            {
                print($"Hit on {inputIndex} note");
                ScoreManager.Hit();
                note.Kill();
                inputIndex++;
            }
        }
        if (noteDistance < Player.Instance.bufferStart)
        {
            print($"Missed {inputIndex} note");
            inputIndex++;
        }
        previousAngle = Player.Instance.currentAngle;
    }

    public void SpawnNote(float assignedAngle)
    {
        Vector3 notePosition = new Vector3(Player.Instance.spawnDistance * Mathf.Cos((assignedAngle + 90) * Mathf.Deg2Rad), Player.Instance.spawnDistance * Mathf.Sin((assignedAngle + 90) * Mathf.Deg2Rad), 0);
        var note = Instantiate(notePrefab, transform);
        notes.Add(note.GetComponent<Note>());
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
}
