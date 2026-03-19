using UnityEngine;

public class TeleportPoint : Interactable
{
    public Transform teleportTarget;

    public override void Interact()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = teleportTarget.position;
    }
}