#region License (Aaron Bockover)
// 
// PlatformHacks.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2007 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
#region License
//
// Copy Directory Tree: PlatformHacks.cs
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
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace CopyDirectoryTree
{
    static class PlatformHacks
    {
        #region Commented Code
        //// For the SEGV trap hack (see below)
        //[DllImport("libc")]
        //private static extern int sigaction(Mono.Unix.Native.Signum sig, IntPtr act, IntPtr oact);

        //private static IntPtr monoJitSegvHandler = IntPtr.Zero;

        //public static void TrapMonoJitSegv()
        //{
        //    if (Environment.OSVersion.Platform != PlatformID.Unix)
        //    {
        //        return;
        //    }

        //    // We must get a reference to the JIT's SEGV handler because 
        //    // GStreamer will set its own and not restore the previous, which
        //    // will cause what should be NullReferenceExceptions to be unhandled
        //    // segfaults for the duration of the instance, as the JIT is powerless!
        //    // FIXME: http://bugzilla.gnome.org/show_bug.cgi?id=391777

        //    try
        //    {
        //        monoJitSegvHandler = Marshal.AllocHGlobal(512);
        //        sigaction(Mono.Unix.Native.Signum.SIGSEGV, IntPtr.Zero, monoJitSegvHandler);
        //    }
        //    catch
        //    {
        //    }
        //}

        //public static void RestoreMonoJitSegv()
        //{
        //    if (Environment.OSVersion.Platform != PlatformID.Unix || monoJitSegvHandler.Equals(IntPtr.Zero))
        //    {
        //        return;
        //    }

        //    // Reset the SEGV handle to that of the JIT again (SIGH!)
        //    try
        //    {
        //        sigaction(Mono.Unix.Native.Signum.SIGSEGV, monoJitSegvHandler, IntPtr.Zero);
        //        Marshal.FreeHGlobal(monoJitSegvHandler);
        //    }
        //    catch
        //    {
        //    }
        //}
        #endregion

        [DllImport("libc")] // Linux
        private static extern int prctl(int option, byte[] arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5);

        [DllImport("libc")] // BSD
        private static extern void setproctitle(byte[] fmt, byte[] str_arg);

        public static void SetProcessName(string name)
        {
            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                return;
            }

            try
            {
                if (prctl(15 /* PR_SET_NAME */, Encoding.ASCII.GetBytes(name + "\0"),
                    IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) != 0)
                {
                    throw new ApplicationException("Error setting process name: " +
                        Mono.Unix.Native.Stdlib.GetLastError());
                }
            }
            catch (EntryPointNotFoundException)
            {
                setproctitle(Encoding.ASCII.GetBytes("%s\0"),
                    Encoding.ASCII.GetBytes(name + "\0"));
            }
        }

        public static void TrySetProcessName(string name)
        {
            try
            {
                SetProcessName(name);
            }
            catch
            {
            }
        }
    }
}