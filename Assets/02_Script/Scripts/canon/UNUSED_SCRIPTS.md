# 🗑️ SCRIPT YANG TIDAK TERPAKAI

## ❌ Script Yang TIDAK Perlu (Bisa Dihapus)

### 1. **AmmoPickup.cs**
**Status:** ❌ TIDAK TERPAKAI (jika pakai AmmoPickupItem.cs)

**Alasan:**
- Ini versi ORIGINAL tanpa integrasi Item.cs
- Anda sudah pakai AmmoPickupItem.cs (yang support Item.cs)
- Duplikat fungsionalitas

**Kapan pakai:**
- ❌ JANGAN pakai jika project punya Item.cs
- ✅ Hanya pakai jika project TANPA Item.cs (standalone)

**Action:** 
```
Bisa DIHAPUS jika Anda pakai AmmoPickupItem.cs
```

---

### 2. **MobileAimAssist.cs**
**Status:** ⚠️ OPTIONAL (Tidak wajib)

**Fungsi:**
- Aim assist untuk mobile agar lebih mudah aim
- Auto-track target terdekat

**Kapan pakai:**
- ✅ Jika game di Android/iOS dan butuh aim assist
- ❌ Jika PC only atau tidak butuh aim assist

**Action:**
```
- SIMPAN jika butuh aim assist mobile
- HAPUS jika tidak pakai mobile atau tidak butuh
```

---

## ✅ Script Yang TERPAKAI (JANGAN Dihapus)

### Core Weapon System:

| Script | Fungsi | Attach Ke | Wajib? |
|--------|--------|-----------|---------|
| **WeaponBase.cs** | Base class semua weapon | ❌ Jangan attach | ✅ WAJIB |
| **Pistol.cs** | Weapon pistol di tangan | Pistol GameObject | ✅ WAJIB |
| **Rifle.cs** | Weapon rifle di tangan | Rifle GameObject | ✅ WAJIB |
| **Shotgun.cs** | Weapon shotgun di tangan | Shotgun GameObject | ✅ WAJIB |
| **WeaponManager.cs** | Manager weapon system | Player/WeaponHolder | ✅ WAJIB |

### Pickup System:

| Script | Fungsi | Attach Ke | Wajib? |
|--------|--------|-----------|---------|
| **WeaponPickupItem.cs** | Weapon pickup (dengan Item.cs) | Weapon pickup di world | ✅ WAJIB |
| **AmmoPickupItem.cs** | Ammo pickup (dengan Item.cs) | Ammo pickup di world | ✅ WAJIB |

### UI & Controls:

| Script | Fungsi | Attach Ke | Wajib? |
|--------|--------|-----------|---------|
| **WeaponUI.cs** | UI ammo/weapon info | WeaponUI Panel | ✅ Wajib untuk PC |
| **MobileWeaponControls.cs** | Mobile button controls | MobileWeaponUI Panel | ✅ Wajib untuk Mobile |
| **PlatformInputDetector.cs** | Auto-detect PC/Mobile | Scene root GameObject | ⚠️ Optional tapi recommended |

---

## 📊 SUMMARY CLEAN-UP

### File yang AMAN dihapus:

```
canon/
├── AmmoPickup.cs              ❌ HAPUS (duplikat)
├── MobileAimAssist.cs         ⚠️ HAPUS jika tidak pakai mobile
```

### File yang HARUS disimpan:

```
canon/
├── WeaponBase.cs              ✅ SIMPAN
├── Pistol.cs                  ✅ SIMPAN
├── Rifle.cs                   ✅ SIMPAN
├── Shotgun.cs                 ✅ SIMPAN
├── WeaponManager.cs           ✅ SIMPAN
├── WeaponPickupItem.cs        ✅ SIMPAN
├── AmmoPickupItem.cs          ✅ SIMPAN
├── WeaponUI.cs                ✅ SIMPAN
├── MobileWeaponControls.cs    ✅ SIMPAN (jika pakai mobile)
├── PlatformInputDetector.cs   ✅ SIMPAN
```

---

## 🎯 REKOMENDASI

### Untuk Project Anda (Punya Item.cs):

**DELETE:**
```
✅ AmmoPickup.cs - Tidak dipakai, pakai AmmoPickupItem.cs
```

**KEEP (jika PC only):**
```
✅ Semua script weapon core
✅ WeaponPickupItem.cs
✅ AmmoPickupItem.cs
✅ WeaponUI.cs
✅ PlatformInputDetector.cs
```

**KEEP (jika support Mobile):**
```
✅ Semua di atas +
✅ MobileWeaponControls.cs
✅ MobileAimAssist.cs (optional)
```

---

## 🔍 CARA CHECK SCRIPT TERPAKAI

### 1. Search di Project

Di Unity:
```
1. Right-click AmmoPickup.cs
2. Find References In Scene
3. Jika tidak ada hasil → AMAN dihapus
```

### 2. Check Dependencies

```
Script yang depend ke script lain:
- Pistol.cs → WeaponBase.cs (JANGAN hapus WeaponBase)
- WeaponPickupItem.cs → Item.cs (sistem Anda)
- AmmoPickupItem.cs → Item.cs (sistem Anda)
```

### 3. Check Usage Count

| Script | Usage Count | Safe to Delete? |
|--------|-------------|-----------------|
| WeaponBase.cs | 3 (Pistol, Rifle, Shotgun) | ❌ NO |
| AmmoPickup.cs | 0 (tidak dipakai) | ✅ YES |
| MobileAimAssist.cs | 0-1 (optional) | ⚠️ Depends |

---

## 🗂️ FOLDER STRUCTURE (Setelah Cleanup)

```
canon/
├── Core/
│   ├── WeaponBase.cs
│   ├── Pistol.cs
│   ├── Rifle.cs
│   ├── Shotgun.cs
│   └── WeaponManager.cs
│
├── Pickup/
│   ├── WeaponPickupItem.cs
│   └── AmmoPickupItem.cs
│
├── UI/
│   └── WeaponUI.cs
│
├── Mobile/
│   ├── MobileWeaponControls.cs
│   └── MobileAimAssist.cs (optional)
│
└── Utilities/
    └── PlatformInputDetector.cs
```

**Total Essential Scripts:** 9-10 files

---

## ✅ ACTION PLAN

### Step 1: Backup
```
Commit atau backup project sebelum delete
```

### Step 2: Delete Unused
```
Delete AmmoPickup.cs (safely)
```

### Step 3: Test
```
Play test semua fitur:
- Weapon pickup → Hold → Use
- Ammo pickup
- Shooting
- Switch weapon
- Mobile controls (jika ada)
```

### Step 4: Clean
```
Delete unused .meta files
Reimport script folder
```

---

**Setelah cleanup, project lebih bersih dan maintainable!** ✨
