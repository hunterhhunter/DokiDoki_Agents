from fastapi import APIRouter
from packages.config import DataInput, ProcessedText


text_preprocessing = APIRouter(prefix='/text')

@text_preprocessing.get('/', tags=['text_pre'])
async def start_text():
    return {'msg': "started text_preprocessing"}

@text_preprocessing.post('/process', tags=['text_pre'], response_model=ProcessedText)
async def text_processed(data_request: DataInput):
    user_name = data_request.user_name
    user_password = data_request.user_password
    user_text = data_request.user_text
    user_id = data_request.user_id

    user_name = '반갑습니다. ' + user_name
    user_password = '당신의 비밀번호는 ' + user_password
    user_text = '당신이 입력한 텍스트는 ' + user_text
    user_id = user_id + 10

    return {'user_name': user_name, 'user_password': user_password, 'user_text': user_text, 'user_id': user_id}