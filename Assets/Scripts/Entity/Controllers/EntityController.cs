using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    public Vector2 movement { get; protected set; } = Vector2.zero;
    public Quaternion lookRotation = Quaternion.identity;

    public bool jumping = false;
    public bool blockMotion = false;
    public bool hasGround = false;
    public bool sprinting = false;
    public bool crouching = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
