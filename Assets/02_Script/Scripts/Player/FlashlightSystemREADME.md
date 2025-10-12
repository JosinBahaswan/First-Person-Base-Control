# Flashlight System Setup Guide

This document explains how to set up the flashlight system in your horror game project for both PC and Android.

## Components Overview

The flashlight system consists of three main scripts:

1. **FlashlightController.cs** - Core script that handles flashlight functionality, battery drain, and intensity states
2. **FlashlightUI.cs** - Helper script for connecting UI elements to the flashlight controller
3. **BatteryItem.cs** - Script for battery pickup items that can recharge the flashlight

## Setup Instructions

### 1. Flashlight Setup (PC & Android)

1. Create a new empty GameObject as a child of your player's camera or attach it to your existing player GameObject.
2. Name it "Flashlight" for organization.
3. Add a **Spot Light** component to this GameObject.
4. Position and rotate the Spot Light to shine forward from the player's view.
5. Adjust the Spot Light settings (Range, Spot Angle, Intensity) as desired.
6. Add the **FlashlightController** script to the Flashlight GameObject.
7. Assign the Spot Light component to the "Spot Light" field in the inspector.

### 2. UI Setup

1. Create a new Canvas for your UI if you don't have one already.
2. Create a new UI panel for the flashlight UI.
3. Add the following elements to the panel:
   - **Image** for the battery icon
   - **Image** for the battery fill (this should be a filled image with fill method set to "Filled")
   - **TextMeshPro - Text** for displaying battery percentage
   - **Button** (for mobile controls, optional for PC)
4. Add the **FlashlightUI** script to the panel GameObject.
5. Assign all UI elements to their respective fields in the FlashlightUI component.
6. Assign your FlashlightController to the "Flashlight Controller" field.

### 3. Mobile Controls Setup (Android)

1. Make sure your Canvas has a Canvas Scaler component set to "Scale with Screen Size".
2. Position the flashlight button in an easily accessible area (usually bottom right corner).
3. Make the button large enough to be easily tapped on mobile devices.
4. Assign a flashlight icon to the button for visual clarity.
5. The FlashlightController automatically hooks up the button's onClick event.

### 4. Battery Item Setup (Optional)

1. Create a GameObject for your battery pickup item.
2. Add a suitable 3D model or sprite for visual representation.
3. Add a Collider component and check "Is Trigger".
4. Add the **BatteryItem** script to this GameObject.
5. Configure battery life amount and visual settings.
6. Assign the visual object for rotation (if desired).

## Configuration Options

### FlashlightController Settings

- **Toggle Key**: The key used to toggle the flashlight (default: F)
- **Battery Life**: Total battery life in seconds
- **Is Using Power Drain**: If true, the flashlight will drain battery when on
- **Is Using Power State**: If true, the flashlight intensity will change based on battery level
- **Battery Drain Rate**: How quickly the battery drains per second
- **Intensity Settings**: Separate intensity values for high, medium, and low battery states
- **Is Show UI**: Toggle to show/hide the UI

## Integration with Existing Systems

To integrate with your existing inventory or player controller:

1. Get a reference to the FlashlightController component.
2. Call `ToggleFlashlight()` method to turn the flashlight on/off.
3. Call `AddBattery(amount)` method to add battery life when the player collects batteries.

```csharp
// Example code to add to your player controller
FlashlightController flashlight;

void Start()
{
    flashlight = GetComponentInChildren<FlashlightController>();
}

// Call this when the player picks up a battery from inventory
public void UseBattery(float amount)
{
    if (flashlight != null)
    {
        flashlight.AddBattery(amount);
    }
}
```

## Troubleshooting

- If the flashlight doesn't turn on, check that the Spot Light component is properly assigned.
- If UI elements aren't updating, ensure all references are set in both FlashlightController and FlashlightUI.
- For mobile controls, verify the button's onClick event is properly wired up.

## Performance Considerations

- The spot light may have performance impact, especially on mobile devices. Consider adjusting quality settings or using a simpler light setup for low-end devices.
