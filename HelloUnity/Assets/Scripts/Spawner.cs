using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject template;
    [SerializeField]
    private float spawnRange;
    [SerializeField]
    private int maxSpawnNum;

    private string collectableTag = "Collectable";
    private GameObject[] collectables;
    private Transform TR;

    // Start is called before the first frame update
    void Start()
    {
        TR = GetComponent<Transform>();
        // this is for the sphere renderer for visualization
        TR.localScale = Vector3.one;
        float newScale = spawnRange / TR.lossyScale.x;
        TR.localScale *= newScale;

        collectables = new GameObject[maxSpawnNum];
        template.SetActive(false);

        // create collectables
        for (int i = 0; i < maxSpawnNum; ++i)
        {
            collectables[i] = Instantiate(template,
                new Vector3(0.0f, 0.0f, 0.0f),
                template.transform.rotation);
        }

        updateCollectables();
    }

    // Update is called once per frame
    void Update()
    {
        updateCollectables();
    }

    private void updateCollectables()
    {
        foreach (GameObject collectable in collectables)
        {
            if (!collectable.activeInHierarchy)
                moveCollectable(collectable);
        }
    }

    // called on collectables not active in hierarchy
    // finds random positions until not colliding with scene
    private void moveCollectable(GameObject collectable)
    {
        Transform templateTR = template.GetComponent<Transform>();
        Transform CTR = collectable.transform;
        CTR.localScale = templateTR.localScale;
        CTR.rotation = templateTR.rotation;

        CTR.position = generatePos();

        // if successful at finding good spawn
        if (Vector3.Distance(CTR.position, TR.position) < spawnRange)
        {
            collectable.tag = collectableTag;
            collectable.SetActive(true);
        }
    }

    private Vector3 generatePos()
    {
        Vector3 finalPosition = new Vector3(0.0f, 0.0f, 0.0f);
        float halfHeight = template.GetComponent<Renderer>().bounds.size.y / 2.0f;
        float offset = halfHeight + 0.01f;

        // does five attempts, otherwise simply does nothiing
        for (int attempts = 0; attempts < 5; ++attempts)
        {
            float rad = Random.Range(0.0f, spawnRange);
            float theta = Random.Range(0.0f, 2.0f * Mathf.PI);

            Vector2 pos = (rad * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)));

            // raycast to surface
            RaycastHit hit;
            Physics.Raycast(new Vector3(pos.x + TR.position.x,
                TR.position.y + spawnRange,
                pos.y + TR.position.z),
                Vector3.down, out hit, spawnRange * 2.0f);

            finalPosition = hit.point + new Vector3(0.0f, offset, 0.0f);

            // if final pos not in circle, try again
            if (Vector3.Distance(finalPosition, TR.position) < spawnRange)
                break;
        }

        return finalPosition;
    }
}
