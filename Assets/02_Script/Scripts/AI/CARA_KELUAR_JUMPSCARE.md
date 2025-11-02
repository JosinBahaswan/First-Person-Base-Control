# 🎮 CARA KELUAR DARI JUMPSCARE - Complete Guide

## 📋 Bagaimana Sistem Keluar dari Jumpscare Bekerja?

Sistem ini menggunakan **"Release Button System"** - Player harus menekan button (default: Space) sebanyak **5 kali** untuk keluar dari jumpscare.

---

## 🎯 SETUP LENGKAP (Step-by-Step)

### **STEP 1: Setup AIHunter Event (Yang Anda Sudah Punya)**

Di AIHunter, Anda setup event:

```
AIHunter Component → onAttackJumpscareEvent:
├─ Jumpscare Camera → SetActive(true)
├─ AIHunter → enabled = false  
└─ Zombie Animator → Play("Jumpscare")
```

**✅ Ini sudah otomatis ditangani oleh AIHunterSupport.TriggerJumpscare()**

---

### **STEP 2: Setup AIHunterSupport di Inspector**

Select enemy GameObject (Zombie) yang sudah ada AIHunter.

**Add Component → AIHunterSupport**

#### **Inspector Settings:**
```
╔══════════════════════════════════════════════╗
║ AIHunterSupport (Script)                    ║
╠══════════════════════════════════════════════╣
║ [Jumpscare Settings]                         ║
║ Zombie Animator: (auto atau drag Animator)  ║
║ Jumpscare Animation Name: "Jumpscare"       ║
║ Jumpscare Camera: (drag JumpscareCamera)    ║ ⭐ PENTING!
║ Releases To Disable: 5                      ║ ⭐ Jumlah press untuk escape
║ Show Debug Logs: ✅ True                     ║
║                                              ║
║ [Release Detection]                          ║
║ Release Key: Space                          ║ ⭐ Tombol untuk escape
║ Release Button Name: "Jump"                 ║
║ Use Mobile Button: ☐ False (PC/Console)     ║
║                     ✅ True (Mobile)         ║
╚══════════════════════════════════════════════╝
```

**YANG PALING PENTING:**
- ✅ **Release Key**: Tombol yang harus ditekan player (default: Space)
- ✅ **Releases To Disable**: Berapa kali harus ditekan (default: 5x)
- ✅ **Jumpscare Camera**: GameObject kamera jumpscare

---

### **STEP 3: Cara Kerja Keluar dari Jumpscare**

#### **🎬 Saat Jumpscare Dimulai:**

```
1. Enemy attack player
2. AIHunter → onAttackJumpscareEvent() triggered
3. AIHunterSupport.TriggerJumpscare() dipanggil
   ├─ Jumpscare Camera ON ✅
   ├─ AIHunter disabled ✅
   ├─ Animation "Jumpscare" play ✅
   └─ isJumpscareActive = true ✅
```

**Player sees:**
```
┌────────────────────────────────────┐
│                                    │
│    [ZOMBIE FACE CLOSE-UP]         │
│                                    │
│    PRESS SPACE! (0/5)             │ ← Prompt muncul
│                                    │
└────────────────────────────────────┘
```

#### **🎮 Cara Player Keluar:**

**Player harus menekan SPACE 5 kali:**

```
Press 1: Space → Counter: 1/5 → Log: "Release pressed! (1/5)"
Press 2: Space → Counter: 2/5 → Log: "Release pressed! (2/5)"
Press 3: Space → Counter: 3/5 → Log: "Release pressed! (3/5)"
Press 4: Space → Counter: 4/5 → Log: "Release pressed! (4/5)"
Press 5: Space → Counter: 5/5 → Log: "Release pressed! (5/5)"
                              → EndJumpscare() dipanggil! ✅
```

**Setelah 5x press:**
```
1. isJumpscareActive = false
2. Jumpscare Camera OFF ✅
3. Main Camera ON (kembali normal) ✅
4. Log: "Jumpscare ended! All releases complete"
```

**Player sees:**
```
┌────────────────────────────────────┐
│                                    │
│    [NORMAL GAMEPLAY VIEW]          │
│                                    │
│    Jumpscare ended!               │ ← Kembali normal
│                                    │
└────────────────────────────────────┘
```

---

## 🎮 TESTING - Cara Test Jumpscare

### **Test di Play Mode:**

1. **Press Play** di Unity
2. **Biarkan zombie chase player**
3. **Biarkan zombie sampai attack distance**
4. **Jumpscare akan trigger:**
   ```
   ✅ Kamera berubah ke jumpscare camera
   ✅ Zombie animation "Jumpscare" play
   ✅ Prompt "PRESS SPACE! (0/5)" muncul
   ✅ Console log: "Jumpscare started!"
   ```

5. **Tekan SPACE 5 kali:**
   ```
   Press 1 → "PRESS SPACE! (1/5)"
   Press 2 → "PRESS SPACE! (2/5)"
   Press 3 → "PRESS SPACE! (3/5)"
   Press 4 → "PRESS SPACE! (4/5)"
   Press 5 → "PRESS SPACE! (5/5)" → ESCAPED!
   ```

6. **Setelah 5x press:**
   ```
   ✅ Kamera kembali normal
   ✅ Jumpscare ended
   ✅ Zombie tidak bisa jumpscare lagi
   ```

---

## 💡 CUSTOMIZATION - Ubah Cara Keluar

### **Option 1: Ubah Jumlah Press (Mudah/Sulit)**

```
Inspector → AIHunterSupport:
Releases To Disable:
  3  = Easy (cuma 3x press)
  5  = Normal (5x press) ← Default
  10 = Hard (10x press!)
```

**Contoh Hard Mode:**
```csharp
Releases To Disable: 10
Release Key: Space
```
Player harus spam Space 10x untuk keluar!

---

### **Option 2: Ubah Tombol Escape**

```
Inspector → AIHunterSupport:
Release Key: E (atau tombol lain)
```

**Pilihan tombol:**
- `Space` (default - mudah dijangkau)
- `E` (interact key)
- `F` (alternative)
- `LeftShift` (sprint key)
- `Mouse0` (click kiri)

---

### **Option 3: Mobile - Pakai UI Button**

#### **Setup Mobile Button:**

**1. Buat UI Button:**
```
Hierarchy:
Canvas
└─ JumpscareEscapeButton (Button)
   ├─ Background (Image)
   └─ Text: "TAP TO ESCAPE!"
```

**2. Style Button:**
```
Button Inspector:
├─ Width: 200
├─ Height: 100
├─ Position: Center screen
├─ Color: Red/Orange (urgent feeling)
└─ Text: "TAP!" atau "ESCAPE!" (besar & bold)
```

**3. Connect Button ke Script:**
```
Select Button → Inspector:
Button (Script) → On Click():
  ├─ Click + (add new)
  ├─ Drag zombie GameObject ke slot
  └─ Function: AIHunterSupport.OnReleaseButtonPressed
```

**4. Enable Mobile Mode:**
```
Zombie → AIHunterSupport Inspector:
Use Mobile Button: ✅ True
```

**5. Optional: Hide/Show Button:**
```csharp
// Simple script untuk hide button saat tidak jumpscare
void Update()
{
    escapeButton.SetActive(hunterSupport.IsJumpscareActive);
}
```

---

### **Option 4: Multiple Button Combo (Advanced)**

Buat sistem combo button (Space + E + F):

```csharp
// Custom modification
[SerializeField] private KeyCode[] comboKeys = { KeyCode.Space, KeyCode.E, KeyCode.F };
private int currentComboIndex = 0;

void Update()
{
    if (isJumpscareActive)
    {
        if (Input.GetKeyDown(comboKeys[currentComboIndex]))
        {
            currentComboIndex++;
            if (currentComboIndex >= comboKeys.Length)
            {
                currentReleaseCount++;
                currentComboIndex = 0;
                // Check if escape complete
            }
        }
    }
}
```

---

## 🎨 VISUAL FEEDBACK - Tambah Effect Saat Press

### **Add Screen Flash per Press:**

Di Inspector → Events:
```
On Release Pressed:
  └─ ScreenFlash.Flash() (white flash)
```

### **Add Sound per Press:**

```
On Release Pressed:
  └─ AudioSource.PlayOneShot(buttonClickSound)
```

### **Add Camera Shake per Press:**

```
On Release Pressed:
  └─ CameraShaker.Shake(0.2f, 0.1f)
```

### **Example Complete Feedback:**

```
AIHunterSupport Inspector → Events:

[On Release Pressed] (setiap kali Space ditekan):
  ├─ ScreenFlash.Flash()
  ├─ AudioSource.PlayOneShot(clickSound)
  └─ CameraShaker.Shake(0.2f, 0.1f)

[On All Releases Complete] (setelah 5x):
  ├─ AudioSource.PlayOneShot(escapeSound)
  ├─ ScreenFlash.FlashGreen() (berhasil escape!)
  └─ AIHunterSupport.DisableEnemy()
```

---

## 🎯 SKENARIO LENGKAP (Full Gameplay Flow)

### **Scenario: Player vs Zombie Jumpscare**

```
[00:00] Player explore area
[00:05] Zombie detect player (AIHunter)
[00:10] Zombie chase player (AIHunter chase mode)
[00:15] Player cornered!
[00:18] Zombie reach attack distance

>>> JUMPSCARE TRIGGERED <<<

[00:18] ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        ║ JUMPSCARE CAMERA ON           ║
        ║ ZOMBIE FACE CLOSE-UP          ║
        ║ PRESS SPACE! (0/5)           ║
        ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

[00:19] Player: *panic* → SPACE!
        Counter: 1/5 ✅
        
[00:20] Player: SPACE!
        Counter: 2/5 ✅
        
[00:21] Player: SPACE!
        Counter: 3/5 ✅
        
[00:22] Player: SPACE!
        Counter: 4/5 ✅
        
[00:23] Player: SPACE!
        Counter: 5/5 ✅

>>> ESCAPED FROM JUMPSCARE <<<

[00:23] ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        ║ JUMPSCARE CAMERA OFF          ║
        ║ MAIN CAMERA ON                ║
        ║ ZOMBIE DISABLED               ║
        ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

[00:24] Player safe, continue gameplay
```

**Total Time in Jumpscare:** ~5 seconds (dengan quick press)

---

## 🐛 TROUBLESHOOTING

### **Problem: Tidak bisa keluar dari jumpscare**

**Check:**
```
✅ Console log muncul "Release pressed!" saat tekan Space?
✅ Counter di screen bertambah? (0/5 → 1/5 → 2/5...)
✅ Release Key di Inspector = Space?
✅ Releases To Disable = 5?
✅ isJumpscareActive = true? (check di Inspector saat jumpscare)
```

**Solution:**
```csharp
// Test manual di Inspector saat Play Mode:
1. Trigger jumpscare (biarkan zombie attack)
2. Check di Inspector: isJumpscareActive = true
3. Press Space 5x
4. Check Console untuk log
5. Check counter: currentReleaseCount harus naik
```

---

### **Problem: Button press tidak detect**

**Check:**
```
✅ Input System benar? (Legacy/New Input System)
✅ Release Key = Space di Inspector?
✅ useMobileButton = false? (jika pakai keyboard)
✅ Game window focus? (klik game window dulu)
```

**Quick Fix:**
```
1. Di Inspector, ubah Release Key ke tombol lain (E atau F)
2. Test dengan tombol baru
3. Check Console log
```

---

### **Problem: Counter tidak bertambah**

**Debug:**
```csharp
// Add debug di Update() untuk test:
void Update()
{
    if (isJumpscareActive)
    {
        Debug.Log($"Checking input... isActive: {isJumpscareActive}");
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space detected!");
            OnReleaseButtonPressed();
        }
    }
}
```

---

### **Problem: Prompt tidak muncul**

**Check:**
```
✅ Show Debug Logs = true di Inspector?
✅ OnGUI() method ada di script?
✅ Game View visible? (tidak minimize)
```

---

## 📊 STATISTICS & BALANCING

### **Recommended Settings per Difficulty:**

| Difficulty | Releases | Key | Time to Escape |
|-----------|----------|-----|----------------|
| Very Easy | 3 | Space | ~2 seconds |
| Easy | 5 | Space | ~3 seconds |
| Normal | 7 | E | ~4 seconds |
| Hard | 10 | F | ~6 seconds |
| Nightmare | 15 | Multiple | ~10 seconds |

### **Player Psychology:**

```
First press: "Oh no!"
Second press: "How many?!"
Third press: "Come on!"
Fourth press: "Almost!"
Fifth press: "YES! ESCAPED!"
```

Jumlah press yang tepat membuat tension perfect!

---

## ✅ CHECKLIST FINAL

Setup Complete Checklist:

- [ ] AIHunterSupport attached ke zombie
- [ ] Jumpscare Camera created & assigned
- [ ] Release Key set (default: Space)
- [ ] Releases To Disable set (default: 5)
- [ ] Show Debug Logs = true (untuk testing)
- [ ] Test di Play Mode
- [ ] Jumpscare trigger correctly
- [ ] Prompt "PRESS SPACE!" muncul
- [ ] Counter naik saat press Space
- [ ] Setelah 5x press, kembali normal
- [ ] Console log shows all steps
- [ ] Zombie tidak jumpscare lagi

---

## 🎮 YOU'RE DONE!

**Cara keluar dari jumpscare:**
1. ✅ Wait for jumpscare to trigger (zombie attack)
2. ✅ Press SPACE 5 times
3. ✅ Escaped!

**Simple as that!** 🎉👻

Player sekarang bisa escape dari jumpscare dengan menekan Space 5x!
