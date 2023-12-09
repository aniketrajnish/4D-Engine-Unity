using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollision4D : IEventWithData
{
    public GameObject collidedObject;

    public OnCollision4D(GameObject gameObject)
    {
        collidedObject = gameObject;
    }
}
