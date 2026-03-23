using System.Collections;
using UnityEngine;

public class ReactFallOff : MonoBehaviour, IReactiveTarget
{
    [SerializeField]
    bool parent = false;
    bool fallen = false;
    public void ReactToHit(float amt = 1)
    {
        if (fallen) return;

        //rip off children
        if (parent)
        {
            var children = GetComponentsInChildren<IReactiveTarget>();
            foreach (var child in children)
            {
                if (child != (IReactiveTarget)this)
                {
                    child.ReactToHit(amt);
                }
            }
        }
        else
        {
            transform.parent = null;
            gameObject.AddComponent<Rigidbody>();
        }
        StartCoroutine(Die());
        fallen = true;
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
