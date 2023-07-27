using UnityEngine;

public class Door : MonoBehaviour {

    [Header("Door settings")]
    [Tooltip("Door animation component")]
    public Animation animationDoor;
    [Tooltip("Door open animation name (type 1)")]
    public string openNameA;
    [Tooltip("Door open animation name (type 2)")]
    public string openNameB;
    [Tooltip("Door close animation name (type 1)")]
    public string closeNameA;
    [Tooltip("Door close animation name (type 2)")]
    public string closeNameB;
    [Tooltip("Animation of trying open door")]
    public string tryOpenName;
    [Tooltip("If door must be locked, make this value true")]
    public bool locked;
    [Tooltip("Door opening sound")]
    public AudioClip openSound;
    [Tooltip("Door closing sound")]
    public AudioClip closeSound;
    [Tooltip("Door try open sound")]
    public AudioClip tryOpenSound;
    [Tooltip("Door unlocking sound")]
    public AudioClip unlockSound;
    [Tooltip("ID of key to open this door (Item.cs)")]
    public int keyID;
    [HideInInspector]
    public int state;
    [Tooltip("The number of locks or boards that close this door")]
    public int locksCount;
    

    private void Awake()
    {
        state = 0;
   
    }

    public void InteractDoor(int sideID)
    {
        if(!animationDoor.isPlaying)
        {
            if (!locked && locksCount == 0)
            {

                if (sideID == 0)
                {
                    if (state == 0)
                    {
                        AudioSource.PlayClipAtPoint(openSound, transform.position);
                        animationDoor.Play(openNameA);
                        state = 1;
                    } else
                    {
                        if (state == 1)
                        {
                            AudioSource.PlayClipAtPoint(closeSound, transform.position);
                            animationDoor.Play(closeNameA);
                            state = 0;

                        }
                        else
                        {
                            AudioSource.PlayClipAtPoint(closeSound, transform.position);
                            animationDoor.Play(closeNameB);
                            state = 0;
                        }
                    }
                }
                else
                {
                    if (state == 0)
                    {
                        AudioSource.PlayClipAtPoint(openSound, transform.position);
                        animationDoor.Play(openNameB);
                        state = 2;
                    }
                    else
                    {
                        if (state == 1)
                        {
                            AudioSource.PlayClipAtPoint(closeSound, transform.position);
                            animationDoor.Play(closeNameA);
                            state = 0;

                        }
                        else
                        {
                            AudioSource.PlayClipAtPoint(closeSound, transform.position);
                            animationDoor.Play(closeNameB);
                            state = 0;
                        }
                    }
                }

            }else
            {
                AudioSource.PlayClipAtPoint(tryOpenSound, transform.position);
                animationDoor.Play(tryOpenName);
               
            }
        }
    }


    public void UnlockDoor()
    {
        AudioSource.PlayClipAtPoint(unlockSound, transform.position);
        locked = false;
    }


    public void UpdateState()
    {
        if(locksCount <= 0 && GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void LoadState()
    {
        if (animationDoor)
        {
            if (state == 1)
            {
                animationDoor.Play(openNameA);
            }

            if (state == 2)
            {
                animationDoor.Play(openNameB);
            }
        }

        if (locksCount <= 0 && GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

}
