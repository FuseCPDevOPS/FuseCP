// Copyright (C) 2025 FuseCP
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
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuseCP.EnterpriseServer;

    public class FileUtils
    {
	private static string EnsurePathUnderRoot(string rootPath, string relativePath)
	{
		string normalizedRoot = Path.GetFullPath(rootPath);
		string candidate = Path.GetFullPath(Path.Join(normalizedRoot, relativePath ?? String.Empty));
		if (!candidate.StartsWith(normalizedRoot.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar,
			StringComparison.OrdinalIgnoreCase) &&
			!String.Equals(candidate, normalizedRoot, StringComparison.OrdinalIgnoreCase))
		{
			throw new InvalidOperationException("Path escapes the allowed root directory.");
		}
		return candidate;
	}

	#region Zip/Unzip Methods

	public static void ZipFiles(string zipFile, string rootPath, string[] files)
	{
		using (var stream = new FileStream(zipFile, FileMode.Truncate, FileAccess.Write))
		using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, false, Encoding.UTF8))
		{
			foreach (string file in files)
			{
				string fullPath = EnsurePathUnderRoot(rootPath, file);
				if (Directory.Exists(fullPath))
				{
					//add directory with the same directory name
					zip.CreateEntry(file.Replace('\\', '/') + "/", CompressionLevel.Optimal);
				}
				else if (File.Exists(fullPath))
				{
					//add file to the root folder
					zip.CreateEntryFromFile(fullPath, file.Replace('\\', '/'), CompressionLevel.Optimal);
				}
			}
		}
	}

	public static List<string> UnzipFiles(string zipFile, string destFolder)
	{
		string normalizedDestination = Path.GetFullPath(destFolder);
		Directory.CreateDirectory(normalizedDestination);

		using (ZipArchive archive = ZipFile.OpenRead(zipFile))
		{
			string destinationRoot = normalizedDestination.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
			foreach (ZipArchiveEntry entry in archive.Entries)
			{
				string entryName = entry.FullName.Replace('/', Path.DirectorySeparatorChar);
				if (Path.IsPathRooted(entryName) || entryName.Contains(".." + Path.DirectorySeparatorChar) || entryName.StartsWith(".."))
					throw new InvalidDataException($"Archive entry '{entry.FullName}' has an invalid path.");

				string destinationPath = Path.GetFullPath(Path.Join(normalizedDestination, entryName));
				if (!destinationPath.StartsWith(destinationRoot, StringComparison.OrdinalIgnoreCase) &&
					!String.Equals(destinationPath, normalizedDestination, StringComparison.OrdinalIgnoreCase))
				{
					throw new InvalidDataException($"Archive entry '{entry.FullName}' escapes destination folder.");
				}
				if (String.IsNullOrEmpty(entry.Name))
				{
					Directory.CreateDirectory(destinationPath);
					continue;
				}

				string destinationDirectory = Path.GetDirectoryName(destinationPath);
				if (!String.IsNullOrEmpty(destinationDirectory))
					Directory.CreateDirectory(destinationDirectory);

				entry.ExtractToFile(destinationPath, true);
			}
		}

		// return extracted files names
		return GetFileNames(destFolder);
	}

	#endregion

	#region Copy
	public static void CopyDirectoryContentUNC(string sourceDirectory, string destinationDirectory) {
			sourceDirectory = Path.GetFullPath(sourceDirectory);
			destinationDirectory = Path.GetFullPath(destinationDirectory);
			foreach(var dir in Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories)) {
				var destinationPath = EnsurePathUnderRoot(destinationDirectory, Path.GetRelativePath(sourceDirectory, dir));
				if(!Directory.Exists(destinationPath)) {
					Directory.CreateDirectory(destinationPath);
                }
            }
            
			foreach(var file in Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories)) {
				var destinationPath = EnsurePathUnderRoot(destinationDirectory, Path.GetRelativePath(sourceDirectory, file));
				File.Copy(file, destinationPath, true);
            }
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// This function enumerates all directories and files of the <paramref name="direcrotyPath"/> specified.
        /// </summary>
        /// <param name="direcrotyPath">Path to the directory.</param>
        /// <returns>
        /// List of files and directories reside for the <paramref name="direcrotyPath"/> specified.
        /// Empty, when no files and directories are or path does not exists.
        /// </returns>
        public static List<string> GetFileNames(string direcrotyPath)
	{
		List<string> items = new List<string>();

		DirectoryInfo root = new DirectoryInfo(direcrotyPath);
		if (root.Exists)
		{
			// list directories
			foreach (DirectoryInfo dir in root.GetDirectories())
			{
				items.Add(
					System.IO.Path.Join(direcrotyPath, dir.Name)
					);
			}

			// list files
			foreach (FileInfo file in root.GetFiles())
			{
				items.Add(
					System.IO.Path.Join(direcrotyPath, file.Name)
					);
			}
		}

		return items;
	}

	#endregion
}
