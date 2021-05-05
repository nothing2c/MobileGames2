using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControllable
{
    GameObject gameObject { get; }
    void OnSelect();
    void MoveTo(Touch t);
    void DeSelect();
}
