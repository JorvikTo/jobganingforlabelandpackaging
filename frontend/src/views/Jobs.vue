<template>
  <div class="jobs">
    <div class="card">
      <h2>Job Import with Artwork</h2>
      <p>Import PDF artwork files with spot color die lines for nesting/imposition</p>
      
      <div class="import-section">
        <h3>Import Job with Artwork</h3>
        <p class="info-text">
          📄 Upload a PDF file containing your artwork with a spot color die line. 
          The system will automatically extract the die line and use it for nesting optimization.
        </p>
        
        <div class="form-group">
          <label>Job Name</label>
          <input 
            v-model="jobName" 
            type="text" 
            placeholder="e.g., Sticker Labels Batch 1"
          />
        </div>
        
        <div class="form-group">
          <label>Die Line Spot Color Name</label>
          <input 
            v-model="spotColorName" 
            type="text" 
            placeholder="CutContour"
          />
          <p class="help-text">Common names: CutContour, DieLine, Cut, Contour</p>
        </div>
        
        <div class="form-group">
          <label>Quantity</label>
          <input 
            v-model.number="quantity" 
            type="number" 
            min="1"
            placeholder="1"
          />
        </div>
        
        <div class="form-group">
          <label>PDF Artwork File</label>
          <input 
            type="file" 
            @change="handleFileUpload" 
            accept=".pdf"
            ref="fileInput"
          />
          <p class="help-text">PDF must contain artwork with a spot color die line layer</p>
        </div>
        
        <button 
          @click="importJob" 
          class="btn btn-primary" 
          :disabled="!selectedFile || importing"
        >
          {{ importing ? 'Importing...' : '📦 Import Job with Artwork' }}
        </button>
      </div>
      
      <div v-if="lastImportResult" class="import-result">
        <h3>✅ Import Successful</h3>
        <p><strong>Job:</strong> {{ lastImportResult.jobName }}</p>
        <p><strong>Message:</strong> {{ lastImportResult.message }}</p>
        <p><strong>Detected Spot Colors:</strong> {{ lastImportResult.detectedSpotColors.join(', ') }}</p>
        <p class="success-note">Die line extracted and ready for nesting optimization!</p>
      </div>
      
      <div class="jobs-list">
        <h3>Imported Jobs</h3>
        <div v-if="loading" class="loading">Loading...</div>
        <div v-else-if="jobs.length === 0" class="empty-state">
          No jobs imported yet. Import your first job with artwork above.
        </div>
        <div v-else class="grid grid-2">
          <div v-for="job in jobs" :key="job.id" class="job-card">
            <div class="job-icon">📦</div>
            <div class="job-info">
              <h4>{{ job.jobName }}</h4>
              <p><strong>File:</strong> {{ job.fileName }}</p>
              <p><strong>Spot Color:</strong> {{ job.dieLineSpotColorName }}</p>
              <p><strong>Quantity:</strong> {{ job.quantity }}</p>
              <p class="date">Imported: {{ formatDate(job.createdAt) }}</p>
              <div class="job-actions">
                <button @click="viewDieLine(job.extractedDieLineId)" class="btn btn-secondary btn-sm">
                  View Die Line
                </button>
                <button @click="deleteJob(job.id)" class="btn btn-danger btn-sm">
                  Delete
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue'
import { jobService, dieLineService } from '../services/api'
import { useRouter } from 'vue-router'

export default {
  name: 'Jobs',
  setup() {
    const router = useRouter()
    const jobs = ref([])
    const selectedFile = ref(null)
    const jobName = ref('')
    const spotColorName = ref('CutContour')
    const quantity = ref(1)
    const loading = ref(false)
    const importing = ref(false)
    const fileInput = ref(null)
    const lastImportResult = ref(null)
    
    const loadJobs = async () => {
      loading.value = true
      try {
        jobs.value = await jobService.getAllJobs()
      } catch (error) {
        console.error('Failed to load jobs:', error)
        alert('Unable to load jobs. Please try again or contact support if the problem persists.')
      } finally {
        loading.value = false
      }
    }
    
    const handleFileUpload = (event) => {
      selectedFile.value = event.target.files[0]
      if (selectedFile.value && !jobName.value) {
        // Auto-fill job name from file name
        jobName.value = selectedFile.value.name.replace('.pdf', '')
      }
    }
    
    const importJob = async () => {
      if (!selectedFile.value) return
      if (!jobName.value) {
        alert('Please enter a job name')
        return
      }
      
      importing.value = true
      lastImportResult.value = null
      
      try {
        const result = await jobService.importJobWithArtwork(
          selectedFile.value,
          jobName.value,
          spotColorName.value,
          quantity.value
        )
        
        lastImportResult.value = result
        
        // Reset form
        selectedFile.value = null
        jobName.value = ''
        spotColorName.value = 'CutContour'
        quantity.value = 1
        fileInput.value.value = ''
        
        // Reload jobs list
        await loadJobs()
        
      } catch (error) {
        console.error('Failed to import job:', error)
        const errorMsg = error.response?.data?.message || error.message || 'Unknown error'
        alert(`Failed to import job: ${errorMsg}. Please check your file and try again.`)
      } finally {
        importing.value = false
      }
    }
    
    const viewDieLine = (dieLineId) => {
      router.push({ path: '/die-lines', query: { highlight: dieLineId } })
    }
    
    const deleteJob = async (id) => {
      if (!confirm('Are you sure you want to delete this job? The extracted die line will also be deleted.')) return
      
      try {
        await jobService.deleteJob(id)
        await loadJobs()
      } catch (error) {
        console.error('Failed to delete job:', error)
        alert('Failed to delete job')
      }
    }
    
    const formatDate = (dateString) => {
      return new Date(dateString).toLocaleDateString()
    }
    
    onMounted(() => {
      loadJobs()
    })
    
    return {
      jobs,
      selectedFile,
      jobName,
      spotColorName,
      quantity,
      loading,
      importing,
      fileInput,
      lastImportResult,
      handleFileUpload,
      importJob,
      viewDieLine,
      deleteJob,
      formatDate
    }
  }
}
</script>

<style scoped>
.import-section {
  background: #f8f9fa;
  padding: 1.5rem;
  border-radius: 8px;
  margin-bottom: 2rem;
}

.import-section h3 {
  margin-bottom: 1rem;
}

.info-text {
  background: #e7f5ff;
  padding: 1rem;
  border-radius: 4px;
  border-left: 4px solid #42b983;
  margin-bottom: 1.5rem;
}

.help-text {
  font-size: 0.9rem;
  color: #666;
  margin-top: 0.25rem;
}

.import-result {
  background: #d4edda;
  border: 1px solid #c3e6cb;
  border-radius: 8px;
  padding: 1.5rem;
  margin-bottom: 2rem;
}

.import-result h3 {
  color: #155724;
  margin-bottom: 0.5rem;
}

.import-result p {
  margin: 0.5rem 0;
  color: #155724;
}

.success-note {
  font-weight: bold;
  color: #155724;
  margin-top: 1rem !important;
}

.jobs-list {
  margin-top: 2rem;
}

.loading, .empty-state {
  text-align: center;
  padding: 2rem;
  color: #666;
}

.job-card {
  background: #f8f9fa;
  border-radius: 8px;
  padding: 1.5rem;
  border: 1px solid #dee2e6;
  display: flex;
  gap: 1rem;
}

.job-icon {
  font-size: 3rem;
  flex-shrink: 0;
}

.job-info {
  flex: 1;
}

.job-info h4 {
  margin-bottom: 0.5rem;
  color: #2c3e50;
}

.job-info p {
  font-size: 0.9rem;
  color: #666;
  margin: 0.25rem 0;
}

.date {
  font-size: 0.8rem;
  color: #999;
  margin-top: 0.5rem;
}

.job-actions {
  display: flex;
  gap: 0.5rem;
  margin-top: 1rem;
}

.btn-sm {
  padding: 0.25rem 0.75rem;
  font-size: 0.9rem;
}
</style>
