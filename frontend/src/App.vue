<template>
  <div id="app">
    <header class="app-header">
      <h1>Job Ganging Software</h1>
      <nav>
        <router-link to="/">Home</router-link>
        <router-link to="/jobs">Jobs</router-link>
        <router-link to="/die-lines">Die Lines</router-link>
        <router-link to="/sheets">Sheets</router-link>
        <router-link to="/ganging">Ganging</router-link>
      </nav>
    </header>
    <main class="app-main">
      <router-view />
    </main>
  </div>
</template>

<script>
export default {
  name: 'App'
}
</script>

<style scoped>
.app-header {
  background: #2c3e50;
  color: white;
  padding: 1rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.app-header h1 {
  margin: 0;
  font-size: 1.5rem;
}

.app-header nav {
  display: flex;
  gap: 2rem;
}

.app-header nav a {
  color: white;
  text-decoration: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  transition: background 0.3s;
}

.app-header nav a:hover,
.app-header nav a.router-link-active {
  background: #34495e;
}

.app-main {
  padding: 2rem;
  max-width: 1400px;
  margin: 0 auto;
}
</style>
