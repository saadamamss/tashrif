import { test, expect } from '@playwright/test'

test.describe('Job Search', () => {
  test('public jobs page loads and shows listings', async ({ page }) => {
    await page.goto('/jobs')
    await expect(page.getByText('فرص موسمية')).toBeVisible()
  })

  test('unauthenticated user can browse jobs', async ({ page }) => {
    await page.goto('/jobs')
    await expect(page.getByText('وظيفتك في انتظارك')).toBeVisible()
  })

  test('job detail navigation from public page', async ({ page }) => {
    await page.goto('/jobs')
    await page.locator('text=قدم الآن').first().click()
    await expect(page.getByRole('button', { name: 'تسجيل الدخول' }).first()).toBeVisible()
  })
})
