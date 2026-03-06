<template>
  <div>
    <n-card title="水害监测实时数据" :bordered="false" class="proCard">
      <n-space vertical :size="16">
        <!-- 统计卡片 -->
        <n-grid :x-gap="16" :y-gap="16" cols="4">
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="监测点总数" :value="statistics.totalCount" />
            </n-card>
          </n-gi>
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="正常" :value="statistics.normalCount" />
            </n-card>
          </n-gi>
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="报警" :value="statistics.alarmCount" />
            </n-card>
          </n-gi>
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="故障" :value="statistics.faultCount" />
            </n-card>
          </n-gi>
        </n-grid>

        <!-- 操作栏 -->
        <n-space>
          <n-button type="primary" @click="loadData">
            <template #icon>
              <n-icon><Refresh /></n-icon>
            </template>
            刷新
          </n-button>
          <n-button type="warning" @click="showAlarmList = true">
            <template #icon>
              <n-icon><Warning /></n-icon>
            </template>
            报警列表
          </n-button>
        </n-space>

        <!-- 搜索栏 -->
        <n-space :size="12">
          <n-select
            v-model:value="searchParams.mineId"
            placeholder="选择煤矿"
            clearable
            :options="mineOptions"
            style="width: 200px"
          />
          <n-select
            v-model:value="searchParams.sensorCode"
            placeholder="传感器编号"
            clearable
            style="width: 180px"
          />
          <n-select
            v-model:value="searchParams.status"
            placeholder="状态"
            clearable
            :options="statusOptions"
            style="width: 120px"
          />
          <n-button type="primary" @click="loadData">搜索</n-button>
          <n-button @click="handleReset">重置</n-button>
        </n-space>

        <!-- 数据表格 -->
        <n-data-table
          :columns="columns"
          :data="dataList"
          :loading="loading"
          :pagination="pagination"
          :row-key="(row) => row.id"
          :row-class-name="getRowClassName"
        />
      </n-space>
    </n-card>

    <!-- 报警列表弹窗 -->
    <n-modal v-model:show="showAlarmList" preset="card" title="水害报警列表" style="width: 800px">
      <n-data-table
        :columns="alarmColumns"
        :data="alarmList"
        :loading="alarmLoading"
        :row-key="(row) => row.id"
      />
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { Refresh, Warning } from '@vicons/tabler'
import { getWaterRealtimePage, getWaterAlarmList } from '@/api/coalmine/water'
import { getCoalMineList } from '@/api/coalmine/coal'
import { useMessage } from 'naive-ui'

const message = useMessage()

const loading = ref(false)
const alarmLoading = ref(false)
const dataList = ref([])
const alarmList = ref([])
const showAlarmList = ref(false)

const pagination = reactive({
  page: 1,
  pageSize: 10,
  itemCount: 0,
  showSizePicker: true,
  pageSizes: [10, 20, 50],
  showQuickJumper: true
})

const searchParams = reactive({
  mineId: null,
  sensorCode: null,
  status: null
})

// 统计
const statistics = reactive({
  totalCount: 0,
  normalCount: 0,
  alarmCount: 0,
  faultCount: 0
})

// 选项
const mineOptions = ref([])
const statusOptions = [
  { label: '正常', value: 0 },
  { label: '报警', value: 1 },
  { label: '故障', value: 2 }
]

// 表格列
const columns = [
  { title: 'ID', key: 'id', width: 60 },
  { title: '煤矿', key: 'mineName', width: 120 },
  { title: '传感器编号', key: 'sensorCode', width: 140 },
  { title: '传感器名称', key: 'sensorName', width: 150 },
  { title: '状态', key: 'status', width: 80, render: (row: any) => getStatusName(row.status) },
  { title: '水位(m)', key: 'waterLevel', width: 100 },
  { title: '流量(m³/h)', key: 'flowRate', width: 100 },
  { title: '温度(℃)', key: 'temperature', width: 80 },
  { title: '更新时间', key: 'updateTime', width: 160 }
]

// 报警列表列
const alarmColumns = [
  { title: '传感器编号', key: 'sensorCode', width: 140 },
  { title: '传感器名称', key: 'sensorName', width: 150 },
  { title: '水位(m)', key: 'waterLevel', width: 100 },
  { title: '状态', key: 'status', width: 80, render: (row: any) => getStatusName(row.status) },
  { title: '更新时间', key: 'updateTime', width: 160 }
]

function getStatusName(status: number) {
  const map = { 0: '正常', 1: '报警', 2: '故障' }
  return map[status] || '未知'
}

function getRowClassName(row: any) {
  if (row.status === 1) return 'row-alarm'
  if (row.status === 2) return 'row-fault'
  return ''
}

async function loadData() {
  loading.value = true
  try {
    const res = await getWaterRealtimePage({
      current: pagination.page,
      size: pagination.pageSize,
      ...searchParams
    })
    dataList.value = res.data.rows || []
    pagination.itemCount = res.data.total || 0
    
    // 统计
    statistics.totalCount = res.data.total || 0
    statistics.normalCount = dataList.value.filter((r: any) => r.status === 0).length
    statistics.alarmCount = dataList.value.filter((r: any) => r.status === 1).length
    statistics.faultCount = dataList.value.filter((r: any) => r.status === 2).length
  } finally {
    loading.value = false
  }
}

async function loadAlarmList() {
  alarmLoading.value = true
  try {
    const res = await getWaterAlarmList(searchParams.mineId)
    alarmList.value = res.data || []
  } finally {
    alarmLoading.value = false
  }
}

async function loadMineOptions() {
  try {
    const res = await getCoalMineList()
    mineOptions.value = (res.data || []).map((m: any) => ({ label: m.name, value: m.id }))
  } catch (e) {
    console.error('加载煤矿列表失败', e)
  }
}

function handleReset() {
  searchParams.mineId = null
  searchParams.sensorCode = null
  searchParams.status = null
  loadData()
}

// 监听报警列表显示
watch(showAlarmList, (val) => {
  if (val) {
    loadAlarmList()
  }
})

onMounted(() => {
  loadMineOptions()
  loadData()
})
</script>

<script lang="ts">
import { watch } from 'vue'

export default {
  setup() {
    return {}
  }
}
</script>

<style scoped>
.proCard {
  background: #fff;
}

.stat-card {
  text-align: center;
}

:deep(.row-alarm) {
  background-color: #fff1f0;
}

:deep(.row-fault) {
  background-color: #fffbe6;
}
</style>
