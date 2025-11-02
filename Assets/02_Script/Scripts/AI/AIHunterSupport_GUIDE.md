# 🎮 AIHunterSupport.cs - Jumpscare Release System

## 📋 Deskripsi
Script **AIHunterSupport.cs** menangani sistem jumpscare yang bisa di-disable dengan menekan button release 5x (atau jumlah custom). Script ini bekerja otomatis dengan `AIHunter.cs` dan mencegah jumpscare terus-menerus.

---

## ✨ Fitur

- ✅ **Auto-trigger jumpscare** saat AIHunter attack event
- ✅ **Release system** - butuh 5x button press untuk escape
- ✅ **Customizable release count** - bisa diubah di Inspector
- ✅ **Multiple input support** - keyboard, gamepad, mobile button
- ✅ **Animator integration** - auto-play jumpscare animation
- ✅ **One-time trigger** - jumpscare hanya sekali per enemy
- ✅ **Unity Events** - hook custom behavior
- ✅ **Debug UI** - tampilan on-screen untuk testing

---

## 🚀 Cara Setup

### **Step 1: Attach Script ke Enemy**

1. Select enemy GameObject yang sudah punya `AIHunter`
2. Add Component → `AIHunterSupport`
3. Script akan otomatis connect dengan AIHunter

**Structure:**
```
Enemy GameObject
├─ NavMeshAgent
├─ Animator
├─ AIHunter ✅ (existing)
├─ AIHunterSupport ⭐ (NEW)
└─ UniversalEnemyHealth (optional)
```

---

### **Step 2: Setup di Inspector**

#### **Jumpscare Settings**
```
Zombie Animator: (auto-detect atau assign manual)
Jumpscare Animation Trigger: "Jumpscare"
Releases To Disable: 5
Show Debug Logs: ✅ (untuk testing)
```

#### **Release Detection**
```
Release Key: Space
Release Button Name: "Jump" (Input Manager)
Use Mobile Button: ☐ (centang jika pakai UI button)
```

#### **Events (Optional)**
```
On Jumpscare Start: (custom event saat jumpscare mulai)
On Jumpscare End: (custom event saat jumpscare selesai)
On Release Pressed: (setiap kali button ditekan)
On All Releases Complete: (setelah 5x release)
```

---

### **Step 3: Setup Animator**

1. **Buka Animator Controller** enemy Anda
2. **Tambahkan Parameter:**
   - Name: (tidak perlu parameter, cukup trigger)
3. **Buat State "Jumpscare":**
   - Add new state: "Jumpscare"
   - Add transition: `Any State → Jumpscare`
   - Condition: Trigger "Jumpscare"
4. **Add Jumpscare Animation:**
   - Assign animation clip ke state "Jumpscare"

**Contoh Animator Structure:**
```
Animator Controller: ZombieAnimator
├─ Parameters:
│  ├─ Move (Float)
│  ├─ Die (Trigger)
│  └─ Jumpscare (Trigger) ← ADD THIS
│
└─ States:
   ├─ Idle
   ├─ Wander
   ├─ Chase
   ├─ Death
   └─ Jumpscare ← ADD THIS
      └─ Transition: Any State → Jumpscare (Condition: Jumpscare trigger)
```

---

### **Step 4: Setup Input (Optional untuk Mobile)**

#### **A. Keyboard/Gamepad (Default):**
Tidak perlu setup tambahan, sudah pakai `KeyCode.Space`

#### **B. Mobile UI Button:**

1. **Buat UI Button** di Canvas:
   ```
   Canvas
   └─ JumpscareReleaseButton
      ├─ Image (background)
      └─ Text "TAP TO ESCAPE!"
   ```

2. **Setup Button OnClick:**
   - Select button
   - Inspector → Button → On Click()
   - Add new entry
   - Drag enemy GameObject ke slot
   - Function: `AIHunterSupport.OnReleaseButtonPressed()`

3. **Set di Inspector:**
   ```
   Use Mobile Button: ✅ True
   ```

---

## 🎮 Cara Kerja

### **Flow Diagram:**

```
[Player detected by AIHunter]
         ↓
[Player too close - Attack distance reached]
         ↓
[AIHunter triggers onAttackJumpscareEvent]
         ↓
[AIHunterSupport.TriggerJumpscare() called]
         ↓
┌─────────────────────────────────────┐
│ JUMPSCARE ACTIVE                    │
│ - Play jumpscare animation          │
│ - Show "PRESS SPACE!" prompt        │
│ - Start counting releases           │
│ - Invoke onJumpscareStart event     │
└─────────────────────────────────────┘
         ↓
[Player press release button (Space)]
         ↓
┌─────────────────────────────────────┐
│ Release Count: 1/5                  │
│ - Invoke onReleasePressed event     │
└─────────────────────────────────────┘
         ↓
[Press again...] → Count: 2/5
         ↓
[Press again...] → Count: 3/5
         ↓
[Press again...] → Count: 4/5
         ↓
[Press again...] → Count: 5/5 ✅
         ↓
┌─────────────────────────────────────┐
│ JUMPSCARE ENDED                     │
│ - Reset animator                    │
│ - Invoke onJumpscareEnd event       │
│ - Invoke onAllReleasesComplete      │
│ - (Optional) Disable enemy          │
└─────────────────────────────────────┘
         ↓
[Enemy tidak bisa jumpscare lagi]
```

---

## 💡 Contoh Konfigurasi

### **Konfigurasi 1: Easy Mode (3 releases)**
```
Releases To Disable: 3
Release Key: Space
```
Player cukup tekan Space 3x untuk escape.

---

### **Konfigurasi 2: Normal Mode (5 releases)**
```
Releases To Disable: 5
Release Key: Space
```
Player harus tekan Space 5x untuk escape (default).

---

### **Konfigurasi 3: Hard Mode (10 releases + faster input)**
```
Releases To Disable: 10
Release Key: E
```
Player harus tekan E 10x untuk escape.

---

### **Konfigurasi 4: Mobile Game**
```
Releases To Disable: 5
Use Mobile Button: ✅ True
```
Player tap UI button 5x untuk escape.

---

### **Konfigurasi 5: With Disable Enemy After**

Setup Event:
```
On All Releases Complete:
  ├─ AIHunterSupport.DisableEnemy
  └─ (custom script method)
```

Enemy akan di-disable setelah jumpscare selesai.

---

## 🔧 Custom Events Usage

### **Event 1: Jumpscare Start**
Gunakan untuk:
- Shake camera
- Blur screen
- Play jumpscare sound
- Stop player movement

**Example:**
```csharp
public class JumpscareEffects : MonoBehaviour
{
    public void OnJumpscareStart()
    {
        // Camera shake
        CameraShaker.instance.Shake(2f, 0.5f);
        
        // Play sound
        AudioSource.PlayClipAtPoint(jumpscareSound, transform.position);
        
        // Freeze player
        PlayerMovement.instance.FreezeMovement(true);
    }
}
```

---

### **Event 2: Release Pressed**
Gunakan untuk:
- Visual feedback (flash screen)
- Sound effect per press
- Particle effect

**Example:**
```csharp
public void OnReleasePress()
{
    // Flash white
    screenFlash.Flash();
    
    // Play click sound
    audioSource.PlayOneShot(clickSound);
}
```

---

### **Event 3: All Releases Complete**
Gunakan untuk:
- Reward player
- Achievement unlock
- Disable enemy
- Spawn item

**Example:**
```csharp
public void OnEscapeSuccess()
{
    // Give player health
    PlayerHealth.instance.Heal(20);
    
    // Achievement
    AchievementManager.Unlock("ESCAPED_JUMPSCARE");
    
    // Disable enemy
    aiHunterSupport.DisableEnemy();
}
```

---

## 📱 Mobile Button Setup Example

### **UI Hierarchy:**
```
Canvas
├─ JumpscarePanel (Image - semi-transparent red)
│  ├─ WarningText ("ZOMBIE ATTACK!")
│  ├─ InstructionText ("TAP BUTTON TO ESCAPE!")
│  ├─ CounterText ("0/5")
│  └─ ReleaseButton (Button)
│     ├─ Image (button background)
│     └─ Text ("TAP!")
```

### **Script untuk Update Counter:**
```csharp
using UnityEngine;
using UnityEngine.UI;

public class JumpscareUI : MonoBehaviour
{
    [SerializeField] private Text counterText;
    [SerializeField] private GameObject jumpscarePanel;
    [SerializeField] private AIHunterSupport hunterSupport;
    
    void Update()
    {
        if (hunterSupport.IsJumpscareActive)
        {
            jumpscarePanel.SetActive(true);
            counterText.text = $"{hunterSupport.CurrentReleaseCount}/{hunterSupport.ReleasesRemaining + hunterSupport.CurrentReleaseCount}";
        }
        else
        {
            jumpscarePanel.SetActive(false);
        }
    }
}
```

---

## 🎯 Public Methods

### **Call dari Script Lain:**

```csharp
AIHunterSupport support = enemy.GetComponent<AIHunterSupport>();

// 1. Trigger jumpscare manual (jika tidak pakai AIHunter event)
support.TriggerJumpscare();

// 2. Simulate button press (untuk testing atau custom input)
support.OnReleaseButtonPressed();

// 3. Force end jumpscare
support.ForceEndJumpscare();

// 4. Reset state (untuk respawn enemy)
support.ResetJumpscareState();

// 5. Disable enemy setelah jumpscare
support.DisableEnemy();

// 6. Check status
if (support.IsJumpscareActive)
{
    Debug.Log($"Releases remaining: {support.ReleasesRemaining}");
}

if (support.HasTriggered)
{
    Debug.Log("Jumpscare already happened");
}
```

---

## 🐛 Troubleshooting

### **Problem: Jumpscare animation tidak muncul**
**Solution:**
```
✅ Check Animator assigned di Inspector
✅ Check trigger name "Jumpscare" ada di Animator Controller
✅ Check animation clip assigned ke state "Jumpscare"
✅ Check transition dari Any State → Jumpscare
```

---

### **Problem: Button release tidak detect**
**Solution:**
```
✅ Check Release Key benar (default Space)
✅ Jika pakai Input Manager, check "Jump" button exists
✅ Jika mobile, check Use Mobile Button = true
✅ Check Console untuk log "Release pressed!"
```

---

### **Problem: Jumpscare trigger terus-menerus**
**Solution:**
```
✅ AIHunterSupport sudah prevent ini otomatis
✅ Jumpscare hanya trigger 1x per enemy
✅ Untuk reset, panggil ResetJumpscareState()
```

---

### **Problem: AIHunter event tidak connect**
**Solution:**
```
✅ Pastikan AIHunter dan AIHunterSupport di GameObject yang sama
✅ Check Console untuk error di Start()
✅ Manually test: panggil TriggerJumpscare() dari Inspector
```

---

## 🎨 Advanced: Multiple Jumpscare Phases

Buat jumpscare dengan phase bertingkat:

```csharp
public class MultiPhaseJumpscare : MonoBehaviour
{
    [SerializeField] private AIHunterSupport support;
    [SerializeField] private int phase = 0;
    
    void Start()
    {
        support.onReleasePressed.AddListener(OnPhaseProgress);
    }
    
    void OnPhaseProgress()
    {
        phase = support.CurrentReleaseCount;
        
        switch(phase)
        {
            case 2:
                // Intensify screen shake
                CameraShaker.instance.Shake(3f, 0.3f);
                break;
            case 4:
                // Spawn more enemies
                SpawnBackupZombies();
                break;
            case 5:
                // Victory effect
                PlayEscapeEffect();
                break;
        }
    }
}
```

---

## 📊 Performance Tips

1. **Disable Debug Logs** di production:
   ```
   Show Debug Logs: ☐ False
   ```

2. **Disable OnGUI** jika tidak butuh debug prompt:
   ```csharp
   // Comment out OnGUI() method di script
   ```

3. **Cache references** di Awake untuk performa lebih baik (sudah ada di script)

---

## 🎮 Integration dengan Sistem Lain

### **Dengan UniversalEnemyHealth:**
```csharp
// Saat enemy mati, reset jumpscare state
UniversalEnemyHealth health = GetComponent<UniversalEnemyHealth>();
AIHunterSupport support = GetComponent<AIHunterSupport>();

// Di respawn event
support.ResetJumpscareState();
```

### **Dengan Game Manager:**
```csharp
// Track jumpscare escapes
GameManager.instance.OnJumpscareEscaped();

// Add di AIHunterSupport events:
On All Releases Complete → GameManager.OnJumpscareEscaped
```

---

## 📝 Summary

**AIHunterSupport Features:**
- ✅ Auto-connect dengan AIHunter
- ✅ Jumpscare animation control
- ✅ Configurable release count
- ✅ Multiple input methods
- ✅ Unity Events support
- ✅ One-time trigger per enemy
- ✅ Debug visualization
- ✅ Mobile ready

**Perfect for:**
- Horror games dengan jumpscare mechanic
- QTE (Quick Time Event) system
- Boss encounter escape sequences
- Stealth game detection escape

---

**Ready to use!** 🎮👻
Attach ke enemy, setup animator, dan test di Play Mode!
