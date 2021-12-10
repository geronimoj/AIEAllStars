using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomKazoo : MonoBehaviour
{
    AudioSource audioSource;
    public float TimeTillKazoo;
    bool HasPlayedKazoo;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        TimeTillKazoo = Random.Range(20, 400);
        HasPlayedKazoo = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(TimeTillKazoo > 0)
        {
            TimeTillKazoo -= Time.deltaTime;
        }
        else if(!HasPlayedKazoo)
        {
            PlayKazoo();
        }
    }

    [ContextMenu("Play Kazoo")]
    public void PlayKazoo()
    {
        audioSource.Play();
        HasPlayedKazoo = true;
    }
}
