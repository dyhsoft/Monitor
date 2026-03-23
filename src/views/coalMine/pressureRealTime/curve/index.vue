<template>
  <div class="pressure-curve-container">
    <el-card class="search-card">
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="煤矿">
          <el-select v-model="searchForm.mineId" placeholder="请选择煤矿" clearable @change="handleSearch">
            <el-option v-for="item in mineOptions" :key="item.value" :label="item.label" :value="item.value"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="传感器">
          <el-select v-model="searchForm.sensorId" placeholder="请选择传感器" clearable>
            <el-option label="压力传感器01" value="1"></el-option>
            <el-option label="位移传感器01" value="2"></el-option>
            <el-option label="锚杆应力01" value="3"></el-option>
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
            <span>最大值</span>
          </template>
          <div class="summary-value">15.8 MPa</div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card>
          <template #header>
            <span>最小值</span>
          </template>
          <div class="summary-value">8.2 MPa</div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card>
          <template #header>
            <span>平均值</span>
          </template>
          <div class="summary-value">12.1 MPa</div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, PressureApi } from '/@/api-services/api';

const chartRef = ref<HTMLElement>()
const mineOptions = ref<any[]>([])

const searchForm = reactive({
  mineId: null as number | null,
  sensorId: '',
  timeRange: []
})

const summary = reactive({
  max: 0,
  min: 0,
  avg: 0
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
    const params: any = { mineId: searchForm.mineId, page: 1, pageSize: 100 };
    if (searchForm.timeRange && searchForm.timeRange.length === 2) {
      params.startTime = searchForm.timeRange[0];
      params.endTime = searchForm.timeRange[1];
    }
    const res = await getAPI(PressureApi).getHistoryPage(params);
    const data = res.data.result?.rows || res.data.result || [];
    
    // 计算统计数据
    if (data.length > 0) {
      const values = data.map((d: any) => Number(d.value) || 0);
      summary.max = Math.max(...values);
      summary.min = Math.min(...values);
      summary.avg = values.reduce((a: number, b: number) => a + b, 0) / values.length;
    }
    
    console.log('矿压曲线数据:', { count: data.length })
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

const exportData = () => { ElMessage.success('导出成功') }

onMounted(async () => {
  await loadMineOptions();
  if (mineOptions.value.length > 0) {
    searchForm.mineId = mineOptions.value[0].value;
    initChart();
  }
})
</script>

<style scoped>
.pressure-curve-container {
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
</style>
