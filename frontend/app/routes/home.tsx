import type { Route } from "./+types/home";

// Reads the raw env var directly for now; lib/env.server.ts (Zod-validated) lands in T2.3.
export async function loader() {
  const apiUrl = process.env.API_INTERNAL_URL ?? "http://localhost:5080";

  try {
    const response = await fetch(`${apiUrl}/health`);
    return { apiStatus: response.ok ? ("ok" as const) : ("error" as const) };
  } catch {
    return { apiStatus: "unreachable" as const };
  }
}

export const meta: Route.MetaFunction = () => {
  return [{ title: "Korner" }, { name: "description", content: "Korner" }];
};

export default function Home({ loaderData }: Route.ComponentProps) {
  return (
    <main className="flex min-h-svh flex-col items-center justify-center gap-2 p-4">
      <h1 className="text-2xl font-semibold">Korner</h1>
      <p className="text-muted-foreground text-sm">API: {loaderData.apiStatus}</p>
    </main>
  );
}
