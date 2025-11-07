<template>
  <div class="die-lines">
    <div class="card">
      <h2>Die Line Management</h2>
      <p>Upload and manage your label and packaging die lines</p>
      
      <div class="upload-section">
        <h3>Upload Die Line</h3>
        <input 
          type="file" 
          @change="handleFileUpload" 
          accept=".pdf,.dxf"
          ref="fileInput"
        />
        <button @click="uploadFile" class="btn btn-primary" :disabled="!selectedFile || uploading">
          {{ uploading ? 'Uploading...' : 'Upload' }}
        </button>
        <p class="help-text">Supported formats: PDF (with spot colors), DXF</p>
      </div>
      
      <div class="die-lines-list">
        <h3>Your Die Lines</h3>
        <div v-if="loading" class="loading">Loading...</div>
        <div v-else-if="dieLines.length === 0" class="empty-state">
          No die lines uploaded yet. Upload your first die line above.
        </div>
        <div v-else class="grid grid-2">
          <div v-for="dieLine in dieLines" :key="dieLine.id" class="die-line-card">
            <div class="die-line-preview">
              <div class="preview-placeholder">
                {{ dieLine.fileType }}
              </div>
            </div>
            <div class="die-line-info">
              <h4>{{ dieLine.fileName }}</h4>
              <p>Size: {{ dieLine.width }} x {{ dieLine.height }} mm</p>
              <p>Type: {{ dieLine.fileType }}</p>
              <p class="date">Uploaded: {{ formatDate(dieLine.createdAt) }}</p>
              <button @click="deleteDieLine(dieLine.id)" class="btn btn-danger btn-sm">
                Delete
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue'
import { dieLineService } from '../services/api'

export default {
  name: 'DieLines',
  setup() {
    const dieLines = ref([])
    const selectedFile = ref(null)
    const loading = ref(false)
    const uploading = ref(false)
    const fileInput = ref(null)
    
    const loadDieLines = async () => {
      loading.value = true
      try {
        dieLines.value = await dieLineService.getAllDieLines()
      } catch (error) {
        console.error('Failed to load die lines:', error)
        alert('Failed to load die lines. Make sure the backend is running.')
      } finally {
        loading.value = false
      }
    }
    
    const handleFileUpload = (event) => {
      selectedFile.value = event.target.files[0]
    }
    
    const uploadFile = async () => {
      if (!selectedFile.value) return
      
      uploading.value = true
      try {
        await dieLineService.uploadDieLine(selectedFile.value)
        selectedFile.value = null
        fileInput.value.value = ''
        await loadDieLines()
        alert('Die line uploaded successfully!')
      } catch (error) {
        console.error('Failed to upload die line:', error)
        alert('Failed to upload die line. Make sure the backend is running.')
      } finally {
        uploading.value = false
      }
    }
    
    const deleteDieLine = async (id) => {
      if (!confirm('Are you sure you want to delete this die line?')) return
      
      try {
        await dieLineService.deleteDieLine(id)
        await loadDieLines()
      } catch (error) {
        console.error('Failed to delete die line:', error)
        alert('Failed to delete die line')
      }
    }
    
    const formatDate = (dateString) => {
      return new Date(dateString).toLocaleDateString()
    }
    
    onMounted(() => {
      loadDieLines()
    })
    
    return {
      dieLines,
      selectedFile,
      loading,
      uploading,
      fileInput,
      handleFileUpload,
      uploadFile,
      deleteDieLine,
      formatDate
    }
  }
}
</script>

<style scoped>
.upload-section {
  background: #f8f9fa;
  padding: 1.5rem;
  border-radius: 8px;
  margin-bottom: 2rem;
}

.upload-section h3 {
  margin-bottom: 1rem;
}

.upload-section input[type="file"] {
  display: block;
  margin: 1rem 0;
}

.help-text {
  font-size: 0.9rem;
  color: #666;
  margin-top: 0.5rem;
}

.die-lines-list {
  margin-top: 2rem;
}

.loading, .empty-state {
  text-align: center;
  padding: 2rem;
  color: #666;
}

.die-line-card {
  background: #f8f9fa;
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid #dee2e6;
}

.die-line-preview {
  background: #e9ecef;
  height: 150px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.preview-placeholder {
  font-size: 2rem;
  font-weight: bold;
  color: #6c757d;
}

.die-line-info {
  padding: 1rem;
}

.die-line-info h4 {
  margin-bottom: 0.5rem;
  color: #2c3e50;
}

.die-line-info p {
  font-size: 0.9rem;
  color: #666;
  margin: 0.25rem 0;
}

.date {
  font-size: 0.8rem;
  color: #999;
  margin-top: 0.5rem;
}

.btn-sm {
  padding: 0.25rem 0.75rem;
  font-size: 0.9rem;
  margin-top: 0.5rem;
}
</style>
