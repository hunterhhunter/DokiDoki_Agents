import sys
sys.path.insert(0, 'C:/FastAPI')


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
    received = [
        UN_Data('HonalduSon', location, ["Franz Alez"]),
        UN_Data('Franz Alez', location, ['HonalduSon'])
        ]

    executions = [] 

    # ---- 초기화
    evc = EventChecker()
    _ = ['Emerald Puyor', 'Franz Alez', 'HonalduSon']
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
        
        print("""============================================
                                    시작                    
                ============================================""")
        perceived = persona.perceive(u_data.get_object(), evc)
        print("""============================================
                                  perceive                    
                ============================================""")
        retrieved = persona.retrieve(perceived)
        print("""============================================
                                  retrieve                  
                ============================================""")
        # planning = persona.plan(u_data.get_location(), personas, 'First day', retrieved)
        planning = persona.plan(u_data.get_location(), personas, False, retrieved)
        print("""============================================
                                  Planning             
                ============================================""")
        execution = persona.execute(planning)
        print("""============================================
                                  execute
                ============================================""")
        
        print("check /bootstrap_memory")
        print(execution)
        executions.append(execution)
        
    for p_name in personas.keys():
        personas[p_name].save(_dir + u_data.get_persona() + "/bootstrap_memory")

    return {'executions': executions}
    