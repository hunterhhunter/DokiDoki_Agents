from fastapi import FastAPI
from pydantic import BaseModel
from pydantic import Field
import asyncio
import time

# app = FastAPI()

# class DataInput(BaseModel):
# 	"""json을 파싱하여 딕셔너리로 저장하는 BaseModel을 상속받음"""
# 	name:str

# @app.get("/")
# def home():
#     return {"Hello": "GET"}

# @app.post("/")
# def home_post(data_request: DataInput): # 위에서 선언한 클래스를 입력값으로 받음
# 	return {"Hello": "POST", "msg" : data_request.name}
# 	# 입력값의 정보를 출력함

#========================================
# async 비동기함수
#========================

async def some_library(num: int, something:str):
    s = 0
    for i in range(num):
        print(" something.. : ", something, i)
        await asyncio.sleep(1)
        s += int(something)
    return s

app = FastAPI()

@app.post('/')
async def read_results(something:str):
    s1 = await some_library(5, something)
    return {'data' : 'data', 's1':s1}