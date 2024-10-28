using System;
using UnityEngine;

public class Notepad : ATakeableObject
{
    [Header("Notepad: ")]
    [SerializeField] private GameObject hintText;
    [SerializeField] private GameObject hintText2;
    [SerializeField] private GameObject keys;
    [SerializeField] private GameObject[] displays;
    private int index;
    private static int maxDisplay = 3;
    private bool taken = false;
    private bool used = false;

    protected override void Awake()
    {
        base.Awake();
        GameObject.FindObjectOfType<SideChanger>().sideChanged += ToggleKeys;
        hintText.SetActive(false);
        hintText2.SetActive(false);
        foreach (GameObject display in displays) display.SetActive(false);
        displays[0].SetActive(true);
    }

    protected override void Update()
    {
        base.Update();
        if (!player.GetCanMove())
        {
            hintText.SetActive(false);
            hintText2.SetActive(false);
        }
    }

    public override void HoverEnter()
    {
        base.HoverEnter();
        if(!taken) hintText.SetActive(true);
    }

    public override void HoverExit()
    {
        base.HoverExit();
        hintText.SetActive(false);
    }

    public override void Take()
    {
        base.Take();
        if (!taken) GameObject.FindObjectOfType<SideChanger>().SetCanChange(true); //La primera vez que la agarra se activa el cambio de lado
        taken = true;
        if (!used) hintText2.SetActive(true);
    }

    private void ToggleKeys(object s, EventArgs e)
    {
        if (this.transform.position.y > 0) keys.SetActive(true);
        else keys.SetActive(false);
    }

    protected override void Use()
    {
        source.Play();
        echoGenerator.GenerateEcho(0.2f);
        displays[index].SetActive(false);
        index++;
        if (index == maxDisplay) index = 0;
        displays[index].SetActive(true);
        hintText2.SetActive(false);
        used = true;
    }

    public override void Throw()
    {
        base.Throw();
        hintText2.SetActive(false);
    }

    public static void AddMaxDisplay()
    {
        GameObject.Find("HintSFX").GetComponent<AudioSource>().Play();
        GameObject.Find("HintSFX").GetComponent<Animator>().SetTrigger("Animate");
        maxDisplay++;
    }
}
