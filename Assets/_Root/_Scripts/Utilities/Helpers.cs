using UnityEngine;

/// <summary>
/// A static class for general helpful methods
/// </summary>
public static class Helpers
{
    /// <summary>
    /// Destroy all child objects of this transform.
    /// Use it like so:
    /// <code>
    /// transform.DestroyChildren();
    /// </code>
    /// </summary>
    public static void DestroyChildren(this Transform t)
    {
        foreach (Transform child in t) Object.Destroy(child.gameObject);
    }

    public static bool IsTargetInRange(this Vector3 center, Vector3 targetPos, float range)
    {
        var distanceToTarget = Vector3.Distance(center, targetPos);
        return distanceToTarget <= range;
    }

    public static bool IsTargetOutOfRange(this Vector3 center, Vector3 targetPos, float range)
    {
        var distanceFromTarget = Vector3.Distance(center, targetPos);
        return distanceFromTarget > range;
    }
}