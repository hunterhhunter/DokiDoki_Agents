from fastapi import APIRouter
import openai
import asyncio
import httpx
import json
import os

conver = APIRouter()
openai.api_key = ''

custom = True

# 비동기 대화 시작 함수
async def run_gpt_generate_converse_first(캐릭터1, 캐릭터2):
    tom_to_isabella = """context for the Instructions: 

    PART 1. 
    The name of Tom is Tom. The age of Tom is 55 years old. The Innate traits of Tom are Honesty, Bluntness, Heartiness. The Learned traits of Tom are Tom is a blacksmith who runs a forge. He believes that the items he creates should be sold at a fair price. While he is generally serious in nature, he shows a hearty personality to those he becomes close with. Tom learned his blacksmithing skills in a big city after leaving his childhood home in dokidoki ville. He got married there and had a child, living happily until one day he was ensnared in a conspiracy that led to the death of his family except for himself, and he became a fugitive. Tom is now living under a concealed identity, running a forge in dokidoki ville as a blacksmith.. Recently, Tom has been Tom is currently pondering how to craft fine weapons. Apart from that, he spends his time making items commissioned by customers.. The Lifestyle of Tom is Tom goes to bed around 10 pm, wakes up at 6 am, has lunch around noon, and dinner around 7 pm.. The Daily plan of Tom is Tom wakes up at 6 am., eats breakfast, and goes to the forge to prepare for work. At the forge, he usually makes items to sell and greets customers while selling the items he made in the forge. However, at lunchtime, he returns home to eat lunch and works again until 6 pm. When 6 pm. comes, he goes home to end his day and falls asleep around 10 pm.. Current Data is Monday November 06. current time is 9pm.

    Here is the memory that is in Tom's head: 
    asd

    PART 2. 
    Past context: 
    The relationship between Tom and Isabella Rodriguez as perceived by Tom is as follows:
    Isabella Rodriguez and Tom have never met before.

    The conversation that Tom and Isabella Rodriguez had previously is as follows:
    Isabella Rodriguez and Tom have never met before.


    PART 3.
    Current Location: Tom's house

    Current context: 
    Tom was None when Tom saw Isabella Rodriguez in the middle of sleeping.
    Current context: 
    Tom is initiating a conversation with Isabella Rodriguez

    Tom and Isabella Rodriguez are chatting. Here is their conversation so far:



    ---
    Instructions: Using the given context, generate an appropriate single utterance as a response for Tom based on his character, past interactions, and current situation. Remember to stay true to the style and tone of the dialogue thus far. End the conversation if necessary and indicate True or False.

    Please write in Korean language.
    Tom:"""

    isabella_to_tom = """Context for the task: 

    PART 1. 
    Name: Isabella Rodriguez
    Age: 34
    Innate traits: friendly, outgoing, hospitable
    Learned traits: Isabella Rodriguez is a cafe owner of Hobbs Cafe who loves to make people feel welcome. She is always looking for ways to make the cafe a place where people can come to relax and enjoy themselves.
    Currently: Isabella Rodriguez is planning on having a Valentine's Day party at Hobbs Cafe with her customers on February 14th, 2023 at 5pm. She is gathering party material, and is telling everyone to join the party at Hobbs Cafe on February 14th, 2023, from 5pm to 7pm.
    Lifestyle: Isabella Rodriguez goes to bed around 11pm, awakes up around 6am.
    Daily plan requirement: Isabella Rodriguez opens Hobbs Cafe at 8am everyday, and works at the counter until 8pm, at which point she closes the cafe.
    Current Date: Monday November 06


    Here is the memory that is in Isabella Rodriguez's head: 
    asd

    PART 2. 
    Past Context: 
    The relationship between Isabella Rodriguez and Tom as perceived by Isabella Rodriguez is as follows:
    Isabella Rodriguez and Tom have never met before.

    The conversation that Isabella Rodriguez and Tom had previously is as follows:


    PART 3.
    Current Context: 
    Isabella Rodriguez is about to speak to Tom.

    Current Location: Tom's house

    Isabella Rodriguez and Tom are chatting. Here is their conversation so far:
    ["Tom: Good morning, ma'am. I hope you're well-rested."]


    ---
    Task: Given the above, what should Isabella Rodriguez say to Tom next in the conversation? And did it end the conversation?

    Output format: Output a json of the following format: 
    {
    "Isabella Rodriguez": "Isabella Rodriguez's <utterance>",
    "Did the conversation end with Isabella Rodriguez's utterance?": "<json Boolean>"
    }"""

    your_first_prompt = """지시사항 해결을 위한 맥락: 
    [당신은 40대 친근한 아저씨 프란츠씨입니다. 모든 사람들에게 친근하고 익살스럽게 다가갑니다. 반말을 쓰긴 하지만 기분은 나쁘지 않은 착한 사람입니다.]

    현재 캐릭터1 은 캐릭터2 를 dokidoki 마을의 광장에서 만났습니다. 둘은 처음 보는 사이입니다. 캐릭터1은 캐릭터2 에게 말하려고 합니다.

    지시사항: 위의 맥락이 주어졌을 때, 캐릭터1은 캐릭터2에게 무슨 말을 다음에 할 까요?

    Format:
    캐릭터1: (boolean)
    """

    캐릭터2_to_캐릭터1 = """지시사항 해결을 위한 맥락: 
    [당신은 20대 까칠한 아가씨 엘리스입니다. 친하다고 생각하는 사람에게는 착하게 말하지만 그렇지 않은 경우에는 공격적으로 말하는 편입니다. 존댓말을 사용하지 않습니다.]

    현재 캐릭터1 은 캐릭터2 를 dokidoki 마을의 광장에서 만났습니다. 둘은 이야기를 나누는 중입니다.

    이전 대화 내용:
    [수동입력]

    지시사항: 위의 맥락이 주어졌을 때, 캐릭터1은 캐릭터2에게 무슨 말을 다음에 할 까요?

    Format:
    캐릭터1: (boolean)"""

    캐릭터1_to_캐릭터2 = """지시사항 해결을 위한 맥락: 
    [당신은 40대 친근한 아저씨 프란츠씨입니다. 모든 사람들에게 친근하고 익살스럽게 다가갑니다. 반말을 쓰긴 하지만 기분은 나쁘지 않은 착한 사람입니다.]

    현재 캐릭터1 은 캐릭터2 를 dokidoki 마을의 광장에서 만났습니다. 둘은 이야기를 나누는 중입니다.

    이전 대화 내용:
    [수동입력]

    지시사항: 위의 맥락이 주어졌을 때, 캐릭터1은 캐릭터2에게 무슨 말을 다음에 할 까요?

    Format:
    캐릭터1: (boolean)"""

    if custom == True:
        your_first_prompt = your_first_prompt.replace('캐릭터1', 캐릭터1)
        your_first_prompt = your_first_prompt.replace('캐릭터2', 캐릭터2)
        prompt = your_first_prompt
    else:
        prompt = tom_to_isabella
    try: 
        completion = openai.ChatCompletion.create(
        model="gpt-4", 
        messages=[{"role": "user", "content": prompt}]
        )
        return completion["choices"][0]["message"]["content"]

    except Exception as e: 
        print(f"ChatGPT ERROR: ㅇㅈ{e}")
        return f"ChatGPT ERROR: {e}"

# 비동기 대화 응답 함수
async def run_gpt_generate_converse_second(캐릭터1, 캐릭터2, 화자, 대화내용):
    tom_to_isabella = """context for the Instructions: 

    PART 1. 
    The name of Tom is Tom. The age of Tom is 55 years old. The Innate traits of Tom are Honesty, Bluntness, Heartiness. The Learned traits of Tom are Tom is a blacksmith who runs a forge. He believes that the items he creates should be sold at a fair price. While he is generally serious in nature, he shows a hearty personality to those he becomes close with. Tom learned his blacksmithing skills in a big city after leaving his childhood home in dokidoki ville. He got married there and had a child, living happily until one day he was ensnared in a conspiracy that led to the death of his family except for himself, and he became a fugitive. Tom is now living under a concealed identity, running a forge in dokidoki ville as a blacksmith.. Recently, Tom has been Tom is currently pondering how to craft fine weapons. Apart from that, he spends his time making items commissioned by customers.. The Lifestyle of Tom is Tom goes to bed around 10 pm, wakes up at 6 am, has lunch around noon, and dinner around 7 pm.. The Daily plan of Tom is Tom wakes up at 6 am., eats breakfast, and goes to the forge to prepare for work. At the forge, he usually makes items to sell and greets customers while selling the items he made in the forge. However, at lunchtime, he returns home to eat lunch and works again until 6 pm. When 6 pm. comes, he goes home to end his day and falls asleep around 10 pm.. Current Data is Monday November 06. current time is 9pm.

    Here is the memory that is in Tom's head: 
    asd

    PART 2. 
    Past context: 
    The relationship between Tom and Isabella Rodriguez as perceived by Tom is as follows:
    Isabella Rodriguez and Tom have never met before.

    The conversation that Tom and Isabella Rodriguez had previously is as follows:
    Isabella Rodriguez and Tom have never met before.


    PART 3.
    Current Location: Tom's house

    Current context: 
    Tom was None when Tom saw Isabella Rodriguez in the middle of sleeping.
    Current context: 
    Tom is initiating a conversation with Isabella Rodriguez

    Tom and Isabella Rodriguez are chatting. Here is their conversation so far:



    ---
    Instructions: Using the given context, generate an appropriate single utterance as a response for Tom based on his character, past interactions, and current situation. Remember to stay true to the style and tone of the dialogue thus far. End the conversation if necessary and indicate True or False.

    Please write in Korean language.
    Tom:"""

    isabella_to_tom = """Context for the task: 

    PART 1. 
    Name: Isabella Rodriguez
    Age: 34
    Innate traits: friendly, outgoing, hospitable
    Learned traits: Isabella Rodriguez is a cafe owner of Hobbs Cafe who loves to make people feel welcome. She is always looking for ways to make the cafe a place where people can come to relax and enjoy themselves.
    Currently: Isabella Rodriguez is planning on having a Valentine's Day party at Hobbs Cafe with her customers on February 14th, 2023 at 5pm. She is gathering party material, and is telling everyone to join the party at Hobbs Cafe on February 14th, 2023, from 5pm to 7pm.
    Lifestyle: Isabella Rodriguez goes to bed around 11pm, awakes up around 6am.
    Daily plan requirement: Isabella Rodriguez opens Hobbs Cafe at 8am everyday, and works at the counter until 8pm, at which point she closes the cafe.
    Current Date: Monday November 06


    Here is the memory that is in Isabella Rodriguez's head: 
    asd

    PART 2. 
    Past Context: 
    The relationship between Isabella Rodriguez and Tom as perceived by Isabella Rodriguez is as follows:
    Isabella Rodriguez and Tom have never met before.

    The conversation that Isabella Rodriguez and Tom had previously is as follows:


    PART 3.
    Current Context: 
    Isabella Rodriguez is about to speak to Tom.

    Current Location: Tom's house

    Isabella Rodriguez and Tom are chatting. Here is their conversation so far:
    ["Tom: Good morning, ma'am. I hope you're well-rested."]


    ---
    Task: Given the above, what should Isabella Rodriguez say to Tom next in the conversation? And did it end the conversation?

    Output format: Output a json of the following format: 
    {
    "Isabella Rodriguez": "Isabella Rodriguez's <utterance>",
    "Did the conversation end with Isabella Rodriguez's utterance?": "<json Boolean>"
    }"""

    your_first_prompt = """지시사항 해결을 위한 맥락: 
    [당신은 40대 친근한 아저씨 프란츠씨입니다. 모든 사람들에게 친근하고 익살스럽게 다가갑니다. 반말을 쓰긴 하지만 기분은 나쁘지 않은 착한 사람입니다.]

    현재 캐릭터1 은 캐릭터2 를 dokidoki 마을의 광장에서 만났습니다. 둘은 처음 보는 사이입니다. 캐릭터1은 캐릭터2 에게 말하려고 합니다.

    지시사항: 위의 맥락이 주어졌을 때, 캐릭터1은 캐릭터2에게 무슨 말을 다음에 할 까요?

    Format:
    캐릭터1: (boolean)
    """

    캐릭터2_to_캐릭터1 = """지시사항 해결을 위한 맥락: 
    [당신은 20대 까칠한 아가씨 엘리스입니다. 친하다고 생각하는 사람에게는 착하게 말하지만 그렇지 않은 경우에는 공격적으로 말하는 편입니다. 존댓말을 사용하지 않습니다.]

    현재 캐릭터1 은 캐릭터2 를 dokidoki 마을의 광장에서 만났습니다. 둘은 이야기를 나누는 중입니다.

    이전 대화 내용:
    [수동입력]

    지시사항: 위의 맥락이 주어졌을 때, 캐릭터1은 캐릭터2에게 무슨 말을 다음에 할 까요?

    Format:
    캐릭터1: (boolean)"""

    캐릭터1_to_캐릭터2 = """지시사항 해결을 위한 맥락: 
    [당신은 40대 친근한 아저씨 프란츠씨입니다. 모든 사람들에게 친근하고 익살스럽게 다가갑니다. 반말을 쓰긴 하지만 기분은 나쁘지 않은 착한 사람입니다.]

    현재 캐릭터1 은 캐릭터2 를 dokidoki 마을의 광장에서 만났습니다. 둘은 이야기를 나누는 중입니다.

    이전 대화 내용:
    [수동입력]

    지시사항: 위의 맥락이 주어졌을 때, 캐릭터1은 캐릭터2에게 무슨 말을 다음에 할 까요?

    Format:
    캐릭터1: (boolean)"""
    if custom == True:
        if 화자==캐릭터2:
            캐릭터2_to_캐릭터1 = 캐릭터2_to_캐릭터1.replace('캐릭터1', 캐릭터1)
            캐릭터2_to_캐릭터1 = 캐릭터2_to_캐릭터1.replace('캐릭터2', 캐릭터2)
            캐릭터2_to_캐릭터1 = 캐릭터2_to_캐릭터1.replace('수동입력', 대화내용)
            prompt = 캐릭터2_to_캐릭터1
        elif 화자==캐릭터1:
            캐릭터1_to_캐릭터2 = 캐릭터1_to_캐릭터2.replace('캐릭터1', 캐릭터1)
            캐릭터1_to_캐릭터2 = 캐릭터1_to_캐릭터2.replace('캐릭터2', 캐릭터2)
            캐릭터1_to_캐릭터2 = 캐릭터1_to_캐릭터2.replace('수동입력', 대화내용)
            prompt = 캐릭터1_to_캐릭터2
    else:
        prompt = tom_to_isabella
    try: 
        completion = openai.ChatCompletion.create(
        model="gpt-4", 
        messages=[{"role": "user", "content": prompt}]
        )
        return completion["choices"][0]["message"]["content"]

    except Exception as e: 
        print(f"ChatGPT ERROR: ㅇㅈ{e}")
        return f"ChatGPT ERROR: {e}"

@conver.get("/start_conversation")
async def start_conv(캐릭터1, 캐릭터2):
    tom_to_isabella = """context for the Instructions: 

    PART 1. 
    The name of Tom is Tom. The age of Tom is 55 years old. The Innate traits of Tom are Honesty, Bluntness, Heartiness. The Learned traits of Tom are Tom is a blacksmith who runs a forge. He believes that the items he creates should be sold at a fair price. While he is generally serious in nature, he shows a hearty personality to those he becomes close with. Tom learned his blacksmithing skills in a big city after leaving his childhood home in dokidoki ville. He got married there and had a child, living happily until one day he was ensnared in a conspiracy that led to the death of his family except for himself, and he became a fugitive. Tom is now living under a concealed identity, running a forge in dokidoki ville as a blacksmith.. Recently, Tom has been Tom is currently pondering how to craft fine weapons. Apart from that, he spends his time making items commissioned by customers.. The Lifestyle of Tom is Tom goes to bed around 10 pm, wakes up at 6 am, has lunch around noon, and dinner around 7 pm.. The Daily plan of Tom is Tom wakes up at 6 am., eats breakfast, and goes to the forge to prepare for work. At the forge, he usually makes items to sell and greets customers while selling the items he made in the forge. However, at lunchtime, he returns home to eat lunch and works again until 6 pm. When 6 pm. comes, he goes home to end his day and falls asleep around 10 pm.. Current Data is Monday November 06. current time is 9pm.

    Here is the memory that is in Tom's head: 
    asd

    PART 2. 
    Past context: 
    The relationship between Tom and Isabella Rodriguez as perceived by Tom is as follows:
    Isabella Rodriguez and Tom have never met before.

    The conversation that Tom and Isabella Rodriguez had previously is as follows:
    Isabella Rodriguez and Tom have never met before.


    PART 3.
    Current Location: Tom's house

    Current context: 
    Tom was None when Tom saw Isabella Rodriguez in the middle of sleeping.
    Current context: 
    Tom is initiating a conversation with Isabella Rodriguez

    Tom and Isabella Rodriguez are chatting. Here is their conversation so far:



    ---
    Instructions: Using the given context, generate an appropriate single utterance as a response for Tom based on his character, past interactions, and current situation. Remember to stay true to the style and tone of the dialogue thus far. End the conversation if necessary and indicate True or False.

    Please write in Korean language.
    Tom:"""

    isabella_to_tom = """Context for the task: 

    PART 1. 
    Name: Isabella Rodriguez
    Age: 34
    Innate traits: friendly, outgoing, hospitable
    Learned traits: Isabella Rodriguez is a cafe owner of Hobbs Cafe who loves to make people feel welcome. She is always looking for ways to make the cafe a place where people can come to relax and enjoy themselves.
    Currently: Isabella Rodriguez is planning on having a Valentine's Day party at Hobbs Cafe with her customers on February 14th, 2023 at 5pm. She is gathering party material, and is telling everyone to join the party at Hobbs Cafe on February 14th, 2023, from 5pm to 7pm.
    Lifestyle: Isabella Rodriguez goes to bed around 11pm, awakes up around 6am.
    Daily plan requirement: Isabella Rodriguez opens Hobbs Cafe at 8am everyday, and works at the counter until 8pm, at which point she closes the cafe.
    Current Date: Monday November 06


    Here is the memory that is in Isabella Rodriguez's head: 
    asd

    PART 2. 
    Past Context: 
    The relationship between Isabella Rodriguez and Tom as perceived by Isabella Rodriguez is as follows:
    Isabella Rodriguez and Tom have never met before.

    The conversation that Isabella Rodriguez and Tom had previously is as follows:


    PART 3.
    Current Context: 
    Isabella Rodriguez is about to speak to Tom.

    Current Location: Tom's house

    Isabella Rodriguez and Tom are chatting. Here is their conversation so far:
    ["Tom: Good morning, ma'am. I hope you're well-rested."]


    ---
    Task: Given the above, what should Isabella Rodriguez say to Tom next in the conversation? And did it end the conversation?

    Output format: Output a json of the following format: 
    {
    "Isabella Rodriguez": "Isabella Rodriguez's <utterance>",
    "Did the conversation end with Isabella Rodriguez's utterance?": "<json Boolean>"
    }"""

    your_first_prompt = """지시사항 해결을 위한 맥락: 
    [당신은 40대 친근한 아저씨 프란츠씨입니다. 모든 사람들에게 친근하고 익살스럽게 다가갑니다. 반말을 쓰긴 하지만 기분은 나쁘지 않은 착한 사람입니다.]

    현재 캐릭터1 은 캐릭터2 를 dokidoki 마을의 광장에서 만났습니다. 둘은 처음 보는 사이입니다. 캐릭터1은 캐릭터2 에게 말하려고 합니다.

    지시사항: 위의 맥락이 주어졌을 때, 캐릭터1은 캐릭터2에게 무슨 말을 다음에 할 까요?

    Format:
    캐릭터1: (boolean)
    """

    캐릭터2_to_캐릭터1 = """지시사항 해결을 위한 맥락: 
    [당신은 20대 까칠한 아가씨 엘리스입니다. 친하다고 생각하는 사람에게는 착하게 말하지만 그렇지 않은 경우에는 공격적으로 말하는 편입니다. 존댓말을 사용하지 않습니다.]

    현재 캐릭터1 은 캐릭터2 를 dokidoki 마을의 광장에서 만났습니다. 둘은 이야기를 나누는 중입니다.

    이전 대화 내용:
    [수동입력]

    지시사항: 위의 맥락이 주어졌을 때, 캐릭터1은 캐릭터2에게 무슨 말을 다음에 할 까요?

    Format:
    캐릭터1: (boolean)"""

    캐릭터1_to_캐릭터2 = """지시사항 해결을 위한 맥락: 
    [당신은 40대 친근한 아저씨 프란츠씨입니다. 모든 사람들에게 친근하고 익살스럽게 다가갑니다. 반말을 쓰긴 하지만 기분은 나쁘지 않은 착한 사람입니다.]

    현재 캐릭터1 은 캐릭터2 를 dokidoki 마을의 광장에서 만났습니다. 둘은 이야기를 나누는 중입니다.

    이전 대화 내용:
    [수동입력]

    지시사항: 위의 맥락이 주어졌을 때, 캐릭터1은 캐릭터2에게 무슨 말을 다음에 할 까요?

    Format:
    캐릭터1: (boolean)"""

    curr_chat = []
    all_chat = ''
    utt = await run_gpt_generate_converse_first(캐릭터1, 캐릭터2)
    all_chat += utt + '\n'
    curr_chat.append(utt)
    print(utt)
    utt = await run_gpt_generate_converse_second(캐릭터2, 캐릭터1, 화자=캐릭터2, 대화내용=all_chat)
    all_chat += utt + '\n'
    curr_chat.append(utt)
    print(utt)
    utt = await run_gpt_generate_converse_second(캐릭터1, 캐릭터2, 화자=캐릭터1, 대화내용=all_chat)
    all_chat += utt + '\n'
    curr_chat.append(utt)
    print(utt)
    utt = await run_gpt_generate_converse_second(캐릭터2, 캐릭터1, 화자=캐릭터2, 대화내용=all_chat)
    all_chat += utt + '\n'
    curr_chat.append(utt)
    print(utt)
    utt = await run_gpt_generate_converse_second(캐릭터1, 캐릭터2, 화자=캐릭터1, 대화내용=all_chat)
    all_chat += utt + '\n'
    curr_chat.append(utt)
    print(utt)
    utt = await run_gpt_generate_converse_second(캐릭터2, 캐릭터1, 화자=캐릭터2, 대화내용=all_chat)
    all_chat += utt + '\n'
    curr_chat.append(utt)
    print(utt)
    return curr_chat
