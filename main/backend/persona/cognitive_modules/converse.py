from persona.cognitive_modules.retrieve import *
from persona.prompt_template.run_gpt_prompt import *

def generate_one_utterance(location, init_persona, target_persona, retrieved, curr_chat): 
  # Chat version optimized for speed via batch generation
  curr_context = (f"{init_persona.scratch.name} " + 
              f"was {init_persona.scratch.act_description} " + 
              f"when {init_persona.scratch.name} " + 
              f"saw {target_persona.scratch.name} " + 
              f"in the middle of {target_persona.scratch.act_description}.\n")
  curr_context += (f"{init_persona.scratch.name} " +
              f"is initiating a conversation with " +
              f"{target_persona.scratch.name}.")

  print ("July 23 5")
  x = run_gpt_generate_iterative_chat_utt(location, init_persona, target_persona, retrieved, curr_context, curr_chat)[0]

  print ("July 23 6")

  print ("adshfoa;khdf;fajslkfjald;sdfa HERE", x)

  return x["utterance"], x["end"]

def generate_summarize_agent_relationship(init_persona, 
                                          target_persona, 
                                          retrieved): 
  all_embedding_keys = list()
  for key, val in retrieved.items(): 
    for i in val: 
      all_embedding_keys += [i.embedding_key]
  all_embedding_key_str =""
  for i in all_embedding_keys: 
    all_embedding_key_str += f"{i}\n"

  summarized_relationship = run_gpt_prompt_agent_chat_summarize_relationship(
                              init_persona, target_persona,
                              all_embedding_key_str)[0]
  return summarized_relationship

def agent_chat_v2(location, init_persona, target_persona): 
  curr_chat = []
  print ("July 23")

  for i in range(8): 
    focal_points = [f"{target_persona.scratch.name}"]
    retrieved = new_retrieve(init_persona, focal_points, 50)
    relationship = generate_summarize_agent_relationship(init_persona, target_persona, retrieved)
    print ("-------- relationshopadsjfhkalsdjf", relationship)
    last_chat = ""
    for i in curr_chat[-4:]:
      last_chat += ": ".join(i) + "\n"
    if last_chat: 
      focal_points = [f"{relationship}", 
                      f"{target_persona.scratch.name} is {target_persona.scratch.act_description}", 
                      last_chat]
    else: 
      focal_points = [f"{relationship}", 
                      f"{target_persona.scratch.name} is {target_persona.scratch.act_description}"]
    retrieved = new_retrieve(init_persona, focal_points, 15)
    utt, end = generate_one_utterance(location, init_persona, target_persona, retrieved, curr_chat)

    curr_chat += [[init_persona.scratch.name, utt]]
    if end:
      break


    focal_points = [f"{init_persona.scratch.name}"]
    retrieved = new_retrieve(target_persona, focal_points, 50)
    relationship = generate_summarize_agent_relationship(target_persona, init_persona, retrieved)
    print ("-------- relationshopadsjfhkalsdjf", relationship)
    last_chat = ""
    for i in curr_chat[-4:]:
      last_chat += ": ".join(i) + "\n"
    if last_chat: 
      focal_points = [f"{relationship}", 
                      f"{init_persona.scratch.name} is {init_persona.scratch.act_description}", 
                      last_chat]
    else: 
      focal_points = [f"{relationship}", 
                      f"{init_persona.scratch.name} is {init_persona.scratch.act_description}"]
    retrieved = new_retrieve(target_persona, focal_points, 15)
    utt, end = generate_one_utterance(location, target_persona, init_persona, retrieved, curr_chat)

    curr_chat += [[target_persona.scratch.name, utt]]
    if end:
      break

  print ("July 23 PU")
  for row in curr_chat: 
    print (row)
  print ("July 23 FIN")

  return curr_chat