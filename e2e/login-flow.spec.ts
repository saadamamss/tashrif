import { test, expect } from '@playwright/test'

test.describe('Login Flow', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/')
  })

  test('opens login modal from landing page', async ({ page }) => {
    await page.getByRole('button', { name: 'تسجيل الدخول' }).first().click()
    await expect(page.getByText('تسجيل الدخول')).toBeVisible()
  })

  test('logs in as individual and redirects to dashboard', async ({ page }) => {
    await page.getByRole('button', { name: 'تسجيل الدخول' }).first().click()
    await page.getByLabel('رقم الهوية').fill('1010101010')
    await page.getByLabel('كلمة المرور').fill('individual')
    await page.getByRole('button', { name: 'تسجيل الدخول' }).last().click()
    await expect(page).toHaveURL(/\/dashboard/)
  })

  test('shows error with invalid credentials', async ({ page }) => {
    await page.getByRole('button', { name: 'تسجيل الدخول' }).first().click()
    await page.getByLabel('رقم الهوية').fill('0000000000')
    await page.getByLabel('كلمة المرور').fill('wrong')
    await page.getByRole('button', { name: 'تسجيل الدخول' }).last().click()
    await expect(page.getByText('بيانات الدخول غير صحيحة')).toBeVisible()
  })

  test('logs in as entity and redirects to entity dashboard', async ({ page }) => {
    await page.getByRole('button', { name: 'تسجيل الدخول' }).first().click()
    await page.getByLabel('رقم الهوية').fill('2020202020')
    await page.getByLabel('كلمة المرور').fill('entity')
    await page.getByRole('button', { name: 'تسجيل الدخول' }).last().click()
    await expect(page).toHaveURL(/\/dashboard/)
  })

  test('authenticated user can access protected routes', async ({ page }) => {
    await page.getByRole('button', { name: 'تسجيل الدخول' }).first().click()
    await page.getByLabel('رقم الهوية').fill('1010101010')
    await page.getByLabel('كلمة المرور').fill('individual')
    await page.getByRole('button', { name: 'تسجيل الدخول' }).last().click()
    await expect(page).toHaveURL(/\/dashboard/)
    await page.goto('/dashboard/jobs-explore')
    await expect(page.getByText('استكشف الوظائف')).toBeVisible()
  })
})
