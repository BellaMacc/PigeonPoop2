 using System.Collections;
using Unity.VectorGraphics;
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
            transform.position = intersection.transform.position;
            //checks which way the road allows
            bool posX = intersection.gameObject.GetComponent<roadRestrictions>().posX;
            bool negX = intersection.gameObject.GetComponent<roadRestrictions>().negX;
            bool posZ = intersection.gameObject.GetComponent<roadRestrictions>().posZ;
            bool negZ = intersection.gameObject.GetComponent<roadRestrictions>().negZ;

            //sees if the way isnt allowed
            bool wrongWay = true;

            //loops until right direction
            while (wrongWay == true)
            {
                //generates random direction
                int newDirection = Random.Range(0, 4);

                //goes through all direction cases
                switch (newDirection)
                {
                    //go posx
                    case 0:
                        if (posX)
                        {
                            //dont turn around
                            if(moveX && !movePos)
                            {
                                break;
                            }
                            moveX = true;
                            movePos = true;
                            wrongWay = false;
                        }
                        break;
                    //go negX
                    case 1:
                        if (negX)
                        {
                            //dont turn around
                            if (moveX && movePos)
                            {
                                break;
                            }

                            moveX = true;
                            movePos = false;
                            wrongWay = false;
                        }
                        break;
                    //go posZ
                    case 2:
                        if (posZ)
                        {
                            //dont turn around
                            if (!moveX && !movePos)
                            {
                                break;
                            }

                            moveX = false;
                            movePos = true;
                            wrongWay = false;
                        }
                        break;
                    //go negZ
                    case 3:
                        if (negZ)
                        {
                            //dont turn around
                            if (!moveX && movePos)
                            {
                                break;
                            }

                            moveX = false;
                            movePos = false;
                            wrongWay = false;
                        }
                        break;
                    //ruh roh thats not right
                    default:
                        break;
                }
            }
        }
        if (intersection.tag == "wall")
        {
            newPos();
        }
    }

    int checkTraffic()
    {
        return (1);
    }

    void newPos()
    {
        //get manager script from parent
        npcManager manager = GetComponentInParent<npcManager>();
        if (manager != null)
        {
            //get the number of children
            int childCount = manager.startLocations.transform.childCount;

            //if there are children then pick random child
            if (childCount > 0)
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
}
