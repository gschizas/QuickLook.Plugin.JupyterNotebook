// Copyright © 2017 Paddy Xu
// 
// This file is part of QuickLook program.
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
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using QuickLook.Common.Plugin;

namespace QuickLook.Plugin.JupyterNotebook
{
    public class Plugin : IViewer
    {
        private WebpagePanel _panel;
        private string tempHtmlPath;

        public int Priority => 0;

        public void Init()
        {
            Helper.SetBrowserFeatureControl();
        }

        public bool CanHandle(string path)
        {
            return !Directory.Exists(path) && path.ToLower().EndsWith(".ipynb");
        }

        public void Prepare(string path, ContextObject context)
        {
            context.PreferredSize = new Size(1000, 600);
        }

        public void View(string path, ContextObject context)
        {
            _panel = new WebpagePanel();
            context.ViewerContent = _panel;

            context.Title = Path.IsPathRooted(path) ? Path.GetFileName(path) : path;
            tempHtmlPath = ConvertNotebookToHtml(path);
            _panel.LoadFile(tempHtmlPath);
            _panel.Dispatcher.Invoke(() => { context.IsBusy = false; }, DispatcherPriority.Loaded);
        }

        private string ConvertNotebookToHtml(string path)
        {
            var outputFilename = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".html";
            var exePath = @"C:\Program Files\Python37\Scripts\pipenv.exe";
            var directory = System.IO.Path.GetDirectoryName(path);

            var paramsBuilder = new StringBuilder();
            paramsBuilder.Append("run jupyter nbconvert ");
            paramsBuilder.Append("\"" + path + "\"");
            paramsBuilder.Append(" --output ");
            paramsBuilder.Append("\"" + outputFilename + "\"");

            var si = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = exePath,
                Arguments = paramsBuilder.ToString(),
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = directory
            };

            using (var process = new Process())
            {
                process.StartInfo = si;
                process.Start();

                if (process.WaitForExit(10 * 1000))
                {
                    return outputFilename;
                }

                // timed out, but maybe there's hope yet
                process.Kill();
                if (!File.Exists(outputFilename))
                {
                    throw new TimeoutException();
                }
            }

            return outputFilename;
        }

        public void Cleanup()
        {
            _panel?.Dispose();
            _panel = null;

            File.Delete(tempHtmlPath);
            tempHtmlPath = null;
        }
    }
}