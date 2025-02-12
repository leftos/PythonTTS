from kokoro import KPipeline
import numpy
import sounddevice as sd

pipeline = KPipeline(lang_code='a')
KOKORO_SAMPLE_RATE = 24000

voice = 'af_heart'
speed = 1

def set_voice(_voice):
    global voice
    voice = _voice
    
def get_voice():
    return voice
    
def set_speed(_speed):
    global speed
    speed = _speed
    
def tts(text):    
    generator = pipeline(text, voice=voice, speed=speed, split_pattern=r'[\n\.!?]')
        
    complete_audio = None
    for _, _, audio in generator:
        if complete_audio is None:
            complete_audio = audio
        else:
            complete_audio = numpy.concatenate((complete_audio, audio))
    
    sd.play(complete_audio, 24000)
    
    duration = complete_audio.shape[0] / KOKORO_SAMPLE_RATE
    return duration

def stop():
    sd.stop()

def is_speaking():
    try:
        return sd.get_stream().active
    except RuntimeError:
        # We haven't called play yet; might still be generating speech, or were never asked to generate any
        return False

if __name__ == "__main__":
    tts("Ladies and gentlemen, this is your Captain speaking. Welcome aboard this Airbus A320. Before we take off, I’d like to go over some important safety information. Please ensure that your seatbelt is fastened low and tight across your lap. It must be worn at all times when seated, and whenever the seatbelt sign is illuminated. In the event of a loss of cabin pressure, oxygen masks will drop down from the panel above you. Pull the mask toward you to start the flow of oxygen, place it over your nose and mouth, and secure it with the elastic band. Remember to put your mask on first before assisting others. Life vests are located under your seat. In the unlikely event of an evacuation over water, pull the vest out, slip it over your head, and pull the straps to tighten. To inflate the vest, pull on the red cord. You can also inflate it by blowing into the tube. For your safety, note the exits located at the front and rear of the cabin. The nearest exit may be behind you. Thank you for your attention. We'll be on our way shortly.")