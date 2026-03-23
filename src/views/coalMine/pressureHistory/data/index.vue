<template>
  <div class="pressure-history-container">
    <el-card class="search-card">
      <el-form :inline="true" :model="searchForm">
        <el-form-item label="煤矿">
          <el-select v-model="searchForm.mineId" clearable @change="handleSearch">
            <el-option v-for="item in mineOptions" :key="item.value" :label="item.label" :value="item.value"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="时间范围">
          <el-date-picker v-model="searchForm.timeRange" type="datetimerange" range-separator="至" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查询</el-button>
          <el-button type="success" @click="exportData">导出</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    <el-card class="table-card">
      <el-table :data="tableData" border stripe v-loading="loading">
        <el-table-column prop="mineName" label="煤矿名称" />
        <el-table-column prop="sensorName" label="传感器名称" />
        <el-table-column prop="location" label="位置" />
        <el-table-column prop="pressure" label="压力(MPa)" />
        <el-table-column prop="displacement" label="位移(mm)" />
        <el-table-column prop="recordTime" label="记录时间" />
      </el-table>
    </el-card>
  </div>
</template>
<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, PressureApi } from '/@/api-services/api';
import { ElMessage } from 'element-plus';

const loading = ref(false)
const mineOptions = ref<any[]>([])
const searchForm = reactive({ mineId: null as number | null, timeRange: [] })
const tableData = ref<any[]>([])

const loadMineOptions = async () => {
  try {
    const res = await getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 });
    mineOptions.value = (res.data.result || []).map((item: any) => ({ label: item.name, value: item.id }));
  } catch (error) {
    console.error('加载煤矿列表失败:', error);
  }
}

const handleSearch = async () => {
  if (!searchForm.mineId) { tableData.value = []; return; }
  loading.value = true;
  try {
    const params: any = { mineId: searchForm.mineId, page: 1, pageSize: 100 };
    if (searchForm.timeRange && searchForm.timeRange.length === 2) {
      params.startTime = searchForm.timeRange[0];
      params.endTime = searchForm.timeRange[1];
    }
    const res = await getAPI(PressureApi).getHistoryPage(params);
    tableData.value = res.data.result?.rows || res.data.result || [];
  } catch (error) {
    console.error('加载历史数据失败:', error);
    tableData.value = [];
  } finally {
    loading.value = false;
  }
}

const exportData = () => { ElMessage.success('导出成功') }

onMounted(async () => {
  await loadMineOptions();
  if (mineOptions.value.length > 0) {
    searchForm.mineId = mineOptions.value[0].value;
    handleSearch();
  }
})
</script>
<style scoped>
.pressure-history-container { padding: 16px; }
.search-card { margin-bottom: 16px; }
</style>
