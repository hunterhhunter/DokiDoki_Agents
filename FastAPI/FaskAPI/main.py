import sys
sys.path.insert(0, 'C:/FastAPI')

from fastapi import FastAPI
from FaskAPI.Router.test_nuri import test_nuri_2

app = FastAPI()

app.include_router(test_nuri_2)

@app.get('/')
def read_results():
    return {'msg': 'Main'}

@app.get('/gogo')
def gogo():
    return {
  "executions": [
    {
      "Sub": "Emerald Puyor",
      "P": "chat with",
      "Obj": "Franz Alez",
      "location": "Franz Alez",
      "duration": 2,
      "chat": [
        "Emerald Puyor",
        " 안녕, Franz! \n낚시 대회 준비는 어떻게 진행되고 있는 것 같아?",
        "Franz Alez",
        " 안녕, Emerald! \n낚시 대회 준비는 잘 진행되고 있어. \n난 전단지를 붙이며 모두를 초대하고 있어.\n 너는 어때? \n너의 상점에서는 모든 게 어떻게 진행되고 있나?",
        "Emerald Puyor",
        "상점은 모든 게 맞아 떨어지지. \n너의 낚시 대회에 참가하려면 \n언제, 어디로 가야하니?",
        "Franz Alez",
        "듣던 중 반가운 말이군! \n내일 아침 10시에 마을의 강변에서\n 대회를 진행할 예정이야. \n내가 심사위원이지.",
        "Franz Alez",
        "오 좋은 아침이군! 영진 자네도 낚시 대회에 참여할 건가?"
      ]
    },
    {
      "Sub": "Franz Alez",
      "P": "chat with",
      "Obj": "Emerald Puyor",
      "location": "Emerald Puyor",
      "duration": 2,
      "chat": None
    }
  ]
}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host='192.168.80.116', port=8000)

# February 14, 2023, 00:00:00
# February 14, 2023, 08:21:00
# February 14, 2023, 09:31:00
