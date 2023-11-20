"""
출처: https://github.com/joonspk-research/generative_agents/blob/main/reverie/backend_server/persona/prompt_template/run_gpt_prompt.py

prompt를 만드는 파일입니다.
"""




import re
import datetime
import sys
import ast
import string
import os

# 작업 디렉토리 변경
current_file_path = os.path.abspath(__file__)
current_file_dir = os.path.dirname(current_file_path)
os.chdir(current_file_dir)

sys.path.append('../../')
sys.path.append('../')
from global_methods import *
from persona.prompt_template.gpt_structure import *
from persona.prompt_template.print_prompt import *

gpt_or_local = 'local'
debug_prompt = True

def get_random_alphanumeric(i=6, j=6):
    """
    i와 j 사이 길이를 가진 랜덤 알파벳 숫자 문자열을 반환한다.

    INPUT:
        i: min_range for the length
        j: max_range for the length
    OUTPUT:
        i와 j 사이의 어디간 길이를 가진 알파벳 숫자 문자열
    """
    k = random.randint(i, j)
    x = ''.join(random.choices(string.ascii_letters + string.digits, k=k))

    return x

#################################################################################
# Run loacl Prompt
#################################################################################

def run_prompt_wake_up_hour(persona, model='local', test_input=None, verbose=False):
        
    def create_prompt_input(persona, test_input=None):
        if test_input: return test_input
        prompt_input = [persona.scratch.get_str_iss(),
                        persona.scratch.get_str_lifestyle(),
                        persona.scratch.get_str_firstname()]
        return prompt_input
    
    # 나온 출력값을 정리해서 정수로 한다
    def __func_clean_up(gpt_response, prompt=""):
        cr = int(gpt_response.strip().lower().split("am")[0])
        return cr
    
    def __func_validate(gpt_response, prompt=""):
        try: __func_clean_up(gpt_response, prompt="")
        except: return False
        return True
    # 실패 했을 때를 대비해서
    def get_fail_safe():
        fs = 8
        return fs

    # gpt_param = {"engine": "text-davinci-002", "max_tokens": 5, 
    #          "temperature": 0.8, "top_p": 1, "stream": False,
    #          "frequency_penalty": 0, "presence_penalty": 0, "stop": ["\n"]}
    gpt_param = {"max_tokens": 5, "temperature": 0.8, "top_p": 1}

    # prompt_template = "persona/prompt_template/local/wake_up_hour_v1.txt"
    prompt_template = "local/wake_up_hour_v1.txt"

    prompt_input = create_prompt_input(persona, test_input)
    prompt = generate_prompt(prompt_input, prompt_template)
    if debug_prompt: 
        print("==============prompt_template==============")
        print(prompt_input) 
        print("==============prompt==============")
        print(prompt)
    fail_safe = get_fail_safe()

    # output = safe_gpt_generate_response(prompt, gpt_param, 5, fail_safe, __func_validate, __func_clean_up)
    if model == 'local':
        output = safe_local_generate_response(prompt, gpt_param, 5, fail_safe, __func_validate, __func_clean_up)
    else:
        output = safe_local_generate_response(prompt, gpt_param, 5, fail_safe, __func_validate, __func_clean_up)

    if verbose:
        print_run_prompts(prompt_template, persona, gpt_param, prompt_input, prompt, output)

    return output

   
    

def run_prompt_generate_daily_request(persona, new_day=None, model='local', test_input=None, verbose=None):

    """오늘 하루 동안 이룰 페르소나의 목표"""
    def create_prompt_input(persona, new_day, test_input=None):
        if test_input: return test_input
        #TODO
        # 프롬프트 수정 필요
        if new_day=='first_day':
            prompt_input = [persona.scratch.get_sentence_iss(),
                        persona.scratch.get_str_lifestyle(),
                        persona.scratch.get_str_firstname()]
        else:
            # 수정된 프롬프트
            prompt_input = [persona.scratch.get_sentence_iss(),
                        persona.scratch.get_str_lifestyle(),
                        persona.scratch.get_str_firstname()]
        return prompt_input
    
    # 나온 출력값을 정리해서 정수로 한다
    def __func_clean_up(gpt_response, prompt=""):
        return gpt_response
    
    def __func_validate(gpt_response, prompt=""):
        try: __func_clean_up(gpt_response, prompt="")
        except: return False
        return True
    # 실패 했을 때를 대비해서
    def get_fail_safe():
        fs = 'I am spending today just like yesterday.'
        return fs

    

    gpt_param = {"max_tokens": 1024, "temperature": 0.8, "top_p": 0.8}

    prompt_template = "local/generate_daily_request_v1.txt"

    prompt_input = create_prompt_input(persona, test_input)
    prompt = generate_prompt(prompt_input, prompt_template)
    if debug_prompt: 
        print("==============prompt_template==============")
        print(prompt_input) 
        print("==============prompt==============")
        print(prompt)
    fail_safe = get_fail_safe()

    # output = safe_gpt_generate_response(prompt, gpt_param, 5, fail_safe, __func_validate, __func_clean_up)
    if model =='local':
        output = safe_local_generate_response(prompt, gpt_param, 5, fail_safe, __func_validate, __func_clean_up)
    
    
    if verbose:
        print_run_prompts(prompt_template, persona, gpt_param, prompt_input, prompt, output)

    return output



def run_prompt_generate_daily_plan(persona,wakeup_time, new_day=None, model='local', test_input=None, verbose=None):

    """오늘 하루의 스케쥴"""
    def create_prompt_input(persona,wakeup_time, test_input=None):
        if test_input: return test_input

        #TODO
        # 프롬프트 수정 필요
        prompt_input = [persona.scratch.get_sentence_iss(),
                        persona.scratch.get_str_lifestyle(),
                        persona.scratch.get_str_firstname()]
        return prompt_input
    
    # 나온 출력값을 정리해서 정수로 한다
    def __func_clean_up(gpt_response, prompt=""):
        return gpt_response
    
    def __func_validate(gpt_response, prompt=""):
        try: __func_clean_up(gpt_response, prompt="")
        except: return False
        return True
    # 실패 했을 때를 대비해서
    def get_fail_safe():
        fs = 'I am spending today just like yesterday.'
        return fs

    

    gpt_param = {"max_tokens": 1024, "temperature": 0.8, "top_p": 0.8}

    # TODO
    prompt_template = "local/generate_daily_request_v1.txt"

    prompt_input = create_prompt_input(persona, test_input)
    prompt = generate_prompt(prompt_input, prompt_template)
    if debug_prompt: 
        print("==============prompt_template==============")
        print(prompt_input) 
        print("==============prompt==============")
        print(prompt)
    fail_safe = get_fail_safe()

    # output = safe_gpt_generate_response(prompt, gpt_param, 5, fail_safe, __func_validate, __func_clean_up)
    if model=='local':
        output = safe_local_generate_response(prompt, gpt_param, 5, fail_safe, __func_validate, __func_clean_up)
    
    
    if verbose:
        print_run_prompts(prompt_template, persona, gpt_param, prompt_input, prompt, output)

    return output




# 일어난 시간을 기준으로 하루의 계획을 짭니다.
def run_local_prompt_daily_plan(persona,
                              wake_up_hour,
                              test_input=None,
                              verbose=False):
    
    """
    일반적으로 하루를 아우르는 장기계획. 오늘 페르소나가 수행할 행동 목록을 반환한다.


    ex) 
    'wake up and complete the morning routine at 6:00 am', 
    'eat breakfast at 7:00 am'

    마침표 없이 나온다는 것에 주의할 것.

    INPUT: 
        persona: 페르소나 클래스 인스턴스
    OUTPUT:
        일일 행동 목록
    """

    def create_prompt_input(persona, wake_up_hour, test_input=None):
        if test_input: return test_input
        prompt_input = []
        prompt_input += [persona.scratch.get_str_iss()]
        prompt_input += [persona.scratch.get_str_lifestyle()]
        prompt_input += [persona.scratch.get_str_curr_date_str()]
        prompt_input += [persona.scractch.get_str_firstname()]
        prompt_input += [f"{str(wake_up_hour)}:00 am"]
        return prompt_input
    
    # 특정 상황에 대해서 정제하기 위한 함수
    # local 일 경우 여러 번 시도해서 특정 형식의 답장이 나오도록 하던가, 아니면 few-shot을 통해 예시를 보여주던가 해야할 것
    # 추후 수정이 필요
    def __func_clean_up(gpt_response, prompt=""):
        cr = []
        # 들어온 gpt_response를 ) 단위로 자른다
        _cr = gpt_response.split(")")
        """
        기존 템플릿은 1) 할일. 2) 할일 과 같은 형식으로 되어있다. 따라서 )를 기준으로 자르고 숫자를 지우는것.
        """
        for i in _cr:
            # i의 마지막이 숫자라면 마지막 문자를 제거하고 양쪽 끝의 공백을 제거한다.
            # 이후 수정된 마지막이 .이 있거나 쉼표가 있는지 확인 한 후, 다시 제거하고 공백을 제거한 후 cr 리스트에 추가한다.
            if i[-1].isdigit():
                i = i[:-1].strip()
                if i[-1] == "." or i[-1] == ",":
                    cr += [i[:-1].strip()]


        return cr
    
    def __func_validate(gpt_response, prompt=""):
        try: __func_clean_up(gpt_response, prompt="")
        except:
            return False
        return True
    
    def get_fail_safe():
        fs = ['wake up and complete the morning routine at 6:00 am', 
          'eat breakfast at 7:00 am', 
          'read a book from 8:00 am to 12:00 pm', 
          'have lunch at 12:00 pm', 
          'take a nap from 1:00 pm to 5:00 pm',
          'have dinner at 6:00 pm',
          'go to bed at 10:00 pm']
        return False

    gpt_param = {"max_tokens": 500, "temperature": 1, "top_p": 1}
    prompt_template = "/persona/prompt_template/local/daily_planning_v6.txt"
    prompt_template = "local/daily_planning_v6.txt"

    prompt_input = create_prompt_input(persona, wake_up_hour,test_input)
    prompt = generate_prompt(prompt_input, prompt_template)
    if debug_prompt: 
        print("==============prompt_template==============")
        print(prompt_input) 
        print("==============prompt==============")
        print(prompt)
    fail_safe = get_fail_safe()

    output = safe_local_generate_response(prompt, gpt_param, 5, fail_safe, __func_validate, __func_clean_up)
    output = ([f"wake up and complete the morning routine at {wake_up_hour}:00 am"] + output)

    if verbose:
        print_run_prompts(prompt_template, persona, gpt_param, prompt_input, prompt, output)

    return output, [output, prompt, gpt_param, prompt_input, fail_safe]


def run_gpt_generate_converse_first(init_persona, target_persona, retrieved, target_relationship):
    def create_prompt_input(init_persona, target_persona, test_input=None):
        if test_input:return test_input
        current_location = '집'
        prompt_input =[]
        prompt_input += [init_persona.scratch.get_str_iss()]
        prompt_input += [init_persona.scratch.name]
        prompt_input += [init_persona.scratch.act_description]
        prompt_input += [retrieved]
        prompt_input += [target_relationship]
        prompt_input += [target_persona.scratch.name]
        prompt_input += [target_persona.scratch.act_description]
        prompt_input += [current_location]
        print('asd')
        past_convo_summarize = ""
        for i, v in init_persona.a_mem.id_to_node.items():
            if v.predicate == 'chat with':
                past_convo_summarize= v.description
                break
        if past_convo_summarize == None:
            past_convo_summarize='There is no previous conversation history'
        prompt_input += [past_convo_summarize]
        prompt_input += [init_persona.a_mem.curr_chat]
        return prompt_input
    def __func_clean_up(gpt_response, prompt=""):
        return gpt_response
    
    def __func_validate(gpt_response, prompt=""):
        try: __func_clean_up(gpt_response, prompt="")
        except: return False
        return True
    # 실패 했을 때를 대비해서
    def get_fail_safe():
        fs = 'I am spending today just like yesterday.'
        return fs

    gpt_param = {"max_tokens": 5, "temperature": 0.8, "top_p": 1}
    prompt_input = create_prompt_input(init_persona, target_persona)
    prompt_template = "local/generate_converse_first_v1.txt"
    prompt = generate_prompt(prompt_input, prompt_template)
    print("==============prompt_template==============")
    print(prompt_input) 
    print("==============prompt==============")
    print(prompt)
    # output = 
    output = safe_local_generate_response(prompt, gpt_param, 5, fail_safe, __func_validate, __func_clean_up)


def run_gpt_generate_convers_second(init_persona, target_persona, current_location, retrieved, target_relationship):
    def create_prompt_input(init_persona, target_persona, test_input=None):
        if test_input:return test_input
        current_location = '집'
        prompt_input =[]
        prompt_input += [init_persona.scratch.get_str_iss()]
        prompt_input += [init_persona.scratch.name]
        prompt_input += [init_persona.scratch.act_description]
        prompt_input += [retrieved]
        prompt_input += [target_relationship]
        prompt_input += [target_persona.scratch.name]
        prompt_input += [target_persona.scratch.act_description]
        prompt_input += [current_location]
        print('asd')
        past_convo_summarize = ""
        for i, v in init_persona.a_mem.id_to_node.items():
            if v.predicate == 'chat with':
                past_convo_summarize= v.description
                break
        if past_convo_summarize == '':
            past_convo_summarize='There is no previous conversation history'
        prompt_input += [past_convo_summarize]
        prompt_input += [init_persona.a_mem.curr_chat]
        return prompt_input


    def __func_clean_up(gpt_response, prompt=""):
        return gpt_response
    
    def __func_validate(gpt_response, prompt=""):
        try: __func_clean_up(gpt_response, prompt="")
        except: return False
        return True
    
    # 실패 했을 때를 대비해서
    def get_fail_safe():
        fs = 'I am spending today just like yesterday.'
        return fs

    gpt_param = {"max_tokens": 5, "temperature": 0.8, "top_p": 1}
    prompt_input = create_prompt_input(init_persona, target_persona)
    print(prompt_input)
    prompt_template = "local/generate_converse_second_v1.txt"
    prompt = generate_prompt(prompt_input, prompt_template)
    print("==============prompt_template==============")
    print(prompt_input) 
    print("==============prompt==============")
    print(prompt)
    output = safe_local_generate_response(prompt, gpt_param, 5, get_fail_safe, __func_validate, __func_clean_up)


#############################################################

# TODO
def run_local_prompt_generate_hourly_schedule(persona,
                                            curr_hour_str,
                                            p_f_ds_hourly_org,
                                            hour_str,
                                            intermission2=None,
                                            test_input=None,
                                            verbose=False):
    def create_prompt_input(persona,
                            curr_hour_str,
                            p_f_ds_hourly_org,
                            hour_str,
                            intermission2=None,
                            test_input=None):
        if test_input: return test_input
        schedule_format = ""
        for i in hour_str:
            schedule_format += f"[{persona.scratch.get_str_curr_date_str()} -- {i}]"
            schedule_format += f" Activity: [Fill in]\n"
        schedule_format = schedule_format[:-1]

        intermission_str = f"Here the originally intended hourly breakdown of"
        intermission_str = f" {persona.scratch.get_str_firstname()}'s schedule today: "
        for count, i in enumerate(persona.scratch.daily_req):
            intermission_str += f"{str(count+1)}"
        intermission_str = intermission_str[:-2]

        prior_schedule = ""
        if p_f_ds_hourly_org:
            prior_schedule = "\n"
            for count, i in enumerate(p_f_ds_hourly_org):
                prior_schedule += f"[(ID:{get_random_alphanumeric()})]"
                prior_schedule += f" {persona.scratch.get_str_curr_date_str()} --"
                prior_schedule += f" {hour_str[count]}] Activity:"
                prior_schedule += f" {persona.scratch.get_str_firstname()}"
                prior_schedule += f" is {i}\n"

        prompt_ending = f"[(ID:{get_random_alphanumeric()})"
        prompt_ending += f" {persona.scratch.get_str_curr_date_str()}"
        prompt_ending += f" -- {curr_hour_str}] Activity:"
        prompt_ending += f" {persona.scratch.get_str_firstname()} is"

        if intermission2:
            intermission2 = f"\n{intermission2}"

        prompt_input = []
        prompt_input += [schedule_format]
        prompt_input += [persona.scratch.get_str_iss()]

        prompt_input += [prior_schedule + "\n"]
        prompt_input += [intermission_str]
        if intermission2:
            prompt_input += [intermission2]
        else:
            prompt_input += [""]
        prompt_input += [prompt_ending]

        return prompt_input

    def custom_create_prompt_input(persona, n_m1_activity,test_input=None):
        if test_input: return test_input
        prompt_input = []
        prompt_input += [persona.scratch.get_str_iss()]
        prompt_input += [persona.scratch.name]
        schedule_format = ""
        for i in hour_str:
            schedule_format += f"[{persona.scratch.get_str_curr_date_str()} -- {i}]"
            schedule_format += f" Activity: [Fill in]\n"
        for i in n_m1_activity:
            schedule_format = schedule_format.replace("[Fill in]", i, 1)
        prompt_input += [schedule_format]
        return prompt_input

    def __func_clean_up(gpt_response, prompt=""):
        cr = gpt_response.strip()
        if cr[-1] == ".":
            cr = cr[:-1]
        return cr
    
    def __func_validate(gpt_response, prompt=""):
        try: 
            __func_clean_up(gpt_response, prompt="")
            # if "-- 11:00 PM] Activity:" in gpt_response:
            #     return True
            # else:
            #     return False
        except: 
            return False
        return True
    
    def get_fail_safe():
        fs = "asleep"
        return fs

    gpt_param = {"max_tokens": 2048, "temperature": 0.9, "top_p": 1}
    # prompt_template = "persona/prompt_template/local/generate_hourly_schedule_v2.txt"
    prompt_template = "local/generate_hourly_schedule_v6.txt"

    # prompt_input = create_prompt_input(persona, 
    #                                  curr_hour_str, 
    #                                  p_f_ds_hourly_org,
    #                                  hour_str, 
    #                                  intermission2,
    #                                  test_input)
    prompt_input =  custom_create_prompt_input(persona, p_f_ds_hourly_org)           
    prompt = generate_prompt(prompt_input, prompt_template)
    debug_prompt = True
    if debug_prompt: 
        print("==============prompt_template==============")
        print(prompt_input) 
        print("==============prompt==============")
        print(prompt)
    fail_safe = get_fail_safe()

    output = safe_local_generate_response(prompt, gpt_param, 10, fail_safe, __func_validate, __func_clean_up, verbose=verbose)

    if verbose:
        print_run_prompts(prompt_template, persona, gpt_param, prompt_input, prompt, output)
    print("==============output==============")
    print(output)
    return output, [output, prompt, gpt_param, prompt_input, fail_safe]


def run_local_prompt_generate_action(persona):
    def custom_create_prompt_input(persona):
        # if test_input: return test_input
        prompt_input =[]
        prompt_input += [persona.scratch.get_str_iss()]
        prompt_input += [persona.name]
        prompt_input += ['forge']
        prompt_input += ['chair']
        prompt_input += ['sit']
        prompt_input += ['Tom contemplates ways to make superior weaponry while working on client orders in his forge.']
        prompt_input += [['shelf', 'refrigerator', 'bar customer seating', 'kitchen sink', 'cooking area', 'chair']]
        prompt_input += [['Retrieve', 'Store', 'Sit', 'Cook' , 'Wash']]
        return prompt_input


    def __func_clean_up(gpt_response, prompt=""):
        return gpt_response
    
    def __func_validate(gpt_response, prompt=""):
        try: 
            __func_clean_up(gpt_response, prompt="")
        except: 
            return False
        return True

    def get_fail_safe():
        return 'wait'

    gpt_param = {"max_tokens": 1024, "temperature": 0.9, "top_p": 0.95, "top_k":100}
    prompt_template = "local/generate_action_v1.txt"

    prompt_input =  custom_create_prompt_input(persona)           
    prompt = generate_prompt(prompt_input, prompt_template)
    debug_prompt = True
    if debug_prompt: 
        print("==============prompt_template==============")
        print(prompt_input) 
        print("==============prompt==============")
        print(prompt)
    fail_safe = get_fail_safe()
    print('=========================')
    output = safe_local_generate_response(prompt, gpt_param, 10, fail_safe, __func_validate, __func_clean_up)
    
    return output

# TODO
def run_local_prompt_task_decomp(persona,
                               task,
                               duration,
                               test_input=None,
                               verbose=False):
    def create_prompt_input(persona, task, duration, test_input=None):
        """
        
        """


        curr_f_org_index = persona.scratch.get_f_daily_schedule_hourly_org_index()
        all_indices = []

        all_indices += [curr_f_org_index]
        if curr_f_org_index+1 <= len(persona.scratch.f_daily_schedule_hourly_org):
            all_indices += [curr_f_org_index+1]
        if curr_f_org_index+2 <= len(persona.scratch.f_daily_schedule_hourly_org):
            all_indices += [curr_f_org_index+2]

        curr_time_range = ""

        if test_input: 
            print("DEBUG")
            print (persona.scratch.f_daily_schedule_hourly_org)
            print (all_indices)

            return test_input


        summ_str = f'Today is {persona.scratch.curr_time.strftime("%B %d, %Y")}. '
        summ_str += f'From '
        for index in all_indices:
            print ("index", index)
            if index < len(persona.scratch.f_daily_schedule_hourly_org):
                start_min = 0
                for i in range(index):
                    start_min += persona.scratch.f_daily_schedule_hourly_org[i][1]
                end_min = start_min + persona.scratch.f_daily_schedule_hourly_org[index][1]
                start_time = (datetime.datetime.strptime("00:00:00", "%H:%M:%S") 
                            + datetime.timedelta(minutes=start_min)) 
                end_time = (datetime.datetime.strptime("00:00:00", "%H:%M:%S") 
                            + datetime.timedelta(minutes=end_min)) 
                start_time_str = start_time.strftime("%H:%M%p")
                end_time_str = end_time.strftime("%H:%M%p")
                summ_str += f"{start_time_str} ~ {end_time_str}, {persona.name} is planning on {persona.scratch.f_daily_schedule_hourly_org[index][0]}, "
                if curr_f_org_index+1 == index:
                    curr_time_range = f'{start_time_str} ~ {end_time_str}'
        summ_str = summ_str[:-2] + "."

        prompt_input = []
        prompt_input += [persona.scratch.get_str_iss()]
        prompt_input += [summ_str]
        prompt_input += [persona.scratch.get_str_firstname()]
        prompt_input += [persona.scratch.get_str_firstname()]
        prompt_input += [task]
        prompt_input += [curr_time_range]
        prompt_input += [duration]
        prompt_input += [persona.scratch.get_str_firstname()]
        return prompt_input
    
    def __func_clean_up(gpt_response, prompt=""):
        print("TODO")
        print(gpt_response)
        print ("-==- -==- -==- ")

        # 현재 이 부분에서 가끔씩 실패한다고 한다.
        temp = [i.strip() for i in gpt_response.split("\n")]
        _cr = []
        cr = []
        for count, i in enumerate(temp):
            if count != 0:
                _cr += [" ".join([j.strip() for j in i.split(" ")][3:])]
            else:
                _cr += [i]

        for count, i in enumerate(_cr):
            k = [j.strip() for j in i.split("(duration in minutes:")]
            task = k[0]
            if task[-1] == ".":
                task = task[:-1]
            duration = int(k[1].split(",")[0].strip())
            cr += [[task, duration]]

        total_expected_min = int(prompt.split("(total duration in minutes")[-1].split("):")[0].strip())

        # 이 아래 부분이 현재 액션 시퀀스의 합과 동일한지 확인해야한다
        curr_min_slot = [["dummy" -1],]
        for count, i in enumerate(cr):
            i_task = i[0]
            i_duration = i[1]

            i_duration -= (i_duration % 5)
            if i_duration > 0:
                for j in range(i_duration):
                    curr_min_slot += [(i_task, count)]
        curr_min_slot = curr_min_slot[1:]

        if len(curr_min_slot) > total_expected_min:
            last_task = curr_min_slot[60]
            for i in range(1, 6):
                curr_min_slot[-1 * i] = last_task
        elif len(curr_min_slot) < total_expected_min:
            last_task = curr_min_slot[-1]
            for i in range(total_expected_min - len(curr_min_slot)):
                curr_min_slot += [last_task]

        cr_ret = [["dummy", -1],]
        for task, task_index in curr_min_slot:
            if task != cr_ret[-1][0]:
                cr_ret += [[task, 1]]
            else:
                cr_ret[-1][1] += 1

        cr = cr_ret[1:]
        return cr
    
    def __func_validate(gpt_response, prompt=""): 
        try: 
            __func_clean_up(gpt_response)
        except: 
            # pass
            return False
        return gpt_response   

    def get_fail_safe():
        fs = ["asleep"]
        return fs
    
    gpt_param = {"max_tokens": 2048, "temperature": 0, "top_p": 1} 
    # prompt_template = "persona/prompt_template/local/task_decomp_v3.txt"
    prompt_template = "local/task_decomp_v3.txt"

    prompt_input = create_prompt_input(persona, task, duration)
    prompt = generate_prompt(prompt_input, prompt_template)
    if debug_prompt: 
        print("==============prompt_template==============")
        print(prompt_input) 
        print("==============prompt==============")
        print(prompt)
    fail_safe = get_fail_safe()
    output = safe_local_generate_response(prompt, gpt_param, 5, get_fail_safe(), __func_validate, __func_clean_up, verbose=verbose)

    print(output)
    time_sum = 0
    for i_task, i_duration in output:
        time_sum += i_duration
        if time_sum <= duration:
            fin_output += [[i_task, i_duration]]
        else:
            break
    ftime_sum = 0
    for fi_task, fi_duration in fin_output:
        ftime_sum += fi_duration

    fin_output[-1][1] += (duration - ftime_sum)
    output = fin_output

    task_decomp = output
    ret = []
    for decomp_task, duration in task_decomp:
        ret += [[f"{task} ({decomp_task})", duration]]
    output = ret

    if verbose:
        print_run_prompts(prompt_template, persona, gpt_param, 
                      prompt_input, prompt, output)
        
    return output, [output, prompt, gpt_param, prompt_input, fail_safe]


def run_local_prompt_action_sector(action_description,
                                 persona,
                                 maze,
                                 test_input=None,
                                 verbose=False):
    def create_prompt_input(action_description, persona, maze, test_input=None):
        if test_input: return test_input

        act_world = f"{maze.access_tile(persona.scratch.curr_tile)['world']}"

        prompt_input = []

        prompt_input += [persona.scratch.get_str_name()]
        prompt_input += [persona.scratch.living_area.split(":")[1]]
        x = f"{act_world}:{persona.scratch.living_area.split(':')[1]}"
        prompt_input += [persona.s_mem.get_str_accessible_sector_arenas(x)]

        prompt_input += [persona.scratch.get_str_name()]
        prompt_input += [f"{maze.access_tile(persona.scratch.curr_tile)['sector']}"]
        x = f"{act_world}:{maze.access_tile(persona.scratch.curr_tile)['sector']}"
        prompt_input += [persona.s_mem.get_str_accessible_sector_arenas(x)]

        if persona.scratch.get_str_daily_plan_req() != "":
            prompt_input += [f"\n{persona.scratch.get_str_daily_plan_req()}"]


        
#############################################################
# Run GPT Prompt
#############################################################


# 일어난 시간을 기준으로 하루의 계획을 짭니다.
def run_gpt_prompt_daily_plan(persona,
                              wake_up_hour,
                              test_input=None,
                              verbose=False):
    
    """
    일반적으로 하루를 아우르는 장기계획. 오늘 페르소나가 수행할 행동 목록을 반환한다.


    ex) 
    'wake up and complete the morning routine at 6:00 am', 
    'eat breakfast at 7:00 am'

    마침표 없이 나온다는 것에 주의할 것.

    INPUT: 
        persona: 페르소나 클래스 인스턴스
    OUTPUT:
        일일 행동 목록
    """

    def create_prompt_input(persona, wake_up_hour, test_input=None):
        if test_input: return test_input
        prompt_input = []
        prompt_input += [persona.scratch.get_str_iss()]
        prompt_input += [persona.scratch.get_str_lifestyle()]
        prompt_input += [persona.scratch.get_str_curr_date_str()]
        prompt_input += [persona.scractch.get_str_firstname()]
        prompt_input += [f"{str(wake_up_hour)}:00 am"]
        return prompt_input
    
    # 특정 상황에 대해서 정제하기 위한 함수
    # local 일 경우 여러 번 시도해서 특정 형식의 답장이 나오도록 하던가, 아니면 few-shot을 통해 예시를 보여주던가 해야할 것
    # 추후 수정이 필요
    def __func_clean_up(gpt_response, prompt=""):
        cr = []
        # 들어온 gpt_response를 ) 단위로 자른다
        _cr = gpt_response.split(")")
        for i in _cr:
            # i의 마지막이 숫자라면 마지막 문자를 제거하고 양쪽 끝의 공백을 제거한다.
            # 이후 수정된 마지막이 .이 있거나 쉼표가 있는지 확인 한 후, 다시 제거하고 공백을 제거한 후 cr 리스트에 추가한다.
            if i[-1].isdigit():
                i = i[:-1].strip()
                if i[-1] == "." or i[-1] == ",":
                    cr += [i[:-1].strip()]


        return cr
    
    def __func_validate(gpt_response, prompt=""):
        try: __func_clean_up(gpt_response, prompt="")
        except:
            return False
        return True
    
    def get_fail_safe():
        fs = ['wake up and complete the morning routine at 6:00 am', 
          'eat breakfast at 7:00 am', 
          'read a book from 8:00 am to 12:00 pm', 
          'have lunch at 12:00 pm', 
          'take a nap from 1:00 pm to 5:00 pm',
          'have dinner at 6:00 pm',
          'go to bed at 10:00 pm']
        return False

    gpt_param = {"max_tokens": 500, "temperature": 1, "top_p": 1}
    prompt_template = "persona/prompt_template/v2/daily_planning_v6.txt"
    prompt_input = create_prompt_input(persona, wake_up_hour,test_input)
    prompt = generate_prompt(prompt_input, prompt_template)
    if debug_prompt: 
        print("==============prompt_template==============")
        print(prompt_input) 
        print("==============prompt==============")
        print(prompt)
    fail_safe = get_fail_safe()

    output = safe_local_generate_response(prompt, gpt_param, 5, fail_safe, __func_validate, __func_clean_up)
    output = ([f"wake up and complete the morning routine at {wake_up_hour}:00 am"] + output)

    if verbose:
        print_run_prompts(prompt_template, persona, gpt_param, prompt_input, prompt, output)

    return output, [output, prompt, gpt_param, prompt_input, fail_safe]


    ########################################################################

    # test
def run_gpt_generate_converse_first(init_persona, target_persona, retrieved, target_relationship):
    def create_prompt_input(init_persona, target_persona, test_input=None):
        if test_input:return test_input
        current_location = '집'
        prompt_input =[]
        prompt_input += [init_persona.scratch.get_sentance_iss()]
        prompt_input += [init_persona.scratch.name]
        prompt_input += [init_persona.scratch.act_description]
        prompt_input += [retrieved]
        prompt_input += [target_relationship]
        prompt_input += [target_persona.scratch.name]
        prompt_input += [target_persona.scratch.act_description]
        prompt_input += [current_location]
        past_convo_summarize = ""
        for i, v in init_persona.a_mem.id_to_node.items():
            if v.predicate == 'chat with':
                past_convo_summarize= v.description
                break
        if past_convo_summarize == None:
            past_convo_summarize='There is no previous conversation history'
        prompt_input += [past_convo_summarize]
        prompt_input += [init_persona.a_mem.curr_chat]
        return prompt_input
    

    gpt_param = {"max_tokens": 1024, "temperature": 0.75, "top_p": 0.9, "top_k":50}
    prompt_input = create_prompt_input(init_persona, target_persona)
    prompt_template = "/home/elicer/main/agent_1111/backend/persona/prompt_template/local/generate_converse_first_v3.txt"
    prompt = generate_prompt(prompt_input, prompt_template)
    print("==============prompt_template==============")
    print(prompt_input) 
    print("==============prompt==============")
    print(prompt)
    # output = 
    output = nonsafe_local_generate_response(prompt, gpt_param)
    print(output)
    data = json.loads(output)

    # uttr과 boolean을 추출

    uttr = list(data.items())[0]  # 첫 번째 항목 (대화)
    boolean = list(data.items())[1][1]
    return uttr, boolean



def run_gpt_generate_converse_second(init_persona, target_persona, retrieved, target_relationship):
    def create_prompt_input(init_persona, target_persona, test_input=None):
        if test_input:return test_input
        current_location = '집'
        prompt_input =[]
        prompt_input += [init_persona.scratch.get_str_iss()]
        prompt_input += [init_persona.scratch.name]
        prompt_input += [init_persona.scratch.act_description]
        prompt_input += [retrieved]
        prompt_input += [target_relationship]
        prompt_input += [target_persona.scratch.name]
        prompt_input += [target_persona.scratch.act_description]
        prompt_input += [current_location]
        past_convo_summarize = ""
        for i, v in init_persona.a_mem.id_to_node.items():
            if v.predicate == 'chat with':
                past_convo_summarize= v.description
                break
        if past_convo_summarize == None:
            past_convo_summarize='There is no previous conversation history'
        prompt_input += [past_convo_summarize]
        prompt_input += [init_persona.a_mem.curr_chat]
        return prompt_input

    gpt_param = {"max_tokens": 1024, "temperature": 0.8, "top_p": 0.9, "top_k":50}
    prompt_input = create_prompt_input(init_persona, target_persona)
    print(prompt_input)
    prompt_template = "/home/elicer/main/agent_1111/backend/persona/prompt_template/local/generate_converse_second_v1.txt"
    prompt = generate_prompt(prompt_input, prompt_template)
    print("==============prompt_template==============")
    print(prompt_input) 
    print("==============prompt==============")
    print(prompt)
    output = nonsafe_local_generate_response(prompt, gpt_param)
    print("==============output==============")
    print(output)
    data = json.loads(output)

    # uttr과 boolean을 추출
    uttr = list(data.items())[0]  # 첫 번째 항목 (대화)
    boolean = list(data.items())[1][1]
 

    return uttr, boolean