using System.Collections;
using UnityEngine;

public class npcMovement : MonoBehaviour
{
    public int lookDirection;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npcManager manager = GetComponentInParent<npcManager>();
        if(manager != null)
        {
            int childCount = manager.startLocations.transform.childCount;

            if(childCount > 0)
            {
                int randomLoc = Random.Range(0, childCount);
                Transform randChild = manager.startLocations.transform.GetChild(randomLoc);
                transform.position = randChild.position;

                if (randChild.CompareTag("sideRoad"))
                {
                    lookDirection = 0;
                }
                else
                {
                    lookDirection = 2;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newCord = transform.position;
        if (lookDirection == 0)
        { 
            newCord.x -= 0.5f;
        }
        else if (lookDirection == 10)
        {
            return;
        }
        else
        {
            newCord.z += 0.5f;
        }
        transform.position = newCord;
    }

    void OnTriggerEnter(Collider intersection)
    {
        Debug.Log("hit");
        lookDirection = 10;
    }
}
