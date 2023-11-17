import os
import json

import sys
sys.path.append('../../')

from global_methods import *

from persona.cognitive_modules.retrieve import *

from persona.prompt_template.run_gpt_prompt import *

def decide(persona, objects, player_action, player_chat):
    if player_action:
        decide_player(persona, objects, player_action, player_chat)

    retrieved = new_retrieve(persona, objects)
    output = True
    # output = run_gpt_generate_decide(persona, objects, retrieved)
    return output



def decide_player(persona, objects, player_action, player_chat):
