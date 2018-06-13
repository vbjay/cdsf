﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.IO.Path;

namespace Cds.Folders
{
    public class OSPath
    {
        public static bool IsWindows => DirectorySeparatorChar == '\\';

        public OSPath(string path)
        {
            Path = path.Trim();
        }

        public static implicit operator OSPath(string path) => new OSPath(path);
        public static implicit operator string(OSPath path) => path.Normalized;
        public override string ToString() => Normalized;

        protected string Path { get; }

        public string Normalized => IsWindows ? Windows : Unix;
        public string Windows => Path.Replace('/', '\\');
        public string Unix => Rootless.Path.Replace('\\', '/');

        public OSPath Relative => Rootless.Path.TrimStart('/', '\\');
        public OSPath Absolute => "/" + Relative;

        public bool IsAbsolute => IsRooted || Unix.StartsWith("/");
        public bool IsRooted => IsPathRooted(Path);
        public OSPath Rootless => IsRooted
            ? "/" + Path.Substring(GetPathRoot(Path).Length)
            : Path;

        public OSPath Parent => GetDirectoryName(Path);

        public bool Contains(OSPath path) =>
            Normalized.Contains(path);

        public static OSPath operator +(OSPath left, OSPath right) =>
            new OSPath(Combine(left, right.Relative));

        public static OSPath operator -(OSPath left, OSPath right) =>
            left.Contains(right)
            ? new OSPath(left.Normalized.Substring(right.Normalized.Length)).Relative
            : left;
    }
}