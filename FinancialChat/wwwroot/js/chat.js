"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", { accessTokenFactory: () => "BotToken" })
    .build();

//Disable the user selection until connection is established.
document.getElementById("usersSelect").disabled = true;

connection.on("ReceiveMessage", function (user, message, dateTime) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.textContent = `${dateTime} - ${user}: ${message}`;
});

connection.start().then(function () {
    document.getElementById("usersSelect").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("usersSelect").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message, null).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function getMessages(user) {
    if (user == 0) {
        document.getElementById("sendButton").disabled = true;
        document.getElementById("messagesList").innerHTML = "";
    }
    else {
        document.getElementById("sendButton").disabled = false;
        $.ajax({
            type: "GET",
            url: "/Chat?handler=messages",
            data: { "user": user },
            dataType: "json",
            success: function (result) {
                console.log(result);
                document.getElementById("messagesList").innerHTML = "";
                result.forEach(x => {
                    var li = document.createElement("li");
                    document.getElementById("messagesList").appendChild(li);
                    li.textContent = `${new Date(x.dateTime).toTimeString().split(' ')[0]} - ${x.userFrom}: ${x.message}`;
                });
            },
            error: function (data) {
                alert(data);
            }
        });
    }
}