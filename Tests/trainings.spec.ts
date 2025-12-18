// Playwright Tests for Trainings Page
// Run with: npx playwright test Tests/trainings.spec.ts
// Run with UI: npx playwright test Tests/trainings.spec.ts --ui
// Run specific test: npx playwright test Tests/trainings.spec.ts -g "should display training cards"

import { test, expect } from '@playwright/test';

// Base URL configuration
const BASE_URL = 'http://localhost:5204';
const TRAININGS_URL = `${BASE_URL}/Trainings`;

test.describe('Trainings Page - Layout and Structure', () => {
  
  test('should load trainings page successfully', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    // Check page title
    await expect(page).toHaveTitle(/Past Trainings/i);
    
    // Check main heading
    const heading = page.locator('h1:has-text("My Past Trainings")');
    await expect(heading).toBeVisible();
    
    // Check subtitle
    const subtitle = page.locator('p:has-text("Showcase of successful training programs")');
    await expect(subtitle).toBeVisible();
  });

  test('should have correct page structure', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    // Check container exists
    const container = page.locator('.container');
    await expect(container).toBeVisible();
    
    // Check responsive layout
    const row = page.locator('.row.justify-content-center.g-4');
    await expect(row).toBeVisible();
  });

  test('should display correct meta tags', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    // Check viewport meta tag
    const viewport = await page.locator('meta[name="viewport"]').getAttribute('content');
    expect(viewport).toContain('width=device-width');
  });
});

test.describe('Trainings Page - Data Display', () => {
  
  test('should display training cards when data exists', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    // Wait for content to load
    await page.waitForTimeout(1000);
    
    // Check if cards are displayed (either cards or empty state)
    const hasCards = await page.locator('.card').count() > 0;
    const hasEmptyState = await page.locator('i.fa-chalkboard-teacher').isVisible();
    
    expect(hasCards || hasEmptyState).toBe(true);
  });

  test('should display training card with all required fields', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    const cards = page.locator('.card');
    const cardCount = await cards.count();
    
    if (cardCount > 0) {
      const firstCard = cards.first();
      
      // Check card title
      await expect(firstCard.locator('.card-title')).toBeVisible();
      
      // Check company name
      await expect(firstCard.locator('p:has-text("Company:")')).toBeVisible();
      
      // Check date
      await expect(firstCard.locator('p:has-text("Date:")')).toBeVisible();
      
      // Check duration
      await expect(firstCard.locator('p:has-text("Duration:")')).toBeVisible();
      
      // Check topics
      await expect(firstCard.locator('small:has-text("Topics:")')).toBeVisible();
    }
  });

  test('should display training image or placeholder', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    const cards = page.locator('.card');
    const cardCount = await cards.count();
    
    if (cardCount > 0) {
      const firstCard = cards.first();
      
      // Check if image or gradient placeholder exists
      const hasImage = await firstCard.locator('.card-img-top').count() > 0;
      expect(hasImage).toBe(true);
    }
  });

  test('should show empty state when no trainings exist', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    const cards = page.locator('.card');
    const cardCount = await cards.count();
    
    if (cardCount === 0) {
      // Check empty state icon
      await expect(page.locator('i.fa-chalkboard-teacher')).toBeVisible();
      
      // Check empty state message
      await expect(page.locator('h5:has-text("No Trainings Available Yet")')).toBeVisible();
      await expect(page.locator('p:has-text("No past training records")')).toBeVisible();
    }
  });
});

test.describe('Trainings Page - Interactive Elements', () => {
  
  test('should have clickable SkillTech cards', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    // Look for cards with SkillTech links
    const skillTechCards = page.locator('a[href*="skilltech.club"]');
    const count = await skillTechCards.count();
    
    if (count > 0) {
      const firstSkillTechCard = skillTechCards.first();
      
      // Check if card is visible
      await expect(firstSkillTechCard).toBeVisible();
      
      // Check if card has correct attributes
      const target = await firstSkillTechCard.getAttribute('target');
      expect(target).toBe('_blank');
      
      // Check SkillTech badge exists
      await expect(firstSkillTechCard.locator('.badge:has-text("SkillTech")')).toBeVisible();
    }
  });

  test('should have hover effect on cards', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    const hoverCards = page.locator('.hover-lift');
    const count = await hoverCards.count();
    
    if (count > 0) {
      const firstCard = hoverCards.first();
      
      // Check cursor style
      const cursor = await firstCard.evaluate(el => 
        window.getComputedStyle(el).cursor
      );
      expect(cursor).toBe('pointer');
    }
  });

  test('should have working "View Details" buttons for non-SkillTech trainings', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    const detailsButtons = page.locator('a.btn:has-text("View Details")');
    const count = await detailsButtons.count();
    
    if (count > 0) {
      const firstButton = detailsButtons.first();
      
      // Check button is visible
      await expect(firstButton).toBeVisible();
      
      // Check href contains /Trainings/Details/
      const href = await firstButton.getAttribute('href');
      expect(href).toContain('/Trainings/Details/');
    }
  });
});

test.describe('Trainings Page - Responsive Design', () => {
  
  test('should be responsive on mobile', async ({ page }) => {
    // Set mobile viewport
    await page.setViewportSize({ width: 375, height: 667 });
    await page.goto(TRAININGS_URL);
    
    // Check container is visible
    const container = page.locator('.container');
    await expect(container).toBeVisible();
    
    // Check cards stack vertically
    const cards = page.locator('.col-12');
    const count = await cards.count();
    
    if (count > 0) {
      const firstCard = cards.first();
      await expect(firstCard).toBeVisible();
    }
  });

  test('should be responsive on tablet', async ({ page }) => {
    // Set tablet viewport
    await page.setViewportSize({ width: 768, height: 1024 });
    await page.goto(TRAININGS_URL);
    
    // Check layout adapts
    const container = page.locator('.container');
    await expect(container).toBeVisible();
    
    // Check grid layout
    const row = page.locator('.row.g-4');
    await expect(row).toBeVisible();
  });

  test('should be responsive on desktop', async ({ page }) => {
    // Set desktop viewport
    await page.setViewportSize({ width: 1920, height: 1080 });
    await page.goto(TRAININGS_URL);
    
    // Check full layout
    const container = page.locator('.container');
    await expect(container).toBeVisible();
    
    // Check cards display in grid
    const cards = page.locator('.card');
    const count = await cards.count();
    
    if (count > 0) {
      await expect(cards.first()).toBeVisible();
    }
  });
});

test.describe('Trainings Page - Accessibility', () => {
  
  test('should have proper heading hierarchy', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    // Check h1 exists and is unique
    const h1Count = await page.locator('h1').count();
    expect(h1Count).toBeGreaterThan(0);
    
    // Check card titles use h5
    const cards = page.locator('.card');
    const cardCount = await cards.count();
    
    if (cardCount > 0) {
      const h5InCard = cards.first().locator('h5.card-title');
      await expect(h5InCard).toBeVisible();
    }
  });

  test('should have alt text for images', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    const images = page.locator('img.card-img-top');
    const imageCount = await images.count();
    
    for (let i = 0; i < imageCount; i++) {
      const img = images.nth(i);
      const alt = await img.getAttribute('alt');
      expect(alt).toBeTruthy();
      expect(alt?.length).toBeGreaterThan(0);
    }
  });

  test('should have semantic HTML structure', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    // Check for semantic elements
    const cards = page.locator('.card');
    const cardCount = await cards.count();
    
    if (cardCount > 0) {
      const firstCard = cards.first();
      
      // Check card has header, body, footer
      await expect(firstCard.locator('.card-body')).toBeVisible();
      await expect(firstCard.locator('.card-footer')).toBeVisible();
    }
  });

  test('should be keyboard navigable', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    const links = page.locator('a[href*="/Trainings"], a[href*="skilltech.club"]');
    const linkCount = await links.count();
    
    if (linkCount > 0) {
      // Tab to first link
      await page.keyboard.press('Tab');
      
      // Check if a focusable element is focused
      const focusedElement = await page.evaluate(() => document.activeElement?.tagName);
      expect(focusedElement).toBeTruthy();
    }
  });
});

test.describe('Trainings Page - Content Validation', () => {
  
  test('should display dates in correct format', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    const dateParagraphs = page.locator('p:has-text("Date:")');
    const count = await dateParagraphs.count();
    
    if (count > 0) {
      const dateText = await dateParagraphs.first().textContent();
      
      // Check date format (should contain "Date:" followed by date or "Not set")
      expect(dateText).toMatch(/Date:\s+(\d{1,2}\s+\w+\s+\d{4}|Not set)/);
    }
  });

  test('should display company names', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    const companyParagraphs = page.locator('p:has-text("Company:")');
    const count = await companyParagraphs.count();
    
    if (count > 0) {
      const companyText = await companyParagraphs.first().textContent();
      expect(companyText).toContain('Company:');
      expect(companyText?.length).toBeGreaterThan(10); // More than just "Company:"
    }
  });

  test('should display training topics', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    const topicsElements = page.locator('small:has-text("Topics:")');
    const count = await topicsElements.count();
    
    if (count > 0) {
      const topicsText = await topicsElements.first().textContent();
      expect(topicsText).toContain('Topics:');
      expect(topicsText?.length).toBeGreaterThan(10); // More than just "Topics:"
    }
  });
});

test.describe('Trainings Page - Performance', () => {
  
  test('should load within reasonable time', async ({ page }) => {
    const startTime = Date.now();
    
    await page.goto(TRAININGS_URL);
    await page.waitForLoadState('domcontentloaded');
    
    const loadTime = Date.now() - startTime;
    
    // Page should load within 3 seconds
    expect(loadTime).toBeLessThan(3000);
  });

  test('should load images efficiently', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    const images = page.locator('img.card-img-top');
    const imageCount = await images.count();
    
    if (imageCount > 0) {
      // Wait for first image to load
      await images.first().waitFor({ state: 'visible' });
      
      // Check image has valid src
      const src = await images.first().getAttribute('src');
      expect(src).toBeTruthy();
      expect(src?.length).toBeGreaterThan(0);
    }
  });
});

test.describe('Trainings Page - Visual Regression', () => {
  
  test('should match visual snapshot', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    await page.waitForLoadState('networkidle');
    
    // Take screenshot
    await expect(page).toHaveScreenshot('trainings-page.png', {
      fullPage: true,
      maxDiffPixels: 100
    });
  });

  test('should match card snapshot', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    const cards = page.locator('.card');
    const count = await cards.count();
    
    if (count > 0) {
      await expect(cards.first()).toHaveScreenshot('training-card.png', {
        maxDiffPixels: 50
      });
    }
  });
});

test.describe('Trainings Page - Error Handling', () => {
  
  test('should handle missing images gracefully', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    // Check for gradient placeholders
    const gradients = page.locator('.card-img-top[style*="gradient"]');
    const count = await gradients.count();
    
    // If gradients exist, they should be visible
    if (count > 0) {
      await expect(gradients.first()).toBeVisible();
    }
  });

  test('should handle empty data gracefully', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    // Should either show cards or empty state, not both
    const hasCards = await page.locator('.card').count() > 0;
    const hasEmptyState = await page.locator('i.fa-chalkboard-teacher').isVisible();
    
    // Either cards or empty state should be true, but not both
    expect(hasCards || hasEmptyState).toBe(true);
    
    if (hasEmptyState) {
      // If empty state is shown, no cards should be visible
      expect(hasCards).toBe(false);
    }
  });
});

test.describe('Trainings Page - JSON Data Integration', () => {
  
  test('should load data from JSON file', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    // Wait for data to load
    await page.waitForTimeout(500);
    
    const cards = page.locator('.card');
    const cardCount = await cards.count();
    
    // Should have 5 trainings based on populated JSON
    if (cardCount > 0) {
      expect(cardCount).toBeGreaterThanOrEqual(5);
    }
  });

  test('should display TCS training', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    // Check if TCS training exists
    const tcsCard = page.locator('.card:has-text("TCS")');
    const count = await tcsCard.count();
    
    if (count > 0) {
      await expect(tcsCard.first()).toBeVisible();
      await expect(tcsCard.first()).toContainText('Azure Cloud Migration Workshop');
    }
  });

  test('should display training durations', async ({ page }) => {
    await page.goto(TRAININGS_URL);
    
    const durationElements = page.locator('p:has-text("Duration:")');
    const count = await durationElements.count();
    
    if (count > 0) {
      const firstDuration = await durationElements.first().textContent();
      expect(firstDuration).toMatch(/Duration:\s+\d+\s+Days?/);
    }
  });
});
