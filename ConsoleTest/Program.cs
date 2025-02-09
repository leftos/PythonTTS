using PythonTTS;

internal class Program
{
    private static void Main(string[] args)
    {
        var kokoro = new KokoroTTS(@"python3.12.9\python312.dll");
        Task speakStartTask = kokoro.StartSpeechAsync("This is a very long text that will be spoken. And this is the second sentence. And this is the third sentence. And this is the fourth sentence. And this is the fifth sentence. And this is the sixth sentence. And this is the seventh sentence. And this is the eighth sentence. And this is the ninth sentence. And this is the tenth sentence.");
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