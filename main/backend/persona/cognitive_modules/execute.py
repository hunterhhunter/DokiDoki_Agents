import sys
import random
sys.path.append('../../')

def execute(persona, plan): 
  ## output
  ## event: (s, p, o)
  ## location: 이동할 위치
  ## expired: 행동할 시간
  if "<random>" in plan and persona.scratch.planned_path == []: 
    persona.scratch.act_path_set = False

  # <act_path_set> is set to True if the path is set for the current action. 
  # It is False otherwise, and means we need to construct a new path. 
  if not persona.scratch.act_path_set: 
    location = None

    if "<persona>" in plan:
      location = [plan.split("<persona>")[-1].strip()]
    
    elif "<waiting>" in plan:
      location = [persona.name]

    elif "<random>" in plan: 
      location = ['<random>']
    else: 
      location = [plan]

  description = f"{persona.scratch.act_description}"
  description += f" @ {persona.scratch.act_address}"

  # execution = location, persona.scratch.act_pronunciatio, description
  execution = persona.scratch.act_event, persona.scratch.act_description, persona.scratch.act_address, persona.act_duration
  return execution















