// frontend/ has no .git of its own (the monorepo's .git is one level up), and
// this also runs during Docker builds where .git isn't in the build context
// at all — so this only wires up the hook when a repo is actually present.
import { existsSync } from "node:fs";
import { execSync } from "node:child_process";
import { resolve, dirname } from "node:path";
import { fileURLToPath } from "node:url";

const frontendDir = dirname(dirname(fileURLToPath(import.meta.url)));
const repoRoot = resolve(frontendDir, "..");

if (existsSync(resolve(repoRoot, ".git"))) {
  execSync("git config core.hooksPath frontend/.husky", { cwd: repoRoot, stdio: "inherit" });
}
