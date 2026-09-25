import { RuleTester } from "eslint";
import { noPhysicalDirectionClasses } from "./no-physical-direction-classes.js";

const ruleTester = new RuleTester({
  languageOptions: {
    ecmaVersion: 2022,
    sourceType: "module",
    parserOptions: { ecmaFeatures: { jsx: true } },
  },
});

// RuleTester drives Mocha-style describe/it itself; it must run at the top
// level, not inside a test()/it() callback.
ruleTester.run("no-physical-direction-classes", noPhysicalDirectionClasses, {
  valid: [
    `<div className="flex ms-2 me-4 items-center" />`,
    `<div className="ps-2 pe-4" />`,
    `<div className="start-0 end-0" />`,
    `<p className="text-start" />`,
    `<div className={cn("flex", isActive && "ms-2")} />`,
    // "left"/"right" only banned when they're a Tailwind spacing/position utility;
    // arbitrary words like "left" in a non-class string aren't flagged.
    `const label = "turn left here";`,
  ],
  invalid: [
    {
      code: `<div className="ml-2" />`,
      errors: [{ messageId: "physicalClass" }],
    },
    {
      code: `<div className={"ml-2"} />`,
      errors: [{ messageId: "physicalClass" }],
    },
    {
      code: `<div className="left-0" />`,
      errors: [{ messageId: "physicalClass" }],
    },
    // text-left/text-right have no value suffix, unlike ml-/mr-/pl-/pr-/left-/right-.
    {
      code: `<p className="text-left" />`,
      errors: [{ messageId: "physicalClass" }],
    },
    {
      code: `<p className="md:text-right" />`,
      errors: [{ messageId: "physicalClass" }],
    },
    {
      code: `<div className={cn("flex", isActive && "mr-4")} />`,
      errors: [{ messageId: "physicalClass" }],
    },
  ],
});
