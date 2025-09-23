import { z } from "zod/v4";

// Define your event schemas here
// The server will watch this file for changes and automatically update clients
export const eventSchemas = {
  // Example event schema, you can add or remove events as you need
  "example.event": z.object({
    id: z.string(),
    name: z.string(),
    createdAt: z.coerce.date(),
  }),
  "high_distinction.event": z.object({
    id: z.string(),
    message: z.string(),
    createdAt: z.coerce.date(),
  }),
} as const;

// Export types for TypeScript inference
export type EventSchemas = typeof eventSchemas;
export type EventTypes = {
  [K in keyof EventSchemas]: z.infer<EventSchemas[K]>;
};
