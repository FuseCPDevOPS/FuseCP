// Copyright (C) 2026 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Reflection;
using System.Web;

namespace FuseCP.WebPortal;

internal static class PortalPathResolver
{
    public static string MapPath(string virtualOrRelativePath)
    {
        if (string.IsNullOrWhiteSpace(virtualOrRelativePath))
            return ResolveFromBaseDirectory(".");

        string path = virtualOrRelativePath.Trim();
        if (Path.IsPathRooted(path))
            return path;

        HttpContext context = HttpContext.Current;
        if (context?.Server != null)
            return context.Server.MapPath(path);

        string hostingMappedPath = TryMapPathViaHostingEnvironment(path);
        if (!string.IsNullOrEmpty(hostingMappedPath))
            return hostingMappedPath;

        return ResolveFromBaseDirectory(path);
    }

    private static string TryMapPathViaHostingEnvironment(string path)
    {
        try
        {
            Type hostingEnvironmentType = Type.GetType("System.Web.Hosting.HostingEnvironment, System.Web", throwOnError: false);
            MethodInfo mapPath = hostingEnvironmentType?.GetMethod("MapPath", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
            return mapPath?.Invoke(null, new object[] { path }) as string;
        }
        catch (System.Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
        {
            return null;
        }
    }

    private static string ResolveFromBaseDirectory(string path)
    {
        string baseDirectory = AppContext.BaseDirectory;
        string normalizedBaseDirectory = baseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        if (normalizedBaseDirectory.EndsWith("bin", StringComparison.OrdinalIgnoreCase))
        {
            baseDirectory = Directory.GetParent(baseDirectory)?.FullName ?? baseDirectory;
        }

        string relativePath = NormalizeVirtualPath(path);
        return Path.GetFullPath(Path.Join(baseDirectory, relativePath));
    }

    private static string NormalizeVirtualPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return ".";

        string normalized = path.Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);
        if (normalized.StartsWith("~" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            normalized = normalized.Substring(2);
        else if (normalized.StartsWith("~", StringComparison.Ordinal))
            normalized = normalized.Substring(1);

        return normalized.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
}
