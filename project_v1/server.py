import requests

url = 'http://192.168.80.2:8000/text/process/'

params = {
    'user_name' : '조영진',
    'user_password' : '영진2',
    'user_text' : '헬로우영진',
    'user_id' : '영진123'
}

res = requests.post(url, json=params)

print(res.json())