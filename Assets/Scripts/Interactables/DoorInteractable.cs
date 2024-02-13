using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    private float moveDuration = 1.0f;
    bool isOpen = false;
    public string GetInteractText()
    {
        return "Open/Close";
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(Transform interactorTransform)
    {
        if (Player.Instance.inventory.HasKey(Inventory.Keys.NormalKey))
        {
            if (!isOpen)
            {
                StartCoroutine(OpenDoorSmoothly(2.3f));
                isOpen = true;
            }
            else{
                StartCoroutine(OpenDoorSmoothly(-2.3f));
                isOpen = false;
            }
        }
    }

    IEnumerator OpenDoorSmoothly(float targetYOffset)
    {
        Vector3 targetPos = transform.position + Vector3.up * targetYOffset;

        float elapsedTime = 0.0f;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, elapsedTime / moveDuration);

            elapsedTime += Time.deltaTime;

            yield return null; // Wait for the next frame
        }

        transform.position = targetPos;

    }
}
