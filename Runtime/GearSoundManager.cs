using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class GearSoundManager : MonoBehaviour
{
    public static GearSoundManager Instance;
    public AudioSource audioSource;

    public AudioClip edgeHit;
    public AudioClip gearHit;

    public Action gearChangedSoundEffect;
    public Action edgeReachedSoundEffect;

    private void OnEnable()
    {
        gearChangedSoundEffect += (GearChanged);
        edgeReachedSoundEffect += (EdgeReached);
    }

    private void OnDisable()
    {
        gearChangedSoundEffect -= (GearChanged);
        edgeReachedSoundEffect -= (EdgeReached);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void GearChanged()
    {
        audioSource.PlayOneShot(gearHit);
    }

    private void EdgeReached()
    {
        audioSource.PlayOneShot(edgeHit);
    }
}