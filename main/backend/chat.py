import sys
import json

from persona.persona import *
sys.stdout.reconfigure(encoding='utf-8')
if __name__ == "__main__":
    _dir = 'C:/Users/gjaischool1/Desktop/git/DokiDoki_Agents/main/data/personas/'

    receivedJson = sys.argv[1]
    receivedJson = json.loads(receivedJson)
    persona = receivedJson['persona']
    convo = receivedJson['convo']
    print(convo)

    # persona = 'Isabella Rodriguez'
    # convo = [["user", "오늘 언제 문을 닫나요?"]]
    try:
        persona = Persona(persona, _dir + persona)
        chat = persona.open_convo(convo)
        print(chat)
    except Exception as e:
        print(e)