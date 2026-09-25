import type { Route } from "./+types/home";

export const meta: Route.MetaFunction = () => {
  return [{ title: "Korner" }, { name: "description", content: "Korner" }];
};

export default function Home() {
  return (
    <main className="flex min-h-svh items-center justify-center p-4">
      <h1 className="text-2xl font-semibold">Korner</h1>
    </main>
  );
}
