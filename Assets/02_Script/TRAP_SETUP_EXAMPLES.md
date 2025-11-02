# 🎯 CONTOH SETUP SCENE - TRAP SYSTEM

## Contoh 1: Koridor dengan Tombak Dinding

### Setup:
```
Corridor
├── WallSpear_Left_01
│   ├── Spear_Model (Model 3D)
│   └── DamageCollider (Empty + Box Collider + AdvancedTrapSystem)
│
├── WallSpear_Right_01
│   └── ... (sama seperti di atas)
│
└── WallSpear_Left_02
    └── ... (sama seperti di atas)
```

### Konfigurasi AdvancedTrapSystem:
- Damage: 25
- Continuous: ☐
- Category: Sharp
- Knockback: ☑ (Force: 4)

---

## Contoh 2: Ruangan dengan Gergaji Berputar

### Setup:
```
SawTrapRoom
└── RotatingSaw_01
    ├── SawBlade_Model (Model + RotatingSawTrap)
    │   └── DamageZone (Empty + Sphere Collider + AdvancedTrapSystem)
    └── SawPivot (Pivot point untuk rotasi)
```

### Konfigurasi:

**RotatingSawTrap** (pada SawBlade_Model):
- Rotation Speed: 360
- Rotation Axis: Z
- Saw Sound: [Sound loop gergaji]

**AdvancedTrapSystem** (pada DamageZone):
- Damage: 30
- Continuous: ☑
- Damage Interval: 0.3
- Category: Saw
- Knockback: ☑ (Force: 6)

---

## Contoh 3: Lantai dengan Tombak Keluar Masuk + Trigger

### Setup:
```
FloorTrapArea
├── PressurePlate_01
│   └── PressurePlate_Visual (Cube + TrapTrigger)
│
└── FloorSpikes_01
    ├── Spike_01
    │   └── DamageCollider (Box Collider + AdvancedTrapSystem)
    ├── Spike_02
    │   └── ... (sama)
    └── Spike_03
        └── ... (sama)
```

### Konfigurasi:

**TrapTrigger** (pada PressurePlate_Visual):
- Trigger Layers: Player
- Single Use: ☐
- Activation Delay: 0.2
- Traps To Activate: [Drag Spike_01, Spike_02, Spike_03]
- Activation Method Name: "ExtendSpike"

**SpikeTrap** (pada Spike_01, 02, 03):
- Extend Distance: 1.5
- Extend Speed: 10
- Extended Duration: 2
- Auto Activate: ☐ (akan diaktifkan oleh trigger)
- Mode: Manual

**AdvancedTrapSystem** (pada DamageCollider):
- Damage: 35
- Continuous: ☐
- Category: Spike

---

## Contoh 4: Area Api dengan Warning System

### Setup:
```
FireHazardArea
├── FireParticles (Particle System)
├── FireLight (Point Light - Orange)
├── FireSound (Audio Source - Loop)
└── DamageZone (Box Collider + AdvancedTrapSystem)
```

### Konfigurasi AdvancedTrapSystem:
- Damage: 10
- Continuous: ☑
- Damage Interval: 0.5
- Category: Fire
- Hit Particle Effect: [Burn effect]
- Hit Sound: [Sizzle sound]
- Knockback: ☐

---

## Contoh 5: Trap Sequence dengan Multiple Triggers

### Setup:
```
TrapSequence
├── Trigger_01 (Pressure Plate)
│   └── TrapTrigger → activates Saw_01
│
├── Trigger_02 (Tripwire)
│   └── TrapTrigger → activates Spikes_01 & Spikes_02
│
├── Saw_01
│   └── RotatingSaw + AdvancedTrapSystem
│
├── Spikes_01
│   └── SpikeTrap (Auto: No, Manual trigger)
│
└── Spikes_02
    └── SpikeTrap (Auto: No, Manual trigger)
```

### Flow:
1. Player menginjak Trigger_01
2. Gergaji mulai berputar dan bergerak
3. Player menghindari gergaji
4. Player kena Trigger_02
5. Spike_01 dan Spike_02 keluar bersamaan

---

## Contoh 6: Pendulum Trap (Gergaji Ayunan)

### Setup:
```
PendulumTrap
└── PendulumPivot (Empty - titik ayunan)
    └── PendulumArm (Capsule - lengan pendulum)
        └── SawBlade (Model + RotatingSawTrap)
            └── DamageZone (Sphere Collider + AdvancedTrapSystem)
```

### Script tambahan untuk PendulumPivot:
```csharp
// Tambahkan script simple pendulum
using UnityEngine;

public class PendulumSwing : MonoBehaviour
{
    public float swingAngle = 45f;
    public float swingSpeed = 1f;
    
    void Update()
    {
        float angle = swingAngle * Mathf.Sin(Time.time * swingSpeed);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
```

---

## 🎨 Tips Visual Design

### Untuk Tombak:
- Tambahkan blood decal saat hit player
- Point light merah untuk dramatic effect
- Particle effect sparks saat keluar dari dinding

### Untuk Gergaji:
- Trail renderer untuk motion blur
- Sparks particle saat berputar
- Orange/red light untuk danger feel

### Untuk Spike:
- Dust particle saat keluar dari lantai
- Screen shake saat spike extended
- Warning sound 1 detik sebelum keluar

### Untuk Fire:
- Multiple particle systems (flame, smoke, embers)
- Dynamic light (flickering)
- Heat distortion shader (optional)
- Ambient sound loop

---

## ⚡ Performance Tips

1. **Use Object Pooling** untuk particle effects yang sering spawn
2. **Disable trap** yang jauh dari player menggunakan trigger zones
3. **Limit particle count** untuk mobile/low-end devices
4. **Use LOD** untuk model trap yang kompleks
5. **Combine meshes** untuk trap yang banyak dan statis

---

## 🎮 Gameplay Tips

1. **Telegraph** - Beri warning sebelum trap aktif
   - Visual cue (light berkedip)
   - Audio cue (clicking sound)
   - Ground marker

2. **Pacing** - Jangan spam trap
   - Beri ruang bernafas untuk player
   - Kombinasikan dengan safe zones

3. **Fairness** - Trap harus bisa dihindari
   - Timing yang konsisten
   - Visual yang jelas
   - Sound yang audible

4. **Variety** - Kombinasi berbagai tipe
   - Statis + Moving
   - Instant + Delayed
   - Single + Multiple

5. **Difficulty Curve**
   - Early game: Slow, telegraphed traps
   - Mid game: Faster, combination traps
   - Late game: Complex sequences, minimal warning

---

**Happy Trapping! 🎯**
