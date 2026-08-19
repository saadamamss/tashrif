import { test, expect } from '@playwright/test'

test.describe('Registration Flow', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/')
  })

  test('opens registration dialog from landing page', async ({ page }) => {
    await page.getByRole('button', { name: 'إنشاء حساب' }).first().click()
    await expect(page.getByText('اختر نوع الحساب')).toBeVisible()
  })

  test('navigates to individual registration page', async ({ page }) => {
    await page.getByRole('button', { name: 'إنشاء حساب' }).first().click()
    await page.getByRole('link', { name: 'متابعة كفرد' }).click()
    await expect(page).toHaveURL(/\/register\/individual/)
  })

  test('navigates to entity registration page', async ({ page }) => {
    await page.getByRole('button', { name: 'إنشاء حساب' }).first().click()
    await page.getByRole('link', { name: 'متابعة كجهة' }).click()
    await expect(page).toHaveURL(/\/register\/entity/)
  })

  test('individual registration form has required fields', async ({ page }) => {
    await page.goto('/register/individual')
    await expect(page.getByLabel('الاسم الأول')).toBeVisible()
    await expect(page.getByLabel('رقم الهوية')).toBeVisible()
    await expect(page.getByLabel('رقم الجوال')).toBeVisible()
    await expect(page.getByLabel('البريد الإلكتروني')).toBeVisible()
    await expect(page.getByLabel('كلمة المرور')).toBeVisible()
  })
})
