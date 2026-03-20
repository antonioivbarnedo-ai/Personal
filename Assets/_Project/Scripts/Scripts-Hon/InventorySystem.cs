using System.Collections.Generic;
using UnityEngine;
using System;

public class InventorySystem : MonoBehaviour
{
    // ── Singleton ──────────────────────────────────────────────
    public static InventorySystem Instance { get; private set; }

    // ── Item name constants (no raw strings in other scripts!) ──
    public const string OFFERING_PLATE = "OfferingPlate";
    public const string MAGICAL_MANGO  = "MagicalMango";

    // ── C# Event so UI can react to item changes ────────────────
    public static event Action<string, bool> OnInventoryChanged;
    // fires as: OnInventoryChanged(itemName, wasAdded)

    private HashSet<string> _items = new HashSet<string>();

    // ────────────────────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddItem(string itemName)
    {
        if (_items.Add(itemName))
        {
            Debug.Log($"[Inventory] + {itemName}");
            OnInventoryChanged?.Invoke(itemName, true);
        }
    }

    public void RemoveItem(string itemName)
    {
        if (_items.Remove(itemName))
        {
            Debug.Log($"[Inventory] - {itemName}");
            OnInventoryChanged?.Invoke(itemName, false);
        }
    }

    public bool HasItem(string itemName) => _items.Contains(itemName);
}