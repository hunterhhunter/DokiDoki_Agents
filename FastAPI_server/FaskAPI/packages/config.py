from pydantic import Field, BaseModel, validator
from typing import List, Optional, Union
from fastapi import FastAPI

class DataInput(BaseModel):
    '''
    gt : 설정된 값보다 큰 
    ge : 설정된 값보다 크거나 같은
    lt : 설정된 값보다 작은 
    le : 설정된 값보다 작거나 같은
    '''
    user_name: str = Field(min_length=3, max_length=10)
    user_password: str = Field(min_length=3, max_length=10)
    user_text: str = Field(min_length=3, max_length=10)
    user_id: int = Field(ge=100, le=200)


class ProcessedText(BaseModel):
    user_name: str
    user_password: str
    user_text: str
    user_id: int