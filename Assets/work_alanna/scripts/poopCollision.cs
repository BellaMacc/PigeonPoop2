using UnityEngine;
using System.Collections.Generic;


public class poopCollision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ParticleSystem poopParticles;
    public GameObject inkSplatPrefab;

    public string npcTag = "npc";

    private List<ParticleCollisionEvent> collisionEvents =
        new List<ParticleCollisionEvent>();

    private void OnParticleCollision(GameObject other)
    {
        int count = poopParticles.GetCollisionEvents(
            other, collisionEvents
        );

        for (int i = 0; i < count; i++)
        {
            ParticleCollisionEvent hit = collisionEvents[i];

            // Check the object that was hit
            if (other.CompareTag(npcTag) ||
                other.transform.root.CompareTag(npcTag))
            {
                //NPCReaction npc =
                //other.GetComponentInParent<NPCReaction>();

                other.SendMessage(
                   "Scream",
                   SendMessageOptions.DontRequireReceiver
               );

                Debug.Log("Poop hit NPC!");
            }
            else
            {
                // Spawn ink splat on the environment
                if (inkSplatPrefab != null)
                {
                    Quaternion rotation =
                        Quaternion.FromToRotation(
                            Vector3.up, hit.normal
                        );

                    Instantiate(
                        inkSplatPrefab,
                        hit.intersection,
                        rotation
                    );
                }
            }
        }
    }
}
