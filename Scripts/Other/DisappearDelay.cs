using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearDelay : MonoBehaviour
{
    [SerializeField] private float delay = 3f;

    private void Awake()
    {
        Invoke(nameof(Disappear), delay);
    }

    private void Disappear()
    {
        Destroy(gameObject);
    }
}
