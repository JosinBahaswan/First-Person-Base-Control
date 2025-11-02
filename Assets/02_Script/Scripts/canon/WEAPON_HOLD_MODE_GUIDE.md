# 🎯 WEAPON HOLD MODE SETUP GUIDE

## 📖 PENJELASAN SISTEM

User ingin weapon **TIDAK HILANG** saat pickup, weapon harus **TETAP ADA DI TANGAN** player sebagai item yang di-hold (seperti sistem Item.cs), dan bisa **DITEMBAK LANGSUNG** dari posisi hold.

---

## 🏗️ ARSITEKTUR SISTEM HOLD MODE

### Dua Mode Weapon Pickup:

```
MODE 1: AUTO UNLOCK (autoUnlockWeapon = TRUE)
├── Press E → Weapon pickup HILANG
├── Weapon unlock di WeaponManager
├── Weapon aktif di WeaponHolder
└── ✅ Bisa tembak dari WeaponManager system

MODE 2: HOLD MODE (autoUnlockWeapon = FALSE) ⭐ NEW!
├── Press E → Weapon pickup PINDAH ke holdItemParent
├── Weapon tetap sebagai pickup object (TIDAK HILANG)
├── Weapon bisa ditembak dari HoldableWeapon script
└── ✅ Bisa tembak langsung dari held item
```

---

## 📦 SETUP WEAPON HOLD MODE

### Step 1: Weapon Pickup GameObject Setup

**Di scene, buat weapon pickup object:**

```
Weapon_Rifle_Pickup:
├── Model rifle 3D
├── BoxCollider (Is Trigger: ✅)
├── Rigidbody (Use Gravity: ✅)
├── WeaponPickupItem.cs
│   ├── Weapon Type: Rifle
│   ├── Ammo Amount: 60
│   ├── Auto Unlock Weapon: ❌ FALSE (PENTING!)
│   └── Destroy Pickup After Unlock: ✅ True
└── HoldableWeapon.cs ⭐ NEW SCRIPT!
    ├── Weapon Name: "Rifle"
    ├── Damage: 15
    ├── Fire Rate: 0.1 (auto fire)
    ├── Range: 100
    ├── Max Ammo: 30
    ├── Current Ammo: 30
    ├── Reserve Ammo: 60
    ├── Reload Time: 2.5
    ├── Use Auto Fire: ✅ TRUE (hold mouse to fire)
    └── Muzzle Point: (create child GameObject)
```

### Step 2: Muzzle Point Setup

**Buat child GameObject untuk muzzle flash:**

```
Weapon_Rifle_Pickup
└── MuzzlePoint (Empty GameObject)
    ├── Position: Di ujung laras rifle (contoh: 0, 0, 0.5)
    └── Drag ke field "Muzzle Point" di HoldableWeapon.cs
```

**Muzzle Point berfungsi untuk:**
- Spawn muzzle flash effect
- Origin point raycast shooting
- Shell ejection position

### Step 3: Effects Setup (Optional)

**Tambahkan visual effects:**

```
Muzzle Flash Effect:
├── Buat particle system atau sprite flash
├── Drag ke field "Muzzle Flash Effect"
└── Auto spawn & destroy saat tembak

Impact Effect:
├── Buat particle system untuk bullet impact
├── Drag ke field "Impact Effect"
└── Auto spawn di hit point

Shell Ejection:
├── Buat particle system untuk bullet shell
├── Attach ke weapon model
├── Drag ke field "Shell Ejection Effect"
└── Play saat tembak
```

### Step 4: Audio Setup (Optional)

**Tambahkan audio clips:**

```
HoldableWeapon Settings:
├── Shoot Sound: rifle_shoot.wav
├── Reload Sound: rifle_reload.wav
└── Empty Sound: gun_empty.wav
```

**AudioSource akan auto-added** oleh script.

---

## 🎮 GAMEPLAY FLOW

### User's Desired Flow:

```
1. Player lihat weapon pickup di dunia ✅
   └── Weapon_Rifle_Pickup visible dengan model 3D

2. Player press E ✅
   └── Weapon PINDAH ke holdItemParent (TIDAK HILANG!)
   └── Transform parent = holdItemParent
   └── Weapon tetap sebagai pickup object

3. Player bisa langsung tembak ✅
   └── Mouse Click Kiri → HoldableWeapon.Fire() dipanggil
   └── Raycast dari camera center
   └── Muzzle flash muncul
   └── Ammo berkurang
   └── Hit detection & damage

4. Player bisa reload ✅
   └── Press R → HoldableWeapon.StartReload()
   └── Wait reloadTime seconds
   └── Ammo refill dari reserve

5. Player pickup ammo ✅
   └── Press E di ammo pickup
   └── AmmoPickupItem detect HoldableWeapon
   └── Reserve ammo bertambah
   └── Debug: "✅ Added 30 ammo to held Rifle"

6. Player drop weapon ✅
   └── Press G (Drop button dari Item.cs)
   └── Weapon jatuh ke tanah
   └── Weapon tetap bisa dipickup lagi

7. Player throw weapon ✅
   └── Press H (Throw button dari Item.cs)
   └── Weapon terlempar
   └── Physics interaction
```

---

## 🔫 WEAPON TYPES SETUP

### Pistol (Semi-Auto):

```
HoldableWeapon Settings:
├── Weapon Name: "Pistol"
├── Damage: 10
├── Fire Rate: 0.3
├── Max Ammo: 12
├── Use Auto Fire: ❌ FALSE (click per shot)
└── Recoil Amount: 0.05
```

### Rifle (Full-Auto):

```
HoldableWeapon Settings:
├── Weapon Name: "Rifle"
├── Damage: 15
├── Fire Rate: 0.1
├── Max Ammo: 30
├── Use Auto Fire: ✅ TRUE (hold to fire)
└── Recoil Amount: 0.08
```

### Shotgun (Pump-Action):

```
HoldableWeapon Settings:
├── Weapon Name: "Shotgun"
├── Damage: 8 (per pellet, override PerformShoot untuk multiple rays)
├── Fire Rate: 0.8
├── Max Ammo: 8
├── Use Auto Fire: ❌ FALSE
└── Recoil Amount: 0.15
```

---

## 📊 HIERARCHY COMPARISON

### OLD SYSTEM (WeaponManager):

```
Player
└── Camera
    └── WeaponHolder
        ├── Pistol (WeaponBase.cs) ← Weapon prefab
        ├── Rifle (WeaponBase.cs)
        └── Shotgun (WeaponBase.cs)

Weapon_Rifle_Pickup (di dunia)
└── WeaponPickupItem.cs (autoUnlock = TRUE)
    └── Press E → HILANG, unlock weapon di WeaponHolder
```

### NEW SYSTEM (Hold Mode):

```
Player
└── Camera
    └── holdItemParent
        └── Weapon_Rifle_Pickup (HoldableWeapon.cs) ← Pickup object yang di-hold
            ├── Model 3D rifle
            ├── MuzzlePoint
            └── Scripts: WeaponPickupItem + HoldableWeapon

Weapon_Rifle_Pickup (di dunia)
└── WeaponPickupItem.cs (autoUnlock = FALSE)
    └── Press E → PINDAH ke holdItemParent, TIDAK HILANG
```

---

## 🔧 AMMO SYSTEM INTEGRATION

### AmmoPickupItem.cs Auto-Detect System:

```csharp
// Priority 1: Check held weapon (HoldableWeapon)
if (PlayerInteractionNoInventory.Instance.holdItem != null)
{
    HoldableWeapon heldWeapon = holdItem.GetComponent<HoldableWeapon>();
    if (heldWeapon != null)
    {
        // Add ammo ke held weapon ✅
        heldWeapon.AddAmmo(ammoAmount);
        return;
    }
}

// Priority 2: Fallback ke WeaponManager (WeaponBase)
WeaponBase currentWeapon = weaponManager.GetCurrentWeapon();
if (currentWeapon != null)
{
    // Add ammo ke weapon aktif di WeaponManager ✅
    currentWeapon.AddAmmo(ammoAmount);
}
```

**Ammo pickup otomatis detect:**
1. ✅ Apakah player hold weapon (HoldableWeapon)?
2. ✅ Atau weapon aktif di WeaponManager (WeaponBase)?
3. ✅ Tambah ammo ke yang sesuai

---

## 🎨 VISUAL FEEDBACK

### Hold Position & Rotation:

**Adjust di Inspector saat Play mode:**

```
Weapon_Rifle_Pickup (saat di holdItemParent):
├── Local Position: (0.3, -0.2, 0.5)
│   └── X: Right/Left position
│   └── Y: Up/Down position
│   └── Z: Forward/Back position
├── Local Rotation: (-90, 0, 0)
│   └── Adjust agar weapon menghadap depan
└── Local Scale: (1, 1, 1) atau adjust size
```

**Recommended Hold Positions:**

```
Pistol:
├── Position: (0.2, -0.1, 0.3)
└── Rotation: (-80, 0, 0)

Rifle:
├── Position: (0.15, -0.15, 0.4)
└── Rotation: (-85, 0, 0)

Shotgun:
├── Position: (0.2, -0.15, 0.5)
└── Rotation: (-85, 0, 0)
```

### Recoil Animation:

**HoldableWeapon.cs auto-handle recoil:**

```csharp
private void ApplyRecoil()
{
    // Push weapon back
    transform.localPosition -= Vector3.forward * recoilAmount;
}

void LateUpdate()
{
    // Return to original position smoothly
    transform.localPosition = Vector3.Lerp(
        transform.localPosition, 
        originalPosition, 
        Time.deltaTime * recoilSpeed
    );
}
```

**Adjust recoil feel:**
- `recoilAmount`: Jarak weapon mundur (0.05 - 0.2)
- `recoilSpeed`: Kecepatan kembali ke posisi (5 - 15)

---

## 🐛 TROUBLESHOOTING

### Problem 1: "Weapon masih hilang saat pickup"

**Check:**
```
✅ WeaponPickupItem.autoUnlockWeapon = FALSE
✅ Item.cs ada method HoldItem() yang move object ke holdItemParent
✅ PlayerInteractionNoInventory.Instance.holdItemParent exists
```

### Problem 2: "Tidak bisa tembak saat hold weapon"

**Check:**
```
✅ HoldableWeapon.cs attached ke weapon pickup
✅ Weapon ada di holdItemParent (check Hierarchy)
✅ Camera tag = "MainCamera"
✅ Console ada message "[HoldableWeapon] Fired! Ammo: X/Y"
```

### Problem 3: "Raycast tidak hit anything"

**Check:**
```
✅ HoldableWeapon.hitLayers include target layers
✅ Target punya collider
✅ Muzzle Point position correct (di ujung laras)
✅ Camera.main tidak null
```

### Problem 4: "Ammo pickup tidak work"

**Check:**
```
✅ Weapon di holdItemParent punya HoldableWeapon.cs
✅ Ammo type compatible (Universal, Pistol, Rifle, Shotgun)
✅ WeaponName contains type string (e.g. "Rifle")
✅ Console ada message "✅ Added X ammo to held Weapon"
```

### Problem 5: "Muzzle flash tidak muncul"

**Check:**
```
✅ Muzzle Point GameObject exists & assigned
✅ Muzzle Flash Effect prefab assigned
✅ Muzzle Flash Effect has particle system or visual
✅ Position muzzle point di ujung laras weapon
```

---

## 💡 ADVANCED FEATURES

### Custom Shooting Pattern (Shotgun):

Override `PerformShoot()` untuk multiple pellets:

```csharp
// Buat script baru: HoldableShotgun.cs inherit HoldableWeapon
protected override void PerformShoot()
{
    int pelletCount = 8;
    float spreadAngle = 5f;

    for (int i = 0; i < pelletCount; i++)
    {
        // Random spread
        Vector3 direction = mainCamera.transform.forward;
        direction += new Vector3(
            Random.Range(-spreadAngle, spreadAngle),
            Random.Range(-spreadAngle, spreadAngle),
            0
        );

        // Raycast per pellet
        RaycastHit hit;
        if (Physics.Raycast(muzzlePoint.position, direction, out hit, range))
        {
            // Apply damage per pellet
            Health health = hit.collider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage); // damage per pellet
            }
        }
    }

    // Call base untuk effects
    base.PerformShoot();
}
```

### Aim Down Sights (ADS):

Add to `HoldableWeapon.cs`:

```csharp
[Header("ADS Settings")]
[SerializeField] private Vector3 adsPosition = new Vector3(0, -0.05f, 0.3f);
[SerializeField] private float adsSpeed = 5f;
private bool isAiming = false;

void Update()
{
    // ... existing code ...

    // Handle ADS
    if (Input.GetMouseButtonDown(1)) // Right click
    {
        isAiming = true;
    }
    if (Input.GetMouseButtonUp(1))
    {
        isAiming = false;
    }

    // Lerp position
    Vector3 targetPos = isAiming ? adsPosition : originalPosition;
    transform.localPosition = Vector3.Lerp(
        transform.localPosition, 
        targetPos, 
        Time.deltaTime * adsSpeed
    );
}
```

### Laser Sight:

Add to weapon:

```csharp
[Header("Laser Sight")]
[SerializeField] private LineRenderer laserLine;
[SerializeField] private float laserRange = 50f;

void Update()
{
    // ... existing code ...

    // Update laser
    if (laserLine != null && isHeld)
    {
        laserLine.SetPosition(0, muzzlePoint.position);
        
        RaycastHit hit;
        if (Physics.Raycast(muzzlePoint.position, mainCamera.transform.forward, out hit, laserRange))
        {
            laserLine.SetPosition(1, hit.point);
        }
        else
        {
            laserLine.SetPosition(1, muzzlePoint.position + mainCamera.transform.forward * laserRange);
        }
    }
}
```

---

## 📞 QUICK SETUP CHECKLIST

Setup weapon hold mode dari awal:

- [ ] Buat weapon pickup GameObject di scene
- [ ] Attach BoxCollider (Trigger) + Rigidbody
- [ ] Attach WeaponPickupItem.cs
- [ ] Set autoUnlockWeapon = FALSE ⭐
- [ ] Attach HoldableWeapon.cs ⭐ NEW!
- [ ] Buat child GameObject "MuzzlePoint"
- [ ] Position MuzzlePoint di ujung laras
- [ ] Assign MuzzlePoint ke HoldableWeapon
- [ ] Set weapon stats (damage, fire rate, ammo)
- [ ] Set useAutoFire (TRUE = auto, FALSE = semi)
- [ ] Optional: Assign muzzle flash effect
- [ ] Optional: Assign impact effect
- [ ] Optional: Assign audio clips
- [ ] Test pickup → Check weapon di holdItemParent ✅
- [ ] Test shooting → Mouse click should fire ✅
- [ ] Test reload → Press R should reload ✅
- [ ] Test ammo pickup → Reserve ammo increase ✅
- [ ] Adjust hold position/rotation di Inspector
- [ ] Adjust recoil amount for feel
- [ ] Done! 🎮✨

---

## 🎓 UNDERSTANDING THE DIFFERENCE

### WeaponManager System (OLD):
```
Pros:
✅ Clean weapon switching (1, 2, 3 keys)
✅ Weapon prefabs stay with player
✅ Professional FPS system

Cons:
❌ Weapon pickup hilang dari dunia
❌ Tidak sesuai request user
```

### HoldableWeapon System (NEW):
```
Pros:
✅ Weapon tetap ada sebagai pickup object ⭐
✅ Sesuai sistem Item.cs (Use/Drop/Throw) ⭐
✅ Weapon tidak hilang dari hierarki ⭐
✅ Bisa ditembak langsung dari hold mode ⭐
✅ Integration dengan Item system existing

Cons:
⚠️ Tidak ada weapon switching (one weapon at a time)
⚠️ Must drop before pickup new weapon
```

---

## 🚀 FINAL ANSWER

**User request:**
> "harusnya tidak di auto unlock baru tidak hilang"

**✅ FIXED!** Default sekarang:
```
autoUnlockWeapon = FALSE
```

**User request:**
> "kalau ammo di simpan di child weapon saat sedang dipegang bisa?"

**✅ YES!** HoldableWeapon.cs punya:
```csharp
private int currentAmmo = 30;
private int reserveAmmo = 90;

public void AddAmmo(int amount) {
    reserveAmmo += amount; // ✅ Ammo disimpan di weapon yang di-hold
}
```

**User request:**
> "lalu nanti mekanisme menembaknya bagaimana termasuk peluru yang keluar dari muzzle?"

**✅ COMPLETE!** HoldableWeapon.cs support:
```
✅ Mouse click untuk tembak (auto-detect semi/auto fire)
✅ Raycast shooting dari camera center
✅ Muzzle flash effect dari MuzzlePoint
✅ Impact effect di hit point
✅ Shell ejection particle
✅ Recoil animation
✅ Damage ke Health component
✅ Audio feedback
✅ Ammo management (current + reserve)
✅ Auto reload saat ammo habis
```

**Setup sekarang:**
1. Attach `HoldableWeapon.cs` ke weapon pickup
2. Set `autoUnlockWeapon = FALSE` di WeaponPickupItem
3. Buat MuzzlePoint child GameObject
4. Test pickup → Weapon tidak hilang! ✅
5. Test shooting → Mouse click fires! ✅

**Semua fitur sudah ready!** 🎮🔥
