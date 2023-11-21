import sys
sys.path.insert(0, 'C:/FastAPI')

from fastapi import FastAPI
from FaskAPI.Router.test_nuri import test_nuri_2

app = FastAPI()

app.include_router(test_nuri_2)

@app.get('/')
def read_results():
    return {'msg': 'Main'}


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host='192.168.80.239', port=8000)

# February 14, 2023, 00:00:00