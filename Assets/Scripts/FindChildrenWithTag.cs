using UnityEngine;
using System.Collections.Generic;

public class FindChildWithTag : MonoBehaviour
{
    public static GameObject FindInChildrenBFS(Transform parent, string tag)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(parent);

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();

            foreach (Transform child in current)
            {
                if (child.CompareTag(tag))
                {
                    return child.gameObject;
                }
                queue.Enqueue(child);
            }
        }
        return null;
    }
}