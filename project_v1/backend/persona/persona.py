import os
import sys
sys.path.append('../../')

from global_methods import *

from persona.memory_structures.spatial_memory import *
from persona.memory_structures.associative_memory import *
from persona.memory_structures.scratch import *

from persona.cognitive_modules.decide import *
from persona.cognitive_modules.converse import *

class Persona: 
    def __init__(self, name, folder_mem_saved=False):
        # PERSONA BASE STATE 
        # <name> is the full name of the persona. This is a unique identifier for
        # the persona within Reverie. 
        self.name = name

        # PERSONA MEMORY 
        # If there is already memory in folder_mem_saved, we load that. Otherwise,
        # we create new memory instances. 
        # <s_mem> is the persona's spatial memory. 
        f_s_mem_saved = f"{folder_mem_saved}/bootstrap_memory/spatial_memory.json"
        self.s_mem = MemoryTree(f_s_mem_saved)
        # <s_mem> is the persona's associative memory. 
        f_a_mem_saved = f"{folder_mem_saved}/bootstrap_memory/associative_memory"
        self.a_mem = AssociativeMemory(f_a_mem_saved)
        # <scratch> is the persona's scratch (short term memory) space. 
        scratch_saved = f"{folder_mem_saved}/bootstrap_memory/scratch.json"
        self.scratch = Scratch(scratch_saved)


    def decide(self, persona, objects, player_action, player_chat):
        decide_boolean = decide_react(persona, objects, player_action, player_chat)

        if decide_boolean[0]:
            description, converse_partner, spo = generate_action_description(persona, decide_boolean[1])
            if converse_partner:
                chat = agent_chat_one_utterance(persona, converse_partner)
            return description, chat, spo
        else:
            pass



    def move(self,persona, objects, player_action, player_chat):
        description, chat, spo = self.decide(persona, objects, player_action, player_chat)

        # 저장하는 코드
        
        return spo, chat




    


