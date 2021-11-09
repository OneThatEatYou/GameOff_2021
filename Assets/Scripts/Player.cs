using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public override IEnumerator Evaluate()
    {
        StartTurn();
        yield return null;
    }

    protected override void Die()
    {
        Debug.Log("Player died.");
    }
}
