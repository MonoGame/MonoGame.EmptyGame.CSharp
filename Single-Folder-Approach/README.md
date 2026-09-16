# Empty-Game

A clean, empty [MonoGame](https://monogame.net/) project for every supported desktop and mobile platform, built with the MonoGame Content Builder.

This solution was generated from the **MonoGame Empty Game** template (`mg-emptygame`). It contains a minimal `Game1` class and no content.

## Solution layout

| Path | Purpose |
| --- | --- |
| `Source/` | Your game code. Start in `Game1.cs`. `Program.cs` and `MainActivity.cs` are the platform entry points. |
| `Resources/` | Your app identity (`Identity.props`), icons and per-platform manifests. |
| `Content/` | The Content Builder project. Raw assets go in `Content/Assets/` and are registered in `Content/Builder/Builder.cs`. Content is built as part of every platform build. |
| `EmptyGame.*.csproj` | One project per platform, next to the solution file. Every project compiles the same `Source/` folder. |

`Source/`, `Resources/` and `Content/` are yours. The project files at the root belong to the template and are the same for every game built from it, so you can regenerate them later to pick up fixes or add a platform without losing your work.

## Naming your game

Everything that identifies your game lives in [`Resources/Identity.props`](Resources/Identity.props):

| Property | What it sets |
| --- | --- |
| `AssemblyName`, `AssemblyTitle` | The name of the built executable. |
| `ApplicationTitle` | The display name on the iOS home screen. |
| `ApplicationId` | Your unique app identity: the Android package name and the iOS bundle identifier. Change it before publishing to a store. |
| `ApplicationVersion` | Build number, incremented every store submission. |
| `ApplicationDisplayVersion` | Public version string. |

`ApplicationId` starts as `com.companyname.` plus your project name in lowercase with only letters and digits, because iOS bundle identifiers forbid underscores and Android package names forbid hyphens. A name that starts with a digit or is a Java reserved word gains an `app` prefix, because the Android package name is a Java package name. Replace `companyname` with your own before publishing.

Every platform project imports this file. The one exception is the Android launcher name, which comes from `Resources/Android/Values/strings.xml` so it can be localised.

## Replacing the placeholder art

All of the project artwork is under `Resources/`. Replace the files in place and keep the names.

| Path | Used by |
| --- | --- |
| `Icon.ico`, `Icon.bmp` | The desktop projects. |
| `Android/drawable-*/icon.png`, `splash.png` | The Android launcher icon and splash screen, one per screen density. |
| `iOS/AppIcon.xcassets/` | The iOS app icon, one image per size. |
| `macOS/Icon.icns` | A macOS `.app` bundle, if you package one by hand. |

Each platform subfolder also holds that platform's manifests: `Android/AndroidManifest.xml`, `iOS/Info.plist` with `Entitlements.plist` and `LaunchScreen.storyboard`, `Windows/app.manifest` and `Windows/app.WindowsDX.manifest` for DPI awareness, and `macOS/Info.plist` for a hand-packaged bundle. Nothing in the build reads the macOS plist.

## Platform-specific code

Everything in `Source/` is compiled into every platform project. Platform-specific code goes behind the compiler symbols the .NET SDK defines per target framework: `__IOS__`, `__ANDROID__` and `WINDOWS` (the `IOS` and `ANDROID` spellings work too). The template does this in `Game1.cs`, `Program.cs` and `MainActivity.cs`, which is wrapped in `__ANDROID__` from top to bottom so it compiles to nothing elsewhere. Copy that pattern for your own platform-only files.

For a platform-only reference, such as a native binding project or a NuGet package, add a `Directory.Build.targets` next to the solution file and condition on `$(MonoGamePlatform)`. MSBuild imports it into every project automatically:

```xml
<Project>
  <ItemGroup Condition="'$(MonoGamePlatform)' == 'Android'">
    <ProjectReference Include="MyNativeBindings\MyNativeBindings.csproj" />
  </ItemGroup>
</Project>
```

## Getting started

```sh
dotnet build EmptyGame.sln
dotnet run --project EmptyGame.DesktopGL.csproj
```

> [!IMPORTANT]
> Build the solution once first. The Content Builder is a separate project that the platforms invoke rather than reference, so it needs one solution-wide restore before a single project will build on its own. Because every project sits in the root folder, always name the `.sln` or `.csproj` you mean. A bare `dotnet build` or `dotnet run` stops with "more than one project or solution file".

Or open the solution in Visual Studio, VS Code or Rider and pick a platform launch profile. The `.vscode` folder holds one build task and one launch profile per selected platform.

## Adding content

1. Drop asset files into `Content/Assets/`.
2. Register them in `Content/Builder/Builder.cs` (see the commented examples in `Builder.GetContentCollection()`).
3. Build. The Content Builder runs before compilation and the built content is bundled per platform.
4. Load assets in code with `Content.Load<T>("assetName")`.

## Notes

- MonoGame package versions are managed centrally in `Directory.Packages.props`.
- `Directory.Build.props` gives each platform project its own `bin/` and `obj/` subfolder named after the platform, such as `bin/DesktopGL/`. Six projects in one folder would otherwise overwrite each other's `obj/project.assets.json` on restore. It also switches off the SDK's default file globbing for the platform projects, so each one lists exactly the files it uses instead of sweeping the whole folder into every build. `Content/Content.csproj` is left alone.

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
