// script.js
function sendMessage() {
    console.log('check')
    const messageInput = $('#message-input');
    const messageText = messageInput.val();
    if (messageText.trim() !== '') {
      const chatMessages = $('#chat-messages');
      const newMessage = `
        <div class="message">
          <span class="user">You:</span>
          <span class="text">${messageText}</span>
        </div>
      `;
      chatMessages.append(newMessage);
      messageInput.val('');
      // Optionally, you can add logic to send the message to a server or perform other actions.
    }
  }
  