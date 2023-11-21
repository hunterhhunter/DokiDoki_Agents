import sys
sys.path.insert(0, 'C:/FastAPI')


from main_back.backend.persona.prompt_template.run_gpt_prompt import *

def generate_poig_score(persona, event_type, description): 
  if "is idle" in description: 
    return 1

  if event_type == "event": 
    return run_gpt_prompt_event_poignancy(persona, description)[0]
  elif event_type == "chat": 
    return run_gpt_prompt_chat_poignancy(persona, 
                           persona.scratch.act_description)[0]
  
def perceive(persona, accepted, event_checker):
  '''
  INPUT
    accepted : 유니티로부터 받는 정보. (persona, location, object) 에서 object리스트

    event_checker : event를 이끌어내는 이벤트 체크 객체.
  '''
  ret_events = []
  perceived_events = [(obj, *event_checker.get_event(obj)) for obj in accepted]
  for p_event in perceived_events: 
    s, p, o, desc = p_event
    if not p: 
      # If the object is not present, then we default the event to "idle".
      p = "is"
      o = "idle"
      desc = "idle"
    desc = f"{s.split(':')[-1]} is {desc}"
    p_event = (s, p, o)

    # We retrieve the latest persona.scratch.retention events. If there is  
    # something new that is happening (that is, p_event not in latest_events),
    # then we add that event to the a_mem and return it. 
    latest_events = persona.a_mem.get_summarized_latest_events(
                                    persona.scratch.retention)
    if p_event not in latest_events:
      # We start by managing keywords. 
      keywords = set()
      sub = p_event[0]
      obj = p_event[2]
      if ":" in p_event[0]: 
        sub = p_event[0].split(":")[-1]
      if ":" in p_event[2]: 
        obj = p_event[2].split(":")[-1]
      keywords.update([sub, obj])

      # Get event embedding
      desc_embedding_in = desc
      if "(" in desc: 
        desc_embedding_in = (desc_embedding_in.split("(")[1]
                                              .split(")")[0]
                                              .strip())
      if desc_embedding_in in persona.a_mem.embeddings: 
        event_embedding = persona.a_mem.embeddings[desc_embedding_in]
      else: 
        event_embedding = get_embedding(desc_embedding_in)
      event_embedding_pair = (desc_embedding_in, event_embedding)
      
      # Get event poignancy. 
      '''
      test
      input : "Here is a brief description of Maria Lopez. \nName: Maria Lopez\nAge: 21\nInnate traits: energetic, enthusiastic, inquisitive\nLearned traits: Maria Lopez is a student at Oak Hill College studying physics and a part time Twitch game streamer who loves to connect with people and explore new ideas.\nCurrently: Maria Lopez is working on her physics degree and streaming games on Twitch to make some extra money. She visits Hobbs Cafe for studying and eating just about everyday.\nLifestyle: Maria Lopez goes to bed around 2am, awakes up around 9am, eats dinner around 6pm. She likes to hang out at Hobbs Cafe if it's before 6pm.\nDaily plan requirement: Maria Lopez spends at least 3 hours a day Twitch streaming or gaming.\nCurrent Date: Monday February 13\n\n\nOn the scale of 1 to 10, where 1 is purely mundane (e.g., brushing teeth, making bed) and 10 is extremely poignant (e.g., a break up, college acceptance), rate the likely poignancy of the following event for Maria Lopez.\n\nEvent: Klaus Mueller is taking a break\nRate (return a number between 1 to 10):"
      output : 7
      '''
      #event_poignancy = generate_poig_score(persona, 
      #                                      "event", 
      #                                      desc_embedding_in)
      event_poignancy = 7
      # If we observe the persona's self chat, we include that in the memory
      # of the persona here. 
      chat_node_ids = []
      if p_event[0] == f"{persona.name}" and p_event[1] == "chat with": 
        curr_event = persona.scratch.act_event
        if persona.scratch.act_description in persona.a_mem.embeddings: 
          chat_embedding = persona.a_mem.embeddings[
                             persona.scratch.act_description]
        else: 
          chat_embedding = get_embedding(persona.scratch
                                                .act_description)
        chat_embedding_pair = (persona.scratch.act_description, 
                               chat_embedding)
        chat_poignancy = generate_poig_score(persona, "chat", 
                                             persona.scratch.act_description)
        chat_node = persona.a_mem.add_chat(persona.scratch.curr_time, None,
                      curr_event[0], curr_event[1], curr_event[2], 
                      persona.scratch.act_description, keywords, 
                      chat_poignancy, chat_embedding_pair, 
                      persona.scratch.chat)
        chat_node_ids = [chat_node.node_id]

      # Finally, we add the current event to the agent's memory. 
      ret_events += [persona.a_mem.add_event(persona.scratch.curr_time, None,
                           s, p, o, desc, keywords, event_poignancy, 
                           event_embedding_pair, chat_node_ids)]
      persona.scratch.importance_trigger_curr -= event_poignancy
      persona.scratch.importance_ele_n += 1

  return ret_events