# Changelog

## [1.4.0] - 09.03.2023
 - Renamed time command to date command
 - Added proper time command
 - Added timescale command
 - Added params command
 - Added BlueConsole namespace
 - Added header entries
 - Added Vector2, Vector3, Vector4 and Quaternion parameter types
 - Separated Extras to StaticCommands and FPSCommand classes
 - Fixed a bug that was crashing application builds
 - Fixed fps text disappearing on console toggle off
 - Fixed log-error and log-warning commands IDs
 - Fixed console input field not being activated in builds

## [1.3.1] - 22.12.2023
 - Renamed Console to ConsoleProcessor
 - Fixed command type parameter
 - Fixed bool type parameter
 - Fixed command input error with no parameters found
 - Fixed enter input in InputSystem method

## [1.3.0] - 5.12.2023
 - Overhauled commands class
 - Added man command
 - Added fps command
 - Added hwinfo command
 - Added osinfo command
 - Added time command
 - Separated ConsoleController to ConsoleVisuals and ConsoleInput classes
 - Fixed console content not wrapping correctly
 - Improved bool type parameter


## [1.2.0] - 24.11.2023
 - Reworked commands system
 - Added input history recalling
 - Added hint accepting
 - Added a check for EventSystem in ConsoleController
 - Added command parameters color
 - Added GUI scaling
 - Improved bool parsing
 - Improved commands parameters formatting
 - Made all of command attributes optional
 - Fixed "ArgumentException" from regex matching while typing in console input
 - Fixed a bug that was causing console content to be cleared after first global toggle

## [1.1.0] - 19.11.2023
 - Added input support for old "InputSystem"
 - Added icons to console related scripts
 - Created unity package

## [1.0.0] - 17.11.2023
 - Created unity project
