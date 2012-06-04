Copy Directory Tree Utility 1.1.0.3
===
This utility allows you to replicate the structure of a directory excluding files.
Once replicated in another point, the directory tree can be compressed for backup
(and restored anywhere).
Some Examples:
	- download directory
	- personal folder
	- directory tree used as database with xml files
When used without a target directory, the program lists the content of given directory.

More info on: http://gsscoder.blogspot.com/.
  
Build:
---
You can build using VCS2010EE (http://www.microsoft.com/visualstudio/en-us/products/2010-editions/visual-csharp-express) or
by using NAnt (in both case only two deps are required nunit.framework.dll
and Mono.Posix.dll - both included in lib subdirectory).

Unit Tests:
---
If you build under *nix/Linux OS you may have problems if the nunit.framework.dll
version used to build the test version is not the same of the test runner.

Dependencies:
---
  - This program uses Command Line Parser Library 1.9.2.4 (http://commandline.codeplex.com/).
  - I've also included a fragment from Banshee 1.4.1 (http://banshee.fm/).

Windows User:
---
Although Mono.Posix.dll is needed for build process you may not redistribute it in a deployment
environment. (For *nix users is already installed in the GAC).

Compatibility:
----
  MS .NET Framework Version 2.0+
  Mono 2.0 Profile+

Licence:
---
  MIT License
  http://www.opensource.org/licenses/mit-license.php

Usage Examples:
---
```
  ;;Help, copyright and license info:
    C:\> cptree --help
  ;;List current directory and its content:
    C:\> cptree
  ;;List the specified directory:
    C:\> cptree C:\Temp\SomeDir
  ;;Copy a directory tree:
    C:\> cptree C:\Temp\MyDir C:\Temp\CopyOfMyDir
```

Comments, bugs and other:
---
  [CodePlex](http://cptree.codeplex.com)
  Giacomo Stelluti Scala
  gsscoder AT gmail DOT com
