# App State Persistence Guide

## Overview

The iOS Experience simulator now includes a complete app state persistence system that allows apps to save and restore their state when moved to background and brought back to foreground. This mimics iOS's multitasking behavior where apps resume where you left off.

## How It Works

### System Architecture

1. **IResumableApp Interface**: Apps that want to support state persistence implement this interface
2. **BackgroundAppManager**: Automatically saves/restores state during app lifecycle transitions
3. **AppContainer**: Coordinates state restoration after component rendering
4. **AppState Model**: Stores serialized state snapshot and component instance

### State Lifecycle

```
App Lifecycle:
┌─────────────┐
│ Not Running │
└──────┬──────┘
       │ Launch
       ▼
┌─────────────┐     Home Button     ┌────────────┐
│ Foreground  │────────────────────▶│ Background │
│             │◀────────────────────│            │
└──────┬──────┘    Reopen App       └─────┬──────┘
       │                                   │
       │ State Restore                     │ State Save
       │ (if resumable)                    │ (if resumable)
       │                                   │
       └───────────────────────────────────┘

       Force Quit (X in App Switcher)
                    │
                    ▼
            State Cleared
                    │
                    ▼
             Not Running
```

## Implementing State Persistence in Your App

### Step 1: Implement IResumableApp

Add the interface to your app component:

```csharp
@using OldiOS.Shared.Models
@implements IResumableApp

@code {
    // Your app state variables
    private string myData = "";
    private int counter = 0;
    
    // Implement SaveState
    public Dictionary<string, object> SaveState()
    {
        return new Dictionary<string, object>
        {
            { "MyData", myData },
            { "Counter", counter }
        };
    }
    
    // Implement RestoreState
    public void RestoreState(Dictionary<string, object> state)
    {
        if (state.TryGetValue("MyData", out var data))
            myData = data.ToString() ?? "";
        
        if (state.TryGetValue("Counter", out var count))
            counter = Convert.ToInt32(count);
    }
}
```

### Step 2: Save All Relevant State

Include all data needed to restore the user's experience:
- Form inputs
- Scroll positions
- Tab selections
- Calculation results
- Text being edited
- Any temporary data

### Step 3: Handle Restoration Gracefully

Always provide default values in RestoreState:

```csharp
public void RestoreState(Dictionary<string, object> state)
{
    // Use null-coalescing and defaults
    if (state.TryGetValue("Text", out var text))
        myText = text.ToString() ?? "Default";
    
    // Use Convert with try-catch for numbers
    if (state.TryGetValue("Number", out var num))
    {
        try 
        {
            myNumber = Convert.ToDecimal(num);
        }
        catch
        {
            myNumber = 0;
        }
    }
}
```

## Examples

### Calculator App (Full Implementation)

```csharp
@using OldiOS.Shared.Models
@implements IResumableApp

@code {
    private string DisplayValue = "0";
    private decimal CurrentValue = 0;
    private decimal StoredValue = 0;
    private string CurrentOperation = "";
    private bool NewNumber = true;
    
    public Dictionary<string, object> SaveState()
    {
        return new Dictionary<string, object>
        {
            { "DisplayValue", DisplayValue },
            { "CurrentValue", CurrentValue },
            { "StoredValue", StoredValue },
            { "CurrentOperation", CurrentOperation },
            { "NewNumber", NewNumber }
        };
    }
    
    public void RestoreState(Dictionary<string, object> state)
    {
        if (state.TryGetValue("DisplayValue", out var displayValue))
            DisplayValue = displayValue.ToString() ?? "0";
        
        if (state.TryGetValue("CurrentValue", out var currentValue))
            CurrentValue = Convert.ToDecimal(currentValue);
        
        if (state.TryGetValue("StoredValue", out var storedValue))
            StoredValue = Convert.ToDecimal(storedValue);
        
        if (state.TryGetValue("CurrentOperation", out var currentOperation))
            CurrentOperation = currentOperation.ToString() ?? "";
        
        if (state.TryGetValue("NewNumber", out var newNumber))
            NewNumber = Convert.ToBoolean(newNumber);
    }
}
```

### Notes Editor (Text Preservation)

```csharp
@using OldiOS.Shared.Models
@implements IResumableApp

@code {
    private string noteTitle = "New Note";
    private string noteContent = "";
    private string lastModified = "November 9, 2013";
    
    public Dictionary<string, object> SaveState()
    {
        return new Dictionary<string, object>
        {
            { "NoteTitle", noteTitle },
            { "NoteContent", noteContent },
            { "LastModified", lastModified }
        };
    }
    
    public void RestoreState(Dictionary<string, object> state)
    {
        if (state.TryGetValue("NoteTitle", out var title))
            noteTitle = title.ToString() ?? "New Note";
        
        if (state.TryGetValue("NoteContent", out var content))
            noteContent = content.ToString() ?? "";
        
        if (state.TryGetValue("LastModified", out var modified))
            lastModified = modified.ToString() ?? "November 9, 2013";
    }
}
```

## Testing Your Implementation

### Manual Testing Steps

1. **Open your app** from the home screen
2. **Make some changes** (enter data, perform actions)
3. **Press the home button** to return to springboard
4. **Reopen your app** from the home screen
5. **Verify state is restored** - all your changes should still be there

### Testing Force Quit

1. **Open your app** and make changes
2. **Double-press home button** to show app switcher
3. **Tap the X button** on your app to force quit
4. **Reopen your app** from the home screen
5. **Verify state is cleared** - app should start fresh

### Testing Multiple Apps

1. **Open App A** and make changes
2. **Press home button**
3. **Open App B** and make changes
4. **Press home button**
5. **Reopen App A** - should show App A's state
6. **Press home button**
7. **Reopen App B** - should show App B's state

## Best Practices

### ✅ DO

- Save all user-entered data
- Use defensive coding in RestoreState (null checks, try-catch)
- Test both save and restore paths thoroughly
- Keep state dictionaries simple (primitives, strings, numbers)
- Clear temporary/sensitive data in force quit scenarios

### ❌ DON'T

- Store large binary data (images, videos) in state
- Save network-fetched data (re-fetch on resume instead)
- Assume state will always be available
- Store sensitive data without encryption
- Rely on state persistence for critical data (use proper storage)

## Technical Details

### BackgroundAppManager Integration

The BackgroundAppManager automatically handles state persistence:

```csharp
private void MoveToBackground(AppState appState)
{
    appState.ExecutionState = AppExecutionState.Background;
    appState.BackgroundedAt = DateTime.Now;
    
    // Automatic state save
    if (appState.ComponentInstance is IResumableApp resumableApp)
    {
        try
        {
            appState.SavedStateSnapshot = resumableApp.SaveState();
        }
        catch
        {
            appState.SavedStateSnapshot = null;
        }
    }
}
```

### State Restoration Flow

1. User reopens app
2. AppContainer renders component with DynamicComponent
3. OnAfterRenderAsync captures component instance
4. BackgroundAppManager.RestoreAppStateIfAvailable() is called
5. If saved state exists and component implements IResumableApp, state is restored
6. Component re-renders with restored state

## Troubleshooting

### State Not Restoring

- Ensure your component implements IResumableApp
- Check that SaveState() returns a valid dictionary
- Verify RestoreState() properly handles the state data
- Check browser console for any exceptions

### State Cleared Unexpectedly

- Check if force quit was used (expected behavior)
- Verify app wasn't removed from background apps list
- Check if SaveState() is throwing exceptions (state won't be saved)

### Partial State Restoration

- Ensure all state variables are included in SaveState()
- Check for typos in dictionary keys
- Verify data types match between save and restore

## Migration Guide

### Updating Existing Apps

1. Add `@implements IResumableApp` to your component
2. Identify all state variables that need persistence
3. Implement SaveState() method
4. Implement RestoreState() method
5. Test thoroughly with the testing steps above

### Apps That Don't Need State Persistence

If your app doesn't need state persistence:
- Simply don't implement IResumableApp
- The app will start fresh each time it's opened
- This is fine for apps like Calculator (if you prefer) or simple utility apps

## Future Enhancements

Potential future improvements to the system:
- Persistent storage (localStorage) for cross-session state
- State compression for large state objects
- State versioning for app updates
- Automatic state migration
- State encryption for sensitive data
