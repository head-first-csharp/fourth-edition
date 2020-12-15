# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.3.8] - 2020-06-08
## Fixed
- Fix incorrect priority of error messages
- Fix Collab button being stuck in inprogress state
- Fix error when partially publishing without the window open

## [1.3.7] - 2020-01-30
## Changed
- Bulk revert is now supported.
- Collab is blocked in play mode.
## Fixed
- Fixed services window's links to open Collab.

## [1.3.6] - 2020-01-21
### Fixed
- Fixed compile errors when removing the NUnit package by removing unnecessary references.

## [1.3.5] - 2020-01-08
### Fixed
- Fix "accept mine" / "accept remote" icon swap in conflicts view.

## [1.3.4] - 2019-12-16
### Changed
- Window state is no longer restored after the window is closed and opened.
### Fixed
- History tab failing to load on startup if it is left open in the previous session.
- Progress bar percentage not matching the bar.
- History list correctly updates after a new revision is published.
- UI instabilities when restoring or going back to a revision with a different package manifest.
- Improve handling of changes to the project id.

## [1.3.3] - 2019-12-10
### Changed
- Disable UI test cases that can be unstable.

## [1.3.2] - 2019-12-05
### Changed
- Update UX to UIElements.
- Increased minimum supported version to 2020.1.
- Update Documentation to required standards.

## [1.2.16] - 2019-02-11
### Fixed
- Update stylesheet to pass USS validation

## [1.2.15] - 2018-11-16
### Changed
- Added support for non-experimental UIElements.

## [1.2.11] - 2018-09-04
### Fixed
- Made some performance improvements to reduce impact on ReloadAssemblies.

## [1.2.9] - 2018-08-13
### Fixed
- Test issues for the Collab History Window are now fixed.

## [1.2.7] - 2018-08-07
### Fixed
- Toolbar drop-down will no longer show up when package is uninstalled.

## [1.2.6] - 2018-06-15
### Fixed
- Fixed an issue where Collab's History window wouldn't load properly.

## [1.2.5] - 2018-05-21
This is the first release of *Unity Package CollabProxy*.

### Added
- Collab history and toolbar windows
- Collab view and presenter classes
- Collab Editor tests for view and presenter
