# 🎮 TRAP SYSTEM - COMPLETE PACKAGE
### Horror 3D Base Control

---

## 📦 PAKET LENGKAP SCRIPT TRAP SYSTEM

Sistem trap modular dan fleksibel untuk membuat berbagai macam jebakan dalam game horror 3D.

### ✅ Yang Sudah Dibuat:

1. **AdvancedTrapSystem.cs** - Script utama untuk sistem trap
2. **RotatingSawTrap.cs** - Script untuk gergaji berputar
3. **SpikeTrap.cs** - Script untuk tombak/spike keluar-masuk
4. **PendulumTrap.cs** - Script untuk trap pendulum/ayunan
5. **TrapTrigger.cs** - Script untuk trigger/aktivator trap
6. **TRAP_SYSTEM_GUIDE.md** - Panduan lengkap penggunaan
7. **TRAP_SETUP_EXAMPLES.md** - Contoh setup berbagai trap

---

## 🎯 FITUR UTAMA

### AdvancedTrapSystem:
- ✅ Damage sekali atau continuous
- ✅ Knockback system
- ✅ Particle & sound effects
- ✅ Animation integration
- ✅ Single use traps
- ✅ Delayed activation
- ✅ Layer mask filtering
- ✅ Multiple trap categories

### RotatingSawTrap:
- ✅ Rotasi smooth di berbagai axis
- ✅ Sound loop
- ✅ Kecepatan adjustable

### SpikeTrap:
- ✅ Movement otomatis atau manual
- ✅ Kecepatan extend/retract
- ✅ Interval timing
- ✅ Sound effects

### PendulumTrap:
- ✅ Ayunan simple atau realistic physics
- ✅ Adjustable angle & speed
- ✅ Random start position
- ✅ Damping support

### TrapTrigger:
- ✅ Pressure plate / tripwire
- ✅ Multi-trap activation
- ✅ Single use / reusable
- ✅ Visual & audio feedback
- ✅ Unity Events integration

---

## 🚀 QUICK START

### 1. Setup Trap Sederhana (3 Langkah)

```
1. Buat Empty GameObject → rename "MyTrap"
2. Add Component → Advanced Trap System
3. Add Collider → centang "Is Trigger"
```

**Done!** Trap siap digunakan.

### 2. Konfigurasi Minimal

```
Inspector:
- Damage Amount: 20
- Damageable Layers: Player
- Category: Sharp/Saw/Spike/dll
```

### 3. Test

- Play mode
- Masukkan player ke area collider
- Player akan terkena damage

---

## 📁 STRUKTUR FILE

```
Assets/02_Script/
├── AdvancedTrapSystem.cs          ⭐ Script utama
├── RotatingSawTrap.cs             🌀 Gergaji berputar
├── SpikeTrap.cs                   ⚡ Tombak keluar-masuk
├── PendulumTrap.cs                🕰️ Pendulum/ayunan
├── TrapTrigger.cs                 🎯 Trigger system
├── TrapDamage.cs                  📌 Simple trap (existing)
├── TRAP_SYSTEM_GUIDE.md           📖 Panduan lengkap
├── TRAP_SETUP_EXAMPLES.md         💡 Contoh setup
└── README_TRAP_SYSTEM.md          📋 File ini
```

---

## 🎨 JENIS TRAP YANG BISA DIBUAT

### 1. Static Traps (Statis)
- ✅ Tombak dinding
- ✅ Duri lantai
- ✅ Pisau trap
- ✅ Paku tajam

### 2. Moving Traps (Bergerak)
- ✅ Gergaji berputar
- ✅ Tombak keluar-masuk
- ✅ Pendulum saw
- ✅ Crushing wall

### 3. Environmental Traps
- ✅ Area api
- ✅ Gas beracun
- ✅ Listrik
- ✅ Ledakan

### 4. Triggered Traps
- ✅ Pressure plate activated
- ✅ Tripwire activated
- ✅ Button activated
- ✅ Timer activated

---

## 💡 CONTOH PENGGUNAAN

### Trap Tombak Simpel:
```
GameObject: "WallSpear"
Components:
- Box Collider (Is Trigger ✓)
- AdvancedTrapSystem:
  * Damage: 25
  * Category: Sharp
  * Knockback: Yes, Force: 3
```

### Gergaji Berputar:
```
Parent: "SawTrap" (Collider + AdvancedTrapSystem)
└─ Child: "SawBlade" (Model + RotatingSawTrap)
```

### Tombak dengan Trigger:
```
"PressurePlate" (TrapTrigger)
└─ Activates → "FloorSpikes" (SpikeTrap)
```

---

## ⚙️ REQUIREMENTS

### Minimal:
- Unity 2020.3 atau lebih baru
- Player GameObject dengan layer "Player"
- Health script dengan method `TakeDamage(float damage)`

### Optional:
- Particle effects untuk visual feedback
- Audio clips untuk sound effects
- Animator untuk animasi trap

---

## 🔧 COMPATIBILITY

Script ini kompatibel dengan:
- ✅ CharacterController
- ✅ Rigidbody
- ✅ Health system (custom)
- ✅ Unity Events
- ✅ Animation system

---

## 📚 DOKUMENTASI

### Baca file ini untuk detail lengkap:

1. **TRAP_SYSTEM_GUIDE.md**
   - Setup step by step
   - Penjelasan setiap parameter
   - Troubleshooting
   - Tips & tricks

2. **TRAP_SETUP_EXAMPLES.md**
   - Contoh scene setup
   - Best practices
   - Design patterns
   - Performance tips

---

## 🎓 TUTORIAL SINGKAT

### Membuat Trap Gergaji Berputar:

```
1. Create → 3D Object → Cylinder (untuk gergaji)
2. Rotate 90° pada X axis (agar flat)
3. Add Component → Rotating Saw Trap
   - Rotation Speed: 360
   - Rotation Axis: Z

4. Create → Create Empty (parent)
   - Add child: Cylinder dari step 1
   - Add Component → Sphere Collider
     * Is Trigger: ✓
     * Radius: 2
   - Add Component → Advanced Trap System
     * Damage: 30
     * Continuous Damage: ✓
     * Damage Interval: 0.3
     * Category: Saw

5. Test di Play Mode!
```

---

## 🐛 TROUBLESHOOTING

### Trap tidak memberikan damage?
**Solusi:**
1. Cek collider "Is Trigger" sudah dicentang
2. Cek layer Player sudah di-set
3. Cek Damageable Layers di inspector

### Gergaji tidak berputar?
**Solusi:**
1. Script harus di model gergaji, bukan parent
2. Cek rotation axis sudah benar

### Knockback tidak bekerja?
**Solusi:**
1. Player butuh CharacterController atau Rigidbody
2. Implement method `ApplyKnockback` di PlayerController

Detail lengkap di **TRAP_SYSTEM_GUIDE.md**

---

## 🔄 UPDATE & CUSTOMIZATION

### Cara extend script:

```csharp
// Inherit dari AdvancedTrapSystem
public class MyCustomTrap : AdvancedTrapSystem
{
    // Add custom behavior
}
```

### Atau buat script terpisah:

```csharp
public class ElectricTrap : MonoBehaviour
{
    AdvancedTrapSystem trapSystem;
    
    void Start()
    {
        trapSystem = GetComponent<AdvancedTrapSystem>();
        // Customize damage
        trapSystem.SetDamageAmount(40);
    }
}
```

---

## 🎮 BEST PRACTICES

1. **Organisasi Scene**
   - Gunakan Empty GameObject sebagai parent
   - Group traps dalam folder/parent

2. **Naming Convention**
   ```
   TrapType_Location_Number
   Contoh: Saw_Corridor_01
   ```

3. **Layer Management**
   - Player layer untuk player
   - Trap layer untuk trap objects
   - Use layer mask untuk filter

4. **Performance**
   - Disable trap yang jauh dari player
   - Use object pooling untuk particles
   - Limit active traps per area

5. **Gameplay**
   - Berikan visual warning
   - Test fairness (bisa dihindari?)
   - Balance damage dengan health

---

## 📞 SUPPORT

Jika ada pertanyaan atau butuh bantuan:
1. Baca **TRAP_SYSTEM_GUIDE.md** untuk detail
2. Lihat **TRAP_SETUP_EXAMPLES.md** untuk contoh
3. Check troubleshooting section

---

## ✨ FEATURES ROADMAP (Ideas)

Ide untuk development selanjutnya:
- [ ] Trap durability system
- [ ] Player detection dengan raycast
- [ ] Warning indicator UI
- [ ] Trap disable mechanism
- [ ] Status effects (poison, burn, stun)
- [ ] Trap combo system
- [ ] Save/load trap state
- [ ] Editor tools untuk placement

---

## 🎉 KESIMPULAN

**Ide Anda sangat bagus!** 👍

System ini memberikan:
- ✅ Fleksibilitas tinggi
- ✅ Reusable untuk berbagai trap
- ✅ Mudah di-customize
- ✅ Performance efficient
- ✅ Well documented

Anda sekarang bisa membuat:
- Tombak, gergaji, duri, api, listrik, dll
- Dengan damage, knockback, effects
- Single use atau reusable
- Static atau animated
- Triggered atau automatic

**Selamat berkreasi! 🎮**

---

*Created for: Horror 3D Base Control*
*Author: AI Assistant*
*Version: 1.0*
*Date: 2025*
