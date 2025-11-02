# 🔧 JUMPSCARE BUTTON TIDAK BERFUNGSI - TROUBLESHOOTING GUIDE

## 🎯 Masalah Yang Dilaporkan
- Tombol tap tidak menghitung key press
- Player stuck di POV jumpscare animator camera setelah 15x press
- Camera tidak kembali normal setelah escape

---

## ✅ PERBAIKAN YANG DITAMBAHKAN

### 1. **Debug Logging Lengkap**
Semua input sekarang di-log untuk troubleshooting:
```
[EnemyJumpscareSystem] Input detected: Mouse Click
[EnemyJumpscareSystem] Key pressed: 1/15
[EnemyJumpscareSystem] OnMobileButtonPressed() called! isActive=true
```

### 2. **Animator Control Saat Escape**
Tambahan setting untuk stop/exit animation:
- `jumpscareExitTrigger` - Trigger untuk exit animation (optional)
- `stopAnimatorOnEscape` - Stop animator completely saat escape

### 3. **Debug Helper Script**
`JumpscareDebugHelper.cs` - Attach ke enemy yang sama:
- **F5** = Show debug state (semua setting dan status)
- **F6** = Simulate button press manual
- **On-screen GUI** = Real-time progress display

### 4. **Context Menu Debug**
Right-click EnemyJumpscareSystem di Inspector:
- "Test Button Press" - Test manual tanpa trigger jumpscare
- "Debug Jumpscare State" - Show semua variable state

---

## 🔍 CARA DEBUG

### Step 1: Setup Debug Helper
1. **Add Component** `JumpscareDebugHelper` ke enemy GameObject
2. **Play game** dan trigger jumpscare
3. **Lihat on-screen GUI** - harus muncul progress counter

### Step 2: Test Button Press
Saat jumpscare active:
1. **Tekan F6** untuk simulate button press
2. **Console harus show:**
   ```
   [JumpscareDebugHelper] F6 pressed - Simulating button press!
   [EnemyJumpscareSystem] OnMobileButtonPressed() called! isActive=true
   [EnemyJumpscareSystem] Key pressed: 1/15
   ```
3. **Jika F6 TIDAK WORK** = Script tidak receive input
4. **Jika F6 WORK tapi button UI tidak work** = Button setup salah

### Step 3: Check Button Setup
1. **Tekan F5** untuk debug state
2. **Console harus show:**
   ```
   === JUMPSCARE STATE DEBUG ===
   isJumpscareActive: true
   enableButtonMashing: true
   mobileEscapeButton: TapButton
   mobileEscapeButton active: true
   enableMobileButton: true
   ```

### Step 4: Verify Button Event
1. **Select Button** di Hierarchy
2. **Inspector → Button component → OnClick()**
3. **Harus ada entry:**
   - Object: Enemy GameObject
   - Function: `EnemyJumpscareSystem.OnMobileButtonPressed()`

---

## 🛠️ CHECKLIST SETUP

### Inspector Settings (EnemyJumpscareSystem)
- [ ] `Enable Jumpscare` = **TRUE**
- [ ] `Enable Button Mashing` = **TRUE**
- [ ] `Required Key Presses` = **15** (atau sesuai keinginan)
- [ ] `Enable Mobile Button` = **TRUE**
- [ ] `Allow Screen Tap` = **TRUE** (optional - tap anywhere)
- [ ] `Allow Mouse Click` = **TRUE** (optional - click anywhere)
- [ ] `Mobile Escape Button` = **Assign UI Button**
- [ ] `Use Custom Animation` = **TRUE** (karena pakai animator camera)
- [ ] `Enable Camera Animation` = **FALSE** (karena pakai animator, bukan script)

### Animation Control (untuk stop camera animator)
**Pilih salah satu:**

#### Option A: Exit Trigger (Recommended)
1. Buat trigger "JumpscareExit" di Animator Controller
2. Buat transition: Jumpscare → Idle dengan condition JumpscareExit
3. Set `Jumpscare Exit Trigger` = "JumpscareExit"
4. Set `Stop Animator On Escape` = **FALSE**

#### Option B: Stop Animator
1. Set `Stop Animator On Escape` = **TRUE**
2. Leave `Jumpscare Exit Trigger` = empty
3. Animator akan di-disable saat escape

#### Option C: Auto-Reset
1. Leave both empty/false
2. Script akan auto-reset trigger
3. Animator harus punya auto-transition kembali ke idle

### UI Button Setup
1. **Hierarchy:** Canvas → Button (TapButton)
2. **RectTransform:**
   - Anchor: Center
   - Size: 300x300 (atau fullscreen 1920x1080)
   - Position: Center screen
3. **Button Component:**
   - Interactable: **TRUE**
   - OnClick(): 
     - Runtime Only
     - Enemy GameObject
     - Function: `EnemyJumpscareSystem.OnMobileButtonPressed`
4. **Text:** "TAP!" atau icon
5. **Default:** Button akan hidden, muncul saat jumpscare

---

## 🐛 COMMON ISSUES

### Issue 1: Button Tidak Muncul Saat Jumpscare
**Symptom:** Jumpscare active tapi button tetap hidden

**Check:**
```
[EnemyJumpscareSystem] TriggerJumpscare() called
```
Harus diikuti log setup UI button.

**Fix:**
- Pastikan `enableMobileButton = true`
- Pastikan `mobileEscapeButton` assigned di Inspector
- Check parent Canvas active
- Check button tidak di-hide oleh script lain

### Issue 2: Button Press Tidak Terhitung
**Symptom:** Click button tapi counter tidak naik

**Check Console:**
```
[EnemyJumpscareSystem] OnMobileButtonPressed() called!
```

**Jika TIDAK ADA log:**
- Button OnClick() tidak setup
- Button tidak interactable
- EventSystem missing di scene

**Jika ADA log tapi tidak count:**
- Check `enableButtonMashing = true`
- Check `isJumpscareActive = true` (tekan F5)

### Issue 3: Camera Stuck Setelah Escape
**Symptom:** Counter sampai 15 tapi masih POV animator

**Root Cause:** Animator masih playing jumpscare animation

**Fix:**
1. **Set Exit Trigger:** Buat transition keluar dari jumpscare state
2. **Stop Animator:** Enable `stopAnimatorOnEscape = true`
3. **Check Animator Controller:** Pastikan ada transition EXIT dari Jumpscare state

### Issue 4: Cursor Hilang Setelah Escape
**Fix:** ✅ Sudah diperbaiki - script sekarang save & restore cursor state

---

## 📊 DEBUG OUTPUT EXAMPLE

### ✅ WORKING (Correct Output)
```
[EnemyJumpscareSystem] TriggerJumpscare() called. isActive=false, enabled=true
[EnemyJumpscareSystem] Jumpscare triggered by Zombie!
[EnemyJumpscareSystem] Input detected: Mouse Click
[EnemyJumpscareSystem] Key pressed: 1/15
[EnemyJumpscareSystem] Input detected: Mouse Click
[EnemyJumpscareSystem] Key pressed: 2/15
...
[EnemyJumpscareSystem] Key pressed: 15/15
[EnemyJumpscareSystem] Player escaped from jumpscare!
[EnemyJumpscareSystem] Triggered exit animation: JumpscareExit
```

### ❌ NOT WORKING (Problem Examples)

**Problem: Button tidak terhitung**
```
[EnemyJumpscareSystem] TriggerJumpscare() called
[EnemyJumpscareSystem] Jumpscare triggered by Zombie!
(tidak ada log "Input detected" atau "Key pressed")
```
→ **Fix:** Button OnClick() tidak setup atau enableButtonMashing = false

**Problem: Jumpscare tidak active**
```
[EnemyJumpscareSystem] OnMobileButtonPressed() called! isActive=false
[EnemyJumpscareSystem] Button pressed but jumpscare is NOT ACTIVE!
```
→ **Fix:** Jumpscare belum di-trigger atau sudah selesai

**Problem: Button mashing disabled**
```
[EnemyJumpscareSystem] OnMobileButtonPressed() called! isActive=true, enableMashing=false
[EnemyJumpscareSystem] Button pressed but button mashing is DISABLED!
```
→ **Fix:** Set `enableButtonMashing = true` di Inspector

---

## 🎮 TESTING PROCEDURE

### Test 1: Manual Trigger (No Enemy Required)
1. **Select Enemy** di Hierarchy
2. **Inspector → EnemyJumpscareSystem**
3. **Right-click component title → "On Attack Triggered"**
4. **Harus:** Jumpscare trigger, button muncul, bisa tap

### Test 2: Button Press Simulation
1. **Trigger jumpscare** (via test atau enemy attack)
2. **Tekan F6** repeatedly
3. **Harus:** Counter naik sampai 15, player escape

### Test 3: UI Button Click
1. **Trigger jumpscare**
2. **Click UI button** dengan mouse
3. **Harus:** Counter naik setiap click

### Test 4: Screen Tap (Mobile/Touch)
1. **Set** `allowScreenTap = true`
2. **Trigger jumpscare**
3. **Click anywhere** di screen
4. **Harus:** Counter naik setiap tap

### Test 5: Keyboard Press
1. **Trigger jumpscare**
2. **Press Space** (atau key yang di-set)
3. **Harus:** Counter naik setiap press

---

## 🔄 ANIMATOR SETUP GUIDE

### Untuk Setup Camera Jumpscare dengan Animator:

1. **Animator Controller:** Enemy Animator Controller
2. **States:**
   - **Idle/Default** (enemy normal state)
   - **Jumpscare** (camera animation state)
3. **Parameters:**
   - `Jumpscare` (Trigger)
   - `JumpscareExit` (Trigger) - optional
4. **Transitions:**
   - Any State → Jumpscare
     - Condition: Jumpscare trigger
     - Has Exit Time: FALSE
     - Duration: 0
   - Jumpscare → Idle
     - Condition: JumpscareExit trigger OR duration ends
     - Has Exit Time: TRUE (jika auto) atau FALSE (jika pakai trigger)
5. **Animation Clip:**
   - Animate: Camera position & rotation
   - Duration: sesuai kebutuhan (2-5 detik)

### Script Settings Untuk Animator Camera:
```
Use Custom Animation: TRUE
Enemy Animator: (assign Animator component)
Jumpscare Animation Trigger: "Jumpscare"
Jumpscare Exit Trigger: "JumpscareExit" (atau kosong)
Enable Camera Animation: FALSE (karena pakai animator)
Stop Animator On Escape: FALSE (jika pakai exit trigger) / TRUE (jika force stop)
```

---

## 📞 SUPPORT CHECKLIST

Jika masih tidak work, provide informasi berikut:

1. **Console Output** (semua log dari TriggerJumpscare sampai escape attempt)
2. **F5 Debug Output** (full state saat jumpscare active)
3. **Inspector Screenshot:**
   - EnemyJumpscareSystem settings
   - Button component OnClick() setup
4. **Animator Controller Screenshot:**
   - Parameters tab
   - Jumpscare state transitions
5. **Test Results:**
   - F6 work? (yes/no)
   - UI button work? (yes/no)
   - Screen tap work? (yes/no)
   - Keyboard work? (yes/no)

---

## 🎯 QUICK FIX SUMMARY

### Jika Button Tidak Count:
1. ✅ Pastikan button setup OnClick() ke `EnemyJumpscareSystem.OnMobileButtonPressed()`
2. ✅ `enableButtonMashing = true`
3. ✅ `enableMobileButton = true`
4. ✅ Attach `JumpscareDebugHelper` dan test dengan F6

### Jika Camera Stuck:
1. ✅ Set `jumpscareExitTrigger = "JumpscareExit"`
2. ✅ Buat transition keluar di Animator Controller
3. ✅ Atau enable `stopAnimatorOnEscape = true`

### Jika Cursor Hilang:
✅ Sudah fixed - script auto save & restore cursor state

---

**Last Updated:** 2024-11-02
**Version:** 2.0 - Full Debug Support
