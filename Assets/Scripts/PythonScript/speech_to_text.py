# speech_to_text.py
import requests
import base64
import sys
import json

def speech_to_text(api_key, audio_file_path):
    with open(audio_file_path, 'rb') as audio_file:
        audio_content = base64.b64encode(audio_file.read()).decode('utf-8')

    url = f'https://speech.googleapis.com/v1/speech:recognize?key={api_key}'
    headers = {
        'Content-Type': 'application/json'
    }
    data = {
        'config': {
            'encoding': 'LINEAR16',
            'sampleRateHertz': 16000,
            'languageCode': 'ko-KR'
        },
        'audio': {
            'content': audio_content
        }
    }

    response = requests.post(url, headers=headers, json=data)
    return response.json()

if __name__ == '__main__':
    api_key = sys.argv[1]
    audio_file_path = sys.argv[2]
    result = speech_to_text(api_key, audio_file_path)
    print(json.dumps(result))
