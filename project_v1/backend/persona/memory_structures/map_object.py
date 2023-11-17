"""
게임 내에서 오브젝트 상태를 받아서 해당 상태를 json 형태로 저장한다.
curr_object_state 는 현재 모든 오브젝트의 상태를 저장하며, persona가 알고있는 object를 아는 spatial_memory 와는 다른 목적을 가진 함수이다.
"""

import json
import os

import sys
sys.path.insert(0, '../')

from global_methods import *

class Object_state:
    def __init__(self, f_saved):
        self.f_saved = f_saved
        if check_if_file_exists(f_saved):
            self.object_state = json.load(open(f_saved))
            
    def perceive_object_state(self, arena, perceive_objects):
        object_state = json.load(open(self.f_saved))
        curr_perceive_objects = {}

        for location, houses in object_state.items():
            for house, rooms in houses.items():
                if arena in rooms:
                    for room, objects in rooms.items():
                        for obj in objects:
                            if obj in perceive_objects:
                                # 객체의 최상위부터 최하위 구조까지 curr_perceive_objects에 추가
                                if location not in curr_perceive_objects:
                                    curr_perceive_objects[location] = {}
                                if house not in curr_perceive_objects[location]:
                                    curr_perceive_objects[location][house] = {}
                                if room not in curr_perceive_objects[location][house]:
                                    curr_perceive_objects[location][house][room] = {}

                                curr_perceive_objects[location][house][room][obj] = objects[obj]

        return curr_perceive_objects
            
    
    def save_object_state(self, arena, object_to_update, new_state):
        # JSON 파일 불러오기
        object_state = json.load(open(self.f_saved))

        # JSON 파일의 모든 레벨을 탐색하여 객체를 찾고 상태를 업데이트
        for location, houses in object_state.items():
            for house, rooms in houses.items():
                if arena in rooms:
                    for room, objects in rooms.items():
                        if object_to_update in objects:
                            # 객체의 상태 업데이트
                            objects[object_to_update] = new_state
                            # 상태 업데이트 후 파일에 저장
                            with open(self.f_saved, 'w') as file:
                                json.dump(object_state, file, indent=4)
                            return True  # 객체를 찾아 업데이트한 경우 True 반환

        return False 

    def convert_list_to_dict_with_idle(json_data):
        for sector in json_data.values():
            for arena in sector.values():
                for object_key, object in arena.items():
                    # arena가 리스트인 경우, 딕셔너리로 변환
                    if isinstance(object, list):
                        new_object = {}
                        for item in object:
                            new_object[item] = 'idle'
                        arena[object_key] = new_object


if __name__=='__main__':
    map = Object_state('/home/elicer/main/agent_test3/backend/env/object_state/object_state.json')
    curr_object_state = map.perceive_object_state("pub", "shelf")
    print(curr_object_state)
    map.save_object_state("pub", "shelf","run")

    