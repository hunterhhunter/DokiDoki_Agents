from persona.persona import *

from persona.cognitive_modules.retrieve import *
from persona.cognitive_modules.plan import *

# # print(os.path.curdir)
# # print(os.listdir('main/data/personas/Emerald Puyor'))
# persona = Persona('Emerald Puyor', 'main/data/personas/Emerald Puyor copy')
# persona.scratch.curr_time = datetime.datetime.strptime("February 14, 2023, 00:02:20", 
#                                                 "%B %d, %Y, %H:%M:%S")
# determine_action(persona, None)
# persona.save('main/data/personas/Emerald Puyor copy/bootstrap_memory')

# #----
# # idle_node = persona.a_mem.add_chat(persona.scratch.curr_time, None,
# #                       curr_event[0], curr_event[1], curr_event[2], 
# #                       persona.scratch.act_description, keywords, 
# #                       chat_poignancy, chat_embedding_pair, 
# #                       persona.scratch.chat)

# idle_node = ConceptNode('node_875', 875, 737, "event", 0, 
#                         "2023-02-13 11:22:40", None, 
#                         "the Ville:Isabella Rodriguez's apartment:main room:bed", "is", "idle", 
#                         "bed is idle", "bed is idle",
#                         1, ['idle', 'bed'], []
#                         )


# thought_node = ConceptNode('node_875', 875, 135, "thought", 1, 
#                         "2023-02-13 11:22:40", None, 
#                         "Isabella Rodriguez", "have", "close relationship", 
#                         "Isabella Rodriguez and Maria Lopez have a close relationship", "Isabella Rodriguez and Maria Lopez have a close relationship",
#                         8, ['Isabella Rodriguez', 'have', '"close relationship"'], ["node_211",
#                                                                                     "node_578",
#                                                                                     "node_391",
#                                                                                     "node_686",
#                                                                                     "node_499"]
#                         )


# chat_node = ConceptNode('node_875', 875, 5, "chat", 0,
#                        "2023-02-13 11:22:40", None, 
#                        "Isabella Rodriguez", "chat with", "Maria Lopez", 
#                        "conversing about preparations for the Valentine's Day party at Hobbs Cafe including decorations and snacks, with Isabella Rodriguez and Maria Lopez", "conversing about preparations for the Valentine's Day party at Hobbs Cafe including decorations and snacks, with Isabella Rodriguez and Maria Lopez", 
#                        6, ["Isabella Rodriguez", "Maria Lopez"], [[
#         "Isabella Rodriguez",
#         "Hey Maria! I'm so glad I ran into you. I wanted to remind you about the Valentine's Day party at Hobbs Cafe tomorrow. It's going to be so much fun! You should definitely come and bring your friends. It starts at 5pm and goes until 7pm. I really hope to see you there!"
#       ],
#       [
#         "Maria Lopez",
#         "Hey Isabella, thanks for the reminder! I wouldn't miss the Valentine's Day party for anything. I'll definitely be there with Klaus. Can I help with anything for the party?"
#       ],
#       [
#         "Isabella Rodriguez",
#         "Thank you so much, Maria! I would really appreciate it if you could help with setting up decorations for the party. Could you bring some balloons and streamers?"
#       ],
#       [
#         "Maria Lopez",
#         "Sure, I can bring balloons and streamers for the party. Just let me know what colors you prefer."
#       ],
#       [
#         "Isabella Rodriguez",
#         "Thank you so much, Maria! I think red and pink would be great colors for the balloons and streamers. That would fit perfectly with the Valentine's Day theme. Can you also bring some heart-shaped decorations? I think that would add a nice touch."
#       ],
#       [
#         "Maria Lopez",
#         "Of course! I can bring some heart-shaped decorations too. Is there anything else you need help with?"
#       ],
#       [
#         "Isabella Rodriguez",
#         "Thank you so much, Maria! I really appreciate your help. Just one more thing, could you also bring some Valentine's Day-themed snacks for the party? That would be a great addition!"
#       ],
#       [
#         "Maria Lopez",
#         "Sure, I can bring some Valentine's Day-themed snacks for the party. Just let me know what kind of snacks you would like."
#       ],
#       [
#         "Isabella Rodriguez",
#         "Thank you, Maria! I think some heart-shaped cookies and chocolate-covered strawberries would be perfect for the Valentine's Day-themed snacks. Can you bring those?"
#       ],
#       [
#         "Maria Lopez",
#         "Sure, I can bring heart-shaped cookies and chocolate-covered strawberries for the party."
#       ],
#       [
#         "Isabella Rodriguez",
#         "That sounds perfect! Thank you so much, Maria. Your help is greatly appreciated. See you at the party tomorrow!"
#       ],
#       [
#         "Maria Lopez",
#         "Is there a specific time you would like me to arrive to help with the decorations?"
#       ],
#       [
#         "Isabella Rodriguez",
#         "Could you please arrive at the cafe around 4pm tomorrow before the party starts? That way we'll have enough time to set up the decorations together."
#       ],
#       [
#         "Maria Lopez",
#         "Sounds good, Isabella! I'll make sure to arrive at Hobbs Cafe tomorrow at 4pm to help with the decorations. See you then!"
#       ],
#       [
#         "Isabella Rodriguez",
#         "Thank you, Maria! I'm really looking forward to seeing you and Klaus at the party tomorrow. It's going to be a great time!"
#       ],
#       [
#         "Maria Lopez",
#         "Is there anything else I can do to help with the party?"
#       ]])



# persona = Persona('Isabella Rodriguez', 'main/data/personas/Isabella Rodriguez')
# print(persona)
# rel_mem = retrieve(persona, [idle_node, chat_node, thought_node])
# print(rel_mem)
# rel_mem = new_retrieve(persona=persona, focal_points=['la Rodriguez and Maria Lopez have a close relationship', "conversing about preparations for the Valentine's Day party at Hobbs Cafe including decorations and snacks, with Isabella Rodriguez and Maria Lopez"])
# print(rel_mem)
# # with open('relative_memory.json', 'w') as f:
# #     f.write(t)


# a = ('a', 'b', 'c')

# a = ('a', *a[:])

# print('end')

from persona.prompt_template.gpt_structure import *


instruct = 'Here is some basic information about Isabella Rodriguez.\nName: Isabella Rodriguez\nAge: 34\nInnate traits: friendly, outgoing, hospitable\nLearned traits: Isabella Rodriguez is a cafe owner of Hobbs Cafe who loves to make people feel welcome. She is always looking for ways to make the cafe a place where people can come to relax and enjoy themselves.\nCurrently: Isabella Rodriguez is planning on having a Valentine\'s Day party at Hobbs Cafe with her customers on February 14th, 2023 at 5pm. She is gathering party material, and is telling everyone to join the party at Hobbs Cafe on February 14th, 2023, from 5pm to 7pm.\nLifestyle: Isabella Rodriguez goes to bed around 11pm, awakes up around 6am.\nDaily plan requirement: Isabella Rodriguez opens Hobbs Cafe at 8am everyday, and works at the counter until 8pm, at which point she closes the cafe.\nCurrent Date: Monday February 13\n\n\n=== \nFollowing is a conversation between Isabella Rodriguez and Interviewer. \n'
instruct += '''
output의 예시 입니다.

input: "오늘은 어떤 물건들이 인기가 있나요?"

output: "인기있는 건 역시 기름이지!"

input: "이 상점은 언제부터 영업을 시작한 건가요?"

output: "언제였던가... 내가 이 일을 시작했던게 ... 그게 그렇게 궁금한가?"

input: "어떤 물건이 가장 잘 팔리나요?"

output: "꾸준히 팔리는 건 역시 향수지!"

input: "어떤 물건이 이번 주에 새로 들어왔나요?"

output: "오늘은 서쪽나라의 종이, 붓, 펜이 들어왔다네. 한 번 보겠나?"

input: "여기 있는 물건들 중에서 추천해 줄 만한 것이 있나요?"

output: "둘 다 사는 것 어떠한가! (웃음)
'''
prompt = '너의 이름은 뭐니 ?'

instruct = '''
Here is some basic information about Isabella Rodriguez. Name: Isabella Rodriguez Age: 34 Innate traits: Impatient, low-class, and bad-tempered Learned traits: Isabella Rodriguez is a cafe owner of Hobbs Cafe who loves to make people feel welcome. She is always looking for ways to make the cafe a place where people can come to relax and enjoy themselves. Currently: Isabella Rodriguez is planning on having a Valentine's Day party at Hobbs Cafe with her customers on February 14th, 2023 at 5pm. She is gathering party material, and is telling everyone to join the party at Hobbs Cafe on February 14th, 2023, from 5pm to 7pm. Lifestyle: Isabella Rodriguez goes to bed around 11pm, awakes up around 6am. Daily plan requirement: Isabella Rodriguez opens Hobbs Cafe at 8am everyday, and works at the counter until 8pm, at which point she closes the cafe. Current Date: Monday February 13
아래의 말투로 말해주십시오.

질문: "오늘은 어떤 물건들이 인기가 있나요?"

답변: "인기있는 건 역시 기름이지!"

질문: "이 상점은 언제부터 영업을 시작한 건가요?"

답변: "언제였던가... 내가 이 일을 시작했던게 ... 그게 그렇게 궁금한가?"

질문: "어떤 물건이 가장 잘 팔리나요?"

답변: "꾸준히 팔리는 건 역시 향수지!"

질문: "어떤 물건이 이번 주에 새로 들어왔나요?"

답변: "오늘은 서쪽나라의 종이, 붓, 펜이 들어왔다네. 한 번 보겠나?"

질문: "여기 있는 물건들 중에서 추천해 줄 만한 것이 있나요?"

답변: "둘 다 사는 것 어떠한가! (웃음)"
'''
output = GPT_chat(instruct, prompt)

print(output)