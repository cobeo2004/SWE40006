import {
  PubSubServer,
  createUniversalServer,
  type PubSubServerConfig,
} from "@cobeo2004/proof-sub";

const config = {
  schemaPath: "./proofsub/schema.ts",
  port: 3299,
} satisfies PubSubServerConfig;

const pubSub = new PubSubServer(config);

// Create a universal server
// This server will define which runtime to use and run the server based on the chosen runtime
createUniversalServer(pubSub);
