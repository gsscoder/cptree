#region License
//
// Copy Directory Tree: DirectoryIterator.cs
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
using System.IO;
#endregion

namespace CopyDirectoryTree
{

    public sealed class DirectoryIterator
    {
        #region Delegate with Arguments
        public sealed class Args
        {
            private string _path;
            private bool _isDirectory;
            public Args(string path, bool isDirectory)
            {
                this._path = path;
                this._isDirectory = isDirectory;
            }
            public string Path
            {
                get { return this._path; }
            }
            public bool IsDirectory
            {
                get { return this._isDirectory; }
            }
        }

        public delegate void Delegate(DirectoryIterator sender, DirectoryIterator.Args args);
        #endregion

        private readonly string _path;
        private DirectoryIterator.Delegate _callback;
        private bool _processFiles;
        private object _tag;
        private Exception _lastException;

        public DirectoryIterator(string path)
        {
            this._path = path;
            this._lastException = null;
        }

        public string Path
        {
            get { return this._path; }
        }

        public bool ProcessFiles
        {
            set { this._processFiles = value; }
        }

        public object Tag
        {
            get { return this._tag; }
            set { this._tag = value; }
        }

        public Exception LastException
        {
            get { return this._lastException; }
        }

        public bool Iterate(DirectoryIterator.Delegate directoryIteratorDelegate)
        {
            bool hasError = false;
            this._callback = directoryIteratorDelegate;
            try
            {
                if (File.Exists(this._path))
                {
                    ProcessFile(this._path);
                }
                else if (Directory.Exists(this._path))
                {
                    ProcessDirectory(this._path);
                }
            }
            catch (UnauthorizedAccessException uaex)
            {
                this._lastException = uaex;
                hasError = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return !hasError;
        }

        private void ProcessFile(string path)
        {
            if (this._processFiles)
            {
                this._callback(this, new Args(path, false));
            }
        }

        private void ProcessDirectory(string path)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                ProcessFile(file);
            }

            string[] subDirectories = Directory.GetDirectories(path);
            foreach (string subDirectory in subDirectories)
            {
                this._callback(this, new Args(subDirectory, true));
                ProcessDirectory(subDirectory);
            }
        }
    }
}