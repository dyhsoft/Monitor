<template>
  <div class="water-curve-container">
    <el-card class="search-card">
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="煤矿">
          <el-select v-model="searchForm.mineId" placeholder="请选择煤矿" clearable @change="handleSearch">
            <el-option v-for="item in mineOptions" :key="item.value" :label="item.label" :value="item.value"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="传感器">
          <el-select v-model="searchForm.sensorId" placeholder="请选择传感器" clearable>
            <el-option label="水位传感器01" value="1"></el-option>
            <el-option label="流量传感器01" value="2"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="曲线类型">
          <el-radio-group v-model="searchForm.curveType">
            <el-radio label="waterLevel">水位曲线</el-radio>
            <el-radio label="flow">流量曲线</el-radio>
          </el-radio-group>
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
          <el-button @click="exportData">导出</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <el-card class="chart-card">
      <div ref="chartRef" style="width: 100%; height: 450px;"></div>
    </el-card>

    <el-row :gutter="16" class="data-summary">
      <el-col :span="8">
        <el-card>
          <template #header>
            <span>最高水位(m)</span>
          </template>
          <div class="summary-value warning">6.2</div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card>
          <template #header>
            <span>最大流量(m³/h)</span>
          </template>
          <div class="summary-value">120.5</div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card>
          <template #header>
            <span>平均排水量(m³/h)</span>
          </template>
          <div class="summary-value">85.2</div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, WaterApi } from '/@/api-services/api';
import { ElMessage } from 'element-plus';

const chartRef = ref<HTMLElement>()
const mineOptions = ref<any[]>([])

const searchForm = reactive({
  mineId: null as number | null,
  sensorId: '',
  curveType: 'waterLevel',
  timeRange: []
})

// 加载煤矿列表
const loadMineOptions = async () => {
  try {
    const res = await getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 });
    mineOptions.value = (res.data.result || []).map((item: any) => ({ label: item.name, value: item.id }));
  } catch (error) {
    console.error('加载煤矿列表失败:', error);
  }
}

const initChart = async () => {
  if (!chartRef.value || !searchForm.mineId) return
  
  try {
    // TODO: 调用真实API获取曲线数据
    const xData = []
    const yData = []
    const now = new Date()
    for (let i = 23; i >= 0; i--) {
      const time = new Date(now.getTime() - i * 3600000)
      xData.push(time.getHours() + ':00')
      yData.push((Math.random() * 3 + 1).toFixed(2))
    }
    console.log('水文曲线数据:', { x: xData, y: yData, type: searchForm.curveType })
  } catch (error) {
    console.error('加载曲线数据失败:', error);
  }
}

const handleSearch = () => {
  if (!searchForm.mineId) {
    ElMessage.warning('请先选择煤矿');
    return;
  }
  initChart()
}

const exportData = () => {
  ElMessage.success('数据导出成功')
}

onMounted(async () => {
  await loadMineOptions();
  if (mineOptions.value.length > 0) {
    searchForm.mineId = mineOptions.value[0].value;
    initChart();
  }
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
  margin-bottom: 16px;
  min-height: 500px;
}
.data-summary {
  margin-top: 16px;
}
.summary-value {
  font-size: 24px;
  font-weight: bold;
  text-align: center;
  padding: 20px;
  color: #409eff;
}
.summary-value.warning {
  color: #e6a23c;
}
</style>
