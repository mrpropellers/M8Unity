# M8Unity
=======
**M8Unity** is a client implementation with Unity for the Dirtywave M8. It will render into Unity the screen and audio of any device running the M8 tracker firmware - either a Dirtywave M8 Tracker or a Teensy 4.1 running the M8 headless build.

See very small demo video here: https://x.com/leftoutdev/status/1755056758471774426

## Installation Instructions
- Plug your M8 or teensy 4.1 w/ headless into a USB port
- Clone this repo into your URP-compatible Project's Packages directory
- Drop the GameBoyRig prefab into your Scene and press Play
- If nothing broke, it should auto-discover your M8 and you should be ready to go!

### Dependencies
This package depends on Cinemachine, URP, and the New Input System. If you would like to use the Package without those dependencies, you will have to make those modifications yourself!


## Project Status

Done:
 - Character display
 - Blit to RenderTexture
 - New Input System support
 - Audio bridge (through Unity microphone pass-through)
 - Auto-discovery of correct port and audio interface

TODO:
 - Oscilloscope display
 - Correct font
 - Draw call optimization
 - Burst optimization

Nice-to-have:
 - MIDI bridge
