using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager
{
    public static Animator PlayAnimation(AnimationClip animationClip, Vector2 pos)
    {
        // create game object
        GameObject animationObject = GameObject.Instantiate((GameObject)Resources.Load("OneShotAnimation", typeof(GameObject)));

        if (!animationObject)
        {
            Debug.LogError("Error loading animation object. Aborting");
            return null;
        }

        animationObject.transform.position = pos;
        Animator animator = animationObject.GetComponent<Animator>();
        // create new animator override controller with generic animator as its base
        AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        // replace the clip of this state
        animatorOverrideController["PlaceholderAnimation"] = animationClip;
        // assign it in the animator
        animator.runtimeAnimatorController = animatorOverrideController;

        // destroy game object after playing the animation
        GameObject.Destroy(animationObject, animationClip.length);

        return animator;
    }
}
