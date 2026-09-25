import { test, expect } from "@playwright/test";

// Real E2E flows (browse → cart → checkout → payment) land with the features
// they cover, starting M2. This proves the Playwright setup itself works.
test("browser launches and can navigate", async ({ page }) => {
  await page.goto("about:blank");

  await expect(page).toHaveURL("about:blank");
});
