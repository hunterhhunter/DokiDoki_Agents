import os
import sys

import json
import datetime

import global_methods

from persona.persona import Persona

from persona.cognitive_modules.plan import *

class Server_open():
    def __init__(self, front_folder):

        with open(f"{front_folder}/server/meta.json") as json_file:  
            game_meta = json.load(json_file)
        current_date = datetime.datetime.now().strftime("%B %d, %Y, 00:00:00")
        # 실제 시작 시간
        if game_meta['real_start_date'] == "":
            start_time = datetime.datetime.strptime(current_date, "%B %d, %Y, %H:%M:%S")
        else:
            self.start_time = datetime.datetime.strptime(
                        f"{game_meta['real_start_date']}, 00:00:00",  
                        "%B %d, %Y, %H:%M:%S")
        # 실제 현재 시간
        if game_meta['real_curr_time'] == "":
            start_time = datetime.datetime.strptime(current_date, "%B %d, %Y, %H:%M:%S")
        else:
            self.start_time = datetime.datetime.strptime(
                        f"{game_meta['real_curr_time']}, 00:00:00",  
                        "%B %d, %Y, %H:%M:%S")
        
        # 게임의 시작 시간
        if game_meta['game_start_date'] == "":
            start_time = datetime.datetime.strptime(current_date, "%B %d, %Y, %H:%M:%S")
        else:
            self.start_time = datetime.datetime.strptime(
                        f"{game_meta['start_date']}, 00:00:00",  
                        "%B %d, %Y, %H:%M:%S")
        # 게임의 현재 시간
        if game_meta['game_curr_time'] == "":
            self.curr_time = datetime.datetime.strptime(current_date, "%B %d, %Y, %H:%M:%S")
        else:
            self.curr_time = datetime.datetime.strptime(game_meta['curr_time'], "%B %d, %Y, %H:%M:%S")
        
        self.personas = dict()


        for persona_name in game_meta['persona_names']:
            persona_folder = f"{front_folder}/personas/{persona_name}"
            curr_persona = Persona(persona_name,persona_folder)
            self.personas[persona_name] = curr_persona

    
    def save(self):
        front_folder = 'asd'
        return
    

    # def perceieve_from_game(self):
    #     persona.perceieve == True
        
    

    # def call_persona(self, persona):
    #     if perceieve_from_game(self):
    #         persona.move()


    def start_server(self, persona):
        persona_folder = f""

        game_obj_cleanup = dict()

        server_close = False

        wake_up_time = generate_wake_up_hour(persona)
        daily_req = generate_first_daily_request(persona):
        daily_plan = generate_first_daily_plan(persona, wake_up_time)

        while (True):
            if server_close == True:
                break
            





if __name__ == '__main__':
    server_start = Server_open(r'C:\Users\gjaischool\project\project_v1\frontend')



        
        
    
