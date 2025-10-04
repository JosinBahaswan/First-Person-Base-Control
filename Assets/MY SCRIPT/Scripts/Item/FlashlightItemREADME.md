# Pickable Flashlight Setup Guide

This guide explains how to set up a flashlight that can be picked up and dropped like other items in your game.

## Components Overview

The pickable flashlight system consists of two main scripts:

1. **FlashlightItem.cs** - Core script that handles flashlight functionality and extends the base Item class
2. **FlashlightToggleButton.cs** - Optional script for a mobile-friendly toggle button

## Setup Instructions

### 1. Basic Flashlight Item Setup

1. Create a new GameObject for your flashlight.
2. Add an appropriate 3D model for your flashlight.
3. Add a **Rigidbody** component (required for all items).
4. Add a **Collider** component (Box, Capsule, or Mesh Collider depending on your model).
5. Add a **Spot Light** component.
6. Position the Spot Light at the front end of your flashlight model.
7. Adjust the Spot Light settings (Range, Spot Angle, Intensity) as desired.
8. Add the **FlashlightItem** script to the GameObject.

### 2. Configure FlashlightItem Component

1. Set the basic Item properties:

   - **Object Name**: Name of the flashlight (e.g., "Flashlight")
   - **Interact Message**: Message to display (e.g., "Pick up")
   - **Object Icon**: Assign a flashlight icon sprite
   - **Is Static**: Uncheck this (should be false for pickable items)

2. Configure hold position and rotation:

   - **Hold Position Offset**: Set the position offset when held (e.g., `0.2, -0.15, 0.3`)
   - **Hold Rotation Offset**: Set the rotation when held (e.g., `0, 180, 0`)

3. Configure Flashlight settings:
   - **Toggle Key**: Key to toggle flashlight on/off (default: F)
   - **Battery Life In Seconds**: Total battery life (e.g., 300 for 5 minutes)
   - **Is Using Power Drain**: Enable/disable battery drain
   - **Is Using Power State**: Enable/disable intensity changes based on battery level
   - **Battery Drain Rate**: How quickly battery drains per second
   - **Battery Thresholds**: Set medium and low thresholds
   - **Light Intensity Settings**: Configure high, medium, and low intensities

### 3. UI Setup

1. Create a new Canvas for your UI if you don't have one already.
2. Create a new UI panel for the flashlight UI.
3. Add the following elements to the panel:
   - **Image** for the battery icon
   - **Image** for the battery fill (this should be a filled image with fill method set to "Filled")
   - **TextMeshPro - Text** for displaying battery percentage
4. Assign these UI elements to the FlashlightItem component.

### 4. Mobile Controls Setup

1. Add a Button to your UI Canvas for toggling the flashlight.
2. Add the **FlashlightToggleButton** script to this button.
3. Position the button in an easily accessible area of the screen.
4. Assign a flashlight icon to the button for visual clarity.

## Pickup and Drop Functionality

The FlashlightItem inherits from the base Item class, which means it automatically works with your existing item interaction system:

1. When the player looks at the flashlight and presses the interact key (E), they will pick it up.
2. When holding the flashlight, the player can:
   - Press F to toggle the flashlight on/off
   - Press G to drop the flashlight
   - Press H to throw the flashlight

## Troubleshooting

- If the flashlight doesn't turn on, check that the Spot Light component is correctly configured.
- If the flashlight doesn't appear in the correct position when held, adjust the Hold Position Offset and Hold Rotation Offset values.
- For mobile controls, verify that the FlashlightToggleButton is correctly set up and assigned.

## Integration with Battery Items

The FlashlightItem has an AddBattery method that you can call from battery pickup items:

```csharp
// Example code for a battery item
private void OnTriggerEnter(Collider other)
{
    FlashlightItem flashlight = other.GetComponentInChildren<FlashlightItem>();

    if (flashlight != null)
    {
        flashlight.AddBattery(60f); // Add 60 seconds of battery life
        Destroy(gameObject);
    }
}
```
