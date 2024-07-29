using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFallingObstacle {
    float GetFallSpeed();

    void SetFallSpeed(float speed);

    int GetDamage();

    void SetInverted(bool i);

    bool IsInverted();
}
