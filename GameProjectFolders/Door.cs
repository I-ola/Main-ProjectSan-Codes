using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public DoorType doorType;
    bool canOpen = false;
    public Animator animator;
    public enum DoorType
    {
        Door1 = 1,
        Door2 = 2,
        Door3 = 3,
        Door4 = 4
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(canOpen);
        if(canOpen)
        {
            Invoke("CloseDoor", 5);
        }
        //DoorAnimation();
    }

    public void CheckKey(Key key, Door door, bool pos)
    {
        if(key != null)
        {
            Debug.Log((int)key.keytype == (int)door.doorType);
            if ((int)key.keytype == (int)door.doorType)
            {
                if (pos)
                {
                    animator.SetBool("OpenDoorFront", true);
                    canOpen = true;
                }
                else
                { 
                    animator.SetBool("OpenDoorBack", true);
                    canOpen = true;
                }
            }
        }
    }

    void DoorAnimation()
    {

    }

    void CloseDoor()
    {
        canOpen = false;
        animator.SetBool("OpenDoorBack", false);
        animator.SetBool("OpenDoorFront", false);

    }
}
