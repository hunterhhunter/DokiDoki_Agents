# 서버 1
from flask import Flask, request, jsonify
import requests

app = Flask(__name__)

@app.route('/send_to_cloud', methods=['GET'])
def send_to_cloud_requests():
    # 이제 여기서는 5002 포트의 서버에 요청을 보냅니다.
    data = '게임 시작'
    response = requests.post('http://127.0.0.1:5002/receive_from_cloud')
    return response.text


@app.route('/receive_from_local', methods=['GET'])
def receive_from_local():
    return ""


@app.route('/receive_from_local_test', methos=['GET'])
def receive_from_local_test():
    print('안녕하세요.')

if __name__ == '__main__':
    app.run(debug=True, port=5001)