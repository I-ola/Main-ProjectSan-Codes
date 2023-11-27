using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public Vector3 soundLocation;
    public float loudness;
    public enum SoundsCategory 
    {
        Walking,
        Running,
        CrouchWalking,
        Shooting
    }

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {

    }
    private void Update()
    {
    
    }
    public void SoundEmitted( Vector3 location, SoundsCategory sound, float intensity)
    {
        //Debug.Log(intensity);
        SoundHeard(location, intensity);
       

    }

    protected virtual void SoundHeard(Vector3 location, float intensity)
    {

        
    }




}



