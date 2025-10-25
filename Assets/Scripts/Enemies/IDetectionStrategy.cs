using UnityEngine;
using Utilities;
public interface IDetectionStrategy
{
    bool Execute(Transform player, Transform detector, CountdownTimer timer);
}
