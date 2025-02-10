using System.Reflection;
using System.Runtime.Loader;

#nullable disable

internal class Program
{
    private static void Main(string[] args)
    {
        for (int i = 0; i < 3; i++)
        {
            var pluginLoadContext = new PluginLoadContext(@"C:\Users\Leftos\source\repos\PythonTTS\PythonTTS\bin\x64\Debug\net9.0\PythonTTS.dll");
            var assembly = pluginLoadContext.LoadFromAssemblyName(new AssemblyName("PythonTTS"));
            dynamic kokoro = Activator.CreateInstance(assembly.GetType("PythonTTS.KokoroTTS"), @"C:\Users\Leftos\source\repos\PythonTTS\PythonTTS\bin\x64\Debug\net9.0\python3.12.9\python312.dll");
            Task speakStartTask = kokoro.StartSpeechAsync("This is a text-to-speech test.");
            while (!speakStartTask.IsCompleted)
            {
                Console.Write($"\rWaiting on speech to start...");
                Thread.Sleep(100);
            }

            Console.WriteLine();
            int secondsElapsed = 0;
            while (kokoro.IsSpeaking())
            {
                Thread.Sleep(1000);
                secondsElapsed++;
                Console.Write($"\rSpeaking for {secondsElapsed} seconds...");
            }

            Console.WriteLine();
        }
    }

    class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath) : base(true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}