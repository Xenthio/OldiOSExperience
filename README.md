# OldiOSExperience

An authentic iOS 5 replica built with Blazor, simulating an iPhone 4 experience in your browser.

## 🌐 Live Demo

**[Try it live on GitHub Pages](https://xenthio.github.io/OldiOSExperience/)**

## 📱 About

This project is a faithful recreation of the iOS 5 operating system interface, built using .NET Blazor WebAssembly. It replicates the look, feel, and behavior of an iPhone 4 running iOS 5, including:

- **Springboard (Home Screen)** with page swiping and app icons
- **Lock Screen** with slide-to-unlock
- **Multitasking** (App Switcher via double-tap home button)
- **22+ Built-in Apps** including Phone, Messages, Mail, Music, Photos, Safari, and more
- **iOS 5 UIKit Components** for authentic styling
- **System Services** mirroring iOS architecture (SpringBoard, Background App Manager, etc.)

## 🏗️ Architecture

The project uses a modern .NET MAUI Blazor Hybrid and Web App architecture:

- **OldiOS.Web** - Standalone Blazor WebAssembly project (web deployment)
- **OldiOS.Shared** - Shared Razor component library (UI components and services)
- **OldiOS** - .NET MAUI project (for native iOS, Android, Windows, macOS apps)

All iOS 5 replica code lives in `OldiOS.Shared` and is shared across platforms.

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Running Locally

```bash
# Clone the repository
git clone https://github.com/Xenthio/OldiOSExperience.git
cd OldiOSExperience

# Build and run the web app
cd OldiOS/OldiOS.Web
dotnet run

# Navigate to http://localhost:5000 or https://localhost:5001
```

### Building for Production

```bash
cd OldiOS/OldiOS.Web
dotnet publish -c Release
# Output will be in bin/Release/net9.0/publish/wwwroot
```

## 📦 Deployment

The web app is automatically deployed to GitHub Pages on every push to the `main` branch via GitHub Actions.

**Workflow**: `.github/workflows/deploy-gh-pages.yml`

The deployment process:
1. Builds the Blazor WebAssembly project
2. Updates the base path for subdirectory deployment
3. Deploys to GitHub Pages using the official actions

See [COPILOT.MD](COPILOT.MD#deployment) for detailed deployment documentation.

## 🎨 Features

### Implemented Apps (22+)

**Communication & Productivity:**
- Phone, Messages, Mail, FaceTime, Contacts
- Calendar, Notes, Reminders

**Media & Entertainment:**
- Music/iPod, Photos, Camera, Videos, YouTube

**Utilities & Tools:**
- Clock, Weather, Stocks, Calculator, Maps, Safari, Compass, Voice Memos

**App Store & Gaming:**
- App Store, iTunes Store, Game Center

**System:**
- Settings (with comprehensive settings panels)

### UIKit Components

Built-in iOS 5 styled components for building apps:
- UIButton, UITextField, UILabel
- UISwitch, UISegmentedControl, UISlider
- UITabBar, UIToolbar, UISearchBar
- UIActivityIndicator

### Animations

Authentic iOS 5 animations including:
- App open/close with zoom and radial scatter
- Lock screen unlock sequence
- Page swiping with rubber-band physics
- Smooth transitions and easing curves

## 📖 Documentation

For detailed architecture notes, implementation details, and contribution guidelines, see [COPILOT.MD](COPILOT.MD).

## 🛠️ Development

### Project Structure

```
OldiOS/
├── OldiOS/              # .NET MAUI native app project
├── OldiOS.Shared/       # Shared Blazor component library
│   ├── Apps/            # Individual app implementations
│   ├── Models/          # Data models
│   ├── Services/        # Core system services
│   ├── System/          # System UI components
│   └── Pages/           # Page-level components
└── OldiOS.Web/          # Blazor WebAssembly project
```

### Building for Native Platforms

To build native apps, you'll need the .NET MAUI workloads:

```bash
dotnet workload install maui

# Build for specific platforms
cd OldiOS/OldiOS
dotnet build -f net9.0-android   # Android
dotnet build -f net9.0-ios       # iOS
dotnet build -f net9.0-windows   # Windows
```

## 🤝 Contributing

Contributions are welcome! Please follow the guidelines in [COPILOT.MD](COPILOT.MD#contributing-guidelines).

## 📝 License

This is a personal project recreating the iOS 5 interface for educational and nostalgic purposes. iOS and iPhone are trademarks of Apple Inc.

## 🙏 Acknowledgments

- Inspired by iOS 5 and the iPhone 4
- Built with [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
- Deployed with [GitHub Pages](https://pages.github.com/)
