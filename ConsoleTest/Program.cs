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
            Task<TimeSpan> speakStartTask = kokoro.StartSpeechAsync("Ladies and gentlemen, this is your Captain speaking. Welcome aboard this Airbus A320. Before we take off, I’d like to go over some important safety information. Please ensure that your seatbelt is fastened low and tight across your lap. It must be worn at all times when seated, and whenever the seatbelt sign is illuminated. In the event of a loss of cabin pressure, oxygen masks will drop down from the panel above you. Pull the mask toward you to start the flow of oxygen, place it over your nose and mouth, and secure it with the elastic band. Remember to put your mask on first before assisting others. Life vests are located under your seat. In the unlikely event of an evacuation over water, pull the vest out, slip it over your head, and pull the straps to tighten. To inflate the vest, pull on the red cord. You can also inflate it by blowing into the tube. For your safety, note the exits located at the front and rear of the cabin. The nearest exit may be behind you. Thank you for your attention. We'll be on our way shortly.");
            while (!speakStartTask.IsCompleted)
            {
                Console.Write($"\rWaiting on speech to start...");
                Thread.Sleep(100);
            }

            Console.WriteLine();
            Console.WriteLine($"Generated speech duration: {speakStartTask.Result}");
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