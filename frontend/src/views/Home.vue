<template>
  <div class="home">
    <div class="card">
      <h1>Welcome to Job Ganging Software</h1>
      <p class="subtitle">Professional label and packaging layout optimization</p>
      
      <div class="features grid grid-2">
        <div class="feature-card">
          <h3>📄 Die Line Import</h3>
          <p>Import PDF (with spot colors) and DXF files for your label and packaging designs.</p>
        </div>
        
        <div class="feature-card">
          <h3>🎯 Automatic Optimization</h3>
          <p>AI-powered nesting algorithm automatically arranges designs to minimize waste.</p>
        </div>
        
        <div class="feature-card">
          <h3>✋ Manual Arrangement</h3>
          <p>Fine-tune layouts with interactive drag-and-drop canvas interface.</p>
        </div>
        
        <div class="feature-card">
          <h3>📊 Waste Calculation</h3>
          <p>Real-time material utilization metrics and waste percentage tracking.</p>
        </div>
      </div>
      
      <div class="quick-start">
        <h2>Quick Start</h2>
        <ol>
          <li>Upload your die lines (PDF or DXF)</li>
          <li>Create or select a sheet size</li>
          <li>Run automatic optimization or arrange manually</li>
          <li>Export print-ready PDF with registration marks</li>
        </ol>
      </div>
      
      <div class="actions">
        <router-link to="/die-lines" class="btn btn-primary">Get Started →</router-link>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Home'
}
</script>

<style scoped>
.home {
  max-width: 1000px;
  margin: 0 auto;
}

.subtitle {
  font-size: 1.2rem;
  color: #666;
  margin-bottom: 2rem;
}

.features {
  margin: 2rem 0;
}

.feature-card {
  background: #f8f9fa;
  padding: 1.5rem;
  border-radius: 8px;
  border-left: 4px solid #42b983;
}

.feature-card h3 {
  margin-bottom: 0.5rem;
  color: #2c3e50;
}

.feature-card p {
  color: #666;
}

.quick-start {
  margin: 2rem 0;
  padding: 1.5rem;
  background: #e7f5ff;
  border-radius: 8px;
}

.quick-start h2 {
  margin-bottom: 1rem;
}

.quick-start ol {
  margin-left: 1.5rem;
}

.quick-start li {
  margin-bottom: 0.5rem;
  color: #495057;
}

.actions {
  text-align: center;
  margin-top: 2rem;
}

.actions .btn {
  font-size: 1.1rem;
  padding: 0.75rem 2rem;
}
</style>
