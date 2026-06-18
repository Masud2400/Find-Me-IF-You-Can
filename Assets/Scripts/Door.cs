using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator anim;
	
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }
	
    private void OnTriggerEnter(Collider other)
    {
        if (anim != null && other.CompareTag("Player"))
        {
            anim.SetBool("IsOpen", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (anim != null && other.CompareTag("Player"))
        {
            anim.SetBool("IsOpen", false);
        }
    }
}
