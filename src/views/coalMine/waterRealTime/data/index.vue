<template>
  <div class="water-data-container">
    <el-card class="search-card">
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="煤矿">
          <el-select v-model="searchForm.mineId" placeholder="请选择煤矿" clearable>
            <el-option label="测试煤矿" value="1"></el-option>
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
        <el-table-column prop="sensorName" label="传感器名称" min-width="120" />
        <el-table-column prop="location" label="安装位置" min-width="150" />
        <el-table-column prop="waterLevel" label="水位(m)" min-width="100">
          <template #default="{ row }">
            <span :class="{ 'warning': row.waterLevel > row.threshold }">
              {{ row.waterLevel }}
            </span>
          </template>
        </el-table-column>
        <el-table-column prop="flow" label="流量(m³/h)" min-width="100" />
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

const loading = ref(false)
const searchForm = reactive({
  mineId: '',
  timeRange: []
})

const tableData = ref([
  {
    mineName: '测试煤矿',
    sensorName: '水位传感器01',
    location: '主井',
    waterLevel: 2.5,
    threshold: 5.0,
    flow: 10.2,
    status: '正常',
    updateTime: '2026-03-10 13:00:00'
  }
])

const pagination = reactive({
  current: 1,
  size: 10,
  total: 1
})

const handleSearch = () => {
  loading.value = true
  setTimeout(() => {
    loading.value = false
  }, 500)
}

const handleReset = () => {
  searchForm.mineId = ''
  searchForm.timeRange = []
  handleSearch()
}

const handleSizeChange = (size: number) => {
  pagination.size = size
  handleSearch()
}

const handleCurrentChange = (current: number) => {
  pagination.current = current
  handleSearch()
}

onMounted(() => {
  handleSearch()
})
</script>

<style scoped>
.water-data-container {
  padding: 16px;
}
.search-card {
  margin-bottom: 16px;
}
.table-card {
  min-height: 400px;
}
.warning {
  color: #f56c6c;
  font-weight: bold;
}
</style>
