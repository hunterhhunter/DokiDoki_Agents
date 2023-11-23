import requests
import json

class UN_Data():
    '''
    json으로 정보 교환.
    '''
    def __init__(self, json_data) -> None:
        self.persona = json_data['persona']
        self.location = json_data['location']
        self.object = json_data['object']

    def get_location(self):
        return self.location

    def get_persona(self):
        return self.persona

    def get_object(self):
        return self.object
    


def request():
    import requests

    url = 'http://192.168.80.2:8000/text/process/'

    params = {
        'user_name' : 'Franz Alez',
        'user_password' : 'Franz',
        'user_text' : 'HelloWorld',
        'user_id' : 123
    }

    res = requests.post(url, json=params)

    print(res.json())

def response():
    import requests

    url = 'http://192.168.80.2:8000/text/process/'

    params = {
        'user_name' : 'Franz Alez',
        'user_password' : 'Franz',
        'user_text' : 'HelloWorld',
        'user_id' : 123
    }

    res = requests.post(url, json=params)

    print(res.json())