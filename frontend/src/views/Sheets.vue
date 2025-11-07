<template>
  <div class="sheets">
    <div class="card">
      <h2>Sheet Management</h2>
      <p>Configure print sheet sizes and materials</p>
      
      <div class="create-sheet">
        <h3>Create New Sheet</h3>
        <div class="form-group">
          <label>Sheet Name</label>
          <input v-model="newSheet.name" type="text" placeholder="e.g., A4 Cardboard" />
        </div>
        <div class="grid grid-2">
          <div class="form-group">
            <label>Width (mm)</label>
            <input v-model.number="newSheet.width" type="number" />
          </div>
          <div class="form-group">
            <label>Height (mm)</label>
            <input v-model.number="newSheet.height" type="number" />
          </div>
        </div>
        <div class="grid grid-2">
          <div class="form-group">
            <label>Material</label>
            <select v-model="newSheet.material">
              <option value="paper">Paper</option>
              <option value="cardboard">Cardboard</option>
              <option value="vinyl">Vinyl</option>
              <option value="plastic">Plastic</option>
            </select>
          </div>
        </div>
        <h4>Margins (mm)</h4>
        <div class="grid grid-2">
          <div class="form-group">
            <label>Top</label>
            <input v-model.number="newSheet.marginTop" type="number" />
          </div>
          <div class="form-group">
            <label>Bottom</label>
            <input v-model.number="newSheet.marginBottom" type="number" />
          </div>
          <div class="form-group">
            <label>Left</label>
            <input v-model.number="newSheet.marginLeft" type="number" />
          </div>
          <div class="form-group">
            <label>Right</label>
            <input v-model.number="newSheet.marginRight" type="number" />
          </div>
        </div>
        <button @click="createSheet" class="btn btn-primary">Create Sheet</button>
      </div>
      
      <div class="sheets-list">
        <h3>Your Sheets</h3>
        <div v-if="loading" class="loading">Loading...</div>
        <div v-else-if="sheets.length === 0" class="empty-state">
          No sheets created yet. Create your first sheet above.
        </div>
        <div v-else class="grid grid-2">
          <div v-for="sheet in sheets" :key="sheet.id" class="sheet-card">
            <h4>{{ sheet.name }}</h4>
            <p><strong>Size:</strong> {{ sheet.width }} x {{ sheet.height }} mm</p>
            <p><strong>Material:</strong> {{ sheet.material }}</p>
            <p><strong>Margins:</strong> T:{{ sheet.marginTop }} B:{{ sheet.marginBottom }} L:{{ sheet.marginLeft }} R:{{ sheet.marginRight }}</p>
            <p class="date">Created: {{ formatDate(sheet.createdAt) }}</p>
            <button @click="deleteSheet(sheet.id)" class="btn btn-danger btn-sm">
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue'
import { sheetService } from '../services/api'

export default {
  name: 'Sheets',
  setup() {
    const sheets = ref([])
    const loading = ref(false)
    const newSheet = ref({
      name: '',
      width: 210,
      height: 297,
      marginTop: 10,
      marginBottom: 10,
      marginLeft: 10,
      marginRight: 10,
      material: 'paper'
    })
    
    const loadSheets = async () => {
      loading.value = true
      try {
        sheets.value = await sheetService.getAllSheets()
      } catch (error) {
        console.error('Failed to load sheets:', error)
        alert('Failed to load sheets. Make sure the backend is running.')
      } finally {
        loading.value = false
      }
    }
    
    const createSheet = async () => {
      if (!newSheet.value.name) {
        alert('Please enter a sheet name')
        return
      }
      
      try {
        await sheetService.createSheet(newSheet.value)
        newSheet.value = {
          name: '',
          width: 210,
          height: 297,
          marginTop: 10,
          marginBottom: 10,
          marginLeft: 10,
          marginRight: 10,
          material: 'paper'
        }
        await loadSheets()
        alert('Sheet created successfully!')
      } catch (error) {
        console.error('Failed to create sheet:', error)
        alert('Failed to create sheet')
      }
    }
    
    const deleteSheet = async (id) => {
      if (!confirm('Are you sure you want to delete this sheet?')) return
      
      try {
        await sheetService.deleteSheet(id)
        await loadSheets()
      } catch (error) {
        console.error('Failed to delete sheet:', error)
        alert('Failed to delete sheet')
      }
    }
    
    const formatDate = (dateString) => {
      return new Date(dateString).toLocaleDateString()
    }
    
    onMounted(() => {
      loadSheets()
    })
    
    return {
      sheets,
      loading,
      newSheet,
      createSheet,
      deleteSheet,
      formatDate
    }
  }
}
</script>

<style scoped>
.create-sheet {
  background: #f8f9fa;
  padding: 1.5rem;
  border-radius: 8px;
  margin-bottom: 2rem;
}

.create-sheet h3, .create-sheet h4 {
  margin-bottom: 1rem;
}

.sheets-list {
  margin-top: 2rem;
}

.loading, .empty-state {
  text-align: center;
  padding: 2rem;
  color: #666;
}

.sheet-card {
  background: #f8f9fa;
  border-radius: 8px;
  padding: 1.5rem;
  border: 1px solid #dee2e6;
}

.sheet-card h4 {
  margin-bottom: 0.5rem;
  color: #2c3e50;
}

.sheet-card p {
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
