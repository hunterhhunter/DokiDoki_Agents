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
    return {"executions": [
    {
      "Sub": "Emerald Puyor",
      "P": "take",
      "Obj": "inventory",
      "location": "dokidoki village:Puyor's Store:supply store:pen",
      "duration": 10,
      "chat": None
    },
    {
      "Sub": "Franz Alez",
      "P": "make",
      "Obj": "sure",
      "location": "dokidoki village:Puyor's Store:supply store:pen",
      "duration": 10,
      "chat": [['Franz Alez', "Hey Emerald! How's your day going so far?"], ['Emerald Puyor', 'Hey Franz! My day is going well, thank you. How about you? Are you excited about the fishing tournament?']]
    }
  ]
}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host='192.168.80.116', port=8000)

# February 14, 2023, 00:00:00
# February 14, 2023, 08:21:00
# February 14, 2023, 09:31:00
