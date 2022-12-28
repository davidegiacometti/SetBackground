# SetBackground

Simple command line utility that allow you to quickly change desktop background.

## Requirements

- Windows 8/8.1/10/11 x64
- [.NET 7.x Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

## Usage

_Set a solid color as background_
```
.\SetBackground.exe color "#225e89"

```
_Set a wallpaper as background_
```
.\SetBackground.exe wallpaper "D:\Wallpapers\1.png"
```

_Set a random wallpaper from the directory as background_
```
.\SetBackground.exe wallpaper "D:\Wallpapers"
```

_Show the list of attached monitors_
```
.\SetBackground.exe list-monitors
0 - Generic PnP Monitor
1 - Dell U2414H(HDMI2)
```

_Set a wallpaper as background only for the specified monitor and set how the wallpaper is displayed_
```
.\SetBackground.exe wallpaper "D:\Wallpapers\1.png" --monitor 1 --position Stretch
```