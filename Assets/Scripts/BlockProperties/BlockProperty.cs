using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockProperty : MonoBehaviour
{
    public enum Property
    {
        Normal,
        Slippery,
        Sticky,
        Bouncy
    }
    public Property property;
}
