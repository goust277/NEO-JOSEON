using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private void Awake()
    {
        panel.SetActive(false);
    }

    public void Interact()
    {
        panel.SetActive(true);
    }

    public void EndInteract()
    {
        panel.SetActive(false);
    }
}
