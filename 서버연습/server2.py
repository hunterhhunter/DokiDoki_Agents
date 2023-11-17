from flask import Flask, request, jsonify
import requests

app = Flask(__name__)

@app.route('/send_to_local', methods=['GET'])
def send_to_local():
    # 이제 여기서는 5001 포트의 서버에 요청을 보냅니다.
    response = requests.get('http://127.0.0.1:5001/receive_from_local')
    return response.text

@app.route('/receive_from_cloud', methods=['GET'])
def receive_from_cloud():
    
    return "HI Local"

if __name__ == '__main__':
    app.run(debug=True, port=5002)