"""

"""
import os

current_file_path = os.path.abspath(__file__)
current_file_dir = os.path.dirname(current_file_path)
os.chdir(current_file_dir)

import sys
sys.path.append('../../')

import json
import datetime

from global_methods import *

from persona.cognitive_modules.retrieve import *

from persona.prompt_template.run_gpt_prompt import *

# 모든 상황은 event다
# action, chat, thought

class ConceptNode:
    def __init__(self,
                 node_id, node_count, event_type,
                 s, p, o, created, expiration,
                 description, thought, embedding_key, 
                 curr_chat, poignancy):
        
        self.node_id = node_id
        self.node_count = node_count
        # action, chat, thought
        self.event_type = event_type

        self.created = created
        self.expiration = expiration
        self.last_accessed = self.created
        
        """
        action 일 경우
            s = 주체
            p = 행동
            o = 대상
        
        chat 일 경우
            s = 주체
            p = chat with
            o = 대상
        """
        self.subject = s
        self.predicate = p
        self.object = o

        self.spo = (s, p, o)

        """
        실제 내용
        action 일 경우
            행동 내용
        chat 일 경우
            대화 요약 내용
        """
        self.description = description
        self.thought = thought

        self.embedding_key = embedding_key

        # 실제 대화 내용
        self.curr_chat = curr_chat
        # 해당 행동에 대해서 perosna의 생각을 저장
        self.poignancy = poignancy

    def spo_summary(self):
        return (self.subject, self.predicate, self.object)


class AssociativeMemory:
    def __init__(self, f_saved):
        self.id_to_node = dict()
        self.f_saved = f_saved

        self.seq_event = []
        self.seq_thought = []
        self.curr_chat = []

        self.relationship = json.load(open(self.f_saved+ "/relationship.json"))
        self.embeddings = json.load(open(self.f_saved + "/embeddings.json"))

        nodes_load = json.load(open(f_saved + "/nodes.json"))
        for count in range(len(nodes_load.keys())):
            node_id = f"node_{str(count+1)}"
            node_details = nodes_load[node_id]
            
            node_count = node_details['node_count']
            event_type = node_details["event_type"]

            created = datetime.datetime.strptime(node_details["created"], 
                                           '%Y-%m-%d %H:%M:%S')
            expiration = None
            if node_details["expiration"]: 
                expiration = datetime.datetime.strptime(node_details["expiration"],'%Y-%m-%d %H:%M:%S')           

            s = node_details["subject"]
            p = node_details["predicate"]
            o = node_details["object"]

            description = node_details["description"]
            thought = node_details["thought"]

            embedding_pair = (node_details["embedding_key"], 
                        self.embeddings[node_details["embedding_key"]])
            
            if event_type=='chat':
                curr_chat = node_details['curr_chat']
            else:
                curr_chat = 'null'
            poignancy = node_details["poignancy"]


            if event_type == "action": 
                self.add_action(s, p, o, 
                        description, thought, embedding_pair,
                        curr_chat, poignancy)
            elif event_type == "chat": 
                self.add_chat(s, p, o, 
                        description, thought, embedding_pair,
                        curr_chat, poignancy)   
            # TODO
            # description 과 thought 를 분리한 후, 저장
            # 이후 retrieve 에서 관련된 사항을 불러올 때, thought와 그와 관련된 컨텍스트에 해당되는 description 를 불러오지 않아도 되나? 에 대한 의문 해결 할것
            # 혹은 따로 불러와서 관계가 있으면 관계를 넣어서 embedding을 저장하는 방법도 고려할 것
            # elif event_type == "thought":
            #     self.add_thought(s, p, o,
            #             description, thought, embedding_pair,
            #             curr_chat, poignancy)      


    def save(self, out_json):
        r = dict()
        for count in range(len(self.id_to_node.keys()), 0, -1):
            node_id = f"node_{str(count)}"
            node= self.id_to_node[node_id]

            r[node_id] = dict()
            r[node_id]["node_count"] = node.node_count
            r[node_id]["event_type"] = node.type 

            r[node_id]["created"] = node.created.strftime(
                '%Y-%m-%d %H:%M:%S')
            r[node_id]["expiration"] = None
            if node.expiration: 
                r[node_id]["expiration"] = (node.expiration
                                        .strftime('%Y-%m-%d %H:%M:%S'))

            r[node_id]["subject"] = node.subject
            r[node_id]["predicate"] = node.predicate
            r[node_id]["object"] = node.object

            r[node_id]["description"] = node.description
            r[node_id]["embedding_key"] = node.embedding_key

            r[node_id]["curr_chat"] = node.curr_chat
            r[node_id]["thought"] = node.thought
            r[node_id]["poignancy"] = node.poignancy

        with open(out_json+"/nodes.json", "w") as outfile:
            json.dump(r, outfile)

        with open(out_json+"/embeddings.json", "w") as outfile:
            json.dump(self.embeddings, outfile)                


    def add_action(self, s, p, o,
                   description, thought, embedding_pair,
                   curr_chat, poignancy):
        
        node_count = len(self.id_to_node.keys())+1
        event_type = "action"
        node_id = f"node_{str(node_count)}"

        embedding_pair[0] = description +' '+ thought

        node = ConceptNode(node_id, node_count, event_type,
                           s, p, o,
                           description, embedding_pair[0], 
                           curr_chat, thought, poignancy)
        self.id_to_node[node_id] = node 
        self.embeddings[embedding_pair[0]] = embedding_pair[1]

        return node
        

    def add_chat(self, s, p, o,
                   description, thought, embedding_pair,
                   curr_chat, poignancy):
        
        node_count = len(self.id_to_node.keys())+1
        event_type = "chat"
        node_id = f"node_{str(node_count)}"

        embedding_pair[0] = description +' '+ thought

        node = ConceptNode(node_id, node_count, event_type,
                           s, p, o,
                           description, embedding_pair[0],
                           curr_chat, thought, poignancy)
        self.id_to_node[node_id] = node 
        self.embeddings[embedding_pair[0]] = embedding_pair[1] 

        return node              


    def add_curr_chat(self, chat, end):
        chat_clean = f"{chat[0]}: {chat[1]}"
        self.curr_chat += [chat_clean]
        if end==True:
            self.add_chat()


    def add_chat_without_thought(self, s, p, o,
                    description, embedding_pair,
                    chat, poignancy):

        node_count = len(self.id_to_node.keys()) + 1
        type_count = len(self.seq_event) + 1
        event_type = "chat"
        node_id = f"node_{str(node_count)}"

        node = ConceptNode(node_id, node_count, type_count, event_type,
                    s, p, o, 
                    description, embedding_pair[0], poignancy)
            
        # chat 의 형태는 전체 chat이 들어가야 한다.
        # [["tom", "안녕 반가워"],["horalson":"메롱"]]
        if chat is not None:
            self.chat = chat
        self.embeddings[embedding_pair[0]] = embedding_pair[1]
        
        return node


    def load_relationship(self, target_persona):
        relationship = json.load(open(self.f_saved+ "/relationship.json"))
        print(relationship)
        target_relationship = relationship[target_persona.scratch.name]
        return target_relationship


    def save_relationship(self, init_persona, target_persona):
        # focal_points = [f"{target_persona.scratch.name}"]
        retrieved = new_retrieve(init_persona, [target_persona.scratch.name], 50)

        all_embedding_keys = list()
        for key, val in retrieved.items():
            for i in val:
                all_embedding_keys += [i.embedding_key]
            all_embedding_key_str = ""
            for i in all_embedding_keys:
                all_embedding_key_str += f"{i}\n"
        
        summarized_relationship = run_gpt_prompt_agent_chat_summarize_relationship(
                              init_persona, target_persona,
                              all_embedding_key_str)[0]

        self.relationship['target_persona'] = summarized_relationship

        with open(self.f_saved+ "/relationship.json", "w") as outfile:
            json.dump(self.relationship, outfile)
        

    def load_summarize(self, persona):
        daily_summarize = json.load(open(self.f_saved+ "/daily_summarize.json"))

    def save_summarizer(self, perosna):
        
