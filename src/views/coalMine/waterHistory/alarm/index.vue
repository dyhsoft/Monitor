<template>
  <div class="water-alarm-container">
    <el-card class="search-card">
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="煤矿">
          <el-select v-model="searchForm.mineId" placeholder="请选择煤矿" clearable @change="handleSearch">
            <el-option v-for="item in mineOptions" :key="item.value" :label="item.label" :value="item.value"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="报警类型">
          <el-select v-model="searchForm.alarmType" placeholder="请选择类型" clearable>
            <el-option label="水位超限" value="waterLevel"></el-option>
            <el-option label="流量超限" value="flow"></el-option>
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
        </el-form-item>
      </el-form>
    </el-card>

    <el-card class="table-card">
      <el-table :data="tableData" border stripe v-loading="loading">
        <el-table-column prop="mineName" label="煤矿名称" min-width="120" />
        <el-table-column prop="sensorName" label="传感器名称" min-width="150" />
        <el-table-column prop="alarmType" label="报警类型" width="100">
          <template #default="{ row }">
            <el-tag type="danger">{{ row.alarmType }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="alarmValue" label="报警值" width="100" />
        <el-table-column prop="threshold" label="阈值" width="100" />
        <el-table-column prop="alarmTime" label="报警时间" width="180" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="row.status === '已处理' ? 'success' : 'warning'">{{ row.status }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="120">
          <template #default>
            <el-button type="primary" link>处理</el-button>
          </template>
        </el-table-column>
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

const loading = ref(false)
const mineOptions = ref<any[]>([])
const searchForm = reactive({
  mineId: null as number | null,
  alarmType: '',
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
    const res = await getAPI(WaterApi).getAlarmList(searchForm.mineId);
    tableData.value = (res.data.result || []).map((item: any) => ({
      mineName: item.mineName,
      sensorName: item.sensorName,
      alarmType: item.alarmType || '水位超限',
      alarmValue: item.value,
      threshold: item.threshold,
      alarmTime: item.alarmTime,
      status: item.status === 1 ? '未处理' : '已处理'
    }));
    pagination.total = tableData.value.length;
  } catch (error) {
    console.error('加载报警数据失败:', error);
    tableData.value = [];
  } finally {
    loading.value = false;
  }
}

const handleReset = () => { searchForm.mineId = null; searchForm.alarmType = ''; searchForm.timeRange = []; pagination.current = 1; handleSearch(); }

onMounted(async () => {
  await loadMineOptions();
  if (mineOptions.value.length > 0) {
    searchForm.mineId = mineOptions.value[0].value;
    handleSearch();
  }
})
</script>

<style scoped>
.water-alarm-container { padding: 16px; }
.search-card { margin-bottom: 16px; }
.table-card { min-height: 400px; }
</style>
