# Hollow Knight Like Mobile 2D Platformer (Starter)

Base project for a Hollow Knight inspired 2D platformer focused on Android and iOS targets. The project is configured with Unity 2022.3 LTS using the 2D URP template and includes an MVP character controller, a sample test level, mobile touch controls, and a fully scripted gameplay camera.

## Requirements

- **Unity 2022.3.16f1 LTS** (or newer 2022.3 LTS patch).
- Unity modules: **Android Build Support** (Android SDK & NDK Tools, OpenJDK) and **iOS Build Support**.
- Git LFS (for binary asset handling).

## Project structure

```
Assets/
  Art/                # Placeholder art and shared textures
  Audio/              # Reserved for SFX and music
  Physics/            # Physics materials for ground and walls
  Prefabs/            # Reusable gameplay prefabs (player, hazards, tiles)
  Scenes/             # Game scenes (TestLevel)
  Scripts/            # Gameplay, UI, and utilities
  Settings/           # Render pipeline and input assets
  Tilemaps/           # Tile assets and palettes
  UI/                 # UI prefabs and runtime builders
Packages/             # Unity package manifest
ProjectSettings/      # Unity project settings
```

## Main features

- **Universal Render Pipeline (2D)** with a dedicated renderer asset for sprite lighting and performance.
- **Input System** configured as the active input backend with support for keyboard, gamepad, and touch.
- **Cinemachine** camera with a framing transposer, dead zone, and 2D confiner tied to the tilemap bounds.
- **Addressables, 2D Animation, 2D Tilemap Extras** and other packages pre-installed in `Packages/manifest.json`.
- **Player controller MVP** (`PlayerController2D`) implementing:
  - Horizontal movement with acceleration/deceleration and slope support.
  - Variable height jumps with coyote time and jump buffering.
  - Wall slide and wall jump, dash, and air-control tweaks.
  - Ground detection via raycasts and layer masks.
- **Physics materials** for ground and wall surfaces.
- **Test level scene** (`Scenes/TestLevel.unity`) featuring:
  - Grid + Tilemap with TilemapCollider2D, CompositeCollider2D, and static Rigidbody2D.
  - One-way platforms and hazard placeholders spawned via `TestLevelBuilder`.
  - Cinemachine virtual camera and runtime mobile control overlay.
- **Mobile touch controls** created at runtime by `MobileUIBootstrapper` using the Input System on-screen workflow.
- **Git configuration** with Unity `.gitignore` and `.gitattributes` enabling Git LFS for binary assets.
- **CI pipeline** (`.github/workflows/build.yml`) using `game-ci/unity-builder` to produce Android builds.

## Getting started

1. Install Unity 2022.3.16f1 LTS with Android and iOS modules.
2. Clone the repository and install Git LFS (`git lfs install`).
3. Open the project root `/home/engine/project` in Unity Hub/Editor.
4. Load `Assets/Scenes/TestLevel.unity` to explore the sample gameplay.

### Controls (TestLevel)

| Action  | Keyboard / Gamepad          | Mobile overlay |
|---------|-----------------------------|----------------|
| Move    | `A/D`, `←/→`, Left Stick    | Left / Right buttons |
| Jump    | `Space` / South button      | Jump button |
| Dash    | `X` / East button           | Dash button |
| Attack* | `C` / West button           | Attack button |

> \*Attack is a placeholder event hook (`PlayerController2D.onAttack`) for future combat logic.

## Building

### Editor builds

- **Android APK**: `File ▸ Build Settings ▸ Android ▸ Build`. Ensure IL2CPP, ARM64 (default in Player Settings).
- **iOS**: `File ▸ Build Settings ▸ iOS ▸ Build` (generates an Xcode project).

Player Settings already include:
- Minimum Android API level 21.
- Landscape orientation locked, 60 FPS target, and mobile-friendly quality profile.

### Continuous Integration

A GitHub Actions workflow (`.github/workflows/build.yml`) is provided for Android builds. Configure the following repository secrets before enabling the pipeline:

- `UNITY_LICENSE`: Your serialized Unity license (for activation).
- `UNITY_EMAIL` and `UNITY_PASSWORD`: Credentials used with the license (optional if using a floating license file).

Adjust `UNITY_VERSION` in the workflow if you upgrade the project to another 2022.3 patch.

Outputs are published to the `build/android` artifact directory.

## Extending the project

- Create new levels by duplicating `TestLevel.unity` and adjusting `TestLevelBuilder` parameters or authoring tilemaps manually.
- Use `Assets/Prefabs/Player.prefab` as the authoritative player setup when spawning the character in new scenes.
- Customize touch controls by editing `MobileUIBootstrapper` or instantiating `UI/MobileControls.prefab` with overridden parameters.
- Add combat, enemies, or abilities by hooking into `PlayerController2D` events and state flags.

## License

This starter kit is delivered as-is for internal development use. Replace or update with your project’s licensing information as needed.
