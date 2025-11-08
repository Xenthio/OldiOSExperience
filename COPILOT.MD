# OldiOSExperience - Architecture Notes

## Overview
This project is an iOS 5 replica built with Blazor WebAssembly, simulating an iPhone 4. The architecture is designed to mirror real iOS components and services for authenticity and maintainability.

## Architecture

### Folder Structure
```
OldiOSExperience/
├── Models/           # Data models (AppInfo, AppState, Animation)
├── Services/         # Core system services (iOS-inspired)
├── System/           # System UI components (StatusBar)
├── Pages/            # Page-level components
│   └── Springboard/  # Home screen implementation
│       └── Components/ # Springboard-specific components (LockScreen, AppSwitcher, AppIcon, etc.)
├── Apps/             # Individual app implementations
│   ├── Settings/
│   └── Shared/       # Shared app UI components
├── Components/       # Reusable UI components (future)
└── wwwroot/          # Static assets
```

### Core Services

#### 1. SpringboardService
- Manages home screen layout and app organization
- Handles page management and app launching
- Equivalent to iOS's SpringBoard daemon

#### 2. BackgroundAppManager
- Implements app multitasking
- Manages app lifecycle (NotRunning, Foreground, Background, Suspended)
- Tracks recent apps for app switcher
- Equivalent to iOS's UIApplication and multitasking system

#### 3. AnimationService
- Provides coordinated multi-element animations
- Defines iOS-style animation sequences
- Equivalent to iOS's Core Animation framework

#### 4. GestureService
- Provides gesture recognition for swipes and taps
- Includes SwipeGestureRecognizer and TapGestureRecognizer classes
- Handles both touch and mouse events for cross-platform testing
- Equivalent to iOS's UIGestureRecognizer system

### Key Models

#### AppInfo
- Basic app metadata (ID, Name, IconPath, ComponentType, BundleId)
- Represents an installed app

#### AppState
- Runtime state of an app
- Tracks execution state and background time
- Can store app-specific state data

#### Animation & AnimationSequence
- Defines reusable animations
- Supports multi-step, coordinated animations
- Pre-defined iOS-style animations in `iOSAnimations` class

### System Components

#### LockScreen (Pages/Springboard/Components/)
- iOS 5 style lock screen with slide-to-unlock
- Touch/mouse gesture support
- State management for locked/unlocked
- Displays time and date
- **Unlock Animation**: lock-status-bar slides up, slide-to-unlock-container slides down
- Triggers springboard entrance animation when unlocked

#### AppSwitcher (Pages/Springboard/Components/)
- iOS 5 style app switcher (multitasking tray)
- Activated by double-tapping home button
- Shows recent apps with preview cards
- Allows switching between apps or closing them
- Slide-up animation from bottom
- Part of SpringBoard (matches iOS architecture)

#### StatusBar (System/)
- Reusable status bar component matching iOS 5 design
- Two styles: Default (semi-transparent) and Black (solid)
- Displays carrier name, WiFi icon, time, and battery indicator
- Real-time clock updates (updates every minute)
- Springboard status bar fades out when app opens
- AppContainer provides independent status bar for apps
- Uses placeholder image paths for WiFi and battery icons

## Animation Framework

The animation system is designed for complex, multi-element animations:

### App Open Sequence
1. Icons scatter radially outward from center (0-300ms)
2. Dock slides down (0-300ms)
3. Everything fades to black except opening app (0-300ms)
4. App zooms in from center (100-400ms)

### App Close Sequence
1. App zooms out to center (0-300ms)
2. Icons gather radially inward (100-400ms)
3. Dock slides up (100-400ms)
4. Everything fades in (100-400ms)

### Unlock Sequence (Lock Screen to Home Screen)
1. Lock-status-bar slides up and fades out (0-400ms)
2. Slide-to-unlock-container slides down and fades out (0-400ms)
3. Icons fly in from scattered positions (0-400ms)
4. Dock slides up from bottom (0-400ms)
5. Status bar and page indicators fade in (0-400ms)
- All animations use ease-out timing for smooth entrance

## Recent Improvements (Current Session)

### Animation Enhancements
- ✅ Improved app open/close animations with better easing curves
- ✅ Added cubic-bezier timing functions for authentic iOS feel
- ✅ Springboard fades out when app opens
- ✅ App container uses AnimationService for consistent timing
- ✅ **NEW: Lock screen unlock animations**
  - Lock-status-bar slides up and fades out
  - Slide-to-unlock-container slides down and fades out
  - Icons fly in from scattered positions (using existing scatter system)
  - Dock slides up from bottom
  - Status bar and page indicators fade in
  - All animations coordinated with 400ms timing

### Multitasking System
- ✅ Implemented AppSwitcher component
- ✅ Double-tap home button shows recent apps
- ✅ Recent apps displayed with close buttons
- ✅ Smooth slide-up animation for app switcher tray
- ✅ Background click to dismiss

### Code Organization
- ✅ Created GestureService for reusable gesture recognition
- ✅ Used DynamicComponent for flexible app loading based on ComponentType
- ✅ Services properly registered in DI container
- ✅ **NEW: Moved LockScreen and AppSwitcher to Pages/Springboard/Components/**
  - Better architecture matching iOS (both are part of SpringBoard)
  - Clearer separation of concerns
  - System/ folder now only contains truly system-level components (StatusBar)

### Code Review Improvements
- ✅ Real-time clock on lock screen (updates every second)
- ✅ Optimized BackgroundAppManager with cached foreground app
- ✅ Refactored GestureService to eliminate code duplication
- ✅ Replaced magic numbers with named constants
- ✅ Proper IDisposable implementation with timer cleanup

## Future Improvements

### Animations
- [x] Implement actual radial icon scatter/gather
- [x] Add dock slide-down/up animations during app open/close
- [x] Unlock animation with lock elements sliding out and icons flying in
- [ ] Implement spring physics for more authentic feel
- [ ] Add "bounce" animation when pressing home on springboard
- [ ] Add parallax effect to wallpaper during page swipe

### Multitasking
- [ ] Add app preview snapshots (currently just icons)
- [ ] Implement background app refresh simulation
- [ ] Add memory pressure simulation
- [ ] Persist app state between sessions (localStorage)

### Lock Screen
- [x] Unlock animations (lock elements slide out, springboard flies in)
- [ ] Add passcode support
- [ ] Implement notification center pull-down
- [ ] Add camera access from lock screen
- [ ] Real-time clock updates

### Status Bar
- [x] Extract status bar to separate component
- [x] Add real time/battery/signal indicators (with placeholder images)
- [x] Implement different status bar styles per app (Default/Black)
- [x] StatusBar component fades out with springboard when app opens
- [x] AppContainer provides independent StatusBar for opened apps
- [ ] Add actual WiFi and battery icon images
- [ ] Implement dynamic battery level display
- [ ] Add signal strength indicators

### Navigation
- [ ] Create navigation service for app-to-app transitions
- [ ] Implement deep linking
- [ ] Add modal/sheet presentation styles

### Performance
- [ ] Virtualize app pages for many apps
- [ ] Lazy load app components
- [ ] Optimize animation performance

### Code Organization
- [ ] Extract gesture handling to separate service
- [ ] Create reusable gesture recognizer components
- [ ] Abstract touch/mouse handling

## Design Patterns

### Service Layer
Services are registered as singletons in Program.cs and injected via dependency injection. This mirrors iOS's system services approach.

### Event-Driven Updates
Components subscribe to service events (e.g., `OnAppStatesChanged`) to react to state changes, similar to iOS's notification system.

### State Management
State is managed at the service level, not in components. Components are stateless views of service state.

### Dynamic Component Loading
Apps can specify a `ComponentType` property to load custom components dynamically using `<DynamicComponent>`. This allows for flexible app implementations without hard-coding app IDs.

### Gesture Recognition
Touch and mouse events are handled through gesture recognizers that provide clean abstractions for swipes, taps, and drags. This pattern is inspired by iOS's UIGestureRecognizer.

## Implementation Details

### Home Button Behavior
The home button in Index.razor implements several behaviors:
1. **When locked**: Does nothing (could add wake screen in future)
2. **Single tap with app open**: Returns to springboard
3. **Single tap on springboard**: Does nothing (could add bounce animation)
4. **Double-tap on springboard**: Shows app switcher
5. Double-tap window is 400ms (matching iOS timing)

### App Switcher Flow
1. User double-taps home button
2. AppSwitcher slides up from bottom with fade-in background
3. Shows recent apps from BackgroundAppManager
4. User can:
   - Tap an app to switch to it
   - Tap X button to close an app
   - Tap background to dismiss switcher

### App Lifecycle
1. **Launch**: App moves to Foreground state
2. **Home button**: App moves to Background state (stays in memory)
3. **Close from switcher**: App moves to NotRunning state (removed from memory)
4. **Switch apps**: Current app to Background, selected app to Foreground

## Known Issues

### Namespace Collision
The `System/` folder conflicts with C#'s `System` namespace. All `System` namespace references must use `global::System` prefix.

Alternative: Rename `System/` to `SystemUI/` in future refactor.

### App Preview Snapshots
AppSwitcher currently shows app icons instead of preview snapshots. To implement previews:
1. Capture DOM snapshot before app goes to background
2. Convert to image or store as virtual DOM
3. Display in app switcher card

## Testing Notes

- Build project: `dotnet build`
- Run project: `dotnet run`
- Project uses .NET 9.0 and Blazor WebAssembly

## Contributing Guidelines

1. Keep services focused and single-responsibility
2. Follow iOS naming conventions where applicable
3. Document complex animations
4. Add XML comments to public APIs
5. Keep components stateless when possible
6. Use services for state management
7. Test on both touch and mouse input

## References

- iOS Human Interface Guidelines (iOS 5 era)
- UIKit Framework documentation
- Core Animation programming guide
- SpringBoard reverse engineering resources
