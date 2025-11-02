# 🎬 AIHunterSupport - Jumpscare Camera System Setup

## 📋 Cara Kerja Update

Saat **onAttackJumpscareEvent()** dipanggil dari AIHunter:

```
1. ✅ Jumpscare Camera → SetActive(true)
2. ✅ AIHunter script → enabled = false
3. ✅ Zombie Animator → Play("Jumpscare")
4. 👤 Player → Press Space 5x to escape
5. ✅ Jumpscare Camera → SetActive(false)
6. ✅ (Optional) AIHunter → enabled = true
```

---

## 🚀 Setup Step-by-Step

### **1️⃣ Setup Jumpscare Camera (2 menit)**

#### **Buat Camera Khusus Jumpscare:**
```
Hierarchy:
├─ Main Camera (existing)
└─ Zombie (enemy)
   └─ JumpscareCamera ← Buat ini (NEW)
      └─ Camera Component
```

**Steps:**
1. Select enemy GameObject (Zombie)
2. Right-click → Create Empty → Rename: "JumpscareCamera"
3. Add Component → Camera
4. Position camera di depan zombie face (close-up)
5. **IMPORTANT:** Set camera inactive by default
   - Inspector → Uncheck checkbox di atas (disable GameObject)
   - Atau via script: `jumpscareCamera.SetActive(false);`

**Example Position:**
```
JumpscareCamera Transform:
Position: (0, 1.5, 0.5)  ← Di depan wajah zombie
Rotation: (0, 180, 0)    ← Menghadap zombie
```

---

### **2️⃣ Setup AIHunterSupport (1 menit)**

Select enemy GameObject → Add Component → AIHunterSupport

**Inspector Settings:**
```
[Jumpscare Settings]
Zombie Animator: (auto-detect)
Jumpscare Animation Name: "Jumpscare"  ← State name di Animator
Jumpscare Camera: (Drag JumpscareCamera GameObject disini) ⭐ IMPORTANT
Releases To Disable: 5
Show Debug Logs: ✅

[Release Detection]
Release Key: Space
Use Mobile Button: ☐
```

---

### **3️⃣ Setup Animator (2 menit)**

**Buka Animator Controller zombie:**

1. **Buat State "Jumpscare":**
   - Klik kanan di Animator window → Create State → Empty
   - Rename state: "Jumpscare"
   - Assign animation clip jumpscare Anda

2. **Transition (Optional):**
   - Bisa buat transition dari Any State → Jumpscare
   - Atau biarkan kosong (script akan langsung Play state ini)

**Animator Structure:**
```
Animator Controller
├─ States:
│  ├─ Idle
│  ├─ Walk
│  ├─ Chase
│  └─ Jumpscare ← State ini yang akan di-Play()
```

**PENTING:** Nama state "Jumpscare" harus sama dengan field `Jumpscare Animation Name` di Inspector!

---

### **4️⃣ Test! (1 menit)**

1. Press Play
2. Biarkan zombie chase dan attack player
3. **Expected behavior:**
   ```
   ✅ Main Camera OFF
   ✅ Jumpscare Camera ON (close-up zombie face)
   ✅ AIHunter disabled (zombie berhenti chase)
   ✅ Animation "Jumpscare" plays
   ✅ Prompt "PRESS SPACE! (0/5)" muncul
   ```
4. Press Space 5x
5. **Expected result:**
   ```
   ✅ Jumpscare Camera OFF
   ✅ Main Camera ON (kembali normal)
   ✅ Zombie tidak bisa jumpscare lagi
   ```

---

## 🎮 Workflow Detail

### **Before Jumpscare:**
```
Main Camera: ✅ Active
Jumpscare Camera: ❌ Inactive
AIHunter: ✅ Enabled (zombie chase player)
Zombie State: Chase/Attack
```

### **During Jumpscare (After Attack Event):**
```
1. Script calls: jumpscareCamera.SetActive(true)
   → Main Camera: ❌ Inactive (karena ada 2 camera, yang terakhir active akan render)
   → Jumpscare Camera: ✅ Active

2. Script calls: aiHunter.enabled = false
   → Zombie stops chasing
   
3. Script calls: zombieAnimator.Play("Jumpscare")
   → Animation plays
   
4. Player sees:
   → Close-up zombie face
   → "PRESS SPACE! (0/5)"
```

### **After 5x Space Press:**
```
1. Script calls: jumpscareCamera.SetActive(false)
   → Main Camera: ✅ Active (kembali normal)
   → Jumpscare Camera: ❌ Inactive

2. AIHunter: ❌ Disabled (stays disabled - zombie won't chase anymore)
   
3. Jumpscare won't trigger again for this zombie
```

---

## 💡 Tips & Customization

### **Tip 1: Adjust Camera Position**
```csharp
// Saat setup di Unity Editor, adjust transform manual:
JumpscareCamera:
  Position: (0, 1.6, 0.3)  ← Lebih dekat = lebih menakutkan
  Rotation: (0, 180, 0)
  FOV: 90 (lebar) atau 60 (normal)
```

### **Tip 2: Multiple Camera Angles**
Buat beberapa kamera jumpscare, random pilih satu:
```csharp
[SerializeField] private GameObject[] jumpscareCameras;

void TriggerJumpscare()
{
    // Random pick camera
    int randomIndex = Random.Range(0, jumpscareCameras.Length);
    jumpscareCameras[randomIndex].SetActive(true);
}
```

### **Tip 3: Smooth Camera Transition**
Pakai Cinemachine atau DOTween untuk smooth transition:
```csharp
// Contoh dengan DOTween
mainCamera.GetComponent<Camera>().DOFieldOfView(90, 0.5f);
```

### **Tip 4: Re-enable AIHunter After Jumpscare**
Jika ingin zombie chase lagi setelah jumpscare:
```csharp
// Di method EndJumpscare(), uncomment baris ini:
if (aiHunter != null)
{
    aiHunter.enabled = true;
}
```

### **Tip 5: Disable Zombie Setelah Jumpscare**
Jika ingin zombie hilang setelah escape:
```csharp
// Di Inspector → Events → On All Releases Complete:
Add → AIHunterSupport.DisableEnemy
```

---

## 🎯 Camera Setup Examples

### **Example 1: Classic Horror (Close-up Face)**
```
JumpscareCamera:
  Position: (0, 1.6, 0.2)
  Rotation: (0, 180, 0)
  FOV: 90
  Clear Flags: Skybox
```

### **Example 2: Side Angle (Cinematic)**
```
JumpscareCamera:
  Position: (0.5, 1.5, 0.3)
  Rotation: (0, 160, 0)
  FOV: 70
```

### **Example 3: POV Attack (Player View)**
```
JumpscareCamera (di Player):
  Position: (0, 1.7, 0)
  Look at: Zombie face
  FOV: 100 (wide for panic effect)
```

---

## 🐛 Troubleshooting

### **Problem: Jumpscare camera tidak muncul**
```
✅ Check camera GameObject inactive di awal (sebelum Play)
✅ Check field "Jumpscare Camera" di Inspector ada isi
✅ Check Console log: "Jumpscare camera activated"
✅ Check Main Camera priority (default priority 0)
```

### **Problem: Kedua camera terlihat bersamaan**
```
✅ Pastikan hanya 1 camera active di satu waktu
✅ Set Main Camera priority = -1
✅ Set Jumpscare Camera priority = 0 (higher)
✅ Atau disable Main Camera saat jumpscare aktif
```

### **Problem: Animation tidak play**
```
✅ Check nama state di Animator = "Jumpscare"
✅ Check field "Jumpscare Animation Name" = "Jumpscare"
✅ Check animation clip assigned ke state
✅ Check Console log: "Playing jumpscare animation: Jumpscare"
```

### **Problem: AIHunter masih chase setelah jumpscare**
```
✅ Check Console log: "AIHunter disabled"
✅ Pastikan AIHunter component ada di GameObject yang sama
✅ Check di Inspector saat jumpscare: AIHunter enabled = false
```

---

## 🎬 Advanced: Multiple Jumpscare Cameras

Setup beberapa camera untuk variasi:

```
Zombie
├─ JumpscareCamera_Front (close-up)
├─ JumpscareCamera_Side (cinematic)
└─ JumpscareCamera_Top (overhead scare)
```

**Script modification:**
```csharp
[SerializeField] private GameObject[] jumpscareCameras;
[SerializeField] private bool randomizeCamera = true;

public void TriggerJumpscare()
{
    // ... existing code ...
    
    // Activate random camera
    if (jumpscareCameras.Length > 0)
    {
        int index = randomizeCamera ? Random.Range(0, jumpscareCameras.Length) : 0;
        jumpscareCameras[index].SetActive(true);
    }
}
```

---

## 📋 Checklist Setup

- [ ] JumpscareCamera created dan positioned
- [ ] JumpscareCamera inactive by default
- [ ] AIHunterSupport attached ke zombie
- [ ] Field "Jumpscare Camera" filled di Inspector
- [ ] Animator state "Jumpscare" created
- [ ] Animation clip assigned
- [ ] Field "Jumpscare Animation Name" = "Jumpscare"
- [ ] Releases To Disable = 5 (atau sesuai keinginan)
- [ ] Test di Play Mode
- [ ] Console log shows all steps
- [ ] Camera switches correctly

---

## ✅ Summary

**Files Modified:**
- ✅ `AIHunterSupport.cs` - Updated jumpscare logic

**New Behavior:**
1. ✅ Jumpscare camera activated saat attack
2. ✅ AIHunter disabled (zombie stops)
3. ✅ Animation plays via Animator.Play()
4. ✅ Player press Space 5x to escape
5. ✅ Camera deactivated setelah escape
6. ✅ One-time jumpscare per zombie

**Ready to use!** 🎬👻
