using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace Linty.UnityApi
{
    public class UnityProject
    {
        private readonly DirectoryInfo _unityProjectFolder;
        private EditorVersion _editorVersion;

        /// <summary>
        /// Represents a Unity project
        /// </summary>
        /// <param name="unityProjectFolder"></param>
        public UnityProject(DirectoryInfo unityProjectFolder)
        {
            if (unityProjectFolder != null && unityProjectFolder.Exists)
            {
                _unityProjectFolder = unityProjectFolder;

                var builder = new DeserializerBuilder();
                builder.IgnoreUnmatchedProperties();
                var deserializer = builder.Build();

                var tagManagerFilePath = Path.Combine(_unityProjectFolder.FullName, @"ProjectSettings\TagManager.asset");
                var tagManagerFileInfo = new FileInfo(tagManagerFilePath);

                using (var textReader = tagManagerFileInfo.OpenText())
                {
                    var inputContent = PreprocessUnityYaml(textReader);


                    var tagManager = deserializer.Deserialize<TagManagerAsset>(inputContent);

                    this.Layers = tagManager.TagManager.Layers;
                    this.Tags = tagManager.TagManager.Tags;

                    Console.WriteLine("tags: " + tagManager.TagManager.Layers.Count);
                }
            }
        }

        private string PreprocessUnityYaml(StreamReader yaml)
        {
            //Based on http://stackoverflow.com/questions/21473076/pyyaml-and-unusual-tags

            //TODO: Optimize the shit out of this!

            var stringBuilder = new StringBuilder();
            const int RemoveLine = 1;
            const int ModifyLine = 2;
            int lineNumber = 0;

            while (!yaml.EndOfStream)
            {
                var line = yaml.ReadLine();

                if (lineNumber == RemoveLine)
                {
                    //ignore this line
                }
                else if (lineNumber == ModifyLine)
                {
                    var segments = line.Split(' ');
                    var newLine = segments[0] + ' ' + segments[2];
                    stringBuilder.AppendLine(newLine);
                }
                else
                {
                    stringBuilder.AppendLine(line);
                }

                lineNumber++;
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// The tags that have been defined in this project
        /// </summary>
        public List<string> Tags { get; private set; }

        /// <summary>
        /// The Layers that have been defined in this project
        /// </summary>
        public List<string> Layers { get; private set; }

        /// <summary>
        /// The version of the Unity Editor that created the unity project
        /// </summary>
        public EditorVersion Version
        {
            get
            {
                if (_editorVersion == null)
                {
                    
                    var projectVersionFilePath = Path.Combine(_unityProjectFolder.FullName, EditorVersion.ProjectVersionPath);
                    var projectVersionFileInfo = new FileInfo(projectVersionFilePath);
                    _editorVersion = new EditorVersion(projectVersionFileInfo);
                }

                return _editorVersion;

            }
        }

        public class EditorVersion
        {
            //Only available after v5 - need to find an alternative method for pre v5
            public const string ProjectVersionPath = @"ProjectSettings\ProjectVersion.txt";

            public EditorVersion(FileInfo projectVersionFilePath)
            {
                if (projectVersionFilePath != null && projectVersionFilePath.Exists)
                {
                    var fileContent = projectVersionFilePath.OpenText().ReadToEnd();
                    var contentArray = fileContent.Split(':');
                    if (contentArray.Length > 0)
                    {
                        var versionInfo = contentArray[1].Trim();
                        UpdateFromString(versionInfo, this);
                    }
                }
            }



            private static void UpdateFromString(string versionInfo, EditorVersion editorVersion)
            {
                var regEx = new Regex(@"^(\d{1,2})\.(\d{1,2})\.(\d{1,2})(a|b|rc|f|p)(\d{1,2})");
                var match = regEx.Match(versionInfo);

                editorVersion.Major = TryGetVersion(match.Groups[1].Value);
                editorVersion.Minor = TryGetVersion(match.Groups[2].Value);
                editorVersion.Revision = TryGetVersion(match.Groups[3].Value);
                editorVersion.ReleaseType = match.Groups[4].Value;
                editorVersion.Release = TryGetVersion(match.Groups[5].Value);
            }

            private static int TryGetVersion(string versionSegment)
            {
                int versionDigit;

                int.TryParse(versionSegment, out versionDigit);

                return versionDigit;
            }

            public int Major { get; private set; }

            public int Minor { get; private set; }

            public int Revision { get; private set; }

            public string ReleaseType { get; private set; }

            public int Release { get; private set; }

            public override string ToString()
            {
                return string.Format("{0}.{1}.{2}{3}{4}", this.Major, this.Minor, this.Revision, this.ReleaseType, this.Release);
            }
        }

        public class TagManagerAsset
        {
            public TagManager TagManager { get; set; }
        }

        public class TagManager
        {
            [YamlMember(Alias = "serializedVersion")]
            public int SerializedVersion { get; set; }

            [YamlMember(Alias = "tags")]
            public List<string> Tags { get; set; }

            [YamlMember(Alias = "layers")]
            public List<string> Layers { get; set; }

            [YamlMember(Alias = "m_SortingLayers")]
            public List<SortingLayer> SortingLayers { get; set; }
        }

        public class SortingLayer
        {
            [YamlMember(Alias = "locked")]
            public int Locked { get; set; }

            [YamlMember(Alias = "uniqueID")]
            public long UniqueId { get; set; }

            [YamlMember(Alias = "name")]
            public string Name { get; set; }
        }
    }
}
