from persona.persona import *
from event_checker import EventChecker
from unity_ipc import UN_Data

_dir = 'C:/Users/gjaischool1/Desktop/git/DokiDoki_Agents/main/data/personas/'

# ----- 유니티로부터 데이터 받는 경우
location = {
    "world":"dokidoki village", "sector":"Puyor's Store", "arena" : "supply store"
}
received = [
    UN_Data('Emerald Puyor', location, ["Franz Alez"]),
    UN_Data('HonalduSon', location, ["Emerald Puyor"]),
    UN_Data('Franz Alez', location, ["HonalduSon"])
]

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
    persona.scratch.curr_time = datetime.datetime.strptime("February 14, 2023, 08:00:00", 
                                                "%B %d, %Y, %H:%M:%S")
    perceived = persona.perceive(u_data.get_object(), evc)
    retrieved = persona.retrieve(perceived)
    # planning = persona.plan(u_data.get_location(), personas, 'First day', retrieved)
    planning = persona.plan(u_data.get_location(), personas, False, retrieved)
    execution = persona.execute(planning)

    persona.save(_dir + u_data.get_persona() + "/bootstrap_memory")

print('end')
