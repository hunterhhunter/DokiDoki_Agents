import os
import sys
sys.path.append('../../')

from global_methods import *



def action_react(persona):
    

    return


def chat_react(persona):


def react(type, persona, focal_points):
    if type=='action':
        action_react(persona)

    elif type='chat':
        chat_react(persona)