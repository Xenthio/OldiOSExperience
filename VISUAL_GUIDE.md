# App State Persistence - Visual Guide

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                     User Interaction                        │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│                     Springboard                             │
│  ┌────────┐  ┌────────┐  ┌────────┐  ┌────────┐           │
│  │  App1  │  │  App2  │  │  App3  │  │  App4  │           │
│  │  📱    │  │  📧    │  │  🔢    │  │  📝    │           │
│  └────────┘  └────────┘  └────────┘  └────────┘           │
└──────────────────────┬──────────────────────────────────────┘
                       │ User taps app
                       ▼
┌─────────────────────────────────────────────────────────────┐
│                  SpringboardService                         │
│  • Manages app layout                                       │
│  • Calls AppManager.LaunchApp()                            │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│               BackgroundAppManager                          │
│  • Manages app lifecycle (Foreground/Background/Suspended)  │
│  • Saves state when moving to background                   │
│  • Restores state when returning to foreground             │
│                                                             │
│  State Storage:                                             │
│  ┌───────────────────────────────────────────────┐         │
│  │ AppState (App1):                              │         │
│  │  - ExecutionState: Foreground                 │         │
│  │  - SavedStateSnapshot: { ... }                │         │
│  │  - ComponentInstance: Calculator              │         │
│  └───────────────────────────────────────────────┘         │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│                    AppContainer                             │
│  • Renders app component with DynamicComponent             │
│  • Captures component instance                             │
│  • Calls RestoreAppStateIfAvailable()                      │
│                                                             │
│  ┌──────────────────────────────────────────────┐          │
│  │  App Content (Calculator, Notes, etc.)       │          │
│  │  implements IResumableApp                    │          │
│  │                                               │          │
│  │  SaveState() → Dictionary<string, object>    │          │
│  │  RestoreState(Dictionary<string, object>)    │          │
│  └──────────────────────────────────────────────┘          │
└─────────────────────────────────────────────────────────────┘
```

## State Lifecycle Flow

```
┌─────────────┐
│ Not Running │  ← Initial state or after force quit
└──────┬──────┘
       │
       │ User opens app
       │
       ▼
┌──────────────────────────────────────────────┐
│            Foreground                        │
│  • App is visible and running               │
│  • User can interact with it                │
│  • RestoreState() called if state exists    │
└──────┬───────────────────────────────────────┘
       │
       │ User presses Home button
       │ SaveState() called ──┐
       │                      │ State saved to
       ▼                      ▼ AppState object
┌──────────────────────────────────────────────┐
│            Background                        │
│  • App is not visible                       │
│  • State is preserved in memory             │
│  • Component instance kept alive            │
└──────┬───────────────────────────────────────┘
       │
       ├─── User reopens app ──────┐
       │                            │
       │                            ▼
       │                    ┌──────────────┐
       │                    │ Foreground   │
       │                    │ (with state) │
       │                    └──────────────┘
       │
       ├─── User force quits (X) ──┐
       │                            │
       │                            ▼
       │                    ┌──────────────┐
       │                    │ Not Running  │
       │                    │ (state clear)│
       │                    └──────────────┘
       │
       └─── System suspends ──────┐
                                   │
                                   ▼
                           ┌──────────────┐
                           │  Suspended   │
                           │ (in memory)  │
                           └──────────────┘
```

## Component Interaction Sequence

```
User Action: Open Calculator
│
├─ 1. User taps Calculator icon
│   └─→ Springboard → SpringboardService.LaunchApp(calculator)
│
├─ 2. BackgroundAppManager.LaunchApp(calculator)
│   ├─→ Move current foreground app to background
│   │   └─→ Call SaveState() on current app (if IResumableApp)
│   └─→ Set calculator to Foreground state
│
├─ 3. AppContainer renders
│   ├─→ DynamicComponent creates Calculator instance
│   └─→ OnAfterRenderAsync captures component reference
│
├─ 4. BackgroundAppManager.RestoreAppStateIfAvailable(calculator)
│   ├─→ Check if saved state exists
│   └─→ If yes, call RestoreState() on Calculator component
│
└─ 5. Calculator displays with restored state
    User sees their previous calculation!

User Action: Press Home Button
│
├─ 1. Springboard.HandleHomeButtonSinglePress()
│   └─→ SpringboardService.HandleHomeButton()
│
├─ 2. BackgroundAppManager.ReturnToSpringboard()
│   ├─→ Call SaveState() on Calculator (implements IResumableApp)
│   │   └─→ Returns { "DisplayValue": "123", "CurrentValue": 123, ... }
│   ├─→ Store state in AppState.SavedStateSnapshot
│   └─→ Set Calculator to Background state
│
└─ 3. Springboard becomes visible
    Calculator state is safely stored in memory!

User Action: Force Quit from App Switcher
│
├─ 1. User double-taps home → App switcher appears
│   └─→ Shows recent apps with [X] buttons
│
├─ 2. User taps [X] on Calculator
│   └─→ AppSwitcher calls HandleAppClose(calculator)
│
├─ 3. BackgroundAppManager.CloseApp(calculator.Id)
│   ├─→ Set state to NotRunning
│   ├─→ Clear SavedStateSnapshot ← State is deleted
│   └─→ Clear ComponentInstance
│
└─ 4. Next time Calculator opens, it starts fresh!
```

## Example: Calculator State Persistence

```
User enters: 5 + 3 (doesn't press =)

SaveState() called:
┌──────────────────────────────────────┐
│ {                                     │
│   "DisplayValue": "3",                │
│   "CurrentValue": 3,                  │
│   "StoredValue": 5,                   │
│   "CurrentOperation": "+",            │
│   "NewNumber": true                   │
│ }                                     │
└──────────────────────────────────────┘
         │
         │ Stored in AppState
         ▼
┌──────────────────────────────────────┐
│ AppState for Calculator:              │
│  - ExecutionState: Background         │
│  - SavedStateSnapshot: { ... }  ←────│
│  - ComponentInstance: Calculator      │
└──────────────────────────────────────┘

User reopens Calculator

RestoreState() called with saved snapshot:
┌──────────────────────────────────────┐
│ Calculator component receives:        │
│  - DisplayValue = "3"                 │
│  - CurrentValue = 3                   │
│  - StoredValue = 5                    │
│  - CurrentOperation = "+"             │
│  - NewNumber = true                   │
└──────────────────────────────────────┘
         │
         │ Component updates its state
         ▼
┌──────────────────────────────────────┐
│ Calculator displays "3"               │
│ Remembers "5 +" operation             │
│ Ready for user to press "=" → "8"    │
└──────────────────────────────────────┘
```

## IResumableApp Interface

```csharp
// Simple contract for state persistence
public interface IResumableApp
{
    // Called when app moves to background
    Dictionary<string, object> SaveState();
    
    // Called when app returns to foreground
    void RestoreState(Dictionary<string, object> state);
}
```

## Implementation Example

```csharp
@implements IResumableApp

@code {
    // Your app state
    private string myData = "";
    private int counter = 0;
    
    // Save state (called automatically)
    public Dictionary<string, object> SaveState()
    {
        return new Dictionary<string, object>
        {
            { "MyData", myData },
            { "Counter", counter }
        };
    }
    
    // Restore state (called automatically)
    public void RestoreState(Dictionary<string, object> state)
    {
        if (state.TryGetValue("MyData", out var data))
            myData = data.ToString() ?? "";
        
        if (state.TryGetValue("Counter", out var count))
            counter = Convert.ToInt32(count);
    }
    
    // That's it! Your app now resumes automatically!
}
```

## Key Benefits

✅ **Automatic** - No manual calls needed, lifecycle handled by system
✅ **Safe** - Exception handling prevents crashes
✅ **Simple** - Just implement 2 methods
✅ **Flexible** - Apps choose what to save/restore
✅ **Clean** - Zero breaking changes to existing apps
✅ **Efficient** - State kept in memory, no disk I/O
✅ **iOS-like** - Mimics real iOS multitasking behavior

## Apps with State Persistence

Current implementations:
- ✅ Calculator - Preserves calculations
- ✅ NotesEditor - Preserves note text

Easy to add to any app:
- Messages - Current conversation
- Safari - Current page
- Music - Current song/position
- Photos - Current album/photo
- And more!
