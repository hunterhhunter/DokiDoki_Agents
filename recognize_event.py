from 시동코드.persona.cognitive_modules.plan import *
from 시동코드.persona.cognitive_modules.reflect import *
from 시동코드.persona.cognitive_modules.retrieve import *
import datetime
import math
import random 
import sys
import time
sys.path.append('../../')


"""
========================================================
                유니티 -> 백엔드 처리 함수
========================================================
"""
import json
from typing import List, Tuple, Union

def perceive(file_path: str) -> Union[Tuple[str, List[str]], bool]:
	"""유니티에서 수신 받은 데이터 반환 함수 json ->"""
	try:
		with open(file_path) as file:
			perceived_obj = json.load(file)
	
		persona_name: str = perceived_obj['persona']
		perceived_objects: List[str] = perceived_obj['perceived_obj']
	
		return persona_name, perceived_objects
		
	except Exception as e:
		print(f'오류 발생: {e}')
		return False


def execute_act(persona, act, target=None):
	"""유니티로 SPO를 보내 행동하게 하는 함수"""

"""
하루 계획을 세우는 함수 (persona.scratch 아래의 내용을 변경)
예: f_daily_schedule_hourly_org: [['sleeping', 60], ['morning routine', 60], ...]
    daily_req: "wake up and complete the morning routine at 7:00 am", 
	           "open the Fishing Tackle Shop at 12:00 pm", ...
"""
plan._long_term_planning(persona, "New day")

"""
====================================================
        계속 실행하며 상태 변화를 감지해야함
====================================================
"""

"""인지 주체와 인지된 객체"""
persona, perceived_objects = perceive(file_path)

"""감지한 것이 있다면"""
if persona and perceived_objects:
	
    # persona_dict: {"persona_name": persona_객체}
    # new_retrieve로 인지된 객체들과 관련된 정보를 n_count개 가져옴
	retrieved: List[str] = retrieve.new_retrieve(persona_dict[persona], perceived_objects, n_count=30)
	# 불러온 관련 정보 중 하나를 선택함
	choosed_retrieved: List[str] = plan._choose_retrieved(persona, retrieved)
	# 선택한 정보를 가지고 반응을 설정
    # 예: Subject:영진  Predicate: chat with   Object: 태경
	decide_react: str = plan._should_react(persona, choosed_retrieved, personas)
	execute_act(persona, decide_react, target=None) # S, P, O 형식