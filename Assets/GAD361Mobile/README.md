# Mobile Template for GAD361
### Murdoch University
### By Brad Power

## Known Issues ##
x Bug with Char2D, Flip, and if characters are made facing left. (fixed in 005)

### v001: Initial release
- Swipe, Tap, Multitouch, Tilt controls
- Supports URP and Standard pipelines
- Game Manager singleton with hooks for scene switching
- Test scenes for 2D and 3D with URP

### v002: Examples update
- Added infinite side scroller
- Added swipe platformer example

### v003: Examples and Base Code update
- Added vertical infinite jumper
- Added tap detection to swipe detector
- Allowed swipe detector input to work on key up.
- Added options to input detectors to convert coords to world space.

### v004: Docs and base code update
- Added documentation in Doc directory
- Auto disable 'isDesktop' if running on mobile
- Fixed bug with objects needing to unsubscribe from MobileControl delegates in OnDestroy()

### v005: Bugfixes
- Swipedata includes pos start and end.
- Fixed Char2D/Flip bug, chars can be made facing either way.
- Added SwipeSlash Example Game

### v006: Bugfixes
- Tilt control axis readings were wrong.
- Made sure delegates with no members (i.e. delegate is null) is not called for MobileControls
- Added Tilt Example Game

### v007: Bugfixes
- Moved code out of GameManager for 'get extents' type functions. Each game's game manager can do it themselves.
- Added FloatyBoi and Traffic Example Games.

