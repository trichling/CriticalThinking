import { createI18n } from 'vue-i18n'
import en from './en.json'
import de from './de.json'

// Detect browser language or use default
function getDefaultLocale(): string {
  const browserLang = navigator.language.split('-')[0]
  const supportedLanguages = ['en', 'de']
  
  // Check if browser language is supported
  if (supportedLanguages.includes(browserLang)) {
    return browserLang
  }
  
  // Check localStorage for saved preference
  const savedLang = localStorage.getItem('language')
  if (savedLang && supportedLanguages.includes(savedLang)) {
    return savedLang
  }
  
  // Default to English
  return 'en'
}

const messages = {
  en,
  de
}

const i18n = createI18n({
  legacy: false, // Use Composition API mode
  locale: getDefaultLocale(),
  fallbackLocale: 'en',
  messages,
  globalInjection: true
})

export default i18n

// Helper function to change language
export function setLanguage(locale: string) {
  if (i18n.global.availableLocales.includes(locale as any)) {
    i18n.global.locale.value = locale as 'en' | 'de'
    localStorage.setItem('language', locale)
    document.documentElement.lang = locale
  }
}

// Helper function to get current language
export function getCurrentLanguage(): string {
  return i18n.global.locale.value
}

// Helper function to get available languages
export function getAvailableLanguages() {
  return [
    { code: 'en', name: 'English', nativeName: 'English' },
    { code: 'de', name: 'German', nativeName: 'Deutsch' }
  ]
}
