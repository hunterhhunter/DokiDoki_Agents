import os
import sys
sys.path.append('../../')

from global_methods import *

from persona.prompt_template.run_gpt_prompt import *


def generate_wake_up_hour(persona):
    """
    페르소나가 깨어나는 시간을 생성한다. 이것은 페르소나의 일일 계획을 생성하는 과정에서 중요한 부분이 된다.

    Persona state: identity stable set(정체성 안정 set), lifestyle, first_name

    INPUT:
        persona: 페르소나 클래스 인스턴스
    OUTPUT:
        페르소나가 깨어나는 시간을 나타내는 정수
    EXAMPLE OUTPUT:
        8
    """
    
    return int(run_prompt_wake_up_hour(persona)[0])


def generate_first_daily_request(persona, new_day):
    """
    페르소나의 오늘 할일 생성
    plan과는 다른 오늘의 하루 목표를 만든다고 보면 된다.
    """
    persona.daily_req = run_prompt_generate_daily_request(persona, new_day)[0]
    return persona.daily_req


def generate_first_daily_plan(persona, wakeup_time):
    """
    하루의 시간당 플랜을 짭니다.
    """
    output = run_prompt_generate_daily_plan(persona, wakeup_time)[0]
    return output



def generate_daily_request(persona):
    """
    이전 일을 고려하여 할일 생성
    원래 코드에선s revise로 했으나, past_summarize 로 대체하여 지난날을 요약 한 내용을 컨텍스트로 넣는다.
    """
    persona.daily_req = run_prompt_generate_daily_request(persona)[0]


def gernate_daily_plan(persona, wakeup_time):
    """
    하루의 시간당 플랜
    """

    persona.daily_plan = run_prompt_generate_daily_plan(persona, wakeup_time)[0]
    # 이후 persona.daily_plan에 전처리 한 후 하나씩 리스트에 넣는다.
    
    return persona.daily_plan

# def past_daily_summarize(persona):
