using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectionGame : MonoBehaviour
{
    [SerializeField]
    private TMP_Text UIText;

    private string collectableTag = "Collectable";
    private int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        UIText.text = score.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == collectableTag)
        {
            collect(other.gameObject);
            UIText.text = score.ToString();
        }
    }

    // increment score, play animation, set object as inactive
    private void collect(GameObject GO)
    {
        ++score;
        StartCoroutine(CollectAnim(GO));
    }

    // collection animation
    IEnumerator CollectAnim(GameObject GO)
    {
        // removes tag so we cannot collect it
        GO.tag = "Untagged";

        // animation lasts half a second
        float duration = 0.7f;
        Transform TR = GO.transform;

        //original information
        Vector3 position = TR.position;
        Vector3 rotation = TR.eulerAngles;
        Vector3 scale = TR.localScale;

        // do a 720 turn, go up by 0.5 units, and scale down
        for (float timer = 0.0f; timer < duration; timer += Time.deltaTime)
        {
            // find percentage done with anim
            float alpha = timer / duration;

            // lerp position
            TR.position = Vector3.Lerp(position, position + new Vector3(0.0f, 2.0f, 0.0f), alpha);

            // lerp rotation
            TR.eulerAngles = Vector3.Lerp(rotation, rotation + new Vector3(80.0f, 720.0f, 100.0f), alpha);

            // lerp scale
            TR.localScale = Vector3.Lerp(scale, new Vector3(0.0f, 0.0f, 0.0f), alpha * 2.0f);

            yield return null;
        }

        GO.SetActive(false);
        yield break;
    }
}
