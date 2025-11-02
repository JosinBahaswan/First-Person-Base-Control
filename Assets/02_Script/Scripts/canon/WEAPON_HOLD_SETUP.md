# 📋 PANDUAN SETUP WEAPON PICKUP - HOLD MODE

## ✅ SOLUSI: Weapon Pindah ke holdItemParent

### **Setting Yang Benar:**

**Di Inspector WeaponPickupItem:**
```
Auto Unlock Weapon: ❌ FALSE  ← PENTING!
Destroy After Unlock: ✅ True
```

**Dengan setting ini:**
- ✅ Weapon TIDAK langsung unlock
- ✅ Weapon pindah ke holdItemParent (seperti item biasa)
- ✅ Player bisa lihat weapon di tangan
- ✅ Bisa Drop/Throw weapon

---

## 🎮 DUA MODE PENGGUNAAN

### MODE 1: Hold Item (Recommended untuk Anda)

**Setup:**
```
WeaponPickupItem Settings:
- Auto Unlock Weapon: ❌ FALSE
- Destroy After Unlock: ✅ True

Item Settings (inherited):
- Is Static: ❌ False
- Hold Position Offset: (0.2, -0.1, 0.3)
- Hold Rotation Offset: (-90, 0, 0)
```

**Cara Kerja:**
1. Player press E/Interact button
2. Weapon pickup → **Pindah ke holdItemParent**
3. Weapon terlihat di tangan player
4. Player bisa:
   - Drop (G button)
   - Throw (H button)
   - Use untuk unlock weapon (perlu setup tambahan)

---

### MODE 2: Auto Unlock

**Setup:**
```
WeaponPickupItem Settings:
- Auto Unlock Weapon: ✅ TRUE
- Destroy After Unlock: ✅ True
```

**Cara Kerja:**
1. Player press E
2. Weapon langsung unlock di WeaponHolder
3. Pickup object destroyed
4. Langsung bisa tembak

---

## 🔧 CARA USE WEAPON YANG DI-HOLD

### Option A: Tambah Button "Use Weapon"

Tambahkan di `PlayerInteractionNoInventory.cs`:

```csharp
void Update()
{
    // ... existing code ...
    
    // Use weapon pickup dengan F key
    if (Input.GetKeyDown(KeyCode.F) && holdItem != null)
    {
        WeaponPickupItem weaponPickup = holdItem.GetComponent<WeaponPickupItem>();
        if (weaponPickup != null)
        {
            weaponPickup.UnlockWeaponFromHold();
            // holdItem sudah di-consume di dalam method
        }
    }
}
```

**Untuk Mobile:** Tambah button UI "Use" yang call method yang sama

---

### Option B: Auto-Use Saat Di-Hold

Tambahkan script `WeaponAutoUse.cs`:

```csharp
using UnityEngine;

public class WeaponAutoUse : MonoBehaviour
{
    [SerializeField] private float autoUseDelay = 2f; // Delay sebelum auto-use
    
    void OnEnable()
    {
        // Check jika di-hold
        if (transform.parent != null && 
            transform.parent.name.Contains("HoldItem"))
        {
            Invoke("AutoUseWeapon", autoUseDelay);
        }
    }
    
    void OnDisable()
    {
        CancelInvoke();
    }
    
    void AutoUseWeapon()
    {
        WeaponPickupItem pickup = GetComponent<WeaponPickupItem>();
        if (pickup != null)
        {
            pickup.UnlockWeaponFromHold();
        }
    }
}
```

Attach script ini ke weapon pickup GameObject.

---

## 📊 HOLD POSITION UNTUK SETIAP WEAPON

### Pistol
```
Hold Position Offset: (0.2, -0.1, 0.3)
Hold Rotation Offset: (-90, 0, 0)
```

### Rifle
```
Hold Position Offset: (0.15, -0.15, 0.4)
Hold Rotation Offset: (-90, 0, 0)
```

### Shotgun
```
Hold Position Offset: (0.2, -0.15, 0.5)
Hold Rotation Offset: (-90, 0, 0)
```

**Cara Adjust:**
1. Play mode
2. Pickup weapon
3. Pause game
4. Adjust position di Inspector
5. Copy values
6. Stop play mode
7. Paste ke prefab

---

## 🐛 DEBUG CHECKLIST

### ✅ Weapon pindah ke holdItemParent?

**Check di Hierarchy (saat Play Mode):**
```
Player
└── Camera
    └── holdItemParent
        └── WeaponPickup_Pistol ← Should be here!
```

**Jika TIDAK muncul:**
- ❌ Check `Auto Unlock Weapon = False`
- ❌ Check `Is Static = False`
- ❌ Check Rigidbody ada
- ❌ Check Collider ada
- ❌ Check base.OnInteract() dipanggil

---

### ✅ Weapon terlihat di tangan?

**Check:**
1. Camera culling mask include holdItemParent layer
2. Hold Position Offset tidak terlalu jauh
3. Model tidak disabled
4. Layer mask di PlayerInteractionNoInventory setup

**Camera Settings:**
```
Player Camera:
- Culling Mask: Everything EXCEPT HoldedItem layer

Interact Camera (if exists):
- Culling Mask: ONLY HoldedItem layer
```

---

## 🎯 QUICK SETUP GUIDE

### Langkah Cepat Setup Hold Mode:

1. **Select weapon pickup di scene**

2. **Inspector WeaponPickupItem:**
   ```
   Auto Unlock Weapon: ❌ Uncheck
   Destroy After Unlock: ✅ Check
   ```

3. **Inspector Item (inherited):**
   ```
   Is Static: ❌ Uncheck
   Hold Position Offset: (0.2, -0.1, 0.3)
   Hold Rotation Offset: (-90, 0, 0)
   ```

4. **Test:**
   - Play mode
   - Press E pada weapon
   - Check Hierarchy → holdItemParent → weapon ada?
   - Adjust position jika perlu

5. **Done!** ✅

---

## 💡 TIPS

### Untuk Game Horror:
- Use Hold Mode untuk puzzle (carry weapon ke tempat tertentu)
- Use Auto Unlock untuk combat (langsung bisa defend)

### Visual Feedback:
- Tambah outline/highlight saat weapon di-hold
- Tambah UI prompt "Press F to equip weapon"
- Animate weapon saat hold (sway, bob)

### Performance:
- Disable rotation saat di-hold (save CPU)
- Use LOD untuk weapon model
- Pool weapon pickups jika respawn

---

**Sekarang weapon pasti masuk ke holdItemParent!** ✅
