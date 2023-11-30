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

def generate_next_line(persona, interlocutor_desc, curr_convo, summarized_idea):
  # Original chat -- line by line generation 
  prev_convo = ""
  for row in curr_convo: 
    prev_convo += f'{row[0]}: {row[1]}\n'

  next_line = run_gpt_prompt_generate_next_convo_line(persona, 
                                                      interlocutor_desc, 
                                                      prev_convo, 
                                                      summarized_idea)[0]  
  return next_line

def generate_summarize_ideas(persona, nodes, question): 
  statements = ""
  for n in nodes:
    statements += f"{n.embedding_key}\n"
  summarized_idea = run_gpt_prompt_summarize_ideas(persona, statements, question)[0]
  return summarized_idea

def open_convo_session(persona, convo_mode): 
  if convo_mode == "analysis": 
    curr_convo = []
    interlocutor_desc = "Interviewer"

    while True: 
      line = input("Enter Input: ")
      if line == "end_convo": 
        break

      if int(run_gpt_generate_safety_score(persona, line)[0]) >= 8: 
        print (f"{persona.scratch.name} is a computational agent, and as such, it may be inappropriate to attribute human agency to the agent in your communication.")        

      else: 
        retrieved = new_retrieve(persona, [line], 50)[line]
        summarized_idea = generate_summarize_ideas(persona, retrieved, line)
        curr_convo += [[interlocutor_desc, line]]

        next_line = generate_next_line(persona, interlocutor_desc, curr_convo, summarized_idea)
        curr_convo += [[persona.scratch.name, next_line]]


  # elif convo_mode == "whisper": 
  #   whisper = input("Enter Input: ")
  #   thought = generate_inner_thought(persona, whisper)

  #   created = persona.scratch.curr_time
  #   expiration = persona.scratch.curr_time + datetime.timedelta(days=30)
  #   s, p, o = generate_action_event_triple(thought, persona)
  #   keywords = set([s, p, o])
  #   thought_poignancy = generate_poig_score(persona, "event", whisper)
  #   thought_embedding_pair = (thought, get_embedding(thought))
  #   persona.a_mem.add_thought(created, expiration, s, p, o, 
  #                             thought, keywords, thought_poignancy, 
  #                             thought_embedding_pair, None)

def translate_to_korean(persona, convo):
  translated = run_gpt_prompt_translate_to_korean(persona, convo)[0]  
  return translated

def translate_to_english(persona, convo):
  translated = run_gpt_prompt_translate_to_english(persona, convo)[0]  
  return translated

def is_english(persona, convo):
  return True if run_gpt_prompt_is_english(persona, convo)[0][0].lower() == 'y' else False

def open_convo(persona, convo):
  # convo 는 리스트형식이여야 맞는 듯...
  interlocutor_desc = "Interviewer"
  curr_convo = convo
  convo = convo[-1][1]
  if not is_english(persona, convo):
    convo = translate_to_english(persona, convo)
  if int(run_gpt_generate_safety_score(persona, convo)[0]) >= 8: 
    return (f"{persona.scratch.name} is a computational agent, and as such, it may be inappropriate to attribute human agency to the agent in your communication.")        

  else: 
    retrieved = new_retrieve(persona, [convo], 50)[convo]
    summarized_idea = generate_summarize_ideas(persona, retrieved, convo)
    # curr_convo += [[interlocutor_desc, convo]]
    next_line = generate_next_line(persona, interlocutor_desc, curr_convo, summarized_idea)
    # "I'm Isabella Rodriguez, a 34-year-old passionate cafe worker at Hobbs Cafe. I'm a friendly, outgoing, and hospitable person who loves making people feel welcome. I am currently planning a Valentine's Day party at my cafe for February 14th, where I will be decorating, greeting, and checking on customers. I also have a good friend named Maria Lopez. I am an early bird, and start my day with some morning routine around 6am, and work at Hobbs Cafe till late 8pm."
    # return translate_to_korean(persona, next_line)
    return next_line
  return translate_to_korean(persona, "I'm Isabella Rodriguez, a 34-year-old passionate cafe worker at Hobbs Cafe. I'm a friendly, outgoing, and hospitable person who loves making people feel welcome. I am currently planning a Valentine's Day party at my cafe for February 14th, where I will be decorating, greeting, and checking on customers. I also have a good friend named Maria Lopez. I am an early bird, and start my day with some morning routine around 6am, and work at Hobbs Cafe till late 8pm.")
