using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISoundManager : SoundManager
{
    private Processor processor;
    public bool hearSomething = false;
    // Start is called before the first frame update
    void Start()
    {
       processor = GetComponent<Processor>();
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    protected override void SoundHeard(Vector3 location, float intensity)
    {
        if(processor != null)
        {
            hearSomething = true;
            processor.soundIntensity = intensity;
            processor.soundLocation = location;
            Invoke("ResetHearing", 3.0f);
        }
       
    }

 
    void ResetHearing()
    {
        hearSomething = false;
    }
  
}
