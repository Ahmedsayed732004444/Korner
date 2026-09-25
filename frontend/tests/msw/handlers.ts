import type { HttpHandler } from "msw";

// Feature tests add their own handlers via server.use(...); this stays empty
// until there's an API client to mock (lib/api/client.ts, task T2.3).
export const handlers: HttpHandler[] = [];
