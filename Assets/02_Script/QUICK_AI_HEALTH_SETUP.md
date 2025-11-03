# 🎯 Quick Setup: AIHunter + UniversalEnemyHealth

## ⚡ SUPER SIMPLE SETUP (5 Menit!)

### 1️⃣ **Add Components**
```
Enemy GameObject
├─ NavMeshAgent ✅ (already have)
├─ Animator ✅ (already have)
├─ Collider ✅ (already have)
├─ AIHunter ✅ (already have)
└─ UniversalEnemyHealth ⭐ (ADD THIS!)
```

### 2️⃣ **Configure UniversalEnemyHealth**
```
Inspector → UniversalEnemyHealth:

[Optional Components]
Scripts To Disable: 
  Element 0: AIHunter
  Element 1: NavMeshAgent

(That's the ONLY thing you need to set!)
```

### 3️⃣ **Done!** ✨

Enemy sekarang bisa:
- ✅ Patrol (AIHunter)
- ✅ Chase player (AIHunter)  
- ✅ Die when shot (UniversalEnemyHealth)
- ✅ Respawn (UniversalEnemyHealth)

---

## 🎬 Workflow Diagram

```
[GAME START]
     ↓
[Enemy Spawns]
     ↓
┌────────────────────────┐
│   AIHunter: ACTIVE     │ ← Enemy wander/patrol
│   Health: ACTIVE       │ ← Ready to receive damage
└────────────────────────┘
     ↓
[Player Detected] (AIHunter)
     ↓
[Enemy Chase Player] (AIHunter)
     ↓
[Player Shoots Enemy] (Weapon Script)
     ↓
┌────────────────────────┐
│ enemyHealth.TakeDamage │ ← Weapon calls this
└────────────────────────┘
     ↓
[DEATH SEQUENCE]
     ↓
┌────────────────────────┐
│ UniversalEnemyHealth:  │
│  1. Play death anim    │
│  2. Play death sound   │
│  3. Spawn death effect │
│  4. DISABLE AIHunter   │ ← AI stops!
│  5. DISABLE NavMesh    │ ← No pathfinding!
│  6. DISABLE Collider   │ ← No collision!
│  7. Hide enemy         │ ← Invisible
└────────────────────────┘
     ↓
[Wait 10 seconds...] (Respawn Time)
     ↓
[RESPAWN SEQUENCE]
     ↓
┌────────────────────────┐
│ UniversalEnemyHealth:  │
│  1. Reset position     │
│  2. ENABLE AIHunter    │ ← AI resume!
│  3. ENABLE NavMesh     │ ← Pathfinding resume!
│  4. ENABLE Collider    │ ← Collision resume!
│  5. Show enemy         │ ← Visible
│  6. Reset animator     │
│  7. Spawn respawn FX   │
└────────────────────────┘
     ↓
[Enemy Wander Again] (AIHunter)
     ↓
[Loop back to top] ↺
```

---

## 🔄 Component State Changes

### **Enemy ALIVE (Normal)**
```
✅ AIHunter         → Enabled  (wandering/chasing)
✅ NavMeshAgent     → Enabled  (pathfinding works)
✅ Collider         → Enabled  (can be hit)
✅ Renderer         → Enabled  (visible)
✅ UniversalHealth  → Enabled  (listening for damage)
```

### **Enemy DEAD (After shot)**
```
❌ AIHunter         → DISABLED (stopped)
❌ NavMeshAgent     → DISABLED (no pathfinding)
❌ Collider         → DISABLED (can't be hit)
❌ Renderer         → DISABLED (invisible)
✅ UniversalHealth  → Enabled  (managing respawn timer)
```

### **Enemy RESPAWN (After timer)**
```
✅ AIHunter         → ENABLED  (resume AI)
✅ NavMeshAgent     → ENABLED  (resume pathfinding)
✅ Collider         → ENABLED  (can be hit again)
✅ Renderer         → ENABLED  (visible again)
✅ UniversalHealth  → Enabled  (ready for next damage)
```

---

## 💾 Inspector Setup Screenshot Guide

```
═══════════════════════════════════════════
  ENEMY GAMEOBJECT INSPECTOR
═══════════════════════════════════════════

📦 Transform
   Position: (0, 0, 0)
   Rotation: (0, 0, 0)
   Scale: (1, 1, 1)

───────────────────────────────────────────

🎭 Animator
   Controller: EnemyAnimator
   ☑ Apply Root Motion
   Update Mode: Normal

───────────────────────────────────────────

🚶 Nav Mesh Agent
   Agent Type: Humanoid
   Speed: 3.5
   Angular Speed: 120
   Acceleration: 8
   Stopping Distance: 0.5

───────────────────────────────────────────

📦 Capsule Collider
   Center: (0, 1, 0)
   Radius: 0.5
   Height: 2
   ☑ Is Trigger: NO

───────────────────────────────────────────

🤖 AI Hunter (Script)
   
   [Wander Settings]
   Wander Points:
     Size: 4
     Element 0: WanderPoint1
     Element 1: WanderPoint2
     Element 2: WanderPoint3
     Element 3: WanderPoint4
   Wander Speed: 3.5
   Idle Wander Delay: 2
   
   [Detection Settings]
   Target Tag: "Target"
   Default Detection Radius: 10
   Chase Detection Radius: 15
   Chase Speed: 5
   Attack Distance: 1.5
   
   [Animation Settings]
   Move Parameter: "Move"
   Wander Blend Anim: "Wander"
   Chase Blend Anim: "Chase"

───────────────────────────────────────────

💚 Universal Enemy Health (Script) ⭐NEW!
   
   [Enemy Settings]
   ☑ Can Respawn
   Respawn Time: 10
   ☐ Destroy On Death
   
   [Death Effects]
   Death Effect Prefab: None
   Death Sound: None
   Death Effect Duration: 2
   
   [Respawn Settings]
   ☑ Respawn At Original Position
   Respawn Point: None
   Respawn Effect Prefab: None
   Respawn Sound: None
   
   [Optional Components]
   Enemy Animator: (auto-detected)
   Death Animation Trigger: "Die"
   
   Colliders To Disable:
     Size: 1
     Element 0: Capsule Collider
   
   Scripts To Disable: ⚠️ IMPORTANT!
     Size: 2
     Element 0: AIHunter          ← ADD THIS
     Element 1: NavMeshAgent      ← ADD THIS

═══════════════════════════════════════════
```

---

## 🎯 Testing Checklist

### **Pre-Flight Check:**
1. [ ] Open Unity scene dengan enemy
2. [ ] Select enemy GameObject
3. [ ] Verify AIHunter script attached
4. [ ] Add UniversalEnemyHealth script
5. [ ] Set "Scripts To Disable" (AIHunter + NavMeshAgent)
6. [ ] Save scene

### **Play Mode Test:**
1. [ ] Press Play
2. [ ] Enemy should wander (AIHunter works)
3. [ ] Get close to enemy
4. [ ] Enemy should chase you (AIHunter detection works)
5. [ ] Shoot enemy with weapon
6. [ ] Enemy should play death animation
7. [ ] Enemy should disappear
8. [ ] Wait 10 seconds
9. [ ] Enemy should respawn
10. [ ] Enemy should resume wandering

### **Success Criteria:**
- ✅ Enemy wanders before shot
- ✅ Enemy chases when detected
- ✅ Enemy dies when shot
- ✅ Enemy disappears after death
- ✅ Enemy respawns after timer
- ✅ Enemy resumes AI after respawn
- ✅ No errors in console

---

## 🚨 Common Issues & Quick Fixes

| Problem | Solution |
|---------|----------|
| Enemy won't die | Check weapon calls `TakeDamage()` |
| Enemy still moves after death | Add AIHunter to Scripts To Disable |
| Enemy won't respawn | Enable "Can Respawn" checkbox |
| Death animation not playing | Set animator & trigger name "Die" |
| Enemy frozen after respawn | Add NavMeshAgent to Scripts To Disable |
| Console error "NullReference" | Check all components assigned |

---

## 💡 Pro Tips

### **Tip #1: Test Without Respawn First**
```
Can Respawn: ☐ False
Destroy On Death: ✅ True
```
Test death mechanism dulu, baru enable respawn.

### **Tip #2: Use Short Respawn Time for Testing**
```
Respawn Time: 3 (instead of 10)
```
Lebih cepat untuk testing!

### **Tip #3: Add Debug Logs**
Di weapon script:
```csharp
if (enemyHealth != null)
{
    Debug.Log("Hit enemy: " + hit.collider.name);
    enemyHealth.TakeDamage(1f);
}
```

### **Tip #4: Visual Gizmos**
UniversalEnemyHealth sudah punya gizmo!
- Green sphere = respawn point (jika ada)
- Visible di Scene view saat select enemy

---

## 📞 Quick Reference Commands

### **From Weapon Script:**
```csharp
// Kill enemy
UniversalEnemyHealth health = enemy.GetComponent<UniversalEnemyHealth>();
health.TakeDamage(1f);
```

### **From Other Scripts:**
```csharp
// Force respawn now
health.ForceRespawn();

// Kill permanently
health.KillPermanently();

// Check if dead
if (health.IsDead) { }
```

### **From AIHunter:**
```csharp
// Distract enemy
AIHunter ai = enemy.GetComponent<AIHunter>();
ai.Distract(distractPosition);
```

---

## 🎮 Gameplay Flow

```
Player enters area
       ↓
Enemy detects player (AIHunter)
       ↓
Enemy chases player (AIHunter)
       ↓
Player shoots enemy (Weapon)
       ↓
Enemy takes damage (UniversalEnemyHealth)
       ↓
Enemy dies (UniversalEnemyHealth)
       ↓
AI stops (AIHunter disabled)
       ↓
Enemy hidden (Renderer disabled)
       ↓
Wait X seconds (Respawn timer)
       ↓
Enemy respawns (UniversalEnemyHealth)
       ↓
AI resumes (AIHunter enabled)
       ↓
Loop ↺
```

---

## ✨ Result

**You now have:**
- ✅ Fully functional AI enemy
- ✅ Health system that works
- ✅ Death animations
- ✅ Auto respawn
- ✅ Clean integration
- ✅ No conflicts
- ✅ Easy to maintain

**Perfect for horror game! 👻🎮**

---

**Ready to use!** 🚀  
Just follow the 3 steps at the top and you're done!
