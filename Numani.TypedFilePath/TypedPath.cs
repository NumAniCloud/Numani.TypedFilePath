﻿using System;
using System.IO;
using Numani.TypedFilePath.Interfaces;
using Numani.TypedFilePath.Routing;

namespace Numani.TypedFilePath
{
	public static partial class TypedPath
	{
		public static IFilePath AsFilePath(this string pathString)
		{
			return Path.IsPathRooted(pathString)
				? AsFilePath(pathString, AbsoluteRoute.Instance)
				: AsFilePath(pathString, RelativeRoute.Instance);
		}

		private static TFile AsFilePath<TFile>(this string pathString,
			Func<TFile> noExt,
			Func<FileExtension, TFile> withExt)
		{
			// パス末尾のスラッシュなどがあれば、それを外したものをファイルパスとして扱う
			if (Path.EndsInDirectorySeparator(pathString))
			{
				pathString.TrimEnd(Path.DirectorySeparatorChar);
			}

			if (!Path.HasExtension(pathString))
			{
				return noExt();
			}
			
			var ext = new FileExtension(Path.GetExtension(pathString));
			var baseName = pathString.Replace(ext.WithDot, "");
			return withExt(ext);
		}
		internal static IFilePath AsFilePath(this string pathString, RoutingBase routingBase)
		{
			return AsFilePath(pathString,
				() => routingBase.GetFilePath(pathString),
				ext => routingBase.GetFilePathWithExtension(pathString, ext));
		}

		internal static IRelativeFilePath AsFilePath(this string pathString, RelativeRoute routingBase)
		{
			return AsFilePath(pathString,
				() => routingBase.GetFilePath(pathString),
				ext => routingBase.GetFilePathWithExtension(pathString, ext));
		}

		internal static IAbsoluteFilePath AsFilePath(this string pathString, AbsoluteRoute routingBase)
		{
			return AsFilePath(pathString,
				() => routingBase.GetFilePath(pathString),
				ext => routingBase.GetFilePathWithExtension(pathString, ext));
		}

		public static IDirectoryPath AsDirectoryPath(this string pathString)
		{
			RoutingBase routingBase = Path.IsPathRooted(pathString)
				? AbsoluteRoute.Instance
				: RelativeRoute.Instance;

			// パス末尾にスラッシュが無ければ、それを付与したものをディレクトリパスとして扱う
			if (!Path.EndsInDirectorySeparator(pathString))
			{
				pathString += Path.DirectorySeparatorChar;
			}

			return routingBase.GetDirectoryPath(pathString);
		}

		public static IFileSystemPath AsAnyPath(this string pathString)
		{
			IFileSystemPath result = Path.EndsInDirectorySeparator(pathString)
				? AsDirectoryPath(pathString)
				: AsFilePath(pathString);

			return result;
		}

		public static IAbsoluteDirectoryPath GetCurrentDirectory()
		{
			var path = Directory.GetCurrentDirectory();
			return new AbsoluteDirectoryPath(path);
		}
	}
}
