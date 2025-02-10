using System.Diagnostics.CodeAnalysis;
using Python.Runtime;

// ReSharper disable UnusedMember.Global

namespace PythonTTS
{
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public class KokoroTTS
    {
        private readonly dynamic _tts;

        public KokoroTTS(string pythonDLL)
        {
            Runtime.PythonDLL = pythonDLL;
            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();

            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.dont_write_bytecode = true;

                string codeToRedirectOutput =
                    "import sys\n" +
                    "from io import StringIO\n" +
                    // ReSharper disable once StringLiteralTypo
                    "sys.stdout = mystdout = StringIO()\n" +
                    "sys.stdout.flush()\n" +
                    // ReSharper disable once StringLiteralTypo
                    "sys.stderr = mystderr = StringIO()\n" +
                    "sys.stderr.flush()\n";
                PythonEngine.Exec(codeToRedirectOutput);

                _tts = Py.Import(@"KokoroTTS");
            }
        }

        public Task StartSpeechAsync(string text)
        {
            return Task.Run(() =>
            {
                using (Py.GIL())
                {
                    _tts.tts(text);
                }
            });
        }

        public void Stop()
        {
            using (Py.GIL())
            {
                _tts.stop(); 
            }
        }

        public bool IsSpeaking()
        {
            using (Py.GIL())
            {
                return _tts.is_speaking();
            }
        }

        public void SetVoice(string voiceId)
        {
            ArgumentNullException.ThrowIfNull(voiceId);

            using (Py.GIL())
            {
                _tts.set_voice(voiceId);
            }
        }
        
        public string GetVoice()
        {
            using (Py.GIL())
            {
                return _tts.get_voice();
            }
        }

        public string GetVoicesJson(string pythonTTSDir) => File.ReadAllText(Path.Combine(pythonTTSDir, @"KokoroTTS_Voices.json"));
    }
}
