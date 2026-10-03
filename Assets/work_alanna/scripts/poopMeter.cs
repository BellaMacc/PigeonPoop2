using UnityEngine;

/* Thinking process: 
 Player collide with food
poop counter +1 

 action poop --> click x for example

 if poop != 0
    object spawn at location 
    poop-1
*/

// The collision I based on this coin collecting tutorial: https://www.youtube.com/watch?v=6iSJ_jh6Rdo
public class poopMeter : MonoBehaviour
{
    public int poopNum = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Food")
        {
            poopNum++;
            Debug.Log(poopNum);
            Destroy(other.gameObject);
        }
    }
    
}
