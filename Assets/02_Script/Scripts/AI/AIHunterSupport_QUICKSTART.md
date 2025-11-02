# ⚡ Quick Setup: AIHunterSupport (5 Menit!)

## 🎯 Step-by-Step Setup

### **1️⃣ Attach Script (30 detik)**
```
Select enemy GameObject → Add Component → AIHunterSupport
```

### **2️⃣ Setup Animator (2 menit)**
1. Buka Animator Controller enemy
2. Klik kanan → Create State → New State "Jumpscare"
3. Klik kanan pada "Parameters" → Create Trigger "Jumpscare"
4. Klik kanan "Any State" → Make Transition → Pilih "Jumpscare" state
5. Klik transition → Set condition: Jumpscare trigger
6. Assign animation clip jumpscare Anda ke state "Jumpscare"

### **3️⃣ Setup Inspector (1 menit)**
```
AIHunterSupport Component:

[Jumpscare Settings]
Zombie Animator: (auto-detect) ✅
Jumpscare Animation Trigger: "Jumpscare"
Releases To Disable: 5
Show Debug Logs: ✅

[Release Detection]
Release Key: Space
Use Mobile Button: ☐ (untuk PC/Console)
                   ✅ (untuk Mobile)
```

### **4️⃣ Test! (1 menit)**
1. Press Play
2. Biarkan enemy attack player
3. Jumpscare animation akan play
4. Tekan Space 5x untuk escape
5. Check Console untuk log

---

## 🎮 Expected Behavior

**What happens:**
```
1. Enemy chase player (AIHunter)
2. Enemy reach attack distance
3. 🎬 JUMPSCARE ANIMATION PLAY
4. Screen shows "PRESS SPACE! (0/5)"
5. Player press Space → "PRESS SPACE! (1/5)"
6. Player press Space → "PRESS SPACE! (2/5)"
7. Player press Space → "PRESS SPACE! (3/5)"
8. Player press Space → "PRESS SPACE! (4/5)"
9. Player press Space → "PRESS SPACE! (5/5)"
10. ✅ ESCAPED! Jumpscare ends
11. Enemy won't jumpscare again
```

**Console Log:**
```
[AIHunterSupport] Zombie - Triggered jumpscare animation
[AIHunterSupport] Zombie - Jumpscare started! Press Space 5x to escape
[AIHunterSupport] Zombie - Release pressed! (1/5)
[AIHunterSupport] Zombie - Release pressed! (2/5)
[AIHunterSupport] Zombie - Release pressed! (3/5)
[AIHunterSupport] Zombie - Release pressed! (4/5)
[AIHunterSupport] Zombie - Release pressed! (5/5)
[AIHunterSupport] Zombie - Jumpscare ended! All releases complete
```

---

## 📱 Mobile Setup (Extra 2 menit)

### **Buat UI Button:**
1. Canvas → Create → UI → Button
2. Rename: "JumpscareReleaseButton"
3. Text: "TAP TO ESCAPE!"
4. Position: Center screen

### **Connect Button:**
1. Select button
2. Inspector → Button (Script)
3. On Click() → + (add new)
4. Drag enemy GameObject ke slot
5. Function dropdown → `AIHunterSupport.OnReleaseButtonPressed`

### **Enable Mobile Mode:**
```
AIHunterSupport Inspector:
Use Mobile Button: ✅ True
```

### **Optional: Hide Button saat tidak jumpscare**
```csharp
// Simple script
void Update()
{
    jumpscareButton.SetActive(hunterSupport.IsJumpscareActive);
}
```

---

## 🎯 Common Issues (Quick Fix)

| Problem | Fix |
|---------|-----|
| Animation tidak play | Check trigger name = "Jumpscare" di Animator |
| Button tidak work | Check Release Key = Space, check Console |
| Jumpscare tidak start | Check AIHunter onAttackJumpscareEvent |
| Enemy jumpscare terus | Ini tidak akan terjadi, script prevent otomatis |

---

## 🔥 Pro Tips

### **Tip 1: Adjust Difficulty**
```
Easy mode: Releases To Disable = 3
Normal: Releases To Disable = 5
Hard: Releases To Disable = 10
```

### **Tip 2: Different Keys per Enemy**
```
Zombie A: Release Key = Space
Zombie B: Release Key = E
Boss: Release Key = F
```

### **Tip 3: Add Sound Effect**
Add event di Inspector:
```
On Release Pressed → AudioSource.PlayOneShot(buttonSound)
```

### **Tip 4: Disable Enemy After Escape**
Add event di Inspector:
```
On All Releases Complete → AIHunterSupport.DisableEnemy
```

### **Tip 5: Camera Shake on Start**
Add event di Inspector:
```
On Jumpscare Start → CameraShaker.Shake(intensity, duration)
```

---

## ✅ Checklist Testing

- [ ] Enemy chase player correctly
- [ ] Jumpscare animation plays on attack
- [ ] Screen prompt shows "PRESS SPACE! (0/5)"
- [ ] Pressing Space increments counter
- [ ] After 5 presses, jumpscare ends
- [ ] Console shows all debug logs
- [ ] Enemy doesn't jumpscare again
- [ ] (Mobile) Button appears during jumpscare
- [ ] (Mobile) Button tap increments counter

---

## 🚀 You're Done!

Script siap digunakan! Features:
- ✅ Auto-connect dengan AIHunter
- ✅ One-time jumpscare per enemy
- ✅ Configurable escape count
- ✅ PC & Mobile support
- ✅ Debug UI included
- ✅ Events for custom behavior

**Test sekarang dan enjoy jumpscare yang bisa di-escape!** 👻🎮
