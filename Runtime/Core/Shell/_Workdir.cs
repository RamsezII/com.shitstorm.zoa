using _ARK_;
using _COBRA_;
using _UTIL_;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace _ZOA_
{
    partial class Shell
    {
        public enum PathModes : byte
        {
            TryMaintain,
            TryLocal,
            ForceFull,
        }

        public readonly ValueHandler<string> workdir = new(ArkPaths.instance.Value.dpath_home);

        public LintedString RegularPrefixe() => new(
            text: $"{ArkMachine.user_name.Value}:{workdir._value}$ ",
            lint: $"{ArkMachine.user_name.Value.SetColor("#73CC26")}:{workdir._value.SetColor("#73B2D9")}$ "
        );

        //--------------------------------------------------------------------------------------------------------------

        internal void ChangeWorkdir(in string path) => workdir.Value = PathCheck(path, PathModes.ForceFull, false, false, out _, out _);

        public string PathCheck(in string path, in PathModes path_mode, in bool check_quotes, in bool force_quotes, out bool is_rooted, out bool is_local_to_shell) => PathCheck(workdir._value, path, path_mode, check_quotes, force_quotes, out is_rooted, out is_local_to_shell);
        public static string PathCheck(in string workdir, in string path, in PathModes path_mode, in bool check_quotes, in bool force_quotes, out bool is_rooted, out bool is_local_to_shell)
        {
            bool empty = string.IsNullOrWhiteSpace(path);

            try
            {
                string result_path = path;

                if (empty)
                {
                    is_rooted = false;
                    is_local_to_shell = true;
                    result_path = workdir;
                }
                else
                {
                    is_rooted = Path.IsPathRooted(path);
                    if (is_rooted)
                    {
                        result_path = Path.GetFullPath(result_path).Replace("\\", "/");
                        is_local_to_shell = result_path.StartsWith(workdir, StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        is_local_to_shell = true;
                        result_path = Path.Combine(workdir, result_path);
                    }
                    result_path = Path.GetFullPath(result_path);
                }

                switch (path_mode)
                {
                    case PathModes.TryMaintain when !is_rooted:
                    case PathModes.TryLocal:
                        if (is_local_to_shell)
                            result_path = Path.GetRelativePath(workdir, result_path);
                        break;
                }

                result_path = result_path.Replace("\\", "/");

                if (force_quotes)
                    result_path = result_path.QuoteStringSafely();
                else if (check_quotes)
                    result_path = result_path.QuotePathIfNeeded();

                return result_path;
            }
            catch
            {
                is_rooted = false;
                is_local_to_shell = false;
                return path;
            }
        }

        public bool TryParsePath(in Signal signal, in FS_TYPES type, in bool read_as_argument, out string path)
        {
            if (!signal.reader.TryParseString(out path, read_as_argument))
                if (signal.reader.sig_error == null)
                    signal.reader.TryReadArgument(out path, false, signal.reader.lint_theme.strings, stoppers: CodeReader._stoppers_paths);

            if (signal.reader.sig_error != null)
                goto failure;

            int read_old = signal.reader.read_i;
            signal.reader.HasNext();
            if (!signal.reader.IsOnCursor())
                signal.reader.read_i = read_old;
            else
            {
                signal.reader.stop_completing = true;
                signal.reader.completion_l = signal.reader.completion_r = null;

                try
                {
                    if (string.IsNullOrWhiteSpace(path))
                        path = "./";

                    string long_path = PathCheck(path, PathModes.ForceFull, false, false, out bool is_rooted, out bool is_local_to_shell);
                    bool ends_with_bar = long_path.EndsWith('/', '\\');

                    if (ends_with_bar)
                        long_path = long_path[..^1];

                    PathModes path_mode = is_rooted ? PathModes.ForceFull : PathModes.TryLocal;
                    DirectoryInfo parent = ends_with_bar ? new(long_path) : Directory.GetParent(long_path);

                    if (parent != null)
                    {
                        signal.reader.completion_l = PathCheck(ends_with_bar ? long_path : parent.FullName, path_mode, true, true, out _, out _);

                        if (parent.Exists)
                        {
                            if (!ends_with_bar)
                                signal.reader.completion_r = (PathCheck(long_path, path_mode, false, false, out _, out _) + "/").QuoteStringSafely();
                            else
                            {
                                DirectoryInfo current = new(long_path);
                                if (current != null && current.Exists)
                                {
                                    string path_r;
                                    if (type == FS_TYPES.DIRECTORY)
                                        path_r = current.EnumerateDirectories().FirstOrDefault()?.FullName ?? long_path;
                                    else
                                        path_r = current.EnumerateFileSystemInfos().FirstOrDefault()?.FullName ?? long_path;
                                    signal.reader.completion_r = PathCheck(path_r, path_mode, true, true, out _, out _);
                                }
                            }

                            if (signal.flags.HasFlag(SIG_FLAGS.CHANGE))
                            {
                                var paths = type switch
                                {
                                    FS_TYPES.DIRECTORY => parent.EnumerateDirectories(),
                                    _ => parent.EnumerateFileSystemInfos(),
                                };

                                foreach (var dir in parent.EnumerateDirectories())
                                    signal.reader.completions_v.Add(PathCheck(dir.FullName, path_mode, true, true, out _, out _));

                                if (type.HasFlag(FS_TYPES.FILE))
                                    foreach (var file in parent.EnumerateFiles())
                                        signal.reader.completions_v.Add(PathCheck(file.FullName, path_mode, true, true, out _, out _));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            return true;

        failure:
            path = null;
            signal.reader.Stderr($"could not parse path '{path}'.");
            return false;
        }
    }
}