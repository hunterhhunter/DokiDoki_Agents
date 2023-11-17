# 서버 1
from flask import Flask, request, jsonify
import requests

from game_start import *

app = Flask(__name__)

@app.route('/send_to_cloud', methods=['GET'])
def send_to_cloud_requests():
    # 이제 여기서는 5002 포트의 서버에 요청을 보냅니다.
    data = '게임에 정보 전달'
    response = requests.post('http://127.0.0.1:5002/receive_from_cloud')
    return response.text

@app.route('/game_start_python')
def game_start():
    print('게임이 시작됨을 python에 알립니다.')
    game_start()
    return True

@app.route('/persona_start')
def persona_start():
    persona_start()

@app.route('/receive_from_local', methods=['GET'])
def receive_from_local():
    return "톰의 현재 주위에는 돌, 나무, 상자가 있음"

if __name__ == '__main__':
    app.run(debug=True, port=5001)