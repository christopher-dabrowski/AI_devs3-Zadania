import os
import whisper
from pathlib import Path

def transcribe_audio_files(directory_path):
    # Check if directory exists
    if not os.path.exists(directory_path):
        print(f"Directory '{directory_path}' does not exist.")
        return

    # Create transcriptions directory if it doesn't exist
    transcriptions_dir = "transcriptions"
    Path(transcriptions_dir).mkdir(exist_ok=True)

    # Load Whisper model
    print("Loading Whisper model...")
    model = whisper.load_model("medium")  # Using medium model for better accuracy

    # Walk through the directory
    for root, _, files in os.walk(directory_path):
        for file in files:
            # Check if file is an audio file
            if file.lower().endswith(('.mp3', '.wav', '.m4a')):
                input_path = os.path.join(root, file)
                output_path = os.path.join(transcriptions_dir, f"{os.path.splitext(file)[0]}.txt")
                
                print(f"Transcribing {file}...")
                try:
                    # Transcribe with Whisper (optimized for Polish)
                    result = model.transcribe(
                        input_path,
                        language="pl",
                        task="transcribe"
                    )
                    
                    # Save transcription
                    with open(output_path, 'w', encoding='utf-8') as f:
                        f.write(result["text"])
                    
                    print(f"Transcription saved to {output_path}")
                
                except Exception as e:
                    print(f"Error transcribing {file}: {str(e)}")

if __name__ == "__main__":
    directory_path = "przesluchania"
    print(f"Transcribing audio files in {directory_path}...")
    transcribe_audio_files(directory_path)
