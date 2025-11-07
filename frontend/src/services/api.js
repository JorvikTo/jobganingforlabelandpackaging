import axios from 'axios'

const API_BASE_URL = 'https://localhost:5001/api'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

export const dieLineService = {
  async uploadDieLine(file) {
    const formData = new FormData()
    formData.append('file', file)
    const response = await api.post('/DieLine/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
    return response.data
  },

  async getAllDieLines() {
    const response = await api.get('/DieLine')
    return response.data
  },

  async getDieLine(id) {
    const response = await api.get(`/DieLine/${id}`)
    return response.data
  },

  async deleteDieLine(id) {
    await api.delete(`/DieLine/${id}`)
  }
}

export const sheetService = {
  async createSheet(sheetData) {
    const response = await api.post('/Sheet', sheetData)
    return response.data
  },

  async getAllSheets() {
    const response = await api.get('/Sheet')
    return response.data
  },

  async getSheet(id) {
    const response = await api.get(`/Sheet/${id}`)
    return response.data
  },

  async updateSheet(id, sheetData) {
    const response = await api.put(`/Sheet/${id}`, sheetData)
    return response.data
  },

  async deleteSheet(id) {
    await api.delete(`/Sheet/${id}`)
  }
}

export const nestingService = {
  async optimizeLayout(request) {
    const response = await api.post('/Nesting/optimize', request)
    return response.data
  },

  async manualAdjust(request) {
    const response = await api.post('/Nesting/manual-adjust', request)
    return response.data
  },

  async calculateWaste(request) {
    const response = await api.post('/Nesting/calculate-waste', request)
    return response.data
  }
}

export const exportService = {
  async exportToPdf(request) {
    const response = await api.post('/Export/pdf', request, {
      responseType: 'blob'
    })
    return response.data
  },

  async generateReport(request) {
    const response = await api.post('/Export/report', request, {
      responseType: 'blob'
    })
    return response.data
  }
}
