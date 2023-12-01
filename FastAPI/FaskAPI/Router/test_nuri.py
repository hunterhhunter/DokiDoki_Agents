import sys
sys.path.insert(0, 'C:/FastAPI')
import openai
openai.api_key = ''

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
    ls = []
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
            ls.append(persona.scratch.chat)

        if ls:
            return_dict['chat'] = ls
        else:
            return_dict['chat'] = None

        print("check /bootstrap_memory")
        print(return_dict)
        executions.append(return_dict)
        

    return {'executions': executions}