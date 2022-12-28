// Copyright (c) Davide Giacometti. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.Shell;

namespace SetBackground
{
    public static class DesktopWallpaperHelper
    {
        public static void SetDesktopWallpaper(string path, uint? singleMonitorId)
        {
            var desktopWallpaper = (IDesktopWallpaper)new DesktopWallpaper();
            unsafe
            {
                uint count;
                desktopWallpaper.GetMonitorDevicePathCount(&count);

                if (singleMonitorId.HasValue && singleMonitorId >= count)
                {
                    Console.WriteLine("The specified monitor ID isn't valid.");
                    return;
                }

                var monitorIndex = singleMonitorId.GetValueOrDefault(0);
                var monitorCount = singleMonitorId.HasValue ? singleMonitorId + 1 : count;

                for (uint i = monitorIndex; i < monitorCount; i++)
                {
                    PWSTR monitorId;
                    desktopWallpaper.GetMonitorDevicePathAt(i, &monitorId);
                    fixed (char* p = path)
                    {
                        var mId = monitorId.Value;
                        var wallpaper = new PCWSTR(p);
                        desktopWallpaper.SetWallpaper(mId, wallpaper);
                        desktopWallpaper.Enable(true);
                    }
                }
            }
        }

        public static void SetBackgroundColor(string colorHex)
        {
            var desktopWallpaper = (IDesktopWallpaper)new DesktopWallpaper();
            var color = ColorTranslator.FromHtml(colorHex);
            var coloref = (COLORREF)(color.R | (uint)color.G << 8 | (uint)color.B << 16);
            desktopWallpaper.SetBackgroundColor(coloref);
            desktopWallpaper.Enable(false);
        }

        public static void SetPosition(Position position)
        {
            var desktopWallpaperPosition = position switch
            {
                Position.Center => DESKTOP_WALLPAPER_POSITION.DWPOS_CENTER,
                Position.Tile => DESKTOP_WALLPAPER_POSITION.DWPOS_TILE,
                Position.Stretch => DESKTOP_WALLPAPER_POSITION.DWPOS_STRETCH,
                Position.Fit => DESKTOP_WALLPAPER_POSITION.DWPOS_FIT,
                Position.Fill => DESKTOP_WALLPAPER_POSITION.DWPOS_FILL,
                Position.Span => DESKTOP_WALLPAPER_POSITION.DWPOS_SPAN,
                _ => throw new ArgumentException("The position isn't valid", nameof(position)),
            };

            var desktopWallpaper = (IDesktopWallpaper)new DesktopWallpaper();
            desktopWallpaper.SetPosition(desktopWallpaperPosition);
        }

        public static List<(uint Id, string DeviceString)> ListMonitors()
        {
            var monitors = new List<(uint, string)>();

            var desktopWallpaper = (IDesktopWallpaper)new DesktopWallpaper();
            unsafe
            {
                uint count;
                desktopWallpaper.GetMonitorDevicePathCount(&count);

                for (uint i = 0; i < count; i++)
                {
                    DISPLAY_DEVICEW displayDevice = default;
                    displayDevice.cb = (uint)Marshal.SizeOf(displayDevice);

                    if (PInvoke.EnumDisplayDevices(null, i, &displayDevice, 256))
                    {
                        if (PInvoke.EnumDisplayDevices(displayDevice.DeviceName.ToString(), 0, ref displayDevice, PInvoke.EDD_GET_DEVICE_INTERFACE_NAME))
                        {
                            monitors.Add((i, displayDevice.DeviceString.ToString()));
                        }
                    }
                }
            }

            return monitors;
        }
    }
}
