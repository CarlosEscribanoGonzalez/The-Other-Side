using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BearRandomizer : MonoBehaviour
{

    [SerializeField] private Transform[] bears;

    private void Awake()
    {
        GameObject.FindObjectOfType<SideChanger>().sideChanged += RandomizeBears;
    }

    private void RandomizeBears(object s, EventArgs e)
    {
        StartCoroutine(MuteBears());
        float posX;
        float posZ;
        NavMeshHit navHit;
        Vector3 newPos;
        foreach(Transform bear in bears)
        {
            if (!bear.GetComponent<Teddy>().taken)
            {
                bool done = false;
                while (!done)
                {
                    posX = UnityEngine.Random.Range(-30, 30);
                    posZ = UnityEngine.Random.Range(-25, 25);
                    newPos = transform.position + new Vector3(posX, 0, posZ);
                    NavMesh.SamplePosition(newPos, out navHit, 20, 1);
                    bear.transform.position = navHit.position;
                    if (Mathf.Abs(bear.transform.position.y - this.transform.position.y) < 3) done = true;
                }
            }
        }
    }

    IEnumerator MuteBears()
    {
        foreach(Transform bear in bears)
        {
            bear.GetComponents<AudioSource>()[1].volume = 0;
        }
        yield return new WaitForSeconds(0.5f);
        foreach (Transform bear in bears)
        {
            bear.GetComponents<AudioSource>()[1].volume = 0.1f;
        }
    }
}
