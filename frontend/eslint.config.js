// @ts-check
import js from "@eslint/js";
import importX from "eslint-plugin-import-x";
import reactHooks from "eslint-plugin-react-hooks";
import tseslint from "typescript-eslint";
import eslintConfigPrettier from "eslint-config-prettier";
import { createTypeScriptImportResolver } from "eslint-import-resolver-typescript";
import { noPhysicalDirectionClasses } from "./eslint-rules/no-physical-direction-classes.js";

/** @type {import("eslint").ESLint.Plugin} */
const kornerPlugin = {
  rules: {
    "no-physical-direction-classes": noPhysicalDirectionClasses,
  },
};

export default tseslint.config(
  {
    ignores: [
      "build/**",
      ".react-router/**",
      "node_modules/**",
      "playwright-report/**",
      "test-results/**",
      "coverage/**",
    ],
  },
  js.configs.recommended,
  ...tseslint.configs.strict,
  ...tseslint.configs.stylistic,
  {
    plugins: {
      "import-x": importX,
      "react-hooks": reactHooks,
      korner: kornerPlugin,
    },
    settings: {
      "import-x/resolver-next": [createTypeScriptImportResolver()],
    },
    rules: {
      "no-console": "error",
      "korner/no-physical-direction-classes": "error",
      ...reactHooks.configs.recommended.rules,
      "import-x/no-restricted-paths": [
        "error",
        {
          zones: [
            {
              target: "./app/lib",
              from: ["./app/routes", "./app/features", "./app/shared"],
              message: "lib/ may not import from routes/, features/ or shared/.",
            },
            {
              target: "./app/shared",
              from: ["./app/routes", "./app/features"],
              message: "shared/ may not import from routes/ or features/.",
            },
            {
              target: "./app/features",
              from: ["./app/routes"],
              message: "features/ may not import from routes/.",
            },
            {
              target: "./app/features",
              from: ["./app/features"],
              except: ["index.ts", "index.tsx"],
              message: "A feature may only import another feature through its index.ts.",
            },
          ],
        },
      ],
    },
  },
  eslintConfigPrettier,
);
