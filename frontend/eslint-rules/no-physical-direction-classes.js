/**
 * Bans physical CSS direction utilities so RTL (Arabic) never breaks.
 * See CLAUDE.md rule 8: use ms-/me-/ps-/pe-/start-/end-/text-start only.
 *
 * `ml-/mr-/pl-/pr-/left-/right-` always take a value (e.g. `ml-4`, `left-0`);
 * `text-left`/`text-right` are complete utilities with no value. Either can
 * be preceded by variant prefixes (`md:`, `dark:`, `hover:`, ...).
 */
const variantPrefix = "(?:[a-z-]+:)*";
const bannedWithValue = "(?:ml|mr|pl|pr|left|right)-[\\w[\\]./%-]+";
const bannedStandalone = "(?:text-left|text-right)(?=$|\\s)";
export const bannedDirectionalClassPattern = new RegExp(
  `(?:^|\\s)${variantPrefix}(?:${bannedWithValue}|${bannedStandalone})`,
);

/** @type {import("eslint").Rule.RuleModule} */
export const noPhysicalDirectionClasses = {
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
        // Skip className="..." and className={"..."} — the JSXAttribute visitor above
        // already checked those; visiting them again here would double-report.
        const parent = node.parent;
        if (parent?.type === "JSXAttribute") return;
        if (parent?.type === "JSXExpressionContainer" && parent.parent?.type === "JSXAttribute")
          return;
        checkValue(node, node.value);
      },
    };
  },
};
