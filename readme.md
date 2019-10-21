UnityEngineAnalyzer
===================

UnityEngineAnalyzer is a set of Roslyn analyzers that aim to detect common problems in Unity3D C# code. Unity3D makes it easy for us to make cross platform games, but there are hidden rules about performance and AOT, which might only come with experience, testing or reading the forums. It is hoped that such problems can be caught before compilation.

Analyzers
---------------------

| Analyzer                               | Name                       | Gategory       | Severity    |
| -------------------------------------- | -------------------------- | ---------------| ------------|
| [UEA0001](Documents/analyzers/UEA0001.md) |DoNotUseOnGUI               | GC             |Info         |
| [UEA0002](Documents/analyzers/UEA0002.md) |DoNotUseStringMethods       | GC             |Info         |
| [UEA0003](Documents/analyzers/UEA0003.md) |EmptyMonoBehaviourMethod    | Miscellaneous  |Warning      |
| [UEA0004](Documents/analyzers/UEA0004.md) |UseCompareTag               | GC             |Warning      |
| [UEA0005](Documents/analyzers/UEA0005.md) |DoNotUseFindMethodsInUpdate | Performance    |Warning      |
| [UEA0006](Documents/analyzers/UEA0006.md) |DoNotUseCoroutines          | GC             |Info         |
| [UEA0007](Documents/analyzers/UEA0007.md) |DoNotUseForEachInUpdate     | Performance    |Warning      |
| [UEA0008](Documents/analyzers/UEA0008.md) |UnsealedDerivedClass        | Performance    |Warning      |
| [UEA0009](Documents/analyzers/UEA0009.md) |InvokeFunctionMissing       | Performance    |Warning      |
| [UEA0010](Documents/analyzers/UEA0010.md) |DoNotUseStateNameInAnimator | Performance    |Warning      |
| [UEA0011](Documents/analyzers/UEA0011.md) |DoNotUseStringPropertyNames | Performance    |Warning      |
| [UEA0012](Documents/analyzers/UEA0012.md) |CameraMainIsSlow            | GC             |Warning      |
| [UEA0013](Documents/analyzers/UEA0013.md) |UseNonAllocMethods          | GC             |Warning      |
| [UEA0014](Documents/analyzers/UEA0014.md) |AudioSourceMuteUsesCPU      | Performance    |Info         |
| [UEA0015](Documents/analyzers/UEA0015.md) |InstantiateTakeParent       | Performance    |Warning      |
| [UEA0015](Documents/analyzers/UEA0016.md) |VectorMagnitudeIsSlow       | Performance    |Info         |
| ----------------------------------------- |----------------------------|--------------- | ----------- |
| [AOT0001](Documents/analyzers/AOT0001.md) |DoNotUseRemoting            | AOT            |Info         |
| [AOT0002](Documents/analyzers/AOT0002.md) |DoNotUseReflectionEmit      | AOT            |Info         |
| [AOT0003](Documents/analyzers/AOT0003.md) |TypeGetType                 | AOT            |Info         |

Building CLI executable
---------------------

CLI requires .NET Core 2.1

```
dotnet publish -c Release -r win10-x64
```
or
```
dotnet publish -c Release -r ubuntu.16.10-x64
```

Command Line Interface
---------------------

In order to use the Command Line Interface (CLI), download the latest release of UnityEngineAnalyzer then unzip the archive (https://github.com/vad710/UnityEngineAnalyzer/releases).

1. Open a Command Prompt or Powershell Window
1. Run `Linty.CLI.exe <project path>`
1. Observe the analysis results
1. (Optional) In the same location as the project file are `report.json` and `UnityReport.html` files containing the results of the analysis
    * Use command `-e customexporter exporter2 ...` to load custom exporters
1. (Optional) configuration file path.
    * Use command `-c configureFilePath.json` to load custom configurations
	* Configuration json, allows to enable / disable analyzers
1. (Optional) minimal severity for reports
	* Use command `-s Info/Warning/Error` to defined used minimal severity for reporting
	* Default is Warning
1.	(Optional) Unity version for check
	* Use command `-v UNITY_2017_1/UNITY_5_5/UNITY_4_0/...` to Unity version
	* For default analyzer will try to find ProjectVersion.txt file and parse version automatically.

Example:

`> Linty.CLI.exe C:\Code\MyGame.CSharp.csproj` 


Visual Studio Integration
-------------------------

In Visual Studio 2017, go to `Tools > Nuget Package Manager > Manage Nuget Packages for Solution...`. Search for and install `UnityEngineAnalyzer`


JetBrains Rider Integration
-------------------------

1. Commandline: `nuget install UnityEngineAnalyzer`
2. Add `Assets/csc.rsp` with the following content:
```
-a:"full_or_relative_path_to_UnityEngineAnalyzer.dll"
-ruleset:"full_or_relative_path_to_rules.ruleset"
```
3. You may need to regenerate csproj files in Unity

> Applies to Unity 2019.2+ with Rider package 1.1.3+. Approach for older versions [#27](https://github.com/vad710/UnityEngineAnalyzer/issues/27)

Configuration
-------------

Under projects right-click `Analyzers` to modify the severity or to disable the rule completely.

![](https://raw.githubusercontent.com/meng-hui/UnityEngineAnalyzer/master/Documents/configuration.png)

Limitations
-----------

- HTML Report requires FireFox or XOR (Corss Origin Request) enabled in other browsers
- It doesn't have rules for all of [Mono's AOT Limitations](https://developer.xamarin.com/guides/ios/advanced_topics/limitations/)
- IL2CPP might change the limitations of AOT compilation

License
-------

See [LICENSE](https://raw.githubusercontent.com/meng-hui/UnityEngineAnalyzer/master/LICENSE)
