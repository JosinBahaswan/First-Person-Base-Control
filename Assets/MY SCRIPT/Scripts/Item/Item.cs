using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : Interactable
{
    [Header("Item Trait")]
    public bool isStatic;

    [Header("Debug")]
    public Vector3 holdPositionOffset;
    public Vector3 holdRotationOffset;
    public Rigidbody rb;
    public Collider coll;

    private void Reset()
    {
        Setup();
    }

    private void Awake()
    {
        Setup();
        if (isStatic) rb.isKinematic = true;
    }

    void Setup()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (coll == null) coll = GetComponent<Collider>();
    }

    public override void OnInteract()
    {
        base.OnInteract();

        PlayerInteractionNoInventory.Instance.HoldItem(this);
    }
}
