from fastapi import FastAPI
from Router.text_pre import text_preprocessing
from Router.conv import conver
app = FastAPI()

app.include_router(text_preprocessing)
app.include_router(conver)

@app.get('/')
def read_results():
    return {'msg': 'Main'}


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host='192.168.80.2', port=8000)