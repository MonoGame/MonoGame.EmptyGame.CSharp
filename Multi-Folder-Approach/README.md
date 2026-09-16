# Empty-Game

A clean, empty [MonoGame](https://monogame.net/) project for all supported desktop and mobile platforms, using the new MonoGame Content Builder.

It contains no game code beyond a minimal `Game1` class and no content - a blank canvas for building any game.

## Solution layout

| Folder | Purpose |
| --- | --- |
| `Source/` | Shared game code, compiled into every platform project. Start in `Game1.cs`. |
| `Source/Resources/` | Your game's identity, icons and platform art, referenced by every platform. One copy of each asset, no duplicates per platform. |
| `Content/` | The Content Builder project. Put your raw assets under `Content/Assets/` and register them in `Content/Builder/Builder.cs`. Content is built automatically as part of each platform build. |
| `DesktopGL/` | Cross-platform OpenGL desktop (Windows, Linux, macOS). |
| `DesktopVK/` | Cross-platform Vulkan desktop (Windows, Linux, macOS). |
| `WindowsDX/` | Windows DirectX 11 desktop. |
| `WindowsDX12/` | Windows DirectX 12 desktop. |
| `Android/` | Android mobile (`net10.0-android`). |
| `iOS/` | iOS mobile (`net10.0-ios`, requires a Mac to build/deploy). |

Each platform folder contains only its entry point and platform-specific configuration; all game logic lives once in `Source/`.

## Naming your game

Everything that identifies your game lives in one file, [`Source/Resources/Identity.props`](Source/Resources/Identity.props). Every platform project imports it, so a value set there applies everywhere and no platform project file needs editing.

> [!TIP]
> You can override any of these by editing the `csproj`, manifest or `Info.plist` directly, if you prefer.

| Property | What it sets |
| --- | --- |
| `AssemblyName`, `AssemblyTitle` | The name of the built executable. |
| `ApplicationTitle` | The display name players see on the iOS home screen. |
| `ApplicationId` | Your unique app identity. Becomes the Android package name and the iOS bundle identifier. Change this before publishing to any store. |
| `ApplicationVersion` | Build number, incremented every store submission. Android `versionCode`, iOS `CFBundleVersion`. |
| `ApplicationDisplayVersion` | Public version string. Android `versionName`, iOS `CFBundleShortVersionString`. |

> [!NOTE]
> The Android name shown under the launcher icon is the one exception: it comes from `Source/Resources/Android/Values/strings.xml`, because Android reads it from a string resource so it can be localised.

## Getting started

```sh
dotnet build
dotnet run --project DesktopGL
```

> [!IMPORTANT]
> Build the solution once first. The Content Builder is a separate project that the platforms invoke rather than reference, so it needs one solution-wide restore before a single project will build on its own. Because every project sits in the root folder, always name the `.sln` or `.csproj` you mean. A bare `dotnet build` or `dotnet run` stops with "more than one project or solution file".

Or open the solution in Visual Studio, VS Code or Rider and pick a platform launch profile. The `.vscode` folder holds one build task and one launch profile per selected platform.

## Adding content

1. Drop asset files into `Content/Assets/`.
2. Register them in `Content/Builder/Builder.cs` (see the commented examples in `Builder.GetContentCollection()`).
3. Build - the Content Builder runs automatically before compilation and the built content is bundled per platform.
4. Load assets in code with `Content.Load<T>("assetName")`.

## Replacing the placeholder art

All project artwork is located under `Source/Resources/`, linked into the projects that need them. Replace the files in place and keep the names, and every platform that uses one picks up the change.

| Path | Used by |
| --- | --- |
| `Icon.png` | The 1024px master, also linked in as the iOS marketing icon. |
| `Icon.ico`, `Icon.bmp` | All four desktop platforms. |
| `Android/drawable-*/icon.png` | The Android launcher icon, one per screen density. |
| `Android/drawable-*/splash.png` | The Android splash screen. |
| `Android/Values/` | The resource entries that reference the Android art, plus the app name. |
| `iOS/AppIcon.xcassets/` | The remaining iOS app icon sizes. |
| `macOS/Icon.icns` | A macOS `.app` bundle, if you package one by hand. |

Each platform subfolder also holds that platform's manifest, where it has one:

- `iOS/Info.plist` sets supported orientations, status bar and device family for mobile devices.
- `macOS/Info.plist` describes a macOS `.app` bundle, for the DesktopGL and DesktopVK platforms only. It is a template: `macOS/MacBundle.targets` fills in the `$(...)` tokens from `Identity.props` and writes the result next to the binaries. Edit this file, not the generated copy.

## Notes

The MonoGame version is managed centrally in `Directory.Packages.props`, to make it easier to update the entire project at once. You can still override it per project if you need to.

<!--#if (false) -->
## Using this template locally

This section is for working on the template itself and is removed from generated projects.

### Installing

`dotnet new` only creates from templates it has indexed. To index this folder into a throwaway hive to leave the machine's template list untouched:

```sh
dotnet new install . --debug:custom-hive ../mg-hive
dotnet new mg-emptygame -n MyGame --Platforms gl -o ../MyGame --debug:custom-hive ../mg-hive
```

Or install normally, so the short name works without the extra flag:

```sh
dotnet new install .
dotnet new mg-emptygame -n MyGame --Platforms gl -o ../MyGame
dotnet new uninstall .
```

Either way the install is a live link to this folder: edits here are picked up by the next `dotnet new` without reinstalling, except changes to `template.json`, which need `dotnet new install` again (add `--force` if it reports the template is already installed). Always pass `-o` so the output does not land inside this folder, because anything generated here would be swept into the next project you create.

### Options

Separate `--Platforms` values with spaces, not commas. The choices are `gl`, `vk`, `dx`, `dx12`, `android` and `ios`, and the default is `gl`. Add `--GameTitle "My Game"` to give the game a display name different from the project name. `dotnet new mg-emptygame --help` lists everything.

```sh
dotnet new mg-emptygame -n MyGame --Platforms gl vk dx dx12 -o ../MyGame
```

### Project names

The name passed with `-n` becomes the namespace, the project file names and the assembly name, so the template reduces it to a valid C# identifier that cannot collide with the template's own types. Spaces, hyphens, dots and other punctuation become underscores, a leading digit gains one in front, and a C# keyword, `Game1` or `Program` gains one at the end. So `-n "My Game"` produces `My_Game.DesktopGL.csproj` and `namespace My_Game`, and `-n class` produces `class_`. The display name players see is kept as typed, in `Resources/Identity.props`, and XML-escaped where it lands in XML, so a game called `Rock & Roll` still produces a valid `Identity.props`. `ApplicationId` is derived separately, from letters and digits only and in lower case, and gains an `app` prefix if the result starts with a digit or is a Java reserved word, so `My Game` gives `com.companyname.mygame`, `3D Game` gives `com.companyname.app3dgame` and `class` gives `com.companyname.appclass`. Every project GUID is regenerated per project.

### Building what you generated

```sh
cd MyGame
dotnet build MyGame.sln
dotnet run --project MyGame.DesktopGL.csproj
```

Check Visual Studio as well as `dotnet build`. Visual Studio's MSBuild runs on .NET Framework, so a property function that exists only in .NET Core builds on the command line and fails in the IDE.

### Trying a change

Delete the previous output folder and generate again, or add `--force` to overwrite in place.

### Seeing the template options

The template describes its own options. Ask it rather than reading `template.json`:

```sh
dotnet new mg-emptygame --help
```

If you installed into a throwaway hive, add `--debug:custom-hive ../mg-hive` to that command too.

```text
MonoGame Empty Game (C#)
Author: MonoGame Foundation
Description: A clean, empty MonoGame solution for all supported desktop and mobile platforms, using the new Content Builder. A single shared Game1 class, an empty content project and one project per platform - a blank canvas for any game.

Usage:
  dotnet new mg-emptygame [options] [template options]

Options:
  -n, --name <name>       The name for the output being created. If no name is specified, the name of the output directory is used.
  -o, --output <output>   Location to place the generated output.
  --dry-run               Displays a summary of what would happen if the given command line were run if it would result in a template creation. [default: False]
  --force                 Forces content to be generated even if it would change existing files. [default: False]
  --no-update-check       Disables checking for the template package updates when instantiating a template. [default: False]
  --project <project>     The project that should be used for context evaluation.
  -lang, --language <C#>  Specifies the template language to instantiate.
  --type <project>        Specifies the template type to instantiate.

Template options:
  -G, --GameTitle <GameTitle>                  The display name for your game that players will see. This is also used as the assembly name. Defaults to the project name.
                                               Type: text
  -P, --Platforms <android|dx|dx12|gl|ios|vk>  Target platforms.
                                               Type: choice
                                                 gl       Cross-platform OpenGL desktop
                                                 vk       Cross-platform Vulkan desktop
                                                 dx       Windows DirectX 11 desktop
                                                 dx12     Windows DirectX 12 desktop
                                                 android  Android mobile
                                                 ios      iOS mobile
                                               Multiple values are allowed: True
                                               Default: gl
```

`Template options` at the bottom is the part specific to this template. Everything above it is standard `dotnet new`. Both template options have short aliases, so `-P gl vk` works as well as `--Platforms gl vk`.

<!--#endif -->
