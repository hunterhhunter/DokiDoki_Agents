"""입력 데이터 검증하기 pydantic - BaseModel"""

# from typing import List, Union, Optional
# from datetime import datetime
# from pydantic import BaseModel

# class Movie(BaseModel):
#     mid: int
#     genre: str
#     rate: Union[int, float]
#     tag: Optional[str] = None
#     date: Optional[datetime] = None
#     some_variable_list:List[int] = []


# tmp_data = {
#     'mid' : '1',
#     'genre' : 'action',
#     'rate' : 1.5,
#     'tag' : None,
#     'date' : '2023-01-03 19:12:11'
# }

# tmp_movie = Movie(**tmp_data)
# print(tmp_movie)

"""데이터 범위 설정하기 pydantic Field"""

# from typing import List, Optional, Union
# from datetime import datetime

# from pydantic import BaseModel, Field

# class Movie(BaseModel):
#     mid: int
#     genre: str
#     rate: Union[int, float]
#     tag: Optional[str] = None
#     date: Optional[datetime] = None
#     some_variable_list:List[int] = []

# class User(BaseModel):
#     '''
#     gt : 설정된 값보다 큰 
#     ge : 설정된 값보다 크거나 같은
#     lt : 설정된 값보다 작은 
#     le : 설정된 값보다 작거나 같은
#     '''
#     uid: int
#     name: str = Field(min_length=2, max_length=7)
#     age: int = Field(gt=1, le=130)

# tmp_movie_data = {
#     'mid' : '1',
#     'genre' : 'action',
#     'rate' : '1.5',
#     'tag'  : None,
#     'date' : '2023-01-03 19:12:11'
# }

# tmp_user_data = {
#     'uid' : '100',
#     'name' : 'soojin',
#     'age' : '16'
# }

# tmp_movie = Movie(**tmp_movie_data)
# tmp_user_data = User(**tmp_user_data)
# print(tmp_movie.model_dump_json())
# print(tmp_user_data.model_dump_json())

"""BaseSettings"""

# from typing import List, Optional, Union
# from datetime import datetime

# from pydantic_settings import BaseSettings
# from pydantic import Field
# from pydantic import validator

# class DBConfig(BaseSettings):
#     host: str = Field(default='127.0.0.1', env='db_host')
#     port: int = Field(default=3306, env='db_port')
    
#     class Config:
#         env_file = 'C:\FaskAPI\.env_ex'
        
#     @validator("port")
#     def check_port(cls, port_input):
#         if port_input not in [3306, 8080]:
#             raise ValueError("port error")
#         return port_input

# print(DBConfig().dict())

"""Pydantic 응용"""

# from typing import List, Optional, Union
# from datetime import datetime

# from pydantic_settings import BaseSettings
# from pydantic import Field, BaseModel
# from pydantic import validator

# class DBConfig(BaseSettings):
#     host: str = Field(default='127.0.0.1', env='db_host')
#     port: int = Field(default=3306, env='db_port')

#     class Config:
#         env_file = 'C:/FaskAPI/.env_ex'

#     @validator("host", pre=True)
#     def check_host(cls, host_input):
#         if host_input == 'localhost':
#             return "127.0.0.1"
#         return host_input
    
#     @validator("port")
#     def check_port(cls, port_input):
#         if port_input not in [3306, 8080]:
#             raise ValueError("port error")
#         return port_input
    
# class ProjectConfig(BaseModel):
#     project_name: str = 'youngjin'
#     db_info: DBConfig = DBConfig()


# data = {
#     'project_name': '영진의 프로젝트',
#     'db_info': {
#         'host': 'localhost',
#         'port' : 3306
#     }
# }

# my_pjt = ProjectConfig(**data)
# print(my_pjt.dict())
# print(my_pjt.db_info)

"""FastAPI와 Pydantic"""

from typing import List, Optional, Union
from datetime import datetime
from fastapi import FastAPI
from pydantic_settings import BaseSettings
from pydantic import Field, BaseModel
from pydantic import validator

app = FastAPI()

class DataInput(BaseModel):
    name: str

class predictOutput(BaseModel):
    prob: float = 1.0
    prediction: int = 1

@app.post("/pydantic", response_model=predictOutput)
def pydantic_post(data_request: DataInput):
    name_length = len(data_request.name)
    calculated_prob = name_length * 0.1
    calculated_prediction = name_length % 2

    # 계산된 값을 사용하여 PredictOutput 인스턴스 생성 및 반환
    return predictOutput(prob=calculated_prob, prediction=calculated_prediction)