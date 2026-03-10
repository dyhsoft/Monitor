<template>
  <div class="water-curve-container">
    <el-card class="search-card">
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="煤矿">
          <el-select v-model="searchForm.mineId" placeholder="请选择煤矿" clearable>
            <el-option label="测试煤矿" value="1"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="传感器">
          <el-select v-model="searchForm.sensorId" placeholder="请选择传感器" clearable>
            <el-option label="水位传感器01" value="1"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="时间范围">
          <el-date-picker
            v-model="searchForm.timeRange"
            type="datetimerange"
            range-separator="至"
            start-placeholder="开始时间"
            end-placeholder="结束时间"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查询</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <el-card class="chart-card">
      <div ref="chartRef" style="width: 100%; height: 400px;"></div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, onUnmounted } from 'vue'

const chartRef = ref<HTMLElement>()
let chartInstance: any = null

const searchForm = reactive({
  mineId: '',
  sensorId: '',
  timeRange: []
})

const initChart = () => {
  if (!chartRef.value) return
  
  // 模拟图表数据
  const xData = []
  const yData = []
  const now = new Date()
  for (let i = 23; i >= 0; i--) {
    const time = new Date(now.getTime() - i * 3600000)
    xData.push(time.getHours() + ':00')
    yData.push((Math.random() * 3 + 1).toFixed(2))
  }
  
  // 使用 ECharts 或简单模拟
  console.log('水位曲线数据:', xData, yData)
}

const handleSearch = () => {
  initChart()
}

onMounted(() => {
  initChart()
})
</script>

<style scoped>
.water-curve-container {
  padding: 16px;
}
.search-card {
  margin-bottom: 16px;
}
.chart-card {
  min-height: 450px;
}
</style>
