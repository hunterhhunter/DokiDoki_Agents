const express = require('express');
const bodyParser = require('body-parser');
const { spawn } = require('child_process');
const path = require('path');
const app = express();
const cors = require('cors');

app.use(cors());
const port = 1177;
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());
app.use(express.static(path.join(__dirname, '../frontend')));
// app.use(express.static(path.join(__dirname, '../backend')));

// 루트 경로에 대한 GET 요청 처리
app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, '../frontend/index.html')); // 'main.html' 파일이 있는지 확인하세요.
});

// '/submit' 경로에 대한 POST 요청 처리
app.post('/submit', (req, res) => {
    const pythonProcess = spawn('python', ['../backend/chat.py', JSON.stringify(req.body)], { encoding: 'utf-8' });

    pythonProcess.stdout.on('data', (data) => {
        text = data.toString().split('\n')
        
        const lastElement = text[text.length - 2];
        // lastElement = Buffer.from(lastElement, 'binary').toString('euc-kr');
        // const lastElement2 = text[text.length - 1];
        console.log(lastElement)
        // console.log(lastElement2)
        // res.setHeader('Content-Type', 'text/html; charset=utf-8')
        res.send(lastElement);
    });

    pythonProcess.stderr.on('data', (data) => {
        console.error(`stderr: ${data}`);
    });
});

app.listen(port, () => {
    console.log(`Server running on port ${port}`);
});
