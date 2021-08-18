# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

## [2.1.1] - 2021-08-18
### Fixed
- Temporary game view resolution now gets added to correct current platform the user is on (Unity Build Settings Platform)

## [2.1.0] - 2021-08-18
### Added
- Editor Coroutines v1.0.0 package dependency
- GameViewSizeHelper.cs + MIT License, for changing game view resolution, credit to https://github.com/Syy9/GameViewSizeChanger
- Editor.asmedf GUID Unity.EditorCoroutines.Editor

### Changed
- Screenshot.ProcessScreenshot() changed to IEnumerator
- Screenshots are now placed in a queue and taken when possible (cause of Game View + UI needs to wait a couple frames)

### Fixed
- Bug where camera wasn't assigned at startup
- Resolution not being added to file name when including UI
- Game View + UI resolution not working correctly
- Generating multiple screenshots now have correct screenshot size assigned

## [2.0.0] - 2021-08-16
### Changed
- Major overhaul of all scripts and editor window look.
- Made everything easier, simpler and better structured.

## [1.0.2] - 2021-02-21
### Added
- filePathDefault string that contains "/SLIDDES Temporary/Editor/"
### Changed
- Screenshot files default store path is now in SLIDDES Temporary/Editor/ so that they dont get accedently included in the build
- UI look of window
- The way fileNames are generated
### Removed
- EditorGUILayout horizontal line

## [1.0.1] - 2021-02-20
### Changed
- Moved Github LICENSE generated file into LICENSE.md
- sliddes.screenshot.editor assembly definition asset now has correct platform of editor
- Fixed build erros regarding SLIDDES ScreenShot

## [1.0.0] - 2021-02-12
### Added
- First Release