namespace Pester.Adapter.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PesterTestDiscovererTests
    {
        private static readonly string TestDirectoryPath = Path.Combine(Path.GetTempPath(), "PesterAdapterTestDirectory");

        [TestInitialize]
        public void Initialize()
        {
            if (Directory.Exists(TestDirectoryPath))
            {
                Directory.Delete(TestDirectoryPath, true);
            }

            Directory.CreateDirectory(TestDirectoryPath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(TestDirectoryPath))
            {
                Directory.Delete(TestDirectoryPath, true);
            }
        }

        #region ListPesterTestScripts Scenarios

        [TestMethod]
        public void ListPesterTestScriptsThrowsForNullOrEmptyDirectory()
        {
            var invalidInputs = new[] { null, string.Empty };
            foreach (var invalidInput in invalidInputs)
            {
                var invalidDirectory = invalidInput;
                var action = new Action(() => PesterTestDiscoverer.ListPesterTestScripts(invalidDirectory));
                action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("projectDirectory");
            }
        }

        [TestMethod]
        public void ListPesterTestScriptsThrowsForInvalidContainer()
        {
            var invalidInputs = new[] { " ", @"\NonExistentDirectory" };
            foreach (var invalidInput in invalidInputs)
            {
                var invalidDirectory = invalidInput;
                var action = new Action(() => PesterTestDiscoverer.ListPesterTestScripts(invalidDirectory));
                action.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("projectDirectory");
            }
        }

        [TestMethod]
        public void ListPesterTestScriptsReturnsScriptsWithPesterNamingConvention()
        {
            var validTestFileNames = new[] { "Tests.Tests.ps1", "Sample.Tests.ps1", @"testdir\Tests.Tests.ps1", @"testdir\Sample.Tests.ps1" };
            foreach (var validTestFileName in validTestFileNames)
            {
                var testFileName = validTestFileName;
                CreateFile(TestDirectoryPath, testFileName, "Sample content");

                var scriptList = PesterTestDiscoverer.ListPesterTestScripts(TestDirectoryPath);
                scriptList.Should().OnlyContain(file => file.Equals(Path.Combine(TestDirectoryPath, testFileName)));

                DeleteFile(TestDirectoryPath, testFileName);
            }
        }

        [TestMethod]
        public void ListPesterTestScriptsReturnsMultipleScripts()
        {
            const string SampleTestScript1 = "SampleTest1.Tests.ps1";
            const string SampleTestScript2 = @"Dir\SampleTest1.Tests.ps1";
            const string SampleTestScript3 = "SampleTest2.Tests.ps1";
            CreateFile(TestDirectoryPath, SampleTestScript1, "Sample content");
            CreateFile(TestDirectoryPath, SampleTestScript2, "Sample content");
            CreateFile(TestDirectoryPath, SampleTestScript3, "Sample content");

            PesterTestDiscoverer.ListPesterTestScripts(TestDirectoryPath)
                                .Should()
                                .Contain(
                                    new List<string>
                                        {
                                            Path.Combine(TestDirectoryPath, SampleTestScript1),
                                            Path.Combine(TestDirectoryPath, SampleTestScript3),
                                            Path.Combine(TestDirectoryPath, SampleTestScript2)
                                        });

            DeleteFile(TestDirectoryPath, SampleTestScript1);
            DeleteFile(TestDirectoryPath, SampleTestScript2);
            DeleteFile(TestDirectoryPath, SampleTestScript3);
        }

        [TestMethod]
        public void ListPesterTestScriptsDoesnotReturnInvalidScripts()
        {
            var invalidTestFileNames = new[] { "Tests.Test.ps1", "Sample.Tests.txt", @"testdir\Tests.Tests.ps1.log" };
            foreach (var invalidTestFileName in invalidTestFileNames)
            {
                var testFileName = invalidTestFileName;
                CreateFile(TestDirectoryPath, testFileName, "Sample content");

                var scriptList = PesterTestDiscoverer.ListPesterTestScripts(TestDirectoryPath);
                scriptList.Count().Should().Be(0);

                DeleteFile(TestDirectoryPath, testFileName);
            }
        }

        #endregion

        private static void CreateFile(string baseDirectory, string filePath, string fileContent)
        {
            var directoryName = Path.GetDirectoryName(filePath);
            if (directoryName != null)
            {
                foreach (var directory in directoryName.Split(Path.DirectorySeparatorChar))
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(Path.Combine(baseDirectory, directory));
                    }
                }
            }

            using (var sw = File.CreateText(Path.Combine(baseDirectory, filePath)))
            {
                sw.Write(fileContent);
            }
        }

        private static void DeleteFile(string baseDirectory, string filePath)
        {
            var directoryName = Path.GetDirectoryName(filePath);
            if (directoryName != null)
            {
                var topLevelDir = directoryName.Split(Path.DirectorySeparatorChar)[0];
                if (string.IsNullOrEmpty(topLevelDir))
                {
                    File.Delete(Path.Combine(baseDirectory, filePath));
                }
                else
                {
                    Directory.Delete(Path.Combine(baseDirectory, topLevelDir), true);
                }
            }
        }
    }
}
