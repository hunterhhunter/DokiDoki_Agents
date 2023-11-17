"""
출처: https://github.com/joonspk-research/generative_agents/blob/main/reverie/backend_server/persona/memory_structures/scratch.py

생성형 에이전트의 단기 메모리 모듈
"""

import datetime
import json
import sys

sys.path.append('../../')

from global_methods import *

class Scratch:
    def __init__(self, f_saved):
        # 페르소나 하이퍼파라미터
        # <vision_r>은 페르소나가 주변에서 볼 수 있는 타일의 수를 나타냅니다.
        self.vision_r = 4

        # <att_bandwidth> 
        self.att_bandwidth = 3

        # <retention>
        self.retention = 5

        # 월드 정보
        # 인지된 세계 시간
        self.curr_time = None
        # self.curr_time = datetime.datetime.strptime("November 6, 2023, 00:00:00", "%B %d, %Y, %H:%M:%S")

        # 페르소나의 현재 x, y 타일 좌표
        self.curr_tile = None

        # 인지된 세계의 일일 요구사항
        self.daily_plan_req = None

        # 페르소나의 핵심 정체성
        # 페르소나에 대한 기본 정보
        self.name = None
        self.first_name = None
        self.last_name = None
        self.age = None
        
        # L0 영구적인 핵심 특성
        self.innate = None

        # L1 안정된 특성
        self.learned = None

        # L2 외부 구현
        self.currently = None
        self.lifestyle = None
        self.living_area = None

        # 반영 변수
        self.concept_forget = 100
        self.daily_reflection_time = 60 * 3
        self.daily_reflection_size = 5
        self.overlap_reflect_th = 2
        self.kw_strg_event_reflect_th = 4
        self.kw_strg_thought_relect_th = 4

        # 새 반영 변수
        self.recency_w = 1
        self.relevance_w = 1
        self.importance_w = 1
        self.recency_decay = 0.99
        self.importance_trigger_max = 150
        self.importance_trigger_curr = self.importance_trigger_max
        self.importance_ele_n = 0
        self.thought_count = 5

        '''
        페르소나 계획

        <daily_req> 는 페르소나가 오늘 달성하려는 다양한 목표의 목록입니다.
        예: ['작품 그리기', 'TV 보면서 휴식'. '점심 만들기', '더 작품 그리기', '일찍 자기']
        이 목록은 하루가 끝날 때마다 갱신되어야 하므로 생성 시간을 추적합니다.
        '''
        self.daily_req = []
        '''
        <f_daily_schedule> 는 장기 계획의 한 형태를 나타냅니다. 이것은 페르소나의 일일 계획을 설정합니다.

        장기 계획과 단기 분해 방식을 취하고 있음을 주목하세요. 즉, 우리는 먼저 시간별 일정을 설정하고 점차 분해해 나갑니다.

        아래 예제에서 주목해야 할 세 가지:

        1) '잠자기'가 분해되지 않았다는 것을 확인하세요 -- 일부 일반적인 이벤트, 주로 잠자기는 분해할 수 없도록 하드 코딩되어 있습니다.
        2) 일부 요소들이 분해되기 시작합니다... 하루가 지남에 따라 더 많은 것들이 분해됩니다. (분해될 때 원래의 시간별 액션 설명은 그대로 남습니다.)
        3) 후반부의 요소들은 분해되지 않습니다. 이벤트가 발생하면, 분해되지 않은 요소들은 무시됩니다.
        예: [['잠자기', 360], ['일어나서 스트레칭...', 5], ['일어나서 아침 루틴 시작(침대에서 나옴)', 10],...['점심 먹기', 60], ['그림 작업하기', 180],...]]
        '''
        self.f_daily_schedule = []

        '''
        <f_daily_schedule_hourly_org> 는 초기에는 'f_daily_schedule'의 복제본이지만, 시간별 일정의 원래 분해되지 않은 버전을 유지합니다.
        예: [['잠자기', 360], ['일어나서 아침 루틴 시작', 120], ['그림 작업하기', 240], ...['잠자리에 들기', 60]]
        '''
        self.f_daily_schedule_hourly_org = []

        '''
        현재 행동(curr action)
        <address> 는 문자열로서 행동이 발생하는 위치의 주소입니다.
        이것은 "{world}:{sector}:{arena}:{game_objects}"의 형태로 나타납니다.

        몇몇 경우에는 주서의 뒷부분 요소가 없을 수 있으므로 음수 인덱싱 (예:[-1])을 사용하지 않고 접근하는 것이 중요합니다.

        예: "dolores double studio:double studio:bedroom 1:bed"
        '''
        self.act_address = None

        '''
        <start_time> 은 파이썬의 datetime 인스턴스로, 행동이 시작된 시간을 나타냅니다.
        '''
        self.act_start_time = None

        '''
        <duration> 은 정수 값으로, 행동이 얼마나 지속되어야 하는지를 분 단위로 나타냅니다.
        '''
        self.act_duration = None

        '''
        <description> 은 행동의 문자열 설명입니다.
        '''
        self.act_description = None

        '''
        <pronunciatio> 는 self.description의 설명적 표현입니다.
        '''
        self.act_pronunciation = None

        '''
        <event_from> 은 현재 페르소나가 참여하고 있는 이벤트 트리플을 나타냅니다.
        '''
        self.act_event = (self.name, None, None)

        '''
        <obj_description> 은 객체 행동의 문자열 설명입니다.
        '''
        self.act_obj_description = None

        '''
        <obj_pronunciatio> 는 객체 행동의 설명적 표현입니다.
        현재는 이모지로 구성되어있습니다.
        '''
        self.act_obj_pronunciatio = None

        '''
        <obj_event_from> 은 현재 행동 객체가 참여하고 있는 이벤트 트리플을 나타냅니다.
        '''
        self.act_obj_event = (self.name, None, None)

        '''
        <chatting_with> 은 현재 페르소나가 대화하고 있는 다른 페르소나의 문자열 이름입니다. 만약 존재하지 않으면 None입니다.
        '''
        self.chatting_with = None

        '''
        <chat> 은 두 페르소나간의 대화를 저장하는 리스트의 리스트입니다.
        다음과 같은 형태로 나타납니다:
        [["Dolores Murphy", "Hi"], ["Maeve Jenson", "Hi"] ...]
        '''
        self.chat = None

        '''
        <chatting_with_buffer> 는 예시와 같이 ['Dolores Murphy']=self.vision_r 형태의 딕셔너리입니다.
        '''
        self.chatting_with_buffer = dict()
        self.chatting_end_time = None

        '''
        <path_set> 은 페르소나가 이 행동을 수행하기 위해 경로를 이미 계산했을 경우 True입니다. 그 경로는 페르소나의 scratch.planned_path 에 저장됩니다.
        '''
        self.act_path_set = False

        '''
        <planned_path> 는 페르소나가 <curr_action>을 실행하기 위해 가야할 경로를 설명하는 x, y 좌표 튜플(tile)의 리스트입니다.

        이 리스트는 페르소나의 현재 타일을 포함하지 않고 목적지 타일을 포함합니다.

        예를 들어, [(50, 10), (49,10), (48, 10),...]
        '''
        self.planned_path = []
        

        if check_if_file_exists(f_saved):
            # bootstrap 파일이 있으면 불러온다
            scratch_load = json.load(open(f_saved))

            # vision_r: 페르소나가 주변에서 볼 수 있는 타일 수
            self.vision_r = scratch_load["vision_r"]
            self.att_bandwidth = scratch_load["att_bandwidth"]
            self.retention = scratch_load["retention"]

            if scratch_load["curr_time"]:
                self.curr_time = datetime.datetime.strptime(scratch_load["curr_time"], "%B %d, %Y, %H:%M:%S")

            else:
                self.curr_time = None
            # 현재 위치
            self.curr_tile = scratch_load["curr_tile"]
            self.daily_plan_req = scratch_load["daily_plan_req"]
            
            self.name = scratch_load["name"]
            self.first_name = scratch_load["first_name"]
            self.last_name = scratch_load["last_name"]
            self.age = scratch_load["age"]
            self.innate = scratch_load["innate"]
            self.learned = scratch_load["learned"]
            self.currently = scratch_load["currently"]
            self.lifestyle = scratch_load["lifestyle"]
            self.living_area = scratch_load["living_area"]

            self.concept_forget = scratch_load["concept_forget"]
            self.daily_reflection_time = scratch_load["daily_reflection_time"]
            self.datily_reflection_size = scratch_load["daily_reflection_size"]
            self.overlap_reflect_th = scratch_load["overlap_reflect_th"]
            self.kw_strg_event_reflect_th = scratch_load["kw_strg_event_reflect_th"]
            self.kw_strg_thought_reflect_th = scratch_load["kw_strg_thought_reflect_th"]

            self.recency_w = scratch_load["recency_w"]
            self.relevance_w = scratch_load["relevance_w"]
            self.importance_w = scratch_load["importance_w"]
            self.recency_decay = scratch_load["recency_decay"]
            self.importance_trigger_max = scratch_load["importance_trigger_max"]
            self.importance_trigger_curr = scratch_load["importance_trigger_curr"]
            self.importance_ele_n = scratch_load["importance_ele_n"]
            self.thought_count = scratch_load["thought_count"]

            self.daily_req = scratch_load["daily_req"]
            self.f_daily_schedule = scratch_load["f_daily_schedule"]
            self.f_daily_schedule_hourly_org = scratch_load["f_daily_schedule_hourly_org"]

            self.act_address = scratch_load["act_address"]
            if scratch_load["act_start_time"]:
                self.act_start_time = datetime.datetime.strptime(
                    scratch_load["act_start_time"], "%B %d, %Y, %H:%M:%S"
                )
            else:
                self.curr_time = None
            self.act_duration = scratch_load["act_duration"]
            self.act_description = scratch_load["act_description"]
            self.act_pronunciatio = scratch_load["act_pronunciatio"]
            self.act_event = tuple(scratch_load["act_event"])

            self.act_obj_description = scratch_load["act_obj_description"]
            self.act_obj_pronunciatio = scratch_load["act_obj_pronunciatio"]
            self.act_obj_event = tuple(scratch_load["act_obj_event"])

            self.chatting_with = scratch_load["chatting_with"]
            self.chat = scratch_load["chat"]
            self.chatting_with_buffer = scratch_load["chatting_with_buffer"]
            if scratch_load["chatting_end_time"]: 
                self.chatting_end_time = datetime.datetime.strptime(
                                                    scratch_load["chatting_end_time"],
                                                    "%B %d, %Y, %H:%M:%S")
            else:
                self.chatting_end_time = None

            self.act_path_set = scratch_load["act_path_set"]
            self.planned_path = scratch_load["planned_path"]


    def save(self, out_json):
        """
        Save persona's scratch. 

        INPUT: 
        out_json: The file where we wil be saving our persona's state. 
        OUTPUT: 
        None
        """
        scratch = dict() 
        scratch["vision_r"] = self.vision_r
        scratch["att_bandwidth"] = self.att_bandwidth
        scratch["retention"] = self.retention

        scratch["curr_time"] = self.curr_time.strftime("%B %d, %Y, %H:%M:%S")
        scratch["curr_tile"] = self.curr_tile
        scratch["daily_plan_req"] = self.daily_plan_req

        scratch["name"] = self.name
        scratch["first_name"] = self.first_name
        scratch["last_name"] = self.last_name
        scratch["age"] = self.age
        scratch["innate"] = self.innate
        scratch["learned"] = self.learned
        scratch["currently"] = self.currently
        scratch["lifestyle"] = self.lifestyle
        scratch["living_area"] = self.living_area

        scratch["concept_forget"] = self.concept_forget
        scratch["daily_reflection_time"] = self.daily_reflection_time
        scratch["daily_reflection_size"] = self.daily_reflection_size
        scratch["overlap_reflect_th"] = self.overlap_reflect_th
        scratch["kw_strg_event_reflect_th"] = self.kw_strg_event_reflect_th
        scratch["kw_strg_thought_reflect_th"] = self.kw_strg_thought_reflect_th

        scratch["recency_w"] = self.recency_w
        scratch["relevance_w"] = self.relevance_w
        scratch["importance_w"] = self.importance_w
        scratch["recency_decay"] = self.recency_decay
        scratch["importance_trigger_max"] = self.importance_trigger_max
        scratch["importance_trigger_curr"] = self.importance_trigger_curr
        scratch["importance_ele_n"] = self.importance_ele_n
        scratch["thought_count"] = self.thought_count

        scratch["daily_req"] = self.daily_req
        scratch["f_daily_schedule"] = self.f_daily_schedule
        scratch["f_daily_schedule_hourly_org"] = self.f_daily_schedule_hourly_org

        scratch["act_address"] = self.act_address
        scratch["act_start_time"] = (self.act_start_time
                                        .strftime("%B %d, %Y, %H:%M:%S"))
        scratch["act_duration"] = self.act_duration
        scratch["act_description"] = self.act_description
        scratch["act_pronunciatio"] = self.act_pronunciatio
        scratch["act_event"] = self.act_event

        scratch["act_obj_description"] = self.act_obj_description
        scratch["act_obj_pronunciatio"] = self.act_obj_pronunciatio
        scratch["act_obj_event"] = self.act_obj_event

        scratch["chatting_with"] = self.chatting_with
        scratch["chat"] = self.chat
        scratch["chatting_with_buffer"] = self.chatting_with_buffer
        if self.chatting_end_time: 
            scratch["chatting_end_time"] = (self.chatting_end_time
                                            .strftime("%B %d, %Y, %H:%M:%S"))
        else: 
            scratch["chatting_end_time"] = None

        scratch["act_path_set"] = self.act_path_set
        scratch["planned_path"] = self.planned_path

        with open(out_json, "w") as outfile:
            json.dump(scratch, outfile, indent=2) 


    def get_f_daily_schedule_index(self, advance=0):
        '''
        self.f_daily_schedule의 현재 인덱스를 얻는다.
        f_daily_schedule는 장기 계획의 한 형태입니다.

        self.f_daily_schedule는 지금까지 분해된 행동 시퀀스와 나머지 하루에 대한 시간별 행동 시퀀스를 저장하고 있습니다. self.f_daily_schedule 이 [작업, 기간]으로 구성된 내부 리스트의 리스트인 점을 고려하면, "if elapsed > today_min_elapsed" 조건에 도달할 때까지 기간을 더해 나갑니다. 멈추는 인덱스가 반환할 인덱스입니다.

        입력:
            advance: 미래의 시간대 인덱스를 얻기 위해 앞으로 몇 분을 살펴볼지의 정수 값입니다.
        출력:
            f_daily_schedule의 현재 인덱스에 대한 정수 값입니다.
        '''

        # f_daily_schedule 는 하루의 계획을 분단위로 작성한 것
        # 현재 시간, 즉 오늘 지난 시간을 분으로 치환한다.
        today_min_elapsed = 0
        today_min_elapsed += self.curr_time.hour * 60
        today_min_elapsed += self.curr_time.minute
        today_min_elapsed += advance

        # 아래는 없어도 되는 코드
        # --------------
        x = 0
        for task, duration in self.f_daily_schedule:
            x += duration
        x = 0
        for task, duration in self.f_daily_schedule_hourly_org:
            x += duration
        # --------------

        # 그 다음 그를 기반으로 현재 인덱스를 계산한다.
        curr_index = 0
        elapsed = 0
        # f_daily_schedule 에서 시간을 가져온다.
        # 해당 시간을 elapsed에 넣는다
        # 만약 elapsed가 today_min_elapsed(현재 시간) 보다 커지면
        # 해당 인덱스를 반환한다
        # 만약 크지 않으면 +1 을 하여 다음 인덱스를 탐색한다.
        for task, duration in self.f_daily_schedule:
            elapsed += duration
            if elapsed > today_min_elapsed:
                return curr_index
            curr_index += 1

        return curr_index
            

    def get_f_daily_schedule_hourly_org_index(self, advance=0):
        """
        self.f_daily_schedule_hourly_org의 현재 인덱스를 가져온다.
        그 위에는 get_f_daily_schedule_index와 동일하다.

        INPUT
            advance: 미래의 시간을 몇 분 뒤로 볼 것인지에 대한 정수 값.
            이를 통해 미래 시점의 인덱스를 얻을 수 있다.
        OUTPUT
            f_daily_schedule의 현재 인덱스에 대한 정수 값
        """
        # 우선 오늘 지난 분의 수를 계산한다.
        today_min_elapsed = 0
        today_min_elapsed += self.curr_time.hour * 60
        today_min_elapsed += self.curr_time.minute
        today_min_elapsed += advance
        # 그 다음 그를 기반으로 현재 인덱스를 계산한다.
        
        curr_index = 0
        elapsed = 0
        # f_daily_schedule 에서 시간을 가져온다.
        # 해당 시간을 elapsed에 넣는다
        # 만약 elapsed가 today_min_elapsed(현재 시간) 보다 커지면
        # 해당 인덱스를 반환한다
        # 만약 크지 않으면 +1 을 하여 다음 인덱스를 탐색한다.
        for task, duration in self.f_daily_schedule_hourly_org:
            elapsed += duration
            if elapsed > today_min_elapsed:
                return curr_index
            curr_index += 1

        return curr_index


    def get_str_iss(self):
        """
        ISS는 "정체성 안정 집합(Identity Stable Set)"를 의미한다. 이것은 이 페르소나의 공통 요약 집합을 설명하는 것으로, 거의 모든 프롬프트에서 페르소나를 호출할 때 사용되는 기본 최소 설명이다

        INPUT:
            None
        OUTPUT:
            문자열 형태로 된 페르소나의 정체성 안정 집합 요약
        EXAMPLE:
            "Name: Dolores Heitmiller
            Age: 28
            Innate traits: hard-edged, independent, loyal
            Learned traits: Dolores is a painter who wants live quietly and paint 
                while enjoying her everyday life.
            Currently: Dolores is preparing for her first solo show. She mostly 
                works from home.
            Lifestyle: Dolores goes to bed around 11pm, sleeps for 7 hours, eats 
                dinner around 6pm.
            Daily plan requirement: Dolores is planning to stay at home all day and 
                never go out."

            들어가는 것은 이름, 나이, 선천적 특성, 후천적 특성, 현재, 라이프스타일, 일일 요구사항이다
        """
        commonset = ""
        commonset += f"Name: {self.name}\n"
        commonset += f"Age: {self.age}\n"
        commonset += f"Innate traits: {self.innate}\n"
        commonset += f"Learned traits: {self.learned}\n"
        commonset += f"Currently: {self.currently}\n"
        commonset += f"Lifestyle: {self.lifestyle}\n"
        commonset += f"Daily plan requirement: {self.daily_plan_req}\n"
        if not isinstance(self.curr_time, datetime.datetime):
            self.curr_time=datetime.datetime.strptime(
                self.curr_time, "%B %d, %Y, %H:%M:%S")
        commonset += f"Current Date: {self.curr_time.strftime('%A %B %d')}\n"
        # curr_time_obj = datetime.datetime.strptime("November 6, 2023, 00:00:00", "%B %d, %Y, %H:%M:%S")
        # 그 다음, datetime 객체에 대해 strftime 메서드를 사용하여 원하는 형식의 문자열로 변환합니다.
        # formatted_date = curr_time_obj.strftime('%A %B %d')
        # # 이제 formatted_date를 문자열에 추가합니다.
        # commonset += f"Current Date: {formatted_date}\n"
        return commonset


    def get_sentance_iss(self):

        text = "The name of <0> is <0>. The age of <0> is <1> years old. The Innate traits of <0> are <2>. The Learned traits of <0> are <3>. Recently, <0> has been <4>. The Lifestyle of <0> is <5>. The Daily plan of <0> is <6>. Current Data is <7>."
        if not isinstance(self.curr_time, datetime.datetime):
            self.curr_time = datetime.datetime.strptime(
                self.curr_time, "%B %d, %Y, %H:%M:%S")
        data = [self.name, str(self.age), self.innate, self.learned, self.currently, self.lifestyle, self.daily_plan_req, self.curr_time.strftime('%A %B %d')]
        
        for index, value in enumerate(data):
            text = text.replace(f"<{index}>", value)

        return text
    
    def get_str_name(self): 
        return self.name


    def get_str_firstname(self): 
        return self.first_name


    def get_str_lastname(self): 
        return self.last_name


    def get_str_age(self): 
        return str(self.age)


    def get_str_innate(self): 
        return self.innate


    def get_str_learned(self): 
        return self.learned


    def get_str_currently(self): 
        return self.currently


    def get_str_lifestyle(self): 
        return self.lifestyle


    def get_str_daily_plan_req(self): 
        return self.daily_plan_req


    def get_str_curr_date_str(self): 
        if not isinstance(self.curr_time, datetime.datetime):
            self.curr_time=datetime.datetime.strptime(
                    self.curr_time, "%B %d, %Y, %H:%M:%S")
        return self.curr_time.strftime("%A %B %d")


    def get_curr_event(self):
        if not self.act_address: 
            return (self.name, None, None)
        else: 
            return self.act_event
        
    def get_curr_event_and_desc(self): 
        if not self.act_address: 
            return (self.name, None, None, None)
        else: 
            return (self.act_event[0], 
                    self.act_event[1], 
                    self.act_event[2],
                    self.act_description)


    def get_curr_obj_event_and_desc(self): 
        if not self.act_address: 
            return ("", None, None, None)
        else: 
            return (self.act_address, 
                    self.act_obj_event[1], 
                    self.act_obj_event[2],
                    self.act_obj_description)


    def add_new_action(self, 
                        action_address, 
                        action_duration,
                        action_description,
                        action_pronunciatio, 
                        action_event,
                        chatting_with, 
                        chat, 
                        chatting_with_buffer,
                        chatting_end_time,
                        act_obj_description, 
                        act_obj_pronunciatio, 
                        act_obj_event, 
                        act_start_time=None): 
        self.act_address = action_address
        self.act_duration = action_duration
        self.act_description = action_description
        self.act_pronunciatio = action_pronunciatio
        self.act_event = action_event

        self.chatting_with = chatting_with
        self.chat = chat 
        if chatting_with_buffer: 
            self.chatting_with_buffer.update(chatting_with_buffer)
        self.chatting_end_time = chatting_end_time

        self.act_obj_description = act_obj_description
        self.act_obj_pronunciatio = act_obj_pronunciatio
        self.act_obj_event = act_obj_event

        self.act_start_time = self.curr_time

        self.act_path_set = False


    def act_time_str(self): 
        """
        Returns a string output of the current time. 

        INPUT
            None
        OUTPUT 
            A string output of the current time.
        EXAMPLE STR OUTPUT
            "14:05 P.M."
        """
        return self.act_start_time.strftime("%H:%M %p")


    def act_check_finished(self): 
        """
        Checks whether the self.Action instance has finished.  

        INPUT
            curr_datetime: Current time. If current time is later than the action's
                            start time + its duration, then the action has finished. 
        OUTPUT 
            Boolean [True]: Action has finished.
            Boolean [False]: Action has not finished and is still ongoing.
        """
        if not self.act_address: 
            return True
            
        if self.chatting_with: 
            end_time = self.chatting_end_time
        else: 
            x = self.act_start_time
            if x.second != 0: 
                x = x.replace(second=0)
                x = (x + datetime.timedelta(minutes=1))
            end_time = (x + datetime.timedelta(minutes=self.act_duration))

        if end_time.strftime("%H:%M:%S") == self.curr_time.strftime("%H:%M:%S"): 
            return True
        return False


    def act_summarize(self):
        """
        Summarize the current action as a dictionary. 

        INPUT
            None
        OUTPUT 
            ret: A human readable summary of the action.
        """
        exp = dict()
        exp["persona"] = self.name
        exp["address"] = self.act_address
        exp["start_datetime"] = self.act_start_time
        exp["duration"] = self.act_duration
        exp["description"] = self.act_description
        exp["pronunciatio"] = self.act_pronunciatio
        return exp


    def act_summary_str(self):
        """
        Returns a string summary of the current action. Meant to be 
        human-readable.

        INPUT
            None
        OUTPUT 
            ret: A human readable summary of the action.
        """
        start_datetime_str = self.act_start_time.strftime("%A %B %d -- %H:%M %p")
        ret = f"[{start_datetime_str}]\n"
        ret += f"Activity: {self.name} is {self.act_description}\n"
        ret += f"Address: {self.act_address}\n"
        ret += f"Duration in minutes (e.g., x min): {str(self.act_duration)} min\n"
        return ret


    def get_str_daily_schedule_summary(self): 
        ret = ""
        curr_min_sum = 0
        for row in self.f_daily_schedule: 
            curr_min_sum += row[1]
            hour = int(curr_min_sum/60)
            minute = curr_min_sum%60
            ret += f"{hour:02}:{minute:02} || {row[0]}\n"
        return ret


    def get_str_daily_schedule_hourly_org_summary(self): 
        ret = ""
        curr_min_sum = 0
        for row in self.f_daily_schedule_hourly_org: 
            curr_min_sum += row[1]
            hour = int(curr_min_sum/60)
            minute = curr_min_sum%60
            ret += f"{hour:02}:{minute:02} || {row[0]}\n"
        return ret    

if __name__ =="__main__":
    # scratch_saved = f"/home/elicer/main/agents_test/frontend/persona/Tom/bootstrap_memory/scratch.json"
    # scratch = Scratch(scratch_saved)
    # print(scratch.get_str_iss())
    # print(scratch.curr_time)
    pass