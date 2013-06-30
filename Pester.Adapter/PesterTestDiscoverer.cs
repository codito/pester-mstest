// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Defines the PesterTestDiscoverer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Pester.Adapter
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The pester test discoverer.
    /// </summary>
    public class PesterTestDiscoverer
    {
        /// <summary>
        /// Lists the pester test scripts in a directory. The scripts must be 
        /// files ending with .Tests.ps1.
        /// </summary>
        /// <param name="projectDirectory">The directory with <code>powershell</code> scripts.</param>
        /// <returns>List of scripts.</returns>
        public static IEnumerable<string> ListPesterTestScripts(string projectDirectory)
        {
            if (string.IsNullOrEmpty(projectDirectory))
            {
                throw new ArgumentNullException("projectDirectory");
            }

            if (!Directory.Exists(projectDirectory))
            {
                throw new ArgumentException("Project directory doesn't exist.", "projectDirectory");
            }

            return Directory.EnumerateFiles(projectDirectory, "*.Tests.ps1", SearchOption.AllDirectories);
        }

        ////public static IEnumerable<object> ListPesterTestsInScript(string scriptPath)
        ////{
        ////    throw new NotImplementedException();
        ////}
    }
}
