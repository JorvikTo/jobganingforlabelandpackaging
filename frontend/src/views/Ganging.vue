<template>
  <div class="ganging">
    <div class="card">
      <h2>Job Ganging - Layout Optimization</h2>
      <p>Arrange die lines on sheets to minimize waste</p>
      
      <div class="ganging-controls">
        <div class="form-group">
          <label>Select Sheet</label>
          <select v-model="selectedSheetId">
            <option value="">-- Select a sheet --</option>
            <option v-for="sheet in sheets" :key="sheet.id" :value="sheet.id">
              {{ sheet.name }} ({{ sheet.width }}x{{ sheet.height }}mm)
            </option>
          </select>
        </div>
        
        <div v-if="selectedSheetId" class="die-line-selection">
          <h3>Select Die Lines to Gang</h3>
          <div v-for="dieLine in dieLines" :key="dieLine.id" class="die-line-item">
            <label>
              <input type="checkbox" v-model="selectedDieLines[dieLine.id]" />
              {{ dieLine.fileName }} ({{ dieLine.width }}x{{ dieLine.height }}mm)
            </label>
            <input 
              v-if="selectedDieLines[dieLine.id]" 
              type="number" 
              v-model.number="dieLineQuantities[dieLine.id]"
              min="1"
              placeholder="Qty"
              class="qty-input"
            />
          </div>
        </div>
        
        <div v-if="selectedSheetId && hasSelectedDieLines" class="optimization-options">
          <h3>Optimization Options</h3>
          <div class="form-group">
            <label>Spacing (mm)</label>
            <input v-model.number="options.spacing" type="number" min="0" />
          </div>
          <div class="form-group">
            <label>
              <input type="checkbox" v-model="options.allowRotation" />
              Allow Rotation
            </label>
          </div>
          <div class="form-group">
            <label>
              <input type="checkbox" v-model="options.optimizeSheetSize" />
              Optimize Sheet Size (find best size within range)
            </label>
          </div>
          <div v-if="options.optimizeSheetSize" class="sheet-size-range">
            <h4>Sheet Size Range</h4>
            <div class="grid grid-2">
              <div class="form-group">
                <label>Min Width (mm)</label>
                <input v-model.number="options.minSheetWidth" type="number" min="100" />
              </div>
              <div class="form-group">
                <label>Max Width (mm)</label>
                <input v-model.number="options.maxSheetWidth" type="number" min="100" />
              </div>
              <div class="form-group">
                <label>Min Height (mm)</label>
                <input v-model.number="options.minSheetHeight" type="number" min="100" />
              </div>
              <div class="form-group">
                <label>Max Height (mm)</label>
                <input v-model.number="options.maxSheetHeight" type="number" min="100" />
              </div>
            </div>
          </div>
          <button @click="optimize" class="btn btn-primary" :disabled="optimizing">
            {{ optimizing ? 'Optimizing...' : '🎯 Optimize Layout' }}
          </button>
          <button @click="clearLayout" class="btn btn-secondary">
            Clear Layout
          </button>
        </div>
      </div>
      
      <div v-if="result" class="optimization-result">
        <h3>Optimization Result</h3>
        <div v-if="result.isOptimizedSize" class="optimized-sheet-info">
          <p class="success-note">✨ Optimized sheet size found!</p>
          <p><strong>Sheet Size:</strong> {{ result.optimizedSheet.width.toFixed(0) }} x {{ result.optimizedSheet.height.toFixed(0) }} mm</p>
        </div>
        <div class="metrics">
          <div class="metric">
            <strong>Placements:</strong> {{ result.placements.length }}
          </div>
          <div class="metric">
            <strong>Utilization:</strong> {{ result.utilization.toFixed(2) }}%
          </div>
          <div class="metric">
            <strong>Waste:</strong> {{ result.wastePercentage.toFixed(2) }}%
          </div>
          <div class="metric">
            <strong>Used Area:</strong> {{ result.usedArea.toFixed(2) }} mm²
          </div>
        </div>
        
        <div class="canvas-container">
          <canvas ref="canvas" width="800" height="600"></canvas>
        </div>
        
        <div class="export-actions">
          <button @click="exportPdf" class="btn btn-primary">
            📄 Export to PDF
          </button>
          <button @click="exportReport" class="btn btn-secondary">
            📊 Generate Report (CSV)
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted, watch, nextTick } from 'vue'
import { dieLineService, sheetService, nestingService, exportService } from '../services/api'

export default {
  name: 'Ganging',
  setup() {
    const sheets = ref([])
    const dieLines = ref([])
    const selectedSheetId = ref('')
    const selectedDieLines = ref({})
    const dieLineQuantities = ref({})
    const options = ref({
      spacing: 5,
      allowRotation: true,
      optimizeSheetSize: false,
      minSheetWidth: 200,
      maxSheetWidth: 1000,
      minSheetHeight: 200,
      maxSheetHeight: 1000
    })
    const result = ref(null)
    const optimizing = ref(false)
    const canvas = ref(null)
    
    const hasSelectedDieLines = computed(() => {
      return Object.values(selectedDieLines.value).some(v => v)
    })
    
    const loadData = async () => {
      try {
        sheets.value = await sheetService.getAllSheets()
        dieLines.value = await dieLineService.getAllDieLines()
      } catch (error) {
        console.error('Failed to load data:', error)
      }
    }
    
    const optimize = async () => {
      if (!selectedSheetId.value) return
      
      optimizing.value = true
      try {
        const request = {
          sheetId: selectedSheetId.value,
          dieLines: Object.keys(selectedDieLines.value)
            .filter(id => selectedDieLines.value[id])
            .map(id => ({
              dieLineId: id,
              quantity: dieLineQuantities.value[id] || 1
            })),
          options: options.value
        }
        
        result.value = await nestingService.optimizeLayout(request)
        
        await nextTick()
        drawLayout()
        
      } catch (error) {
        console.error('Optimization failed:', error)
        alert('Optimization failed. Make sure the backend is running.')
      } finally {
        optimizing.value = false
      }
    }
    
    const clearLayout = () => {
      result.value = null
      selectedDieLines.value = {}
      dieLineQuantities.value = {}
    }
    
    const drawLayout = () => {
      if (!canvas.value || !result.value) return
      
      const ctx = canvas.value.getContext('2d')
      // Use optimized sheet if available, otherwise use selected sheet
      const sheet = result.value.isOptimizedSize && result.value.optimizedSheet
        ? result.value.optimizedSheet
        : sheets.value.find(s => s.id === selectedSheetId.value)
      
      if (!sheet) return
      
      // Clear canvas
      ctx.clearRect(0, 0, canvas.value.width, canvas.value.height)
      
      // Calculate scale
      const scale = Math.min(
        (canvas.value.width - 40) / sheet.width,
        (canvas.value.height - 40) / sheet.height
      )
      
      // Draw sheet
      ctx.strokeStyle = '#000'
      ctx.lineWidth = 2
      ctx.strokeRect(20, 20, sheet.width * scale, sheet.height * scale)
      
      // Draw margins
      ctx.strokeStyle = '#999'
      ctx.lineWidth = 1
      ctx.setLineDash([5, 5])
      ctx.strokeRect(
        20 + sheet.marginLeft * scale,
        20 + sheet.marginTop * scale,
        (sheet.width - sheet.marginLeft - sheet.marginRight) * scale,
        (sheet.height - sheet.marginTop - sheet.marginBottom) * scale
      )
      ctx.setLineDash([])
      
      // Draw placements
      const colors = ['#FF6B6B', '#4ECDC4', '#45B7D1', '#FFA07A', '#98D8C8']
      result.value.placements.forEach((placement, index) => {
        const dieLine = dieLines.value.find(d => d.id === placement.dieLineId)
        if (!dieLine) return
        
        ctx.fillStyle = colors[index % colors.length] + '80'
        ctx.strokeStyle = colors[index % colors.length]
        ctx.lineWidth = 2
        
        const x = 20 + placement.x * scale
        const y = 20 + placement.y * scale
        const w = dieLine.width * scale
        const h = dieLine.height * scale
        
        ctx.fillRect(x, y, w, h)
        ctx.strokeRect(x, y, w, h)
        
        // Draw label
        ctx.fillStyle = '#000'
        ctx.font = '12px sans-serif'
        ctx.fillText(`${index + 1}`, x + 5, y + 15)
      })
    }
    
    const exportPdf = async () => {
      if (!result.value) return
      
      try {
        const blob = await exportService.exportToPdf({
          sheetId: selectedSheetId.value,
          placements: result.value.placements,
          includeRegistrationMarks: true,
          includeCropMarks: true,
          bleedSize: 3.0
        })
        
        const url = window.URL.createObjectURL(blob)
        const a = document.createElement('a')
        a.href = url
        a.download = `ganging-${Date.now()}.pdf`
        a.click()
      } catch (error) {
        console.error('Export failed:', error)
        alert('Export failed')
      }
    }
    
    const exportReport = async () => {
      if (!result.value) return
      
      try {
        const blob = await exportService.generateReport({
          sheetId: selectedSheetId.value,
          placements: result.value.placements
        })
        
        const url = window.URL.createObjectURL(blob)
        const a = document.createElement('a')
        a.href = url
        a.download = `report-${Date.now()}.csv`
        a.click()
      } catch (error) {
        console.error('Report generation failed:', error)
        alert('Report generation failed')
      }
    }
    
    watch(selectedSheetId, () => {
      result.value = null
    })
    
    onMounted(() => {
      loadData()
    })
    
    return {
      sheets,
      dieLines,
      selectedSheetId,
      selectedDieLines,
      dieLineQuantities,
      options,
      result,
      optimizing,
      canvas,
      hasSelectedDieLines,
      optimize,
      clearLayout,
      exportPdf,
      exportReport
    }
  }
}
</script>

<style scoped>
.ganging-controls {
  background: #f8f9fa;
  padding: 1.5rem;
  border-radius: 8px;
  margin-bottom: 2rem;
}

.die-line-selection {
  margin-top: 1.5rem;
}

.die-line-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.5rem;
  background: white;
  border-radius: 4px;
  margin-bottom: 0.5rem;
}

.die-line-item label {
  flex: 1;
}

.qty-input {
  width: 80px;
}

.optimization-options {
  margin-top: 1.5rem;
  padding-top: 1.5rem;
  border-top: 1px solid #dee2e6;
}

.sheet-size-range {
  background: #e7f5ff;
  padding: 1rem;
  border-radius: 4px;
  margin: 1rem 0;
  border-left: 4px solid #42b983;
}

.sheet-size-range h4 {
  margin-bottom: 0.75rem;
  color: #2c3e50;
}

.optimization-result {
  margin-top: 2rem;
}

.optimized-sheet-info {
  background: #d4edda;
  border: 1px solid #c3e6cb;
  border-radius: 4px;
  padding: 1rem;
  margin-bottom: 1rem;
}

.optimized-sheet-info .success-note {
  color: #155724;
  font-weight: bold;
  margin-bottom: 0.5rem;
}

.optimized-sheet-info p {
  color: #155724;
  margin: 0.25rem 0;
}

.metrics {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.metric {
  background: #e7f5ff;
  padding: 1rem;
  border-radius: 4px;
  text-align: center;
}

.metric strong {
  display: block;
  margin-bottom: 0.25rem;
  color: #2c3e50;
}

.canvas-container {
  margin: 1.5rem 0;
}

.canvas-container canvas {
  border: 1px solid #dee2e6;
  border-radius: 4px;
  max-width: 100%;
}

.export-actions {
  display: flex;
  gap: 1rem;
  margin-top: 1.5rem;
}
</style>
