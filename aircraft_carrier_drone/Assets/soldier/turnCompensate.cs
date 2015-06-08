using UnityEngine;
using System.Collections;

public class turnCompensate : StateMachineBehaviour {

	public bool left;
	public bool right;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	//override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		Vector3 euler = animator.rootRotation.eulerAngles;
		if (left)
			euler -= new Vector3 (0.0f, 0.0f, 0.0f);
			//transform.eulerAngles = transform.eulerAngles + new Vector3 (0.0f, 1.0f, 0.0f);
		if (right)
			euler += new Vector3 (0.0f, 0.0f, 0.0f);
			//transform.eulerAngles = transform.eulerAngles + new Vector3 (0.0f, -1.0f, 0.0f);
		animator.rootRotation = Quaternion.Euler (euler);
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
