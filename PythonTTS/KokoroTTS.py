import io

from kokoro import KPipeline
import soundfile as sf
import sounddevice as sd

pipeline = KPipeline(lang_code='a')

voice = 'af_heart'
speed = 1

def set_voice(_voice):
    global voice
    voice = _voice
    
def set_speed(_speed):
    global speed
    speed = _speed
    
def tts(text):
    generator = pipeline(text, voice=voice, speed=speed, split_pattern=r'\n+')
    
    for (_, _, audio) in generator:
        sd.play(audio, 24000)

def stop():
    sd.stop()

def is_speaking():
    try:
        return sd.get_stream().active
    except RuntimeError:
        # We haven't called play yet; might still be generating speech, or were never asked to generate any
        return False

if __name__ == "__main__":
    tts("Hello, my name is Jessica!")