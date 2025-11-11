# OldiOSExperience - Architecture Notes

## Overview
This project is an iOS 5 replica built with Blazor, simulating an iPhone 4. The architecture is designed to mirror real iOS components and services for authenticity and maintainability.

**As of November 2025**: The project has been migrated to a .NET MAUI Blazor Hybrid and Web App structure, enabling it to run as both a web app and native mobile/desktop applications.

## Project Structure (Post-Migration)

The project now consists of:
- **OldiOS/**: .NET MAUI project for native apps (iOS, Android, Windows, macOS)
- **OldiOS.Shared/**: Shared Razor component library (used by all platforms)
- **OldiOS.Web/**: ASP.NET Core web server
- **OldiOS.Web.Client/**: Blazor WebAssembly client
- **OldiOSExperience/**: Original Blazor WebAssembly project (preserved for reference)

All iOS 5 replica code now lives in **OldiOS.Shared** and is shared across web and native platforms.

## Architecture

### Folder Structure
```
OldiOS/
├── OldiOS/                    # .NET MAUI native app project
│   ├── Platforms/             # Platform-specific code (iOS, Android, Windows, Mac)
│   ├── Resources/             # App icons, splash screens, fonts
│   ├── wwwroot/               # MAUI-specific static files
│   ├── MauiProgram.cs         # MAUI app configuration and DI
│   └── MainPage.xaml          # MAUI page hosting BlazorWebView
├── OldiOS.Shared/             # Shared Blazor component library
│   ├── Models/                # Data models (AppInfo, AppState, Animation)
│   ├── Services/              # Core system services (iOS-inspired)
│   ├── System/                # System UI components (StatusBar, DebugMenu)
│   ├── Pages/                 # Page-level components
│   │   ├── Index.razor        # Main entry point
│   │   └── Springboard/       # Home screen implementation
│   │       └── Components/    # Springboard components (LockScreen, AppSwitcher, etc.)
│   ├── Apps/                  # Individual app implementations
│   │   ├── Settings/
│   │   ├── UIKit/             # iOS 5 UIKit components library
│   │   └── Shared/            # Shared app UI components
│   ├── Layout/                # Layout components (MainLayout)
│   ├── wwwroot/               # Static assets (CSS, JS, images)
│   └── Routes.razor           # Blazor routing configuration
├── OldiOS.Web/                # ASP.NET Core web server
│   ├── Components/
│   │   └── App.razor          # Web app root component
│   └── Program.cs             # Web server configuration
└── OldiOS.Web.Client/         # Blazor WebAssembly client
    └── Program.cs             # WebAssembly client DI configuration

OldiOSExperience/              # Original project (preserved for reference)
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

### UIKit Components (Apps/UIKit/)
iOS 5 styled reusable UI components library for building apps:

#### UIButton (UIButton.razor, UIButton.razor.css)
- Implements iOS 5 glossy button styles
- **Styles**: Default (blue gradient), Bordered (white), Destructive (red), Custom
- Supports disabled state, full-width mode, and icons
- Authentic iOS 5 gradients and shadows

#### UITextField (UITextField.razor, UITextField.razor.css)
- iOS 5 styled text input with rounded borders
- Features: placeholder text, clear button, left/right icons
- Focus state with blue border highlight
- Supports all input types (text, email, password, etc.)

#### UILabel (UILabel.razor, UILabel.razor.css)
- Text display with iOS 5 typography
- **Styles**: Title (44px), Headline (38px), Body (34px), Subheadline (30px), Footnote (26px), Caption (22px)
- **Alignments**: Left, Center, Right
- Bold and italic variants

#### UISwitch (UISwitch.razor, UISwitch.razor.css)
- Standalone iOS 5 toggle switch component
- Green gradient when ON, gray when OFF
- Animated sliding knob with ON/OFF text
- Disabled state support

#### UISegmentedControl (UISegmentedControl.razor, UISegmentedControl.razor.css)
- iOS 5 segmented control for multiple options
- Metallic gradient styling matching iOS 5 navigation bars
- Full-width and auto-width modes
- Selected state with inset shadow

#### UISlider (UISlider.razor, UISlider.razor.css)
- Range slider with iOS 5 styling
- Blue gradient fill track
- Optional min/max icons (e.g., volume icons)
- Smooth dragging with white circular thumb

#### UIActivityIndicator (UIActivityIndicator.razor, UIActivityIndicator.razor.css)
- Loading spinner matching iOS 5 design
- **Sizes**: Small (40px), Medium (60px), Large (80px)
- 12-spoke radial animation
- Can be toggled on/off

#### UIKitDemoApp (UIKitDemoApp.razor)
- Demo application showcasing all UIKit components
- Interactive examples of each component
- Can be registered as an app to test components in the simulator

**Usage Example**:
```razor
@using OldiOSExperience.Apps.UIKit

<UIButton Text="Save" Style="UIButtonStyle.Default" OnClick="@HandleSave" />
<UITextField @bind-Value="username" Placeholder="Username" />
<UISwitch @bind-Value="enabled" />
```

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

### New Default Apps Implementation (November 2025)

Implemented base/skeleton versions of core iOS 5 default apps:

**New Apps Created (Total: 13 apps):**
- ✅ **Phone App** - Full tab-based interface with Favorites, Recents, Contacts, Keypad, and Voicemail tabs
  - Working keypad with number input and delete functionality
  - Call button (visual only)
  - Empty states for all sections
- ✅ **Messages App** - Conversation list with search bar
  - Empty state with compose hint
- ✅ **Music/iPod App** - Tab-based music library interface
  - Playlists, Artists, Songs, Albums, and More tabs
  - Dark theme matching iOS 5 Music app
  - Empty states for all sections
- ✅ **Calendar App** - Month view with day grid
  - iOS 5 style calendar grid with today highlighting
  - Weekday headers
  - Events section with empty state
  - Toolbar with List, Day, and Add buttons
- ✅ **Photos App** - Tab-based photo library
  - Albums, Photos, Events, Places, and Faces tabs
  - Black theme matching iOS 5 Photos app
  - Empty states for all sections
- ✅ **Weather App** - Current weather and forecast display
  - Current conditions with temperature, location
  - Hourly forecast strip (scrollable)
  - Daily forecast list with 7-day outlook
  - Weather details grid (sunrise, sunset, humidity, wind, etc.)
  - Blue gradient background matching iOS 5 Weather app
- ✅ **Stocks App** - Stock list with charts
  - Stock symbols with company names
  - Price display with change indicators (green for positive, red for negative)
  - Mini charts using SVG
  - Market index footer (DJIA)
  - Black theme matching iOS 5 Stocks app
- ✅ **Calculator App** - Fully functional basic calculator
  - iOS 5 authentic button layout and styling
  - All basic operations (+, -, ×, ÷)
  - Percentage, sign change, clear functions
  - Zero button spans two columns
  - Proper decimal point handling
  - Black background with colored buttons (orange operators, gray functions)
- ✅ **Clock App** - Full clock app with all tabs
  - World Clock tab with empty state
  - Alarm tab with empty state
  - Stopwatch tab with timer display and controls
  - Timer tab with picker interface and sound selection
  - Black theme matching iOS 5 Clock app
- ✅ **Safari App** - Browser interface
  - Address bar with search/URL input
  - Refresh button
  - Toolbar with back, forward, bookmarks, reading list, and more buttons
  - Placeholder browser view
- ✅ **Maps App** - Map interface
  - Search bar for locations
  - Map placeholder view with gradient
  - Location and compass control buttons
  - Toolbar with Directions, Bookmark, and more options
- ✅ **Videos App** - Video library
  - Tabbed interface for Movies, TV Shows, and Music Videos
  - Black theme matching iOS 5 Videos app
  - Empty states for all categories
- ✅ **Reminders App** - Task management
  - List view with Completed and Reminders sections
  - iOS 5 table view styling
  - Add button in navigation bar
- ✅ **Camera App** - Camera interface
  - Camera viewfinder placeholder
  - Camera controls (mode, shutter, flip)
  - Black theme with iOS 5 style controls

**New UIKit Components:**
- ✅ **UITabBar** - Bottom tab bar for multi-section apps
  - iOS 5 metallic gradient styling
  - Selected state highlighting
  - Badge support for notifications
  - Icon and title display
- ✅ **UIToolbar** - Bottom toolbar for actions
  - Metallic gradient matching iOS 5
  - Flexible content with RenderFragment
- ✅ **UISearchBar** - Search input with iOS 5 styling
  - Rounded corners with inset shadow
  - Search icon and clear button
  - Optional cancel button
  - Focus state handling

**App Registrations:**
All new apps have been registered in SpringboardService:
- Phone (dock)
- Safari (dock)
- Music (dock)
- Messages (page 1)
- Calendar (page 1)
- Photos (page 1)
- Camera (page 1)
- Stocks (page 1)
- Maps (page 1)
- Weather (page 1)
- Clock (page 1)
- Reminders (page 2)
- Videos (page 2)
- Calculator (page 2)

**Implementation Quality:**
- All apps have iOS 5 authentic styling and layouts
- Consistent use of UIKit components
- Empty states for all data-driven views
- Proper navigation bars and toolbars
- Tab-based navigation where appropriate
- Dark/light themes matching original iOS 5 apps

### Performance Optimizations for Mobile Devices (Latest - November 2025)
- ✅ **Using iOS native shadow images (Final Solution)**
  - Replaced CSS `filter: drop-shadow()` with authentic iOS shadow images
  - `icon_shadow.png` (206×206px) for regular icons, positioned 40px lower
  - `icon_shadow_dock.png` (118×120px) for dock icons, positioned directly behind
  - **Zero CSS filter overhead** - static images are the most performant solution
  - Authentic iOS 5 appearance using original shadow assets from iOS
  - Tested and confirmed smooth performance on iPhone X
- ✅ **Removed dynamic scatter vector JavaScript calculations**
  - AppIcon component no longer subscribes to DisplaySettings changes
  - Removed OnAfterRenderAsync JavaScript interop calls (calculateScatterVector)
  - Now uses static CSS scatter vectors defined in Springboard.razor.css
  - Eliminates 30+ JavaScript calls on every page render and resolution change
  - Static vectors are sufficient for most screen sizes and much more performant
- ✅ **Added GPU acceleration hints throughout**
  - AppIcon: Added `will-change: transform, opacity` and `translateZ(0)`
  - Springboard filmstrip: Added `will-change: transform` and `translateZ(0)`
  - Springboard pages: Added `translateZ(0)` to force GPU layer creation
  - Wallpaper: Added `translateZ(0)` for smoother rendering
  - Dock: Added `will-change: transform, opacity` for animations
  - Home button: Added `will-change: transform` and `translateZ(0)` for press effect
- ✅ **Optimized home button transition**
  - Changed from `transition: all` to `transition: transform` only
  - Reduces unnecessary property checks during animation
  
**Performance Impact:**
- Eliminates 30+ JavaScript interop calls on initial render
- **Removes all CSS filter overhead** - uses static images for best performance
- Enables hardware acceleration for all major animated elements
- Confirmed smooth performance on iPhone X during page swiping, multitasking, and home button press
- Authentic iOS 5 appearance with native shadow assets

**Shadow Implementation Evolution:**
1. Original: `filter: drop-shadow()` on parent container (slow, affected text too)
2. First optimization: `box-shadow` on icon (fast but didn't respect transparency)
3. Second optimization: `filter: drop-shadow()` on icon only (better but still had filter overhead)
4. **Final solution**: Static iOS shadow images (best performance + authentic look)

### Display Settings - Dynamic Scatter Vector Recalculation (Removed for Performance)
- ❌ **Removed dynamic scatter vector calculation** (see Performance Optimizations above)
  - Previous implementation caused performance issues on mobile devices
  - Static CSS scatter vectors are used instead for better performance
  - Animations still work correctly with static vectors

### Home Screen Page Swiping Improvements

#### Fourth Iteration (November 2025) - Swiping Animation via JavaScript requestAnimationFrame
- ✅ **Fixed persistent sliding/bouncing by using JavaScript requestAnimationFrame-based animations (November 2025)**
  - **Problem**: Despite multiple animation attempts, pages still slid over instantly and bounced off edges
  - **Root Cause**: Custom animation logic was fighting with CSS transitions and page index changes
  - **Solution**: Removed conflicting CSS transition logic and implemented smooth page swiping using custom JavaScript animation (`springboardAnimation.js`) with `requestAnimationFrame`
  - **Implementation**:
    - When page change threshold is met, simply:
      1. Update `currentPageIndex` (which moves filmstrip via JS animation)
      2. Reset `dragOffset = 0`
      3. Set `isDragging = false`
    - The JavaScript animation smoothly transitions the filmstrip position, mimicking iOS-like deceleration and bounce
  - **Result**: Clean, smooth page transitions with no overshoot, bounce, or instant sliding
  - **Why This Works**: 
    - The filmstrip style is animated via JavaScript for precise control and iOS-like feel
    - By resetting dragOffset to 0 immediately, we avoid conflicting transforms
    - JavaScript requestAnimationFrame animation provides the exact iOS-like feel we want, outperforming native CSS transitions for this use case

#### Third Iteration (November 2025) - Fixed "Instant Speed Up" and Improved Tiny Swipe Detection
- ✅ **Fixed "instant speed up" when lifting finger during swipe (November 2025)**
  - **Problem**: When user lifted finger, page would suddenly accelerate/snap to final position
  - **Root Cause**: Setting `isDragging = false` and `dragOffset = 0` caused CSS transition to take over instantly
  - **Solution**: Created `AnimatePageChange()` method that smoothly decelerates from current position to final position
  - **Result**: Page changes now smoothly decelerate into place with no jarring speed changes
- ✅ **Improved velocity threshold for tiny swipes**
  - Increased velocity threshold from 0.5 to 0.8 pixels/ms (800px/s) to be more selective
  - Only fast flicks trigger velocity-based page changes, not slow drags
  - Prevents accidental page changes from tiny movements
- ✅ **Consistent animation for all transitions**
  - Both page changes and rubber-band use same ease-out cubic animation
  - Provides consistent, predictable feel across all gestures

#### Second Iteration (November 2025) - Fixed Velocity Calculation and Smooth Tracking
- ✅ **Fixed inaccurate velocity calculation (November 2025)**
  - **Problem**: Previous implementation calculated velocity from total drag distance and total time, which didn't reflect the user's final swipe momentum
  - **Problem**: Very tiny swipes would cause instant transitions because `touchCurrentX` wasn't updated properly
  - **Solution**: Implemented touch point history tracking (last 10 positions)
  - **Solution**: Calculate velocity from recent movement (last 100ms) instead of entire drag
  - **Solution**: Added `StateHasChanged()` calls during `HandleTouchMove` for smooth real-time tracking
  - **Result**: Velocity now accurately reflects the user's swipe momentum at release
  - **Result**: Tiny swipes no longer cause jumps - they smoothly rubber-band back
- ✅ **Improved state transition timing**
  - Reset `dragOffset` before `isDragging` to prevent CSS transition jitter
  - Ensures smooth transitions between drag and animation states

**Technical Details:**
- Touch point buffer stores last 10 positions with timestamps
- Velocity calculated from points within last 100ms window
- Falls back to all points if fewer than 2 in time window
- Avoids division by zero with minimum 1ms deltaTime check

#### First Iteration - iOS-like Behavior
- ✅ **Fixed choppy and instant jumping during page swipes**
  - Replaced simple 50px threshold with iOS-like behavior
  - Added half-page (50%) threshold for page changes
  - Implemented velocity-based swiping (>0.5px/ms ≈ 500px/s)
  - Added smooth rubber-band animation when swipe doesn't complete
  - Prevents instant jumps by animating back to current page
  - Added boundary resistance (30% dampening) when dragging past edges
- ✅ **Improved touch/mouse tracking**
  - Track last touch position and time for velocity calculation
  - Smooth 60fps animation loop for rubber-band effect
  - Use ease-out cubic easing for natural deceleration
- ✅ **Enhanced CSS transitions**
  - Changed from `ease-out` to `cubic-bezier(0.25, 0.46, 0.45, 0.94)` for smoother page snapping
  - Disable transitions during drag AND during rubber-band animation
- ✅ **Added DisplaySettings.Width property** for easier width access

**How it works (iOS-like behavior):**
1. **During drag**: Pages follow finger with smooth tracking, with 30% resistance at boundaries
2. **On release**: 
   - If dragged > 50% of screen width OR velocity > 500px/s → change page
   - Otherwise → smooth rubber-band animation back to current page (300ms, ease-out cubic)
3. **At boundaries**: Attempting to swipe past first/last page shows resistance and rubber-bands back

## Recent Improvements (Previous Session)

### Display Settings Implementation (Latest)
- ✅ **Created DisplaySettings Service** - Dynamic resolution management
  - Converted from static class to singleton service with events
  - DevicePreset enum: iPhone4, iPhone5, iPad, Custom
  - DeviceType enum: iPhone, iPad
  - OnResolutionChanged event for reactive updates
  - Methods to calculate grid layout based on resolution
- ✅ **Resolution Controls** in Debug Menu
  - Device Preset dropdown (iPhone 4, iPhone 5, iPad Retina, Custom)
  - Custom resolution textboxes (Width/Height in pixels)
  - Apply Custom Resolution button
  - Real-time display of current resolution and grid size
- ✅ **Dynamic Grid Layout** on Springboard
  - CSS variables (--icon-rows, --icon-columns) for dynamic sizing
  - Automatic app distribution across pages based on grid capacity
  - iPhone 4 (640×960): 4×4 grid (16 apps per page)
  - iPhone 5 (640×1136): 4×5 grid (20 apps per page)
  - iPad Retina (1536×2048): 5×5 grid (25 apps per page)
- ✅ **Scale Factor Recalculation** in Index.razor
  - Subscribes to resolution changes
  - Automatically adjusts viewport scaling
  - Maintains proper aspect ratio for all resolutions
- ✅ **Bug Fix**: Device type detection (iPhone 5 was incorrectly detected as iPad)

### Debug Menu Implementation
- ✅ **Created DebugMenu.razor component** - Side popover accessible from anywhere
  - Positioned on the right side of the screen with slide-in animation
  - Toggle button with gear icon always visible
  - Dark themed panel with sections for different debug controls
- ✅ **Created BatteryService** - Manages battery state independently
  - Allows manual override of browser Battery API
  - Provides plug/unplug controls and battery percentage slider
  - Integrated with StatusBar component for real-time updates
- ✅ **Battery Controls** in Debug Menu
  - Manual Override checkbox to enable/disable browser API
  - Plug/Unplug buttons to simulate charging state
  - Battery percentage slider (0-100%)
  - Real-time status bar updates when manual override is enabled
- ✅ **Moved "Show Icons" button** from Springboard to Debug Menu
  - Cleaner springboard interface
  - Animation debug control now accessible globally
- ✅ **Device Controls** in Debug Menu
  - Sleep/Wake button (visual state tracking)
  - Volume Up/Down buttons with visual percentage display
- ✅ **Additional Debug Features**
  - Lock Orientation checkbox
  - Rotate Device button
  - Override Time checkbox
- ✅ **Integration** into Index.razor for global accessibility
- ✅ **Styling** with modern dark UI, smooth animations, custom scrollbar

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
- [x] **Optimize animation performance** (November 2025)
  - Removed expensive drop-shadow filters
  - Added GPU acceleration hints (will-change, translateZ)
  - Eliminated dynamic JavaScript scatter vector calculations
  - Significantly improved mobile performance (especially iPhone X)
- [ ] Virtualize app pages for many apps (only render visible pages)
- [ ] Lazy load app components (load on-demand)

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

**Original Project (OldiOSExperience):**
- Build: `cd OldiOSExperience && dotnet build`
- Run: `cd OldiOSExperience && dotnet run`
- Uses .NET 9.0 and Blazor WebAssembly

**New Web App (OldiOS.Web):**
- Build: `cd OldiOS/OldiOS.Web && dotnet build`
- Run: `cd OldiOS/OldiOS.Web && dotnet run`
- Navigate to http://localhost:5000 or https://localhost:5001

**Shared Library (OldiOS.Shared):**
- Build: `cd OldiOS/OldiOS.Shared && dotnet build`
- Contains all reusable components

**MAUI Native App (OldiOS):**
- Requires .NET MAUI workloads: `dotnet workload install maui`
- Build for specific platform: `dotnet build -f net9.0-android`
- Note: May not build in CI/CD without platform SDKs

## Contributing Guidelines

1. Keep services focused and single-responsibility
2. Follow iOS naming conventions where applicable
3. Document complex animations
4. Add XML comments to public APIs
5. Keep components stateless when possible
6. Use services for state management
7. Test on both touch and mouse input

## Migration to MAUI Blazor Hybrid and Web App (November 2025)

### Overview
The project was successfully migrated from a standalone Blazor WebAssembly project to a .NET MAUI Blazor Hybrid and Web App architecture. This enables the iOS 5 replica to run as:
- **Web App**: Blazor WebAssembly hosted via ASP.NET Core
- **Native Apps**: iOS, Android, Windows, and macOS via .NET MAUI

### Migration Steps Completed

1. **Created OldiOS.Shared Project**
   - Copied all source code from OldiOSExperience to OldiOS.Shared
   - Updated all namespaces from `OldiOSExperience.*` to `OldiOS.Shared.*`
   - Updated using statements and component references
   - Added comprehensive `_Imports.razor` with all necessary namespaces

2. **Service Registration**
   - Registered iOS system services in `OldiOS/MauiProgram.cs` for MAUI apps
   - Registered services in `OldiOS.Web.Client/Program.cs` for WebAssembly
   - Services include: DisplaySettings, AnimationService, BackgroundAppManager, SpringboardService, BatteryService

3. **Static Assets**
   - Copied all wwwroot assets (CSS, JavaScript, images) to OldiOS.Shared/wwwroot
   - Updated HTML files to reference scripts from `_content/OldiOS.Shared/`
   - Added JavaScript interop scripts: app-animations.js, utils.js, battery.js

4. **Render Mode Configuration**
   - Disabled server-side pre-rendering using `InteractiveWebAssemblyRenderMode(prerender: false)`
   - Ensures all services run on the client side where they belong
   - Prevents DI errors during server-side pre-rendering

5. **Removed Template Files**
   - Removed default Home.razor page that conflicted with Index.razor
   - Kept original OldiOSExperience folder for reference

### Running the Projects

**Web App:**
```bash
cd OldiOS/OldiOS.Web
dotnet run
# Navigate to https://localhost:5001 or http://localhost:5000
```

**MAUI App (requires MAUI workloads):**
```bash
cd OldiOS/OldiOS
dotnet build -f net9.0-android   # For Android
dotnet build -f net9.0-ios       # For iOS
dotnet build -f net9.0-windows   # For Windows
```

### Benefits of New Architecture

1. **Code Sharing**: All iOS 5 replica code is in one place (OldiOS.Shared)
2. **Multi-Platform**: Same codebase runs on web and native platforms
3. **Better Performance**: Native apps can leverage platform-specific optimizations
4. **Easier Maintenance**: Single source of truth for all components
5. **Future Ready**: Easy to add more platforms or deployment targets

### Known Limitations

- MAUI workloads must be installed to build native apps
- Some platform-specific features may need conditional compilation
- Image assets work best when optimized for each platform

## Bug Fixes

### 3D Touch Detection Fix (November 2025)

**Issue:** On non-3D Touch devices, the home button became unresponsive because the pressure detection logic incorrectly identified them as pressure-sensitive devices.

**Root Cause:** The detection logic in `Index.razor` assumed devices without pressure support would report `pressure = 0.5`. However, many devices report `pressure = 0` instead. The original logic `if (pressure != 0.5)` would incorrectly mark these devices as supporting pressure.

**Mozilla Documentation Insight:** Per MDN Web Docs, hardware without pressure support reports:
- `pressure = 0` when pointer is inactive (no buttons pressed)
- `pressure = 0.5` when pointer is active (buttons pressed)
- Always exactly 0 or 0.5, never intermediate values

Pressure-sensitive hardware reports continuous values between 0 and 1 based on applied force.

**Fix Evolution:**
- Original: `if (pressure != 0.5)` → incorrectly triggers on devices reporting 0
- First attempt: `if (pressure > 0.6)` → too high, missed 3D Touch devices with light initial touch
- Second attempt: `if (pressure > 0.51)` → incorrectly triggers on non-pressure devices reporting 0.5 when pressed
- Final fix: `if (pressure != 0 && pressure != 0.5)` → only triggers on intermediate values, correctly identifying pressure-capable devices

This ensures:
- Non-pressure devices (reporting exactly 0 or 0.5) fall back to pointer events
- Pressure-capable devices (reporting intermediate values like 0.51, 0.6, 0.8) use pressure detection
- Home button works correctly on all device types

**Files Modified:**
- `OldiOS.Shared/Pages/Index.razor` - Updated C# detection logic
- `OldiOS.Shared/wwwroot/js/pressure.js` - Updated JS comments for clarity

## References

- iOS Human Interface Guidelines (iOS 5 era)
- UIKit Framework documentation
- Core Animation programming guide
- SpringBoard reverse engineering resources
- .NET MAUI Blazor Hybrid documentation
- Blazor WebAssembly documentation
