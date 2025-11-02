# 🔧 CARA SETUP WEAPON DI HOLDITEMPARENT

## 📋 MASALAH & SOLUSI

### ❌ **Masalah: Weapon hilang saat interact**

**Penyebab:**
- Script WeaponPickupItem langsung destroy GameObject setelah pickup
- Weapon tidak masuk ke holdItemParent

**✅ Solusi:**
- Set `Destroy After Pickup = False` di Inspector
- Atau gunakan mode "Hold Item" dengan setting khusus

---

## 🎯 DUA MODE PENGGUNAAN

### MODE 1: Direct Unlock (Default) ✅ Recommended

Weapon langsung unlock di WeaponManager, tidak perlu hold.

**Setup:**
```
WeaponPickupItem Settings:
- Give Weapon If Not Owned: ✅ True
- Add Ammo If Already Owned: ✅ True
- Destroy After Pickup: ✅ True
```

**Cara Kerja:**
1. Player press E
2. Weapon langsung unlock di tangan (WeaponHolder)
3. Pickup object destroyed
4. Player bisa langsung tembak

**Kelebihan:**
- ✅ Cepat dan simpel
- ✅ Tidak perlu hold item
- ✅ Langsung bisa digunakan

---

### MODE 2: Hold Item First (Manual)

Weapon di-hold di holdItemParent dulu, baru unlock manual.

**Setup:**
```
WeaponPickupItem Settings:
- Give Weapon If Not Owned: ❌ False
- Add Ammo If Already Owned: ❌ False
- Destroy After Pickup: ❌ False

Item.cs Settings:
- Is Static: ❌ False
- Hold Position Offset: (0, 0, 0.3)
- Hold Rotation Offset: (0, 90, 0)
```

**Cara Kerja:**
1. Player press E
2. Weapon masuk ke holdItemParent (seperti item biasa)
3. Player bisa Drop/Throw
4. Atau buat button "Use" untuk unlock weapon

**Kelebihan:**
- ✅ Weapon bisa di-hold seperti item lain
- ✅ Bisa drop/throw sebelum unlock
- ✅ Konsisten dengan sistem item

**Kekurangan:**
- ⚠️ Perlu extra step untuk unlock weapon
- ⚠️ Tidak langsung bisa digunakan

---

## 🛠️ SETUP UNTUK HOLDITEMPARENT

### 1. Setup Hold Position untuk Weapon

Weapon pickup perlu offset yang benar agar terlihat bagus di tangan:

**Pistol:**
```
Hold Position Offset: (0.2, -0.1, 0.3)
Hold Rotation Offset: (-90, 0, 0)
```

**Rifle:**
```
Hold Position Offset: (0.15, -0.15, 0.4)
Hold Rotation Offset: (-90, 0, 0)
```

**Shotgun:**
```
Hold Position Offset: (0.2, -0.15, 0.5)
Hold Rotation Offset: (-90, 0, 0)
```

### 2. Adjust di Inspector

1. **Saat Play Mode:**
   - Pickup weapon
   - Adjust position & rotation di Inspector
   - Copy nilai yang pas

2. **Paste ke Prefab:**
   - Stop Play Mode
   - Paste nilai ke weapon pickup prefab

---

## 🎮 CARA PAKAI MODE HOLD ITEM

### Option A: Auto-Convert saat Hold

Tambahkan script ini ke weapon pickup:

```csharp
public class WeaponHoldConverter : MonoBehaviour
{
    void OnEnable()
    {
        // Check jika di-hold
        if (transform.parent != null && 
            transform.parent.name.Contains("HoldItem"))
        {
            // Auto-convert ke weapon setelah delay
            Invoke("ConvertToWeapon", 1f);
        }
    }
    
    void ConvertToWeapon()
    {
        WeaponPickupItem pickup = GetComponent<WeaponPickupItem>();
        if (pickup != null)
        {
            // Simulate interact untuk unlock weapon
            pickup.OnInteract();
        }
    }
}
```

### Option B: Manual Use Button

Tambahkan method di PlayerInteractionNoInventory.cs:

```csharp
void Update()
{
    // ... existing code ...
    
    // Use weapon pickup
    if (Input.GetKeyDown(KeyCode.F) && holdItem != null)
    {
        WeaponPickupItem weaponPickup = holdItem.GetComponent<WeaponPickupItem>();
        if (weaponPickup != null)
        {
            weaponPickup.OnInteract();
            holdItem = null; // Clear hold
        }
    }
}
```

**Mobile:** Tambahkan button "Use Weapon"

---

## 🔍 DEBUG: Cek Weapon Hilang

### Step 1: Check di Console

Buka Console dan lihat message:
```
✅ "Unlocked Pistol with 30 ammo" → Success
✅ "Added 30 ammo to Pistol" → Success
❌ "WeaponManager tidak ditemukan" → Fix WeaponManager
```

### Step 2: Check di Hierarchy (saat Play Mode)

```
Player
└── Camera
    └── WeaponHolder
        ├── Pistol ← Should be ACTIVE after pickup
        ├── Rifle
        └── Shotgun
```

**Jika Pistol tetap disabled:**
- Check weapons array di WeaponManager
- Check weapon name match (case-sensitive)
- Check UnlockWeapon() dipanggil

### Step 3: Check Destroy After Pickup

**Jika True:**
- Pickup object destroyed setelah E
- Weapon muncul di tangan (WeaponHolder)

**Jika False:**
- Pickup object SetActive(false)
- Weapon muncul di tangan

---

## 📊 COMPARISON TABLE

| Mode | Destroy After Pickup | Give Weapon | Hold di holdItemParent | Langsung Bisa Tembak |
|------|---------------------|-------------|------------------------|----------------------|
| **Direct Unlock** | ✅ True | ✅ True | ❌ No | ✅ Yes |
| **Hold First** | ❌ False | ❌ False | ✅ Yes | ❌ No (perlu convert) |
| **Hybrid** | ❌ False | ✅ True | ✅ Yes (sementara) | ✅ Yes (setelah convert) |

---

## 💡 RECOMMENDED WORKFLOW

### Untuk Game Horror Survival:

**Early Game (Player belum punya weapon):**
```
Setup:
- Give Weapon If Not Owned: ✅ True
- Destroy After Pickup: ✅ True

Result: Langsung unlock, player bisa defend
```

**Mid-Late Game (Player sudah punya weapon):**
```
Setup:
- Add Ammo If Already Owned: ✅ True
- Destroy After Pickup: ✅ True

Result: Langsung tambah ammo ke weapon yang ada
```

**Weapon sebagai Puzzle Item:**
```
Setup:
- Give Weapon If Not Owned: ❌ False
- Destroy After Pickup: ❌ False
- Can Be Held: ✅ True

Result: Weapon bisa dibawa kemana-mana, drop di tempat tertentu
```

---

## 🐛 TROUBLESHOOTING

### ❌ Weapon tetap hilang setelah pickup

**Check:**
1. ✅ WeaponManager ada di scene?
2. ✅ Weapons array di WeaponManager terisi?
3. ✅ Weapon GameObject di WeaponHolder ada?
4. ✅ Console ada error?

**Debug:**
```csharp
// Tambahkan di WeaponPickupItem.OnInteract():
Debug.Log($"WeaponManager: {weaponManager != null}");
Debug.Log($"Already has weapon: {alreadyHasWeapon}");
Debug.Log($"Give weapon: {giveWeaponIfNotOwned}");
```

### ❌ Weapon di holdItemParent tapi tidak visible

**Check:**
1. ✅ Hold Position Offset benar?
2. ✅ Camera culling mask include holdItemParent layer?
3. ✅ Model visible (not disabled)?

**Fix:**
```
Adjust Hold Position Offset sampai terlihat
Atau change camera culling settings
```

---

## 📞 QUICK FIX CHECKLIST

Jika weapon hilang saat interact:

- [ ] Set `Destroy After Pickup = True` (untuk direct unlock)
- [ ] Check WeaponManager ada di scene
- [ ] Check weapons array di WeaponManager terisi
- [ ] Check weapon name match di console
- [ ] Test dengan Debug.Log di OnInteract()
- [ ] Verify weapon GameObject aktif setelah pickup

**95% masalah solved dengan checklist ini!** ✅

---

**Need more help? Check WEAPON_FAQ.md** 📚
