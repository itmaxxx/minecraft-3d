using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateAppearOnScreen : MonoBehaviour
{
    private Vector3 startPos;
    public float moveStep = 1f;

    void Awake()
    {
        startPos = this.transform.position;

        this.transform.position = new Vector3(startPos.x, startPos.y - 100f, startPos.z); 
    }

    void Update()
    {
        if (this.transform.position.y < startPos.y) {
            this.transform.position += Vector3.up * moveStep * Time.deltaTime;
        }
    }
}
