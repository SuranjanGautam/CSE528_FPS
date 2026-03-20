using UnityEngine;

public class StringRenderer : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField]
    Transform[] string_locations;

    Vector3[] postions;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        postions = new Vector3[string_locations.Length];
    }

    // Update is called once per frame
    void LateUpdate()
    {
        postions[0] = string_locations[0].position;
        postions[1] = string_locations[1].position;
        postions[2] = string_locations[2].position;
        lineRenderer.SetPositions(postions);
    }
}
