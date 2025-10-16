import { test, expect } from '@playwright/test';

test('homepage renders hero content', async ({ page }) => {
  await page.goto('/');
  await expect(page.getByRole('heading', { name: /phase 0 scaffold/i })).toBeVisible();
  await expect(page.getByText(/postgres/i)).toBeVisible();
});
