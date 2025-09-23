import { PubSubServer, createUniversalServer } from "@cobeo2004/proof-sub";

const pubSub = new PubSubServer({
  schemaPath: "./proofsub/schema.ts",
  port: 3299,
});

createUniversalServer(pubSub);
