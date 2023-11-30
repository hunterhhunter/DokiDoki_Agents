
document.addEventListener("DOMContentLoaded", function() {
    var element = document.querySelector('.sector-chat-container');
    element.addEventListener('mouseenter', function() {
    // 스크롤이 있는지 확인
    if (element.scrollHeight > element.clientHeight) {
        element.style.overflowY = 'scroll';
    } else {
        element.style.overflowY = 'hidden';
    }
    });

    element.addEventListener('mouseleave', function() {
    element.style.overflowY = 'hidden';
    });
});

function addUserChat(text) {
    var parentElement = document.querySelector('.sector-chat-content');
    var newChildElement = document.createElement('div');
    newChildElement.className = 'sector-chat-content-user';
    var newChildElement2 = document.createElement('div');
    newChildElement2.className = 'sector-chat-content-user-converse inline-block converse-box';
    newChildElement2.textContent = text

    newChildElement.appendChild(newChildElement2);
    parentElement.appendChild(newChildElement)
}

function addNPCChat(text) {
    var parentElement = document.querySelector('.sector-chat-content');
    var wrapImage = document.createElement('div');
    wrapImage.className = "sector-chat-content-wrap-image inline-block";
    var shotimage = document.createElement('div');
    shotimage.className = "sector-chat-content-npc-shotimage";
    wrapImage.appendChild(shotimage)
    
    var newChildElement = document.createElement('div');
    newChildElement.className = 'sector-chat-content-npc';
    var newChildElement2 = document.createElement('div');
    newChildElement2.className = 'sector-chat-content-npc-converse inline-block converse-box';
    newChildElement2.textContent = text

    newChildElement.appendChild(wrapImage);
    newChildElement.appendChild(newChildElement2);
    
    parentElement.appendChild(newChildElement)
}

var chats = []

document.querySelector('.chat-input-text').addEventListener('keypress', function (e) {
    
    if (e.key === 'Enter') {
        this.disabled = true;
        // persona = 'Isabella Rodriguez'
        persona = 'Franz Alez'
        userChat = this.value
        addUserChat(userChat)
        chats.push(['User', userChat])
        this.value = ''
        e.preventDefault();
        console.log(chats)
        dataToSend = {
            'persona' : persona,
            'convo' : chats
        }
        fetch('http://localhost:1177/submit', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(dataToSend)
        })
        .then(response => response.text())
        .then(data => {
            addNPCChat(data)
            chats.push([persona, data])
            this.disabled = false;
        })
        .catch(error => {
            console.error('Error:', error);
            this.disabled = false;
        });
        
    }
});