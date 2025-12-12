// Playwright test for Bento Grid About section
// Run with: npx playwright test

// Note: Install Playwright first with: npm install --save-dev @playwright/test
// This is a placeholder test file. Uncomment when Playwright is installed.

/*
import { test, expect } from '@playwright/test';

test('about page loads bento grid and first tile is focusable', async ({ page }) => {
  await page.goto('http://localhost:5000/about');
  
  // Confirm Bento section is present
  const bentoSection = page.locator('.bento-grid-section');
  await expect(bentoSection).toBeVisible();
  
  // Find first tile
  const firstTile = page.locator('[data-bento-tile]').first();
  await expect(firstTile).toBeVisible();
  
  // Focus the tile
  await firstTile.focus();
  await expect(firstTile).toBeFocused();
  
  // Check if tile has a link
  const linkUrl = await firstTile.getAttribute('data-link-url');
  
  if (linkUrl) {
    // Simulate Enter key press
    await firstTile.press('Enter');
    
    // Wait for navigation
    await page.waitForTimeout(500);
    
    // Assert navigation occurred
    const currentUrl = page.url();
    expect(currentUrl).toContain(linkUrl);
  }
});

test('bento tiles are keyboard accessible', async ({ page }) => {
  await page.goto('http://localhost:5000/about');
  
  const tiles = page.locator('[data-bento-tile]');
  const tileCount = await tiles.count();
  
  expect(tileCount).toBeGreaterThan(0);
  
  // Tab through tiles and ensure they receive focus
  for (let i = 0; i < tileCount; i++) {
    const tile = tiles.nth(i);
    await tile.focus();
    await expect(tile).toBeFocused();
  }
});
*/

console.log('Playwright tests are commented out. Install @playwright/test to run: npm install --save-dev @playwright/test');
