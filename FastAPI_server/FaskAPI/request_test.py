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