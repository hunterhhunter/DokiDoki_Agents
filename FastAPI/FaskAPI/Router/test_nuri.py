import sys
sys.path.insert(0, 'C:/FastAPI')
import openai
openai.api_key = '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'

from fastapi import APIRouter
from main_back.backend.persona.personas import *
from main_back.backend.event_checker import EventChecker
from main_back.backend.unity_ipc import UN_Data


test_nuri_2 = APIRouter(prefix='/test')


@test_nuri_2.get('/nuri')
def start_test(time: str):
    _dir = 'C:/FastAPI/main_back/data/personas/'

    # ----- 유니티로부터 데이터 받는 경우
    location = {
    "world":"dokidoki village", "sector":"Puyor's Store", "arena" : "supply store"
    }
    location2 = {"world":"dokidoki village", "sector":"Franz Alez's Fishing Tackle Shop", "arena" : "Shop"}
    received = [
        UN_Data('Emerald Puyor', location, ['Franz Alez']),
        UN_Data('Franz Alez', location, ['Emerald Puyor'])
        ]

    executions = [] 

    # ---- 초기화
    evc = EventChecker()
    _ = ['Emerald Puyor', 'Franz Alez']
    personas = {}
    for p_name in _:
        persona = Persona(p_name, _dir + p_name)
        evc.add_event(persona.scratch.get_curr_event_and_desc())
        personas[p_name] = persona

        # ----- 서버에서 반복
    for u_data in received:
        persona = personas[u_data.get_persona()]
        persona.scratch.curr_time = datetime.datetime.strptime(time,
                                                               "%B %d, %Y, %H:%M:%S")
        
        print("""\t\t============================================
                                    시작 %s                  
                ============================================""".format(persona.scratch.name))
        
        print("""\t\t============================================
                                  perceive                    
                ============================================""") 
        perceived = persona.perceive(u_data.get_object(), evc)

        print("""\t\t============================================
                                  retrieve                  
                ============================================""")
        retrieved = persona.retrieve(perceived)

        for p_name in personas.keys():
            personas[p_name].save(_dir + p_name + "/bootstrap_memory")

        print("""\t\t============================================
                                  Planning             
                ============================================""")
        # planning = persona.plan(u_data.get_location(), personas, 'First day', retrieved)
        planning = persona.plan(u_data.get_location(), personas, False, retrieved)

        for p_name in personas.keys():
            personas[p_name].save(_dir + p_name + "/bootstrap_memory")

        print("""\t\t============================================
                                  execute
                ============================================""")
        execution = persona.execute(planning)

        
        return_dict = {"Sub": execution[0][0], "P": execution[0][1], "Obj": execution[0][2], "location": execution[1][0], "duration": execution[2]}

        if persona.scratch.chat:
            ls = list()
            prompt = ''
            prompt += 'You are a translator that can translate English into Korean, tailoring the tone of the conversation to match the personalities of the characters. For example, a conversation involving a man in his 40s might be translated with a tone like "~ 하겠군" while for a man in his 70s, it might be more appropriate to use a tone like "~하는구만.."'
            for i in persona.scratch.chat:
                print(type(i[0]))
                per = personas[i[0]]
                prompt += per.scratch.get_str_iss() + "\n\n"
                prompt += 'Read the descriptions of the characters given above and translate the following conversation into Korean, matching their personalities" in English.\n----------------------------------------------------------\nconversation\n'

            for name, text in persona.scratch.chat:
                prompt += f'"{name}: {text}" \n'
            prompt += 'output form(python string):\n"name(english): 대화내용"<spliter>\n"name(english): 대화내용"\n<spliter>"name(english): 대화내용"\n\nnegetive keyword: descriptions, comma in sentence'

            completion = openai.ChatCompletion.create(
                model="gpt-4",
                messages=[{"role": "assistant", "content": prompt}]
            )

            chatting = completion["choices"][0]["message"]["content"]
            print(f'chatting: {chatting}')
            if '<spliter>' in chatting:
                cha_lis = chatting.split('<spliter>')
                print(f'cha_lis: {cha_lis}')
                for i in cha_lis:
                    namesz = i.split(':')
                    print(f'namesz {namesz}')
                    names = namesz[0][1:]
                    text1 = namesz[1][:-1]
                    ls.append(names)
                    ls.append(text1)

            else:
                cha_lis = chatting.split('\n')
                print(f'cha_lis: {cha_lis}')
                for i in cha_lis:
                    namesz = i[0].split(':')
                    print(f'names {namesz}')
                    names = namesz[0][1:]
                    text1 = namesz[1][:-1]
                    ls.append(names)
                    ls.append(text1)

        print(f'ls: {ls}')
        if ls:
            return_dict['chat'] = ls
        else:
            return_dict['chat'] = None

        print("check /bootstrap_memory")
        print(return_dict)
        executions.append(return_dict)
        

    return {'executions': executions}