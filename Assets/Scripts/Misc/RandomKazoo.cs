using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomKazoo : MonoBehaviour
{
    AudioSource audioSource;
    public float TimeTillKazoo;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        TimeTillKazoo = Random.Range(20, 400);
    }

    // Update is called once per frame
    void Update()
    {
        if(TimeTillKazoo > 0)
        {
            TimeTillKazoo -= Time.deltaTime;
        }
        else
        {
            PlayKazoo();
        }
    }

    [ContextMenu("Play Kazoo")]
    public void PlayKazoo()
    {
        audioSource.Play();
    }
}
