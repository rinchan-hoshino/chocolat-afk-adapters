# Chocolat AFK Adapters

Character-specific adapter presets for using Chocolat's standard VRSuya AFK animations with other VRChat avatars.

This repository is the **adapter pack layer**. It depends on the separate core plugin:

- https://github.com/rinchan-hoshino/vrchat-afk-motion-patcher

It does not include Chocolat, target avatar assets, source animation clips, or generated remapped clips. The core plugin creates remapped clips transiently during NDMF processing.

## Current adapter presets

- Chocolat -> Plum

This repository is where additional target-avatar presets should be added. The core AFK patching logic stays in `vrchat-afk-motion-patcher`.

## What it does

The installer creates a configured `AfkMotionPatch` component under the selected avatar and fills:

- Chocolat standard AFK source clips;
- target avatar AFK motion keys;
- target-specific renderer path and blendShape remaps.

The actual build/preprocess patch is performed by `vrchat-afk-motion-patcher` through NDMF.

## Requirements

- Unity 2022.3.x VRChat avatar project
- VRChat SDK Avatars 3.x
- NDMF
- Core plugin: `vrchat-afk-motion-patcher`
- User-owned Amatousagi Chocolat assets installed at standard paths
- User-owned target avatar assets installed at standard paths

## Installation

Install/copy both repositories into the same Unity project:

```text
Assets/RinChan/AfkMotionPatcher
Assets/RinChan/ChocolatAfkAdapters
```

Then:

1. Select a supported avatar in the Hierarchy.
2. Use `GameObject > RinChan > Add Chocolat AFK Adapter...`.
3. Select the target preset.
4. Click `Add Chocolat AFK Adapter`.

## Validation

Use the core plugin validator:

```text
Tools > RinChan > AFK Motion Patcher > Validate Selected Avatar
```

## License

MIT. See [LICENSE](LICENSE).

Avatar assets and source animation clips referenced by path are not included and remain under their respective owners' licenses.
