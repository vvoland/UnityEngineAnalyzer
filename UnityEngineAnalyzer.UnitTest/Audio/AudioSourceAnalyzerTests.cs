using Linty.Analyzers;
using Linty.Analyzers.Audio;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;

namespace UnityEngineAnalyzer.Test.Audio
{
    [TestFixture]
    sealed class AudioSourceAnalyzerTests : AnalyzerTestFixture
    {
        protected override DiagnosticAnalyzer CreateAnalyzer() => new AudioSourceAnalyzer();

        [Test]
        public void AudioSourceShouldRaiseInfoTextAboutAudioSourceMuteUsingCPUCycles()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    AudioSource audioSource;

    void Update()
    {
        [|audioSource.mute = true;|]
    }
}";

            HasDiagnostic(code, DiagnosticIDs.AudioSourceMuteUsesCPU);
        }
    }
}
