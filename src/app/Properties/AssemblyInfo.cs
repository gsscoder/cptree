#region License
//
// Copy Directory Tree: AssemblyInfo.cs
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

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Resources;
using CommandLine.Text;

[assembly: AssemblyTitle(ThisAssembly.Title)]
[assembly: AssemblyProduct("Copy Directory Tree Utility")]
[assembly: AssemblyDescription(ThisAssembly.Title)]
[assembly: AssemblyCopyright(ThisAssembly.Copyright)]
[assembly: AssemblyVersion(ThisAssembly.Version)]
[assembly: AssemblyInformationalVersion(ThisAssembly.InformationalVersion)]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: AssemblyCulture("")]
[assembly: InternalsVisibleTo("CopyDirectoryTree.Tests")]
[assembly: AssemblyLicense(
    "This is free software. You may redistribute copies of it under the terms of",
    "the MIT License <http://www.opensource.org/licenses/mit-license.php>.",
    "  - Uses Command Line Parser Library, Version 1.9.2.4 (http://commandline.codeplex.com/).",
    "  - Includes a code fragment from Banshee, Version 1.4.1 (http://banshee-project.org/).")]
[assembly: AssemblyUsage(
    "Usage: cptree [OPTION]... DIR-TO-LIST",
    "       cptree [OPTION]... SOURCE-DIR TARGET-DIR")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
//[assembly: AssemblyCompany("")]
//[assembly: AssemblyTrademark("")]
