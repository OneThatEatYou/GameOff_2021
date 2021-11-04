using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
public class BattleManager : MonoBehaviour
{
    private static BattleManager instance;
    public static BattleManager Instance
    {
        get
        {
            // check if BattleManager exists in scene
            if (instance == null) instance = FindObjectOfType<BattleManager>();
            if (instance == null)
            {
                Debug.LogError("BattleManager not found in scene");
                instance = new GameObject("BattleManager").AddComponent<BattleManager>();
            }

            return instance;
        }
    }

    [SerializeField, ReadOnly] private Targetable target;

    public void SetTarget(Targetable t)
    {
        target = t;
    }
}
