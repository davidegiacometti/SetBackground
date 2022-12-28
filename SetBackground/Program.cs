// Copyright (c) Davide Giacometti. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine;
using System.CommandLine.Parsing;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using SetBackground;

var rootCommand = new RootCommand();
var wallpaperCommand = new Command(
    name: "wallpaper",
    description: "Set a wallpaper as background.");

var wallpaperArgument = new Argument<string?>(
    name: "path",
    parse: result =>
    {
        if (result.Tokens.Count > 0)
        {
            var path = result.Tokens[0].Value;
            if (File.Exists(path))
            {
                return Path.GetFullPath(path);
            }
            else if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(Path.GetFullPath(path));
                if (files.Any())
                {
                    var randomIndex = RandomNumberGenerator.GetInt32(0, files.Length);
                    return files[randomIndex];
                }
                else
                {
                    result.ErrorMessage = $"{path} doesn't contain any files.";
                }
            }
            else
            {
                result.ErrorMessage = $"{path} isn't a valid path.";
            }
        }

        return null;
    },
    description: "Path of the image to use as a background. If the path is a directory a random file will be picked.");

var positionOption = new Option<Position?>(
    name: "--position",
    description: "Specify how the wallpaper is displayed.");

var monitorIdOption = new Option<uint?>(
    name: "--monitor",
    description: "Change the wallpaper only for the specified monitor ID.");

var colorCommand = new Command(
    name: "color",
    description: "Set a color as background.");

var colorArgument = new Argument<string?>(
    name: "color",
    parse: result =>
    {
        if (result.Tokens.Count > 0)
        {
            var color = result.Tokens[0].Value;
            if (ColorRegex().IsMatch(color))
            {
                return color;
            }
            else
            {
                result.ErrorMessage = $"{color} isn't a valid color.";
            }
        }

        return null;
    },
    description: "Color to set as background.");

var listMonitorsCommand = new Command(
    name: "list-monitors",
    description: "Show the list of attached monitors.");

rootCommand.AddCommand(wallpaperCommand);
wallpaperCommand.AddArgument(wallpaperArgument);
wallpaperCommand.AddOption(positionOption);
wallpaperCommand.AddOption(monitorIdOption);
wallpaperCommand.SetHandler(SetWallpaper, wallpaperArgument, positionOption, monitorIdOption);

rootCommand.AddCommand(colorCommand);
colorCommand.AddArgument(colorArgument);
colorCommand.SetHandler(SetColor, colorArgument);

rootCommand.AddCommand(listMonitorsCommand);
listMonitorsCommand.SetHandler(ListMonitors);

await rootCommand.InvokeAsync(args);

static void SetWallpaper(string? path, Position? position, uint? monitorId)
{
    if (path == null)
    {
        return;
    }

    DesktopWallpaperHelper.SetDesktopWallpaper(path, monitorId);

    if (position.HasValue)
    {
        DesktopWallpaperHelper.SetPosition(position.Value);
    }
}

static void SetColor(string? color)
{
    if (color == null)
    {
        return;
    }

    DesktopWallpaperHelper.SetBackgroundColor(color);
}

static void ListMonitors()
{
    var monitors = DesktopWallpaperHelper.ListMonitors();

    foreach (var m in monitors)
    {
        Console.WriteLine("{0} - {1}", m.Id, m.DeviceString);
    }
}

public partial class Program
{
    [GeneratedRegex("[#][0-9A-Fa-f]{6}\\b")]
    private static partial Regex ColorRegex();
}
