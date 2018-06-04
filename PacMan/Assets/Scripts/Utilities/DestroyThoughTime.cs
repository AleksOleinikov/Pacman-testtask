using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyThoughTime : MonoBehaviour {

    [SerializeField] private float timeToDestroy;

    private void OnEnable()
    {
        Destroy(this.gameObject, timeToDestroy);
    }
}
