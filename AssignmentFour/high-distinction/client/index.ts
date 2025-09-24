import { PubSubClient } from "./auto-generated";

const client = new PubSubClient("http://localhost:3299");

client.connect().then(async () => {
  for (let i = 0; i < 10; i++) {
    client.publish("high_distinction.event", {
      id: "1",
      message: "Hello, world!",
      createdAt: new Date(),
    });
    await new Promise((resolve) => setTimeout(resolve, 1000));
  }
});

client.subscribe("high_distinction.event", (data) => {
  console.log(data);
});

client.disconnect();
