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
	"""유니티에서 수신 받은 데이터 반환 함수"""
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


_long_term_planning(persona, "New day")

persona, perceived_objects = perceive(file_path)

if persona and perceived_objects:
	retrieved: List[str] = retrieve.new_retrieve(persona, persona_dict[persona], n_count=30)
	choosed_retrieved: List[str] = plan._choose_retrieved(persona, retrieved)
	decide_react: str = plan._should_react(persona, choosed_retrieved, personas)
	execute_act(persona, decide_react, target=None) # S, P, O 형식