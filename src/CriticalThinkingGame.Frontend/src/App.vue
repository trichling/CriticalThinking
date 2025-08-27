<script setup lang="ts">
import { RouterView } from 'vue-router'
</script>

<template>
  <div id="app">
    <header class="header">
      <div class="container">
        <h1 class="title">Critical Thinking Game</h1>
        <p class="subtitle">Train your logical reasoning by identifying fallacies</p>
      </div>
    </header>

    <main class="main">
      <div class="container">
        <RouterView />
      </div>
    </main>
  </div>
</template>

<style scoped>
#app {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.header {
  padding: 2rem 0;
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
}

.title {
  font-size: 2.5rem;
  font-weight: bold;
  margin: 0 0 0.5rem 0;
  text-align: center;
}

.subtitle {
  font-size: 1.2rem;
  opacity: 0.9;
  margin: 0;
  text-align: center;
}

.main {
  padding: 2rem 0;
  flex: 1;
}

* {
  box-sizing: border-box;
}

body {
  margin: 0;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}
</style>
