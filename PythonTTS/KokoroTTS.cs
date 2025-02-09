using Python.Runtime;

namespace PythonTTS
{
    public class KokoroTTS
    {
        private readonly dynamic tts;

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
                    "sys.stdout = mystdout = StringIO()\n" +
                    "sys.stdout.flush()\n" +
                    "sys.stderr = mystderr = StringIO()\n" +
                    "sys.stderr.flush()\n";
                PythonEngine.Exec(codeToRedirectOutput);

                tts = Py.Import(@"KokoroTTS");
            }
        }

        public Task StartSpeechAsync(string text)
        {
            return Task.Run(() =>
            {
                using (Py.GIL())
                {
                    tts.tts(text);
                }
            });
        }

        public void Stop()
        {
            using (Py.GIL())
            {
                tts.stop(); 
            }
        }

        public bool IsSpeaking()
        {
            using (Py.GIL())
            {
                return tts.is_speaking();
            }
        }
    }
}
