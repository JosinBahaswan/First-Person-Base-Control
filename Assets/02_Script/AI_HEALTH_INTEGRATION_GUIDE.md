# 🤝 Integrasi AIHunter dengan UniversalEnemyHealth

## ✅ GOOD NEWS: TIDAK ADA KONFLIK!

Script **AIHunter.cs** dan **UniversalEnemyHealth.cs** dapat bekerja bersama dengan **SEMPURNA**! 

Tidak ada konflik karena:
- ✅ AIHunter menghandle **behavior AI** (wander, chase, attack)
- ✅ UniversalEnemyHealth menghandle **health system** (damage, death, respawn)
- ✅ Keduanya **tidak overlap** functionality
- ✅ Bisa di-disable secara independent

---

## 🚀 Cara Setup (Super Mudah!)

### **Step 1: Attach Both Scripts ke Enemy GameObject**

1. Select enemy GameObject Anda
2. Add Component → `AIHunter` (sudah ada)
3. Add Component → `UniversalEnemyHealth` (script baru)

**That's it!** Kedua script sudah bisa bekerja bersama.

---

### **Step 2: Configure UniversalEnemyHealth Inspector**

```
[Enemy Settings]
✅ Can Respawn: True
⏱ Respawn Time: 10

[Optional Components]
📜 Scripts To Disable: 
   - AIHunter           ← PENTING! Tambahkan ini
   - NavMeshAgent       ← Dan ini juga
```

**Mengapa perlu disable AIHunter?**
- Agar AI berhenti chase/wander saat enemy mati
- NavMeshAgent juga perlu dimatikan agar tidak error
- Saat respawn, script akan auto-enable lagi

---

## 🎯 Setup Lengkap di Inspector

### **GameObject: Enemy (Zombie/Monster/etc)**

#### **Components:**
1. ✅ Transform
2. ✅ NavMeshAgent
3. ✅ Animator
4. ✅ Capsule Collider (atau collider lain)
5. ✅ **AIHunter** ← Script AI
6. ✅ **UniversalEnemyHealth** ← Script Health (BARU)

#### **AIHunter Settings:**
```
[Wander Settings]
🚶 Wander Points: (assign patrol points)
🏃 Wander Speed: 3.5
⏱ Idle Wander Delay: 2

[Detection Settings]
🎯 Target Tag: "Target" (player)
👁️ Default Detection Radius: 10
🔍 Chase Detection Radius: 15
🏃 Chase Speed: 5

[Attack Settings]
⚔️ Attack Distance: 1.5
🎬 On Attack Jumpscare Event: (setup event)

[Animation Settings]
🎭 Move Parameter: "Move"
🎨 Wander Blend Anim: "Wander"
🏃 Chase Blend Anim: "Chase"
```

#### **UniversalEnemyHealth Settings:**
```
[Enemy Settings]
✅ Can Respawn: True
⏱ Respawn Time: 10
☐ Destroy On Death: False

[Death Effects]
💥 Death Effect Prefab: (optional)
🔊 Death Sound: (optional)
⏱ Death Effect Duration: 2

[Respawn Settings]
✅ Respawn At Original Position: True
📍 Respawn Point: (leave empty untuk respawn di tempat awal)
✨ Respawn Effect Prefab: (optional)

[Optional Components]
🎭 Enemy Animator: (auto-detect)
🎬 Death Animation Trigger: "Die"
📦 Colliders To Disable: [Capsule Collider]
📜 Scripts To Disable: [AIHunter, NavMeshAgent]  ← PENTING!
```

---

## 🎮 Cara Kerja System

### **Saat Enemy Hidup (Normal State):**
```
AIHunter: ✅ ACTIVE
├─ Wander di patrol points
├─ Detect player
├─ Chase player
└─ Attack player

UniversalEnemyHealth: ✅ ACTIVE
└─ Menunggu damage dari weapon
```

### **Saat Player Menembak Enemy:**
```
1. Weapon memanggil: enemyHealth.TakeDamage(1f)
2. UniversalEnemyHealth:
   ├─ Trigger death animation ("Die")
   ├─ Play death sound & effect
   ├─ Disable AIHunter script          ← AI BERHENTI
   ├─ Disable NavMeshAgent            ← No more pathfinding
   ├─ Disable Collider                ← No more collision
   └─ Hide renderer (enemy invisible)
```

### **Saat Respawn (After 10 seconds):**
```
UniversalEnemyHealth:
├─ Reset position ke spawn point
├─ Enable AIHunter script           ← AI AKTIF KEMBALI
├─ Enable NavMeshAgent              ← Pathfinding works
├─ Enable Collider                  ← Collision works
├─ Show renderer (enemy visible)
├─ Reset animator
└─ Play respawn effect & sound

AIHunter:
└─ Continue wander/patrol (fresh start)
```

---

## 💡 Contoh Konfigurasi

### **Konfigurasi 1: Zombie yang Respawn**
```
Enemy GameObject: "Zombie"
├─ AIHunter
│  ├─ Wander Speed: 2.5
│  ├─ Chase Speed: 4.5
│  └─ Attack Distance: 1.5
│
└─ UniversalEnemyHealth
   ├─ Can Respawn: ✅ True
   ├─ Respawn Time: 15 seconds
   ├─ Scripts To Disable: [AIHunter, NavMeshAgent]
   └─ Death Animation Trigger: "Die"
```

**Behavior:**
- Zombie patrol area
- Chase player saat detected
- Mati saat ditembak dengan death animation
- Respawn setelah 15 detik di posisi awal
- Mulai patrol lagi

---

### **Konfigurasi 2: Monster Boss (No Respawn)**
```
Enemy GameObject: "Boss_Monster"
├─ AIHunter
│  ├─ Wander Speed: 3.0
│  ├─ Chase Speed: 6.0
│  ├─ Attack Distance: 2.0
│  └─ On Attack Event: (kill player)
│
└─ UniversalEnemyHealth
   ├─ Can Respawn: ☐ False
   ├─ Destroy On Death: ✅ True
   ├─ Scripts To Disable: [AIHunter, NavMeshAgent]
   ├─ Death Effect Prefab: BossExplosion
   └─ Death Effect Duration: 5
```

**Behavior:**
- Boss patrol dan chase player
- Mati permanent (no respawn)
- GameObject destroyed setelah 5 detik
- Epic explosion effect

---

### **Konfigurasi 3: Patrol Guard dengan Custom Respawn Point**
```
Enemy GameObject: "Guard"
├─ AIHunter
│  ├─ Wander Points: [Point1, Point2, Point3, Point4]
│  ├─ Randomize Wander Point: ✅ True
│  └─ Chase Speed: 5.5
│
└─ UniversalEnemyHealth
   ├─ Can Respawn: ✅ True
   ├─ Respawn Time: 8
   ├─ Respawn At Original Position: ☐ False
   ├─ Respawn Point: GuardSpawnRoom
   └─ Scripts To Disable: [AIHunter, NavMeshAgent]
```

**Behavior:**
- Guard patrol random points
- Mati saat ditembak
- Respawn di guard room (bukan tempat mati)
- Fresh patrol lagi dari spawn room

---

## 🔫 Weapon Integration

Script weapon sudah bekerja! Tambahkan ini di weapon script Anda:

```csharp
// Di method shooting (raycast hit detection)
if (hit.collider.CompareTag("Enemy"))
{
    // Coba get UniversalEnemyHealth
    UniversalEnemyHealth enemyHealth = hit.collider.GetComponent<UniversalEnemyHealth>();
    
    if (enemyHealth != null && !enemyHealth.IsDead)
    {
        enemyHealth.TakeDamage(1f);
        Debug.Log("Enemy killed!");
    }
}
```

---

## 🎭 Animation Setup

### **Animator Controller untuk Enemy:**

**Required Parameters:**
1. `Float: Move` (untuk walking animation)
2. `Trigger: Die` (untuk death animation)

**States:**
```
Idle → Wander (Move > 0)
Wander → Chase (AIHunter triggers "Chase" blend)
Any State → Death (Trigger "Die")
```

**Contoh Animator Structure:**
```
Animator Controller: EnemyAnimator
├─ Parameters:
│  ├─ Move (Float)
│  └─ Die (Trigger)
│
├─ Layers:
│  └─ Base Layer
│     ├─ Idle
│     ├─ Wander (blend tree)
│     ├─ Chase (blend tree)
│     └─ Death
│        └─ Transitions:
│           └─ Any State → Death (Condition: Die trigger)
```

---

## 🐛 Troubleshooting

### **Problem: Enemy masih bergerak setelah mati**
**Solution:**
```
✅ Check di UniversalEnemyHealth Inspector:
   Scripts To Disable harus berisi:
   - AIHunter
   - NavMeshAgent
```

---

### **Problem: Enemy tidak respawn**
**Solution:**
```
✅ Check settings:
   - Can Respawn: harus centang
   - Destroy On Death: harus TIDAK centang
   - Check console untuk error
```

---

### **Problem: Death animation tidak muncul**
**Solution:**
```
✅ Check Animator:
   - Enemy Animator di-assign
   - Death Animation Trigger name sesuai ("Die")
   - Animator Controller punya trigger "Die"
   - Death animation state ada
```

---

### **Problem: Enemy respawn tapi tidak bergerak**
**Solution:**
```
✅ Check NavMeshAgent:
   - Area masih punya NavMesh baked
   - Respawn position di atas NavMesh
   - NavMeshAgent tidak disabled permanent
```

---

### **Problem: AIHunter error setelah respawn**
**Solution:**
```
✅ Pastikan wander points masih valid
✅ Check animator tidak null
✅ Pastikan NavMeshAgent ter-enable kembali
```

---

## 📊 Performance Tips

### **Untuk Multiple Enemies:**

1. **Use Object Pooling** (Optional tapi recommended)
2. **Limit Detection Range** saat player jauh
3. **Disable AI Update** saat player sangat jauh
4. **Use LOD** untuk enemy model

### **Contoh Optimization:**

```csharp
// Tambahkan di AIHunter.cs (optional)
[Header("Performance")]
public float maxPlayerDistance = 50f;

private void Update()
{
    // Skip AI update jika player terlalu jauh
    float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
    if (distToPlayer > maxPlayerDistance)
    {
        return; // AI sleep
    }
    
    // ... existing AI code ...
}
```

---

## ✨ Advanced Tips

### **Tip 1: Multiple Hit Kill (Optional)**
Jika ingin enemy perlu beberapa tembakan:

```csharp
// Buat script baru: EnemyMultipleHits.cs
using UnityEngine;

public class EnemyMultipleHits : MonoBehaviour
{
    [SerializeField] private int hitsToKill = 3;
    [SerializeField] private UniversalEnemyHealth enemyHealth;
    private int currentHits = 0;
    
    public void TakeHit()
    {
        currentHits++;
        
        if (currentHits >= hitsToKill)
        {
            enemyHealth.Die();
            currentHits = 0; // Reset for respawn
        }
        else
        {
            // Visual feedback (blood, hit effect)
            Debug.Log($"Hit {currentHits}/{hitsToKill}");
        }
    }
}
```

---

### **Tip 2: Headshot Support**

```csharp
// Attach script ini ke "Head" child object di enemy
public class EnemyHeadshot : MonoBehaviour
{
    [SerializeField] private UniversalEnemyHealth enemyHealth;
    [SerializeField] private bool instantKillOnHeadshot = true;
    
    void Start()
    {
        if (enemyHealth == null)
            enemyHealth = GetComponentInParent<UniversalEnemyHealth>();
    }
    
    public void OnHeadshot()
    {
        if (instantKillOnHeadshot)
        {
            enemyHealth.Die();
            Debug.Log("HEADSHOT!");
        }
    }
}
```

---

### **Tip 3: Enemy Wave Spawner**

```csharp
// Untuk spawn multiple enemies dengan AIHunter + Health
public class EnemyWaveSpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int enemiesPerWave = 5;
    
    public void SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            int randomSpawn = Random.Range(0, spawnPoints.Length);
            GameObject enemy = Instantiate(enemyPrefab, 
                spawnPoints[randomSpawn].position, 
                spawnPoints[randomSpawn].rotation);
            
            // Both scripts will work automatically!
            // AIHunter will start wandering
            // UniversalEnemyHealth ready to receive damage
        }
    }
}
```

---

## 🎯 Quick Checklist

Sebelum test di Play Mode:

- [ ] AIHunter script attached dan configured
- [ ] UniversalEnemyHealth script attached
- [ ] Scripts To Disable berisi: AIHunter & NavMeshAgent
- [ ] Enemy punya tag "Enemy"
- [ ] Enemy punya Collider
- [ ] Enemy punya NavMeshAgent
- [ ] Animator configured dengan trigger "Die"
- [ ] Wander points di-assign (minimal 1)
- [ ] Weapon script call `TakeDamage()` method
- [ ] NavMesh sudah di-bake di scene

---

## 📝 Summary

### **AIHunter (AI Behavior):**
- ✅ Wander/Patrol system
- ✅ Player detection
- ✅ Chase behavior
- ✅ Attack event

### **UniversalEnemyHealth (Health System):**
- ✅ Take damage
- ✅ Death handling
- ✅ Auto respawn
- ✅ Component management

### **Together:**
- ✅ **NO CONFLICTS!**
- ✅ AI controls behavior
- ✅ Health controls life/death
- ✅ Perfect integration
- ✅ Easy to setup
- ✅ Flexible & scalable

---

## 🎮 Final Result

**Player Experience:**
1. Enemy patrol area (AIHunter)
2. Enemy detect dan chase player (AIHunter)
3. Player shoot enemy (Weapon → UniversalEnemyHealth)
4. Enemy play death animation dan hilang (UniversalEnemyHealth)
5. Enemy respawn setelah beberapa detik (UniversalEnemyHealth)
6. Enemy mulai patrol lagi (AIHunter auto-resume)

**Perfect Horror Game Enemy! 👻🎮**

---

**Created for Unity 3D Horror Game Project**  
**Scripts:** AIHunter.cs + UniversalEnemyHealth.cs  
**Compatibility:** ✅ 100% Compatible  
**Version:** 1.0  
**Date:** November 2025
