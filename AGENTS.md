# AGENTS.md

## Cursor Cloud specific instructions

This repository is a **single Unity game project** (`ah-light`, a first-person
horror corridor game). Engine version is pinned in
`ProjectSettings/ProjectVersion.txt` (`6000.2.6f2`, Unity 6.2, URP). There is
**no backend, database, or networked service** — everything is an offline Unity
client. There are currently **no automated test assemblies** (no `.asmdef` test
projects), so `-runTests` produces an empty run.

### Where the Unity toolchain lives
- The Unity Editor is installed at `/opt/unity/6000.2.6f2/Editor/Unity` and is
  symlinked to `/usr/local/bin/unity-editor`. This install (and the system libs
  it needs) is baked into the VM snapshot; the startup update script only
  re-installs it if it is somehow missing.

### Licensing (important, non-obvious)
- **No Unity account / license activation is required for CLI compile and
  builds.** Running the editor in `-batchmode` logs repeated
  `[Licensing::Client] Error: Code 404 ... Found 0 entitlement groups` messages,
  but these are **non-fatal**: asset import, script compilation, and player
  builds all still succeed. Do not treat those 404 lines as a failure.
- Interactive Editor GUI usage (opening the project in the Editor window) would
  still need a license; use the CLI batchmode flows below instead.

### Compile / "lint" check (no license needed)
Importing the project compiles all scripts. A clean compile means
`Library/ScriptAssemblies/Assembly-CSharp.dll` is produced and the log contains
no `error CS` lines:
```
unity-editor -batchmode -nographics -quit -logFile /tmp/unity_compile.log -projectPath .
```
- First import is slow: it restores UPM packages (Unity registry + OpenUPM +
  two Git-URL packages — needs outbound internet) and builds a ~1.7 GB
  `Library/`. `Library/` is git-ignored but persists in the VM snapshot, so
  subsequent imports are fast. NuGet deps are already vendored under
  `Assets/Packages/`, so no NuGet restore is needed.

### Building a player
There is **no committed build script**; normally builds go through the Editor's
Build Settings UI. For headless CLI builds, add a temporary editor method that
calls `BuildPipeline.BuildPlayer` and invoke it with
`-executeMethod <Class>.<Method>` (StandaloneLinux64 build support is bundled
with the Linux editor). Do not commit such helper scripts unless the project
adopts one intentionally.

### Running the built player headlessly
The VM has no GPU; use Xvfb + Mesa software GL (`llvmpipe`):
```
export LIBGL_ALWAYS_SOFTWARE=1
xvfb-run -a -s "-screen 0 1280x720x24" ./ah-light.x86_64 \
  -logFile player.log -screen-width 1280 -screen-height 720 -screen-fullscreen 0
```
- Software rendering is **very slow**. The game boots quickly through the state
  machine (`Bootstrap -> Loading`), but the additive Main-scene load and URP
  shader warmup can take a long time under `llvmpipe`; budget generously before
  expecting `[GameplayState] Gameplay started.` in the log.
- `FMOD failed to initialize the output device` / ALSA errors are expected in
  the VM (no audio device) and are harmless.
- Boot progression to look for in the player log:
  `[GameStateMachine] Registered 4 states.` ->
  `[BootstrapEntryPoint] Bootstrap scene initialized.` -> `[BootstrapState]` ->
  `[LoadingState] Loading scene: 1` -> `[GameplayState] Gameplay started.`
