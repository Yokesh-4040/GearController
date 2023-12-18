# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

Version Updated
The version number for this package has increased due to a version update of a related graphics package.

## [1.0.10] - 2023-12-17
### Fixed 
- Fixed 2nd and 4th gears not turning off even after threshold
- Added Update Checker

## [1.0.9] - 2023-12-17
### Added
- Added Override for the auto neutral
- Added import editor files and cannot be remove at any situation


## [1.0.8] - 2023-12-17
### Fixed
- [Optimization] optimized to not calculate gears pos when its not grabbed
- made the default grab speed to private and set the value to -20
- introduce new variable to check if the gear is at origin

## [1.0.7] - 2023-12-17
### Fixed
- [Major fix] Fixed End A not turning to false when the threshold is crossed

## [1.0.6] - 2023-12-17
### Added
- Added Gear State Extention asset now user can added n number of script to the Gear controller to define its states.
- Added on Neutral event Update Function, this function will work like an update to give the neutral state


## [1.0.5] - 2023-12-17
### Fixed
- Fixed Naming Conventions for few variables (Note in next update formaly tag will be removed)
- Fixed default value for move speed on the grabbable to -20f.

## [1.0.4] - 2023-12-17
### Fixed
- not able customize the gear angle values, changed from const to normal variable, should be able to edit the values now

## [1.0.3] - 2023-12-17
### Fixed
- Sensitive too less modified constants by half

## [1.0.2] - 2023-12-17
### Fixed
- Fixed isGrabbed bool variable can be changed using events


## [1.0.1] - 2023-12-17
### Added
- Added Changelog file
