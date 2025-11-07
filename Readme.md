# VR boxing 

A VR shadow-boxing game built with Unity for Meta Quest 2 and other VR headsets.

![Unity Version](https://img.shields.io/badge/Unity-2022.3.7f1-blue)
![Platform](https://img.shields.io/badge/Platform-Meta%20Quest%202-brightgreen)

## Overview

VR_boxing is an immersive first-person VR boxing experience featuring real-time hand tracking, punch detection, and dynamic feedback systems. Train your reflexes and technique in a virtual boxing environment with responsive haptics and visual effects.

### Key Features

- **Immersive VR Boxing** - Full head and hand tracking with natural punch detection
- **Cross-Platform Support** - OpenXR compatibility for multiple VR headsets
- **Haptic Feedback** - Controller vibration on successful hits (not implemented yet)

## Prerequisites

### Software Requirements

- **Unity Editor**: 2022.3.7f1 LTS
  - Download from [Unity Hub](https://unity.com/download)
  
### Required Unity Packages

Install via **Window → Package Manager**:

| Package | Purpose |
|---------|---------|
| XR Plugin Management | Core VR functionality |
| OpenXR Plugin | Cross-platform VR support |
| XR Interaction Toolkit | Controller interaction handling |
| TextMeshPro | UI text rendering |
| Input System | Modern input handling (optional) |

### Platform-Specific Requirements

#### Meta Quest 2 (Android Build)
- Android SDK & NDK
- JDK (Java Development Kit)
- Oculus PC app or Oculus Integration package

> **Note**: Unity Hub can auto-install Android build tools via **Preferences → External Tools**

#### PC VR (Windows)
- SteamVR or Oculus PC runtime
- No additional SDK required

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/malakhbiyel/VR_boxing.git
cd VR_boxing
```

### 2. Open in Unity

1. Launch Unity Hub
2. Click **Add** → Select the cloned project folder
3. Open with Unity 2022.3 LTS

### 3. Configure XR Settings

#### Project Settings Setup

1. Navigate to **Edit → Project Settings**
2. **Player → Other Settings**:
   - Set **Active Input Handling** to "Both"
3. **XR Plugin Management**:
   - Enable OpenXR for your target platform (PC/Android)
4. **OpenXR**:
   - Enable controller profiles:
     - ✅ Oculus Touch Controller
     - ✅ Valve Index Controller
     - ✅ Windows Mixed Reality Controller (if applicable)

#### Android Build Configuration (Quest 2)

1. **File → Build Settings**:
   - Switch Platform to **Android**
2. **Player Settings → Other Settings**:
   - **Scripting Backend**: IL2CPP
   - **Target Architectures**: ARM64 only
3. **XR Plugin Management**:
   - ✅ Enable OpenXR for Android
   - ✅ Check "Initialize XR on Startup"

### 4. Test in Editor

1. Connect your VR headset
2. Open the main scene: `Assets/Scenes/MainGameplay`
3. Press **Play** ▶️
4. Start punching with your controllers!

## Building to Device

### Quest 2 Deployment

1. **Enable Developer Mode** on your Quest 2:
   - Install Meta Quest mobile app
   - Enable Developer Mode in headset settings

2. **Connect via USB-C**:
   - Connect Quest 2 to PC
   - Allow USB debugging on headset

3. **Build and Run**:
   ```
   File → Build Settings → Build and Run
   ```
   - Select Android platform
   - Wait for build and automatic installation


## Controls

| Action | Input |
|--------|-------|
| Look Around | Head tracking (automatic) |
| Punch | Move controllers forward rapidly |
| Trigger Haptics | Land successful hits |


## Contributing

We welcome contributions! Here's how:

1. **Fork** this repository
2. **Create a branch**: `git checkout -b feature/amazing-feature`
3. **Test** with your VR headset
4. **Commit changes**: `git commit -m 'Add amazing feature'`
5. **Push**: `git push origin feature/amazing-feature`
6. **Open a Pull Request** with:
   - Description of changes
   - Screenshots/video if applicable
   - Testing steps

**Ready to throw some punches?** 🥊 Clone the repo and start boxing in VR!
