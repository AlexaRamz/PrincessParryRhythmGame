using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public AudioSource hitSFX;
    public AudioSource missSFX;
    public TMP_Text scoreText;
    static int notesHit;
    void Start()
    {
        Instance = this;
        notesHit = 0;
    }
    public static void Hit()
    {
        notesHit += 1;
        //Instance.hitSFX.Play();
    }
    public static void Miss()
    {
        //Instance.missSFX.Play();
    }
    private void Update()
    {
        scoreText.text = notesHit.ToString() + "/" + SongManager.totalNotes.ToString();
    }
}