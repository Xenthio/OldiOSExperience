# OldiOSExperience - Architecture Notes

## Overview
This project is an iOS 5 replica built with Blazor WebAssembly, simulating an iPhone 4. The architecture is designed to mirror real iOS components and services for authenticity and maintainability.

## Architecture

### Folder Structure
```
OldiOSExperience/
├── Models/           # Data models (AppInfo, AppState, Animation)
├── Services/         # Core system services (iOS-inspired)
├── System/          # System UI components (LockScreen, StatusBar)
├── Pages/           # Page-level components
│   └── Springboard/ # Home screen implementation
├── Apps/            # Individual app implementations
│   ├── Settings/
│   └── Shared/      # Shared app UI components
├── Components/      # Reusable UI components (future)
└── wwwroot/        # Static assets
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

#### LockScreen
- iOS 5 style lock screen with slide-to-unlock
- Touch/mouse gesture support
- State management for locked/unlocked

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

### Unlock Sequence
- Lock screen slides right with opacity fade (0-400ms)

## Future Improvements

### Animations
- [ ] Implement actual radial icon scatter/gather (currently simple fade)
- [ ] Add dock slide-down/up animations during app open/close
- [ ] Implement spring physics for more authentic feel
- [ ] Add "bounce" animation when pressing home on springboard

### Multitasking
- [ ] Implement app switcher UI (double-tap home button)
- [ ] Add app preview snapshots
- [ ] Implement background app refresh simulation
- [ ] Add memory pressure simulation

### Lock Screen
- [ ] Add passcode support
- [ ] Implement notification center pull-down
- [ ] Add camera access from lock screen
- [ ] Real-time clock updates

### Status Bar
- [ ] Extract status bar to separate component
- [ ] Add real time/battery/signal indicators
- [ ] Implement different status bar styles per app

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

## Known Issues

### Namespace Collision
The `System/` folder conflicts with C#'s `System` namespace. All `System` namespace references must use `global::System` prefix.

Alternative: Rename `System/` to `SystemUI/` in future refactor.

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
