using System.Collections;
using UnityEngine;

public class npcMovement : MonoBehaviour
{
    public bool moveX;
    public bool movePos;
    public float speed = 0.5f;
    public int moveDirection = 0;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get manager script from parent
        npcManager manager = GetComponentInParent<npcManager>();
        if(manager != null)
        {
            //get the number of children
            int childCount = manager.startLocations.transform.childCount;

            //if there are children then pick random child
            if(childCount > 0)
            {
                //pick random start location and move there
                int randomLoc = Random.Range(0, childCount);
                Transform randChild = manager.startLocations.transform.GetChild(randomLoc);
                transform.position = randChild.position;

                //if on side road move negatively along x
                if (randChild.CompareTag("sideRoad"))
                {
                    moveX = true;
                    movePos = false;
                }
                //if on main road move positively along z
                else
                {
                    moveX = false;
                    movePos = true;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //get position
        Vector3 newCord = transform.position;

        //makes it stop
        if (moveDirection != 0)
        {
            return;
        }

        //if moving z
        if (moveX == false)
        {
            //moving neg
            if (movePos == false)
            {
                newCord.z -= speed;
            }
            //moving pos
            else
            {
                newCord.z += speed;
            }
        }
        //if moving x
        else
        {
            //moving neg
            if (movePos == false)
            {
                newCord.x -= speed;
            }
            //moving pos
            else
            {
                newCord.x += speed;
            }
        }
        //set new position
        transform.position = newCord;
    }

    void OnTriggerEnter(Collider intersection)
    {
        //if you get to a corner intersection
        if (intersection.CompareTag("corner"))
        {
            int newDirection = Random.Range(0, 3);
            //keep going straight
            if (newDirection == 0)
            {
                return;
            }
            else
            {
                moveX = !moveX;
                if (newDirection == 1)
                {
                    movePos = true;
                }
                else
                {
                    movePos = false;
                }
                
            }
            Debug.Log(newDirection);
        }
        
    }
}
