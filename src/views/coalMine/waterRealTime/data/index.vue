<template>
  <div class="water-data-container">
    <el-card class="search-card">
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="煤矿">
          <el-select v-model="searchForm.mineId" placeholder="请选择煤矿" clearable @change="handleSearch">
            <el-option v-for="item in mineOptions" :key="item.value" :label="item.label" :value="item.value"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="测点类型">
          <el-select v-model="searchForm.sensorType" placeholder="请选择类型" clearable>
            <el-option label="水位传感器" value="waterLevel"></el-option>
            <el-option label="流量传感器" value="flow"></el-option>
            <el-option label="排水设备" value="drainage"></el-option>
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

    <el-row :gutter="16" class="stats-row">
      <el-col :span="6">
        <el-card shadow="hover">
          <div class="stat-item">
            <div class="stat-label">水位超限报警</div>
            <div class="stat-value warning">2</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover">
          <div class="stat-item">
            <div class="stat-label">流量超限报警</div>
            <div class="stat-value danger">1</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover">
          <div class="stat-item">
            <div class="stat-label">排水设备运行</div>
            <div class="stat-value success">5</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover">
          <div class="stat-item">
            <div class="stat-label">最大排水量(m³/h)</div>
            <div class="stat-value">120.5</div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-card class="table-card">
      <el-table :data="tableData" border stripe v-loading="loading">
        <el-table-column prop="mineName" label="煤矿名称" min-width="120" />
        <el-table-column prop="sensorName" label="传感器名称" min-width="150" />
        <el-table-column prop="location" label="安装位置" min-width="150" />
        <el-table-column prop="sensorType" label="传感器类型" width="100">
          <template #default="{ row }">
            <el-tag :type="getSensorTypeColor(row.sensorType)">{{ row.sensorType }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="waterLevel" label="水位(m)" min-width="100">
          <template #default="{ row }">
            <span :class="{ 'warning': row.waterLevel > row.waterThreshold, 'danger': row.waterLevel > row.waterThreshold * 1.2 }">
              {{ row.waterLevel }}
            </span>
          </template>
        </el-table-column>
        <el-table-column prop="flow" label="流量(m³/h)" min-width="100">
          <template #default="{ row }">
            <span :class="{ 'warning': row.flow > row.flowThreshold }">
              {{ row.flow }}
            </span>
          </template>
        </el-table-column>
        <el-table-column prop="drainageStatus" label="排水状态" width="100">
          <template #default="{ row }">
            <el-tag :type="row.drainageStatus === '运行' ? 'success' : 'info'">
              {{ row.drainageStatus }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.status === '正常' ? 'success' : 'danger'">
              {{ row.status }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="updateTime" label="更新时间" width="180" />
      </el-table>
      <el-pagination
        v-model:current-page="pagination.current"
        v-model:page-size="pagination.size"
        :total="pagination.total"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
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
  sensorType: '',
  timeRange: []
})

const tableData = ref<any[]>([])

const pagination = reactive({
  current: 1,
  size: 10,
  total: 0
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

const getSensorTypeColor = (type: string) => {
  const map: Record<string, string> = {
    '水位传感器': 'primary',
    '流量传感器': 'warning',
    '排水设备': 'success'
  }
  return map[type] || 'info'
}

const handleSearch = async () => {
  if (!searchForm.mineId) {
    loading.value = false;
    return;
  }
  loading.value = true
  try {
    const params = {
      mineId: searchForm.mineId,
      page: pagination.current,
      pageSize: pagination.size,
      sensorType: searchForm.sensorType || undefined
    };
    const res = await getAPI(WaterApi).getRealtimePage(params);
    const result = res.data.result;
    tableData.value = result?.rows || result || [];
    pagination.total = result?.total || tableData.value.length;
  } catch (error) {
    console.error('加载水文数据失败:', error);
    tableData.value = [];
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  searchForm.mineId = null;
  searchForm.sensorType = '';
  searchForm.timeRange = [];
  pagination.current = 1;
  handleSearch();
}

const handleSizeChange = (size: number) => {
  pagination.size = size;
  handleSearch();
}

const handleCurrentChange = (current: number) => {
  pagination.current = current;
  handleSearch();
}

onMounted(async () => {
  await loadMineOptions();
  if (mineOptions.value.length > 0) {
    searchForm.mineId = mineOptions.value[0].value;
  }
  handleSearch();
})
</script>

<style scoped>
.water-data-container {
  padding: 16px;
}
.search-card {
  margin-bottom: 16px;
}
.stats-row {
  margin-bottom: 16px;
}
.stat-item {
  text-align: center;
  padding: 10px;
}
.stat-label {
  font-size: 14px;
  color: #909399;
  margin-bottom: 8px;
}
.stat-value {
  font-size: 28px;
  font-weight: bold;
}
.stat-value.success { color: #67c23a; }
.stat-value.danger { color: #f56c6c; }
.stat-value.warning { color: #e6a23c; }
.table-card {
  min-height: 400px;
}
.warning {
  color: #e6a23c;
  font-weight: bold;
}
.danger {
  color: #f56c6c;
  font-weight: bold;
}
</style>
