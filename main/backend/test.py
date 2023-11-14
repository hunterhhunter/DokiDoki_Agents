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


a = ('a', 'b', 'c')

a = ('a', *a[:])

print('end')