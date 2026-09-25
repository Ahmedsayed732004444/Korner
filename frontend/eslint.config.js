// @ts-check
import js from "@eslint/js";
import importX from "eslint-plugin-import-x";
import reactHooks from "eslint-plugin-react-hooks";
import tseslint from "typescript-eslint";
import eslintConfigPrettier from "eslint-config-prettier";

/**
 * Bans physical CSS direction utilities so RTL (Arabic) never breaks.
 * See CLAUDE.md rule 8: use ms-/me-/ps-/pe-/start-/end-/text-start only.
 */
const bannedDirectionalClassPattern =
  /(^|\s)(ml|mr|pl|pr|left|right|text-left|text-right)-[\w[\]./%-]+/;

/** @type {import("eslint").Rule.RuleModule} */
const noPhysicalDirectionClasses = {
  meta: {
    type: "problem",
    docs: {
      description:
        "Disallow physical CSS direction utilities (ml-/mr-/pl-/pr-/left-/right-); use logical ms-/me-/ps-/pe-/start-/end- instead.",
    },
    schema: [],
    messages: {
      physicalClass:
        "Use logical Tailwind utilities (ms-/me-/ps-/pe-/start-/end-/text-start) instead of physical '{{match}}' — the storefront must work in RTL.",
    },
  },
  create(context) {
    function checkValue(node, raw) {
      if (typeof raw !== "string") return;
      const match = raw.match(bannedDirectionalClassPattern);
      if (match) {
        context.report({ node, messageId: "physicalClass", data: { match: match[0].trim() } });
      }
    }

    return {
      JSXAttribute(node) {
        if (node.name.type !== "JSXIdentifier") return;
        if (node.name.name !== "className" && node.name.name !== "class") return;
        if (node.value?.type === "Literal") {
          checkValue(node.value, node.value.value);
        } else if (node.value?.type === "JSXExpressionContainer") {
          const expr = node.value.expression;
          if (expr.type === "Literal") checkValue(expr, expr.value);
          if (expr.type === "TemplateLiteral") {
            for (const quasi of expr.quasis) checkValue(quasi, quasi.value.raw);
          }
        }
      },
      Literal(node) {
        // Catches clsx/cva class strings passed as plain string arguments.
        if (node.parent?.type === "JSXAttribute") return; // already handled above
        checkValue(node, node.value);
      },
    };
  },
};

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
              from: ["./app/routes", "./app/features", "./app/shared", "./app/components"],
              message: "lib/ may not import from routes/, features/, shared/ or components/.",
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
