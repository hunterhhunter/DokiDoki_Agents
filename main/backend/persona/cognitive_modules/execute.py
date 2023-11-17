import sys
import random
sys.path.append('../../')

def execute(persona, plan): 
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

  execution = location, persona.scratch.act_pronunciatio, description
  return execution















