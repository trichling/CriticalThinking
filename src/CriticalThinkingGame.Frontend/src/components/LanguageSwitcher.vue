<template>
  <div class="language-switcher">
    <label for="language-select" class="sr-only">Choose Language</label>
    <select 
      id="language-select"
      v-model="currentLanguage" 
      @change="changeLanguage"
      class="language-select"
    >
      <option 
        v-for="lang in availableLanguages" 
        :key="lang.code" 
        :value="lang.code"
      >
        {{ lang.nativeName }}
      </option>
    </select>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { setLanguage, getCurrentLanguage, getAvailableLanguages } from '@/locales'

const { locale } = useI18n()
const currentLanguage = ref(getCurrentLanguage())
const availableLanguages = getAvailableLanguages()

onMounted(() => {
  currentLanguage.value = getCurrentLanguage()
})

function changeLanguage() {
  setLanguage(currentLanguage.value)
  locale.value = currentLanguage.value as 'en' | 'de'
}
</script>

<style scoped>
.language-switcher {
  display: flex;
  align-items: center;
}

.language-select {
  padding: 0.5rem;
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 0.25rem;
  background: rgba(255, 255, 255, 0.1);
  color: white;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.language-select:hover {
  background: rgba(255, 255, 255, 0.2);
}

.language-select:focus {
  outline: none;
  border-color: rgba(255, 255, 255, 0.5);
}

.language-select option {
  background: #333;
  color: white;
}

.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border: 0;
}
</style>
