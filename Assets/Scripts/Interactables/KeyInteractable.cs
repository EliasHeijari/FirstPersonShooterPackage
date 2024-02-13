using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private int key = Inventory.Keys.NormalKey;
    public string GetInteractText()
    {
        return "Get Key";
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(Transform interactorTransform)
    {
        Player.Instance.inventory.AddKey(key);
        Destroy(gameObject);
    }
}
