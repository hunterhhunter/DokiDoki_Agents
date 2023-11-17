import os
import sys
sys.path.append('../../')

from global_methods import *

from persona.

def generate_agent_chat_summarize(init_persona, target_persona):
    
    init_persona_curr_chat = init_persona.a_mem.curr_chat
    target_persona_curr_chat = target_persona.a_mem.curr_chat

    chat_summarized = run_gpt_prompt_agent_chat_summarize(init_persona_curr_chat)
    chat_summarize_embedding = get_embedding(chat_summarized)

    embedding_pair= (chat_summarized, chat_summarize_embedding)
    init_persona.a_mem.add_chat_without_thought(init_persona.scaratch.name, 'chat with', target_persona.scratch.name,
                                chat_summarized, embedding_pair, 
                                init_persona_curr_chat)
    target_persona.a_mem.add_chat_without_thought(target_persona.scratch.name, "chat with", init_persona.scaratch.name,
                                chat_summerized, embedding_pair,
                                init_persona_curr_chat)
    init_persona.a_mem.curr_chat = []
    target_persona.a_mem.curr_chat = []


def agent_chat_one_utterance(init_persona, target_persona):
    curr_chat = []
    focal_points = [f"{target_persona.scratch.name}"]
    # retrieved = new_retrieve(init_persona, focal_points, 50)
    retrieved = 'asd'
    target_relationship = init_persona.a_mem.load_relationship(target_persona)
    last_chat = ""
    if init_persona.a_mem.curr_chat == []:
        utt, end = run_gpt_generate_converse_first(init_persona, target_persona, retrieved, target_relationship)
    else:
        utt, end = run_gpt_generate_converse_second(init_persona, target_persona, retrieved, target_relationship)
    
    
    if end:
        init_persona.a_mem.add_curr_chat(utt, end)
        generate_agent_chat_summarize
    init_persona.a_mem.add_curr_chat(utt, end)
    target_persona.a_mem.add_curr_chat(utt, end)

    return init_persona.a_mem.curr_chat