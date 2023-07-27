using UnityEngine;


public class IKControl : MonoBehaviour {

	private Animator animator;
	public bool ikActive = false;
    public Transform lookObj = null;

	void Start () 
	{
		animator = GetComponent<Animator>();
        lookObj = GetComponent<Enemy>().player.transform;
	}

	void OnAnimatorIK()
	{
	  if(ikActive)
      {             
       if (lookObj != null)
       {
         animator.SetLookAtWeight(1, 1, 1, 1, 1);                        
         animator.SetLookAtPosition(lookObj.position);
       }
	  }			
	}    
}
