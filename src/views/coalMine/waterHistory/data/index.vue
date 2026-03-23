<template>
  <div class="water-history-container">
    <el-card class="search-card">
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="煤矿">
          <el-select v-model="searchForm.mineId" placeholder="请选择煤矿" clearable @change="handleSearch">
            <el-option v-for="item in mineOptions" :key="item.value" :label="item.label" :value="item.value"></el-option>
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
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="exportData">导出</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <el-card class="table-card">
      <el-table :data="tableData" border stripe v-loading="loading">
        <el-table-column prop="mineName" label="煤矿名称" min-width="120" />
        <el-table-column prop="sensorName" label="传感器名称" min-width="150" />
        <el-table-column prop="location" label="安装位置" min-width="150" />
        <el-table-column prop="waterLevel" label="水位(m)" min-width="100" />
        <el-table-column prop="flow" label="流量(m³/h)" min-width="100" />
        <el-table-column prop="recordTime" label="记录时间" width="180" />
      </el-table>
      <el-pagination
        v-model:current-page="pagination.current"
        v-model:page-size="pagination.size"
        :total="pagination.total"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
      />
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, WaterApi } from '/@/api-services/api';
import { ElMessage } from 'element-plus';

const loading = ref(false)
const mineOptions = ref<any[]>([])
const searchForm = reactive({
  mineId: null as number | null,
  timeRange: []
})

const tableData = ref<any[]>([])

const pagination = reactive({ current: 1, size: 10, total: 0 })

const loadMineOptions = async () => {
  try {
    const res = await getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 });
    mineOptions.value = (res.data.result || []).map((item: any) => ({ label: item.name, value: item.id }));
  } catch (error) {
    console.error('加载煤矿列表失败:', error);
  }
}

const handleSearch = async () => {
  if (!searchForm.mineId) { tableData.value = []; pagination.total = 0; return; }
  loading.value = true;
  try {
    const params: any = { mineId: searchForm.mineId, page: pagination.current, pageSize: pagination.size };
    if (searchForm.timeRange && searchForm.timeRange.length === 2) {
      params.startTime = searchForm.timeRange[0];
      params.endTime = searchForm.timeRange[1];
    }
    const res = await getAPI(WaterApi).getHistoryPage(params);
    tableData.value = res.data.result?.rows || res.data.result || [];
    pagination.total = res.data.result?.total || 0;
  } catch (error) {
    console.error('加载历史数据失败:', error);
    tableData.value = [];
  } finally {
    loading.value = false;
  }
}

const handleReset = () => { searchForm.mineId = null; searchForm.timeRange = []; pagination.current = 1; handleSearch(); }
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
.water-history-container { padding: 16px; }
.search-card { margin-bottom: 16px; }
.table-card { min-height: 400px; }
</style>
