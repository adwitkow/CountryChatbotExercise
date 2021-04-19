// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

$(document).ready(function () {
    var lastElement = null;

    var conversationId = null
    var $userMessage = $('#user-message');
    var $messageHistory = $('#message-history')
    $userMessage.keyup(function (e) {
        if (e.keyCode == 13) {
            var content = $userMessage.val().trim();

            if (content.length == 0) {
                return false;
            }

            var userMessageData = {
                conversationId: conversationId,
                messageType: 0,
                body: $userMessage.val(),
                timestamp: new Date().toISOString()
            }

            $userMessage.get(0).value = '';

            createMessageElement(userMessageData);

            var tempElement = createMessageElement({
                messageType: 1,
                body: '...',
                timestamp: new Date()
            });

            $.ajax({
                type: 'POST',
                url: '/api/chat/PostMessage',
                data: JSON.stringify(userMessageData),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (data) {
                    conversationId = data.conversationId;
                    tempElement.remove();
                    createMessageElement(data);
                }
            });
        }
    })

    function createMessageElement(messageData) {
        var li = document.createElement('li');
        li.className = 'clearfix';

        var bubble = document.createElement('div');
        if (messageData.messageType == 0) {
            bubble.className = 'bubble left'
        } else {
            bubble.className = 'bubble right'
        }

        var content = document.createElement('div')
        content.className = 'bubble-content';
        content.textContent = messageData.body;

        var timestamp = document.createElement('div')
        timestamp.className = 'bubble-timestamp';
        var parsedDate = new Date(messageData.timestamp)
        timestamp.textContent = parsedDate.toLocaleTimeString();

        li.appendChild(bubble);
        bubble.appendChild(content);
        bubble.appendChild(timestamp);

        var firstChild = $messageHistory.get(0).firstElementChild;
        if (firstChild == null) {
            $messageHistory.append(li);
        } else {
            $messageHistory.get(0).insertBefore(li, $messageHistory.get(0).children[0]);
        }

        return li;
    }
})