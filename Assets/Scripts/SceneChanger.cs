using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private float timeToSkip;
    [SerializeField] private bool autoSkip;
    [SerializeField] private string nextScene;
    [SerializeField] private float fadeinSpeed = 1;
    [SerializeField] private float fadeoutSpeed = 1;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.speed = fadeinSpeed;
    }

    private void Update()
    {
        if (!autoSkip) return;
        timeToSkip -= Time.deltaTime;
        if (timeToSkip <= 0) ChangeScene();
    }

    public void ChangeScene()
    {
        anim.speed = fadeoutSpeed;
        anim.SetTrigger("Fadeout");
    }

    public void PerformChange()
    {
        SceneManager.LoadScene(nextScene);
    }
}
