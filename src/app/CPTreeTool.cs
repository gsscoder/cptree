#region License
//
// Copy Directory Tree: CPTreeTool.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2008 - 2011 Giacomo Stelluti Scala
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CommandLine;
using CommandLine.Text;
#endregion

namespace CopyDirectoryTree
{
    static class CPTreeTool
    {
        #region Exit Code Constants
        private const int EXIT_SUCCESS = 0;
        private const int EXIT_FAILURE = 1;
        private const int EXIT_FAILURE_CRITICAL = 2;
        #endregion
        internal static HeadingInfo Heading = new HeadingInfo(ThisAssembly.Name, ThisAssembly.MajorMinorVersion);

        sealed class Options : CommandLineOptionsBase
        {
            const int _sourcePathIndex = 0;
            const int _targetPathIndex = 1;

            [ValueList(typeof(List<string>))]
            public IList<string> Paths { get; set; }

            [Option("v", "verbose",
                    HelpText = "Explain what is being done.")]
            public bool Verbose { get; set; }

            #region Parsed value list (with defaults)
            public string SourcePath
            {
                get
                {
                    if (this.Paths.Count > 0)
                    {
                        return this.Paths[_sourcePathIndex];
                    }
                    else
                    {
                        return Environment.CurrentDirectory;
                    }
                }
            }

            public string TargetPath
            {
                get
                {
                    if (this.Paths.Count > 1)
                    {
                        return this.Paths[_targetPathIndex];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            #endregion

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                    (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }

            /*[HelpOption]
            public string GetUsage()
            {
                HelpText info = new HelpText(CPTreeTool.Heading);
                info.Copyright = ThisAssembly.Copyright;
                info.AddPreOptionsLine("This is free software. You may redistribute copies of it under the terms of");
                info.AddPreOptionsLine("the MIT License <http://www.opensource.org/licenses/mit-license.php>.");
                info.AddPreOptionsLine("  - Uses CommandLine.dll, Version 1.8 (http://commandline.codeplex.com/).");
                info.AddPreOptionsLine("  - Includes a code fragment from Banshee, Version 1.4.1 (http://banshee-project.org/)." +
                    Environment.NewLine);
                info.AddPreOptionsLine("Usage: cptree [OPTION]... DIR-TO-LIST");
                info.AddPreOptionsLine("       cptree [OPTION]... SOURCE-DIR TARGET-DIR");
                info.AddOptions(this);
                return info;
            }*/

            #region Extra validation stuff
            public bool Validate()
            {
                if (this.Paths.Count > 2)
                {
                    ReportError("too much arguments");
                    return false;
                }

                if (!this.SourcePath.Equals(Environment.CurrentDirectory, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (File.Exists(this.SourcePath))
                    {
                        ReportError(string.Format("'{0}' is a file", this.SourcePath));
                        return false;
                    }
                    if (!Directory.Exists(this.SourcePath))
                    {
                        ReportError(string.Format("'{0}' does not exist", this.SourcePath));
                        return false;
                    }
                }
                if (this.TargetPath != null)
                {
                    if (File.Exists(this.TargetPath))
                    {
                        ReportError(string.Format("'{0}' is a file", this.TargetPath));
                        return false;
                    }
                }
                return true;
            }
            #endregion
        }

        static void Main(string[] args)
        {
            PlatformHacks.TrySetProcessName(ThisAssembly.Name);

            Options options = new Options();
            ICommandLineParser parser = new CommandLineParser();
            //if (!Parser.ParseArguments(args, options, Console.Error))
            if (!parser.ParseArguments(args, options, Console.Error))
            {
                Environment.Exit(EXIT_FAILURE);
            }
            if (!options.Validate())
            {
                Console.Error.WriteLine("Try '{0} --help' for more information.", ThisAssembly.Name);
                Environment.Exit(EXIT_FAILURE);
            }

            if (options.Verbose)
            {
                Console.WriteLine(Heading.ToString());
                Console.WriteLine();
            }

            // If the target path is not specified, the program simply list the content of the
            // current directory; like dir or ls.
            bool success;
            if (string.IsNullOrEmpty(options.TargetPath))
            {
                if (options.Verbose)
                {
                    success = ExecuteIterator(options, new DirectoryIterator.Delegate(FileSystemObjectPrinterVerbose), true);
                }
                else
                {
                    success = ExecuteIterator(options, new DirectoryIterator.Delegate(FileSystemObjectPrinter), true);
                }
            }
            else
            {
                success = ExecuteIterator(options,
                        new DirectoryIterator.Delegate(DirectoryBuilder), false);
            }

            #region Helper Code while running inside an IDE (uncomment when needed)
            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.Write(">>>press any key<<<");
            //Console.ResetColor();
            //Console.ReadKey();
            #endregion

            Environment.Exit(success ? EXIT_SUCCESS : EXIT_FAILURE_CRITICAL);
        }

        static bool ExecuteIterator(Options options, DirectoryIterator.Delegate callback, bool processFiles)
        {
            bool hasError = false;
            DirectoryIterator dirIter = new DirectoryIterator(options.SourcePath);
            dirIter.ProcessFiles = processFiles;
            dirIter.Tag = options;
            if (!dirIter.Iterate(callback))
            {
                ReportIterateError(dirIter);
                hasError = true;
            }
            return hasError;
        }

        static void FileSystemObjectPrinter(DirectoryIterator sender, DirectoryIterator.Args args)
        {
            string fsoName = PathUtility.GetBaseName(sender.Path, args.Path);
            if (args.IsDirectory)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                // If the filesystem object is a directory, we add a directory separator char
                // and change the color to blue (UNIX ls command default).
                Console.WriteLine("{0}{1}", fsoName, Path.DirectorySeparatorChar);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(fsoName);
            }
        }

        static void FileSystemObjectPrinterVerbose(DirectoryIterator sender, DirectoryIterator.Args args)
        {
            //[d/-][a/-][r/-][h/-][s/-]
            StringBuilder builder = new StringBuilder();
            FileAttributes attr = File.GetAttributes(args.Path);
            DateTime lwt;
            if (args.IsDirectory)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                builder.Append("d");
                lwt = Directory.GetLastWriteTime(args.Path);
            }
            else
            {
                builder.Append("-");
                lwt = File.GetLastWriteTime(args.Path);
            }
            if ((attr & FileAttributes.Archive) == FileAttributes.Archive) builder.Append("a");
            else builder.Append("-");
            if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) builder.Append("r");
            else builder.Append("-");
            if ((attr & FileAttributes.Hidden) == FileAttributes.Hidden) builder.Append("h");
            else builder.Append("-");
            if ((attr & FileAttributes.System) == FileAttributes.System) builder.Append("s");
            else builder.Append("-");

            builder.Append("    "); //4 spaces
            builder.Append(lwt.ToShortDateString());
            builder.Append("    "); //4 spaces

            if (!args.IsDirectory)
            {
                string ln = new FileInfo(args.Path).Length.ToString(CultureInfo.InvariantCulture);
                int ts = 10 - ln.Length;
                for (int i = 0; i < ts; i++) builder.Append(' ');
                builder.Append(ln);
            }
            else
                builder.Append("          "); //10 spaces

            builder.Append("    "); //4 spaces
            builder.Append(PathUtility.GetBaseName(sender.Path, args.Path));
            if (args.IsDirectory) builder.Append(PathUtility.DirectorySeparatorString);
            builder.Append("    "); //4 spaces
            Console.WriteLine(builder.ToString());
            Console.ResetColor();
        }

        static void DirectoryBuilder(DirectoryIterator sender, DirectoryIterator.Args args)
        {
            Options options = (Options)sender.Tag;
            string dirName = PathUtility.GetBaseName(sender.Path, args.Path);
            string newTargetPath = Path.Combine(options.TargetPath, dirName);
            Directory.CreateDirectory(newTargetPath);
            if (options.Verbose)
            {
                Console.WriteLine("'{0}' directory created", dirName);
            }
        }

        static void ReportError(string message)
        {
            StringBuilder builder = new StringBuilder(message.Length * 2);
            builder.Append(ThisAssembly.Name);
            builder.Append(": ");
            builder.Append(message);
            Console.Error.WriteLine(builder.ToString());
        }

        static void ReportIterateError(DirectoryIterator dirIter)
        {
            Exception ex = dirIter.LastException;
            if (ex != null)
            {
                ReportError(ex.Message);
            }
        }
    }
}