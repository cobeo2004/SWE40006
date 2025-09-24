/**
 * @description Get the config from the Jinja template, which is passed on the index.jinja template
 * @typedef {{client_id: number}} JinjaConfig
 * @type {JinjaConfig} config from getJinjaConfig()
 */
const jinjaConfig = getJinjaConfig();

const ws = new WebSocket(`ws://localhost:8000/chat/${jinjaConfig.client_id}`);

ws.onmessage = function (event) {
  const messages = document.getElementById("messages");
  const messageElement = document.createElement("div");
  messageElement.className =
    "bg-gray-100 p-3 rounded-lg mb-2 max-w-md break-words";
  messageElement.innerHTML = `<span class="text-sm text-gray-600">${new Date().toLocaleTimeString()}</span><br>${
    event.data
  }`;
  messages.appendChild(messageElement);
  messages.scrollTop = messages.scrollHeight;
};

ws.onopen = function () {
  console.log("WebSocket connected");
  const status = document.getElementById("connection-status");
  status.textContent = "Connected";
  status.className = "text-green-600 font-semibold";
};

ws.onclose = function () {
  const status = document.getElementById("connection-status");
  status.textContent = "Disconnected";
  status.className = "text-red-600 font-semibold";
};

ws.onerror = function () {
  const status = document.getElementById("connection-status");
  status.textContent = "Connection Error";
  status.className = "text-red-600 font-semibold";
};

function sendMessage(event) {
  event.preventDefault();
  const messageInput = document.getElementById("message");
  const message = messageInput.value.trim();
  if (message) {
    ws.send(message);
    messageInput.value = "";
  }
}

// Auto-focus message input when page loads
window.onload = function () {
  document.getElementById("message").focus();
};
