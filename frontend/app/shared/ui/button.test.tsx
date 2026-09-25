import { render, screen } from "@testing-library/react";
import { Button } from "./button";

test("renders its children", () => {
  render(<Button>Buy now</Button>);

  expect(screen.getByRole("button", { name: "Buy now" })).toBeInTheDocument();
});
