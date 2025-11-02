# 🎯 WEAPON SYSTEM SETUP - PENJELASAN LENGKAP

## ❓ KENAPA WEAPON HILANG DARI HIERARKI?

### Penjelasan Sistem:

Ada **2 OBJEK BERBEDA** dalam weapon system:

```
1. WEAPON PICKUP (world object)
   └── WeaponPickupItem.cs attached
   └── Model 3D weapon di dunia
   └── FUNGSI: Trigger untuk unlock weapon
   └── HARUS DIHAPUS setelah pickup ✅

2. WEAPON PREFAB (player's weapon)
   └── WeaponBase.cs (Pistol/Rifle/Shotgun) attached
   └── Model 3D weapon di tangan player
   └── FUNGSI: Weapon yang bisa ditembak
   └── TETAP ADA di WeaponHolder ✅
```

### Flow yang Benar:

```
Player press E di Weapon Pickup
        ↓
WeaponPickupItem.OnInteract() dipanggil
        ↓
WeaponManager.UnlockWeapon("Pistol", 30) dipanggil
        ↓
Weapon Prefab di WeaponHolder diaktifkan ✅
        ↓
Weapon Pickup dihapus dari dunia ✅ (Normal behavior!)
        ↓
Player bisa tembak dengan weapon di tangan ✅
```

---

## 📦 SETUP HIERARKI YANG BENAR

### Di Scene:

```
Player
├── Camera
│   └── WeaponHolder (child of Camera)
│       ├── Pistol ← WeaponBase weapon prefab (INACTIVE by default)
│       │   └── PistolModel (visual)
│       ├── Rifle ← WeaponBase weapon prefab (INACTIVE by default)
│       │   └── RifleModel (visual)
│       └── Shotgun ← WeaponBase weapon prefab (INACTIVE by default)
│           └── ShotgunModel (visual)
└── WeaponManager.cs (atau di Player root)

WeaponPickup_Pistol (di dunia) ← Ini HARUS HILANG saat pickup!
├── WeaponPickupItem.cs
└── PistolModel_Pickup (visual untuk display)

WeaponPickup_Rifle (di dunia) ← Ini HARUS HILANG saat pickup!
├── WeaponPickupItem.cs
└── RifleModel_Pickup (visual untuk display)
```

---

## ✅ CARA SETUP WEAPON SYSTEM

### Step 1: Setup Weapon Prefabs di Player

**Lokasi:** `Player → Camera → WeaponHolder`

```
1. Buat empty GameObject "WeaponHolder" sebagai child Camera
   Position: (0, 0, 0.5) atau sesuai kebutuhan
   
2. Di dalam WeaponHolder, buat weapon prefabs:
   
   Pistol GameObject:
   ├── Script: Pistol.cs (inherits WeaponBase)
   ├── Model: Import pistol 3D model
   ├── Set INACTIVE di Inspector ❌ (disabled by default)
   
   Rifle GameObject:
   ├── Script: Rifle.cs (inherits WeaponBase)
   ├── Model: Import rifle 3D model
   ├── Set INACTIVE di Inspector ❌ (disabled by default)
   
   Shotgun GameObject:
   ├── Script: Shotgun.cs (inherits WeaponBase)
   ├── Model: Import shotgun 3D model
   ├── Set INACTIVE di Inspector ❌ (disabled by default)
```

### Step 2: Setup WeaponManager

**Lokasi:** Player root atau Player → Camera → WeaponHolder

```
WeaponManager Settings:
├── Weapons Array [3]:
│   ├── Element 0: Pistol (drag from WeaponHolder)
│   ├── Element 1: Rifle (drag from WeaponHolder)
│   └── Element 2: Shotgun (drag from WeaponHolder)
├── Current Weapon Index: 0
├── Weapon Holder: (drag WeaponHolder GameObject)
├── Use New Input System: ✅ atau ❌ sesuai project
└── Use Mobile Controls: ✅ jika Android
```

### Step 3: Setup Weapon Pickups (di dunia)

**Lokasi:** Di scene, tempat player bisa pickup

```
Weapon_Pistol_Pickup:
├── WeaponPickupItem.cs:
│   ├── Weapon Type: Pistol
│   ├── Ammo Amount: 30
│   ├── Auto Unlock Weapon: ✅ TRUE (recommended)
│   └── Destroy Pickup After Unlock: ✅ TRUE (MUST be true!)
├── BoxCollider (Trigger: ✅)
├── Rigidbody
└── Model (PistolModel_Display)

Weapon_Rifle_Pickup:
├── WeaponPickupItem.cs:
│   ├── Weapon Type: Rifle
│   ├── Ammo Amount: 60
│   ├── Auto Unlock Weapon: ✅ TRUE
│   └── Destroy Pickup After Unlock: ✅ TRUE
├── BoxCollider (Trigger: ✅)
├── Rigidbody
└── Model (RifleModel_Display)
```

---

## 🔍 TROUBLESHOOTING

### Problem 1: "Weapon hilang dari hierarki saat pickup"

**✅ INI NORMAL!** Yang hilang adalah **Weapon Pickup** (world object).

**Check:**
```
1. Buka Hierarchy saat Play mode
2. Pickup weapon
3. Check: Player → Camera → WeaponHolder → Pistol
4. Apakah ada GameObject "Pistol" yang ACTIVE? ✅
```

**Jika Pistol GameObject ada dan ACTIVE:**
✅ **SYSTEM BEKERJA DENGAN BENAR!**
- Pickup object dihapus (normal)
- Weapon di WeaponHolder aktif (correct)
- Bisa tembak dengan mouse click

**Jika Pistol GameObject TIDAK aktif:**
❌ **ADA MASALAH:**
- Check WeaponManager.weapons array berisi weapon references
- Check weapon name match (console: "Unlocked Pistol...")
- Check WeaponManager.UnlockWeapon() dipanggil

### Problem 2: "Tidak bisa tembak setelah pickup"

**Check List:**
```
1. ✅ Weapon GameObject aktif di WeaponHolder?
2. ✅ Console ada message "✅ Unlocked Pistol with 30 ammo"?
3. ✅ Weapon punya ammo? (check WeaponBase.currentAmmo)
4. ✅ Camera ada tag "MainCamera"?
5. ✅ WeaponManager.currentWeapon tidak null?
```

**Debug Command:**
```csharp
// Di WeaponManager.cs Update()
if (Input.GetKeyDown(KeyCode.P)) {
    Debug.Log($"Current Weapon: {currentWeapon?.WeaponName}");
    Debug.Log($"Ammo: {currentWeapon?.GetCurrentAmmo()}");
    Debug.Log($"Weapon Active: {currentWeapon?.gameObject.activeSelf}");
}
```

### Problem 3: "Weapon pickup tidak mau pickup"

**Check List:**
```
1. ✅ WeaponPickupItem.cs attached?
2. ✅ BoxCollider ada dan Is Trigger = true?
3. ✅ Rigidbody attached?
4. ✅ Layer bukan "Ignore Raycast"?
5. ✅ Player bisa interact dengan objek lain?
```

---

## 🎮 EXPECTED BEHAVIOR

### Normal Gameplay Flow:

```
1. Player melihat weapon pickup di dunia ✅
   └── Weapon_Pistol_Pickup visible

2. Player press E ✅
   └── Pickup object HILANG dari scene ✅ (NORMAL!)
   └── Console: "✅ Unlocked Pistol with 30 ammo"

3. Weapon aktif di tangan player ✅
   └── Hierarchy: Player → Camera → WeaponHolder → Pistol (ACTIVE)
   └── Bisa tembak dengan mouse click

4. Player pickup ammo ✅
   └── Console: "Collected 30 Universal ammo"
   └── Weapon ammo bertambah

5. Player switch weapon dengan scroll wheel ✅
   └── Weapon berganti di tangan
```

---

## 💡 IMPORTANT NOTES

### 1. Dua Objek Berbeda!

```
Weapon PICKUP (WeaponPickupItem.cs):
├── Di scene world
├── Player interact dengan ini
├── HARUS DIHAPUS setelah pickup ✅
└── Hanya trigger untuk unlock

Weapon PREFAB (WeaponBase.cs):
├── Di player WeaponHolder
├── Weapon yang sebenarnya ditembak
├── TETAP ADA selama game ✅
└── Di-enable/disable saat switch
```

### 2. Destroy Pickup = Normal Behavior!

```
destroyPickupAfterUnlock = true ✅ CORRECT!

Kenapa?
- Pickup adalah consumable item
- Tidak boleh pickup 2x dari objek yang sama
- Resource management (RAM)
- Standard game design pattern
```

### 3. Weapon Model Harus Ada 2x!

```
1. Pickup Model (di scene):
   └── Low poly, simple visual
   └── Bisa rotate/glow untuk visual feedback
   └── Dihapus setelah pickup

2. Weapon Model (di player):
   └── High quality, detailed
   └── Position/rotation tepat di tangan
   └── Weapon yang sebenarnya player gunakan
```

---

## 🚀 QUICK START CHECKLIST

Untuk setup weapon system dari awal:

- [ ] Import weapon models ke project
- [ ] Buat WeaponHolder di Player → Camera
- [ ] Tambah weapon prefabs ke WeaponHolder (Pistol, Rifle, Shotgun)
- [ ] Attach Pistol.cs, Rifle.cs, Shotgun.cs ke masing-masing weapon
- [ ] Set semua weapon INACTIVE by default
- [ ] Attach WeaponManager.cs ke Player
- [ ] Populate WeaponManager.weapons array
- [ ] Buat weapon pickup objects di scene
- [ ] Attach WeaponPickupItem.cs ke pickup objects
- [ ] Set Auto Unlock Weapon = TRUE
- [ ] Set Destroy Pickup After Unlock = TRUE
- [ ] Add BoxCollider (Trigger) + Rigidbody ke pickup
- [ ] Test pickup → Check weapon aktif di WeaponHolder
- [ ] Test shooting → Mouse click should fire
- [ ] Test ammo pickup → Ammo should increase
- [ ] Done! ✅

---

## 📞 FINAL ANSWER

### Q: "Senjatanya jadi hilang lagi dari hierarki saat saya tekan tombol interact"

### A: **INI BEHAVIOR YANG BENAR!** ✅

Yang hilang adalah **Weapon_Pickup object** (world item), **BUKAN** weapon yang player gunakan.

**Check ini:**
```
1. Play mode
2. Press E di weapon pickup
3. Weapon pickup di dunia HILANG ✅ (correct!)
4. Buka Hierarchy: Player → Camera → WeaponHolder
5. Lihat ada GameObject "Pistol" yang ACTIVE ✅ (correct!)
6. Try shoot dengan mouse click ✅ (should work!)
```

**Jika weapon tidak muncul di WeaponHolder:**
- Check WeaponManager.weapons array ada reference ke weapon prefabs
- Check console ada message "✅ Unlocked Pistol with 30 ammo"
- Check weapon GameObject ada script Pistol.cs/Rifle.cs/Shotgun.cs

**Jika masih bingung:**
Screenshot Hierarchy setelah pickup weapon, saya cek lagi! 📸

---

## 🎓 UNDERSTANDING THE SYSTEM

Think of it like this:

```
Weapon Pickup = 🎁 Gift Box
└── Once you open it, box is thrown away ✅
└── You get the actual item inside ✅

Weapon Prefab = 🔫 The Real Gun
└── Stays in your hands ✅
└── This is what you use to shoot ✅
```

**The pickup object MUST disappear** - that's how pickups work in every game! 🎮
