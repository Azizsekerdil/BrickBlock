# Brick Block

A colour-matching physics puzzle prototype for mobile. Loose blocks are dragged across the
board into fixed blocks; a colour match sends the block flying to its nearest twin and
shatters it, while a mismatch locks it in place for good.

| | |
|---|---|
| **Engine** | Unity `6000.0.38f1` (Unity 6) |
| **Render pipeline** | Universal RP `17.0.3` |
| **Platform** | Mobile (touch) |
| **Version** | `0.1.0` |
| **Development** | March – April 2025 |
| **Status** | Prototype — single scene |

---

## Gameplay

Blocks are dragged with touch. Selection raycasts from the touch point, and the block is
then driven by **velocity toward the finger** rather than teleported, so it collides
naturally with everything on the way (`MovableCubeController`). Releasing freezes it.

On contact with a block tagged `FixedCube`, the colours are compared:

- **Match** — the loose block searches every `FixedCube` for the nearest one sharing its
  colour, tweens to it with DOTween (`Ease.InExpo`), then bursts into 20 shards that
  inherit its colour and scatter with random impulse before despawning after 2 seconds.
- **Mismatch** — velocity is zeroed, all constraints freeze, and the block is permanently
  locked; it cannot be picked up again.

Two axis-constrained variants (`MovableCubeControllerContrains_X`,
`MovableCubeControllerContrains_Y`) restrict dragging to a single axis for puzzle layouts
that need rail-style movement.

---

## Technical Notes

- **Input:** legacy touch API (`Input.GetTouch`) with camera raycast picking
- **Physics:** Rigidbody constraint juggling — frozen at rest, rotation-locked while
  dragging
- **Colour logic:** compared directly on `Renderer.material.color`, so match groups are
  defined by the materials in `Assets/BrickBlock` (Black, Blue, Green, Grey, Orange, Red,
  White, Yellow)
- **Art:** first-party model (`Brick Block.fbx`), explosion prefab, and sprite set under
  `Assets/BrickBlock/Sprites/Mine`
- **Scripts:** 3 gameplay files

### Packages

| Package | Version |
|---------|---------|
| `com.unity.render-pipelines.universal` | 17.0.3 |
| `com.unity.inputsystem` | 1.13.0 |
| `com.unity.ugui` | 2.0.0 |
| `com.unity.timeline` | 1.8.7 |
| `com.unity.ai.navigation` | 2.0.5 |

---

## Required Third-Party Assets

This repository contains only first-party work. The packages below are redistributed under
their own licences and are **excluded** — import them before opening the project, or the
scripts will not compile:

| Asset | Publisher | Used for |
|-------|-----------|----------|
| **DOTween** | Demigiant | Match tween (`DG.Tweening`) |
| **Heathen Engineering — Free Flat Icons** | Asset Store | UI icon set |
