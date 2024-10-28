using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform text1;
    [SerializeField] private Transform text2;

    private void Update()
    {
        text1.position += new Vector3(0, speed * Time.deltaTime, 0);
        text2.position += new Vector3(0, speed * Time.deltaTime, 0);
    }
}
