using UnityEngine;

/* Thinking process: 
 Player collide with food
poop counter +1 

 action poop --> click x for example

 if poop != 0
    object spawn at location 
    poop-1
*/
public class hungerBar : MonoBehaviour
{
    public int poop = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Food")
        {
            poop ++;
            Debug.Log(poop);
            Destroy(other.gameObject);
        }
    }

    
}
